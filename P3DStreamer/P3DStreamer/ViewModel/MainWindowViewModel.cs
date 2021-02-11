using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Linq;
using Rect = System.Drawing.Rectangle;
using System.Net;
using System.Windows.Threading;
using P3DStreamer.Interop;
using P3DStreamer.Extensions;
using P3DStreamer.DataModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace P3DStreamer.ViewModel
{
    class MainWindowViewModel
    {
        public ICommand Reset { get; }
        public ICommand Smaller { get; }
        public ICommand Larger { get; }
        public ICommand Quit { get; }
        public ICommand Hide { get; }

        private bool m_checkPanelOnRender;
        private DesktopDuplicator m_capture;
        private List<NetworkCaptureWindow> m_windows = new List<NetworkCaptureWindow>();
        private System.Windows.Forms.Screen m_screen;
        private System.Windows.Forms.Screen m_primaryScreen;
        private Dictionary<IntPtr, WindowFlags> m_windowFlags = new Dictionary<IntPtr, WindowFlags>();
        private bool m_isFlip;
        private bool m_isCduFlip;
        public Window _window;
        private AppViewModel _appVm;
        private int m_gameWindowSize = 3;

        void PositionFS2020Window()
        {
            var gameHwnd = User32.FindWindow("AceApp", "Microsoft Flight Simulator - 1.12.13.0");
            if (gameHwnd == IntPtr.Zero)
            {
                _window.Dispatcher.InvokeAsync(() =>
                {
                    _window.Left = 50;
                    _window.Height = 31;
                    _window.Width = m_primaryScreen.WorkingArea.Width - 300;
                    _window.Cloak(false);
                });
                return;
            }

            var gameLeft = -8;
            var gameWidth = 2560 + 16;
            var gameHeight = 1440 - 32;

            switch (m_gameWindowSize)
            {
                case 2:
                    gameWidth += 2560 / 2;
                    gameLeft -= 2560 / 4;
                    break;
                case 3:
                    gameWidth += 2560;
                    gameLeft -= 2560 / 2;
                    break;
                case 4:
                    gameWidth += 2560;
                    gameWidth += 2560;
                    gameLeft -= 2560;
                    break;
            }

            User32.SetWindowPos(gameHwnd, 0, gameLeft, 0, gameWidth, gameHeight, User32.SWP_NOZORDER);

            var blackout = User32.FindWindow_ByCaption(IntPtr.Zero, "TaskbarBlackout");
            User32.SetWindowPos(blackout, 0, gameLeft, gameHeight - 6, gameWidth, 40, User32.SWP_NOZORDER);
            DwmApi.CloakWindow(blackout, false);
            _window.Dispatcher.InvokeAsync(() =>
            {
                _window.Height = 31;
                _window.Left = gameLeft;
                _window.Width = gameWidth - 50;
                _window.Cloak(false);
            });
        }

        void Setup_A5_FS2020()
        {
            PositionFS2020Window();

            var windows = new List<NetworkCaptureWindow>();
            var defaultClip = new WindowClip();
            var pfdClip = new WindowClip();
            var mfdClip = new WindowClip();

            var panel_height = 478;
            var panel_width = 600;
            var mfd_ext_width = 190;
            var pfd_ext_width = 65;
            var pfd_ext_height = panel_height;

            if (_appVm.CurrentAircraftAvionics == AvionicsKind.G3000)
            {
                panel_height = 404;
                mfdClip.LClip += (int)(panel_height * 0.01);
                pfdClip.BottomClip += 5;
                mfd_ext_width = 84;
                pfd_ext_height = 68;
                pfd_ext_width = panel_width;
            }
            else if (_appVm.CurrentAircraftAvionics == AvionicsKind.G3X)
            {
                panel_height = pfd_ext_height = 468;
                panel_width = 700;
                mfd_ext_width = 84;
                pfd_ext_height = 68;
                pfd_ext_width = panel_width;
            }
            else if (_appVm.CurrentAircraftAvionics == AvionicsKind.G1000)
            {
                panel_height = pfd_ext_height = 478;
                pfdClip.BottomClip += (int)(panel_height * 0.04);
                mfdClip.BottomClip += (int)(panel_height * 0.04);
            }
            else if (_appVm.CurrentAircraftAvionics == AvionicsKind.F22)
            {
                panel_height = pfd_ext_height = 478;
                panel_width = 540; // updated below


                pfdClip.TopClip += (int)(panel_height * 0.022);
               // pfdClip.BottomClip += (int)(panel_height * 0.07);
                pfdClip.BottomClip += (int)(panel_height * 0.12);

                mfdClip.BottomClip += (int)(panel_height * 0.12);
                mfdClip.TopClip += (int)(panel_height * 0.12);
            }

            var ptrs = new List<IntPtr>();
            var ptrs2 = new List<IntPtr>();

            var hwnd = User32.FindWindow("AceApp", "");
            ptrs.Add(hwnd);
            var hwnd2 = User32.FindWindowEx(IntPtr.Zero, hwnd, "AceApp", "");
            ptrs.Add(hwnd2);

            var hwnd3 = User32.FindWindowEx(IntPtr.Zero, hwnd2, "AceApp", "");
            ptrs.Add(hwnd3);

            var hwnd4 = User32.FindWindowEx(IntPtr.Zero, hwnd3, "AceApp", "");
            ptrs.Add(hwnd4);

            ptrs.Sort((one, two) => one.ToInt64().CompareTo(two.ToInt64()));

            var topfullPtrs = ptrs.Where(p => m_windowFlags.ContainsKey(p) && m_windowFlags[p] == WindowFlags.TopFull);
            var fullPtrs = ptrs.Where(p => m_windowFlags.ContainsKey(p) && m_windowFlags[p] == WindowFlags.Full);
            var cduptrs = ptrs.Where(p => m_windowFlags.ContainsKey(p) && m_windowFlags[p] == WindowFlags.CDU);

            ptrs2.AddRange(topfullPtrs);
            ptrs2.AddRange(fullPtrs);

            if (m_isCduFlip)
            {
                ptrs2.AddRange(cduptrs.Reverse());
            }
            else
            {
                ptrs2.AddRange(cduptrs);
            }

            if (ptrs2.Count == 0 || ptrs2.Count < 4)
            {
                Trace.Write("RESET PTRS");
                ptrs2.Clear();
                var ptrs3 = ptrs.GroupBy(x => x).Select(y => y.First());
                ptrs2.AddRange(ptrs3.Where(ptr => ptr != IntPtr.Zero));

                if (ptrs2.Count == 2 && m_windowFlags.ContainsKey(ptrs2[1]) &&
                    m_windowFlags[ptrs2[1]] == WindowFlags.TopFull)
                {
                    var tmp = ptrs2[0];
                    ptrs2[0] = ptrs2[1];
                    ptrs2[1] = tmp;
                }
            }

            if (ptrs2.Count < 1) return;

            var PFD = new CaptureableWindow(ptrs2[0], new Rect(m_screen.WorkingArea.Left, m_screen.WorkingArea.Top, panel_width, panel_height), pfdClip);
            windows.Add(new NetworkCaptureWindow(PFD, "192.168.0.105", 11001)); // SP4

            hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "PFD_EXT");
            var PFD_EXT = new CaptureableWindow(hwnd, new Rect(m_screen.WorkingArea.Left + panel_width, m_screen.WorkingArea.Top, pfd_ext_width, pfd_ext_height), defaultClip);
            var PFD_EXTw = new NetworkCaptureWindow(PFD_EXT, "192.168.0.105", 11004);
            PFD_EXTw.IsOverlay = true;
            windows.Add(PFD_EXTw); // SP4


            hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "PFDTop");
            var PFDTop = new CaptureableWindow(hwnd, new Rect(m_screen.WorkingArea.Left + defaultClip.LClip, m_screen.WorkingArea.Top + pfdClip.TopClip, panel_width - defaultClip.RClip - defaultClip.LClip, panel_height - pfdClip.TopClip - pfdClip.BottomClip), defaultClip);
            windows.Add(new NetworkCaptureWindow(PFDTop, "localhost", 0, false));

            var cdu_width = 290;

            CaptureableWindow CDU = PFD;

            hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "MFD_EXT");
            var MFD_EXT = new CaptureableWindow(hwnd, new Rect(m_screen.WorkingArea.Left + panel_width, m_screen.WorkingArea.Top + panel_height, mfd_ext_width, panel_height), defaultClip);
            var MFD_EXTw = new NetworkCaptureWindow(MFD_EXT, "192.168.0.117", 11003);
            MFD_EXTw.IsOverlay = true;
            windows.Add(MFD_EXTw); // Nike

            if (ptrs2.Count > 1)
            {
                if (_appVm.CurrentAircraftAvionics == AvionicsKind.F22)
                {
                    panel_width = 456;
                }

                var ND = new CaptureableWindow(ptrs2[1], new Rect(m_screen.WorkingArea.Left, m_screen.WorkingArea.Top + panel_height, panel_width, panel_height), mfdClip);
                windows.Add(new NetworkCaptureWindow(ND, "192.168.0.117", 11002)); // Nike


                CDU = ND;




                hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "MFDTop");
                var MFDTop = new CaptureableWindow(hwnd, new Rect(m_screen.WorkingArea.Left + defaultClip.LClip, m_screen.WorkingArea.Top + panel_height + mfdClip.TopClip, panel_width - defaultClip.RClip - defaultClip.LClip, panel_height - mfdClip.BottomClip - mfdClip.TopClip), defaultClip);
                windows.Add(new NetworkCaptureWindow(MFDTop, "localhost", 0, false));


                if (ptrs2.Count > 2)
                {

                    CDU = new CaptureableWindow(ptrs2[2], new Rect(m_screen.WorkingArea.Left + panel_width + mfd_ext_width + 100, m_screen.WorkingArea.Top + panel_height, cdu_width, panel_height), defaultClip);
                    var CDUw = new NetworkCaptureWindow(CDU, "192.168.0.107", 11005);
                    CDUw.IsCDU = true;
                    windows.Add(CDUw); // NUVision

                    if (ptrs2.Count > 3)
                    {
                        var CDU2 = new CaptureableWindow(ptrs2[3], new Rect(m_screen.WorkingArea.Left + panel_width + mfd_ext_width + 100 + cdu_width, m_screen.WorkingArea.Top + panel_height, cdu_width, panel_height), defaultClip);
                        var CDU2w = new NetworkCaptureWindow(CDU2, "192.168.0.107", 11009);
                        CDU2w.IsCDU = true;
                        windows.Add(CDU2w); // NUVision

                        hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "CDUTop1");
                        var CDUTop1 = new CaptureableWindow(hwnd, CDU.Location, defaultClip);
                        windows.Add(new NetworkCaptureWindow(CDUTop1, "localhost", 0, false));

                        hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "CDUTop2");
                        var CDUTop2 = new CaptureableWindow(hwnd, CDU2.Location, defaultClip);
                        windows.Add(new NetworkCaptureWindow(CDUTop2, "localhost", 0, false));
                    }


                }
                else
                {
                    hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "CDUTop1");
                    DwmApi.CloakWindow(hwnd);
                    hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "CDUTop2");
                    DwmApi.CloakWindow(hwnd);
                }

            }

            hwnd = User32.FindWindow_ByCaption(IntPtr.Zero, "CDUExt");
            var CDUExt = new CaptureableWindow(hwnd, new Rect(CDU.Location.Left, CDU.Location.Bottom, cdu_width, 110), defaultClip);
            var CDUExtw = new NetworkCaptureWindow(CDUExt, "192.168.0.107", 11006);
            CDUExtw.IgnoreFromRelayout = true;
            windows.Add(CDUExtw);

            m_windows = windows;
        }

        void InitializeCaptureWindows()
        {
            foreach (var sc in System.Windows.Forms.Screen.AllScreens)
            {
                Trace.WriteLine("Screen: " + sc.Bounds);
            }

            m_screen = System.Windows.Forms.Screen.AllScreens.First(s => s.Bounds.Height == 1080);
            m_primaryScreen = System.Windows.Forms.Screen.PrimaryScreen;

            Setup_A5_FS2020();
        }

        void InitializeCaptureSystem()
        {
            m_capture = new DesktopDuplicator(2);
        }

        async void PositionWindows()
        {
            m_windows.Clear();
            Setup_A5_FS2020();

            foreach (var win in m_windows)
            {
                win.Window.Position();
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
            m_checkPanelOnRender = true;
        }

        void RenderWindows(Bitmap desktopFrame)
        {
            bool needsPosition = false;

            var isCheckValidity = m_checkPanelOnRender;

            if (isCheckValidity)
            {
                Trace.WriteLine("--------------------");
            }

            foreach (var win in m_windows.ToArray())
            {
                if (win.IsRender)
                {
                    var cropped = win.RenderToRemote(desktopFrame, m_screen);

                    if (isCheckValidity && win.IsRender && !win.IsOverlay && !win.IgnoreFromRelayout)
                    {
                        if (win.IsCDU)
                        {
                            var px = cropped.GetPixel(cropped.Width / 2, 4);
                            Trace.WriteLine($"CDU {win.Window.Handle} {px}");
                            if (px.R + px.G + px.B < 7)
                            {
                                m_windowFlags[win.Window.Handle] = WindowFlags.Full;
                                needsPosition = true;
                                Trace.WriteLine("Marking as full (not CDU)");
                            }
                            else
                            {
                                m_windowFlags[win.Window.Handle] = WindowFlags.CDU;

                            }
                        }
                        else
                        {
                            var px = cropped.GetPixel(4, cropped.Height / 2);
                            Trace.WriteLine($"FULL {win.Window.Handle} {px}");
                            if (px.R + px.G + px.B < 7 && _appVm.CurrentAircraftAvionics != AvionicsKind.F22)
                            {
                                m_windowFlags[win.Window.Handle] = WindowFlags.CDU;
                                needsPosition = true;
                                Trace.WriteLine("Marking as CDU (not full)");
                            }
                            else
                            {
                                int etot = 0;
                                if (_appVm.CurrentAircraftAvionics == AvionicsKind.G3X)
                                {
                                    px = cropped.GetPixel(cropped.Width / 2, 155 - 31);
                                    etot = 147 + 207; // 86 + 157;
                                }
                                else if (_appVm.CurrentAircraftAvionics == AvionicsKind.F22)
                                {
                                    px = cropped.GetPixel(cropped.Width / 2, 60);
                                    etot = 86 + 157;
                                }
                                else if (_appVm.CurrentAircraftAvionics == AvionicsKind.G1000)
                                {
                                    px = cropped.GetPixel(cropped.Width / 2, 155 - 31);
                                    etot = 86 + 157;
                                }
                                else // G3000
                                {
                                    px = cropped.GetPixel(cropped.Width - 4, cropped.Height - 4);
                                    etot = 26 + 29 + 33;
                                }
                                
                                Trace.WriteLine($"CHECKFULL {win.Window.Handle} {px}");

                                var tot = px.R + px.G + px.B;
                                
                                
                               
                                if (tot < etot + 5 && tot > etot - 5)
                                {
                                    if (!m_windowFlags.ContainsKey(win.Window.Handle) ||
                                        m_windowFlags[win.Window.Handle] != WindowFlags.TopFull)
                                    {
                                        m_windowFlags[win.Window.Handle] = WindowFlags.TopFull;
                                        Trace.WriteLine($"Marking as TOP");
                                        needsPosition = true;
                                    }
                                }
                                else
                                {
                                    m_windowFlags[win.Window.Handle] = WindowFlags.Full;
                                }
                            }
                        }
                    }
                }

            }

            if (isCheckValidity)
            {
                Trace.WriteLine("--------------------");
                m_checkPanelOnRender = false;
            }

            if (needsPosition)
            {
                _window.Dispatcher.InvokeAsync(() => PositionWindows());
            }
        }

        public MainWindowViewModel(AppViewModel appVm)
        {
            _appVm = appVm;
            Reset = new RelayCommand(() => PositionWindows());
            Quit = new RelayCommand(() => Environment.Exit(0));
            Smaller = new RelayCommand(() =>
            {
                m_gameWindowSize--;
                if (m_gameWindowSize < 1) m_gameWindowSize = 1;
                PositionFS2020Window();
            });
            Larger = new RelayCommand(() =>
            {
                m_gameWindowSize++;
                if (m_gameWindowSize > 4) m_gameWindowSize = 4;
                PositionFS2020Window();
            });
            Hide = new RelayCommand(async () =>
            {
                _window.Cloak(true);
                await Task.Delay(TimeSpan.FromSeconds(5));
                _window.Cloak(false);
            });

            Initialize();
        }

        private async void Initialize()
        {
            var inputDispatcherThread = new Thread(() => Dispatcher.Run());
            inputDispatcherThread.Start();

            var t = new Thread(() =>
            {
                using (var udpClient = new UdpClient(7777))
                {
                    var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    while (true)
                    {
                        var bytes = udpClient.Receive(ref remoteEndPoint);

                        _window.Dispatcher.InvokeAsync(() =>
                          {
                              int sourcePort = BitConverter.ToInt32(bytes, 0);
                              double x = BitConverter.ToDouble(bytes, 0 + 4);
                              double y = BitConverter.ToDouble(bytes, 0 + 4 + 8);

                              var window = m_windows.FirstOrDefault(w => w.SourcePort == sourcePort);
                              if (window != null)
                              {
                                  var rect = window.Window.Location;
                                  rect = new Rect(rect.Left + window.Window.Clip.LClip,
                                                  rect.Top + window.Window.Clip.TopClip,
                                                  rect.Width - window.Window.Clip.LClip - window.Window.Clip.RClip,
                                                  rect.Height - window.Window.Clip.TopClip - window.Window.Clip.BottomClip);

                                  Dispatcher.FromThread(inputDispatcherThread).InvokeAsync(() =>
                                  {
                                      User32.SetForegroundWindow(window.Window.Handle);
                                      User32.LeftMouseClick(
                                          (int)(rect.Left + (rect.Width * x)),
                                          (int)(rect.Top + (rect.Height * y)));
                                      var owner = User32.GetWindow(window.Window.Handle, 4);
                                      User32.SetForegroundWindow(owner);
                                      User32.SetActiveWindow(owner);
                                  });
                              }
                          });
                    }
                }
            });
            t.Start();

            t = new Thread(() =>
            {
                InitializeCaptureSystem();
                InitializeCaptureWindows();

                while (true)
                {
                    try
                    {
                        var frame = m_capture.GetLatestFrame()?.DesktopImage;
                        if (frame != null)
                        {
                            RenderWindows(frame);
                        }
                        else
                        {
                            InitializeCaptureSystem();
                        }
                    }
                    catch (DesktopDuplicationException)
                    {
                        InitializeCaptureSystem();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                }
            });
            t.Start();

            await Task.Delay(TimeSpan.FromSeconds(2));

            PositionWindows();
        }

        private void FlipFull()
        {
            m_isFlip = !m_isFlip;
            PositionWindows();
        }

        public void FlipCDUs()
        {
            m_isCduFlip = !m_isCduFlip;
            PositionWindows();
        }
    }
}
