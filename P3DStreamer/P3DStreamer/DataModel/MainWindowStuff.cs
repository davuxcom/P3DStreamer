using P3DStreamer.Extensions;
using P3DStreamer.Interop;
using System;
using System.Drawing;
using Rect = System.Drawing.Rectangle;


namespace P3DStreamer.DataModel
{
    public class CaptureableWindow
    {
        public Rect Location { get; private set; }
        public WindowClip Clip { get; private set; }
        public IntPtr Handle { get; private set; }

        public CaptureableWindow(IntPtr handle, Rect location, WindowClip clip)
        {
            Location = location;
            Handle = handle;
            Clip = clip;
        }

        public void Position()
        {
            if (Handle != IntPtr.Zero)
            {
                User32.SetWindowPos(Handle, 0, (int)Location.Left, (int)Location.Top, (int)Location.Width, (int)Location.Height, User32.SWP_NOZORDER);
                DwmApi.CloakWindow(Handle, false);
            }
        }

        public Bitmap RenderFromDesktop(Bitmap desktop, System.Windows.Forms.Screen screen)
        {
            // Convert screen rect to monitor rect
            var r = new Rect(Location.Left - screen.Bounds.Left, Location.Top - screen.Bounds.Top, Location.Width, Location.Height);

            var InnerLocation = new Rect(
                r.Left + Clip.LClip,
                r.Top + Clip.TopClip,
                r.Width - Clip.LClip - Clip.RClip,
                r.Height - Clip.TopClip - Clip.BottomClip
                );

            return ImageExtensions.Copy(desktop, InnerLocation);
        }
    }

    public class NetworkCaptureWindow
    {
        public CaptureableWindow Window { get; set; }
        public int SourcePort { get; private set; }
        public int Quality { get; private set; }
        public bool IsRender { get; private set; }
        public bool IsCDU { get; set; }
        public bool IsOverlay { get; set; }
        public bool IgnoreFromRelayout { get; set; }
        
        int m_port;
        string m_address;
        int render_counter = 0;
        const int max_size = 65535 - 100;

        public NetworkCaptureWindow(CaptureableWindow window, string address, int port, bool isRender = true)
        {
            IsRender = isRender;
            Window = window;
            m_address = address;
            SourcePort = m_port = port;
            Quality = 100;
        }


        public Bitmap RenderToRemote(Bitmap desktop, System.Windows.Forms.Screen screen)
        {

            render_counter++;

            var cropped = Window.RenderFromDesktop(desktop, screen);

            if (cropped == null) return null;

            byte[] imageBytes = ImageExtensions.ToByteArray(cropped, Quality);

            if (render_counter % 100 == 0)
            {
                while ((imageBytes.Length < (0.98 * max_size)) && (Quality < 100))
                {
                    Quality += 2;
                    if (Quality > 100) Quality = 100;
                    imageBytes = ImageExtensions.ToByteArray(cropped, Quality);
                    // Trace.WriteLine("Qutlity 1");

                    break;
                }
            }

            while (imageBytes.Length > max_size)
            {
                Quality -= 1;
                if (Quality < 1)
                {
                    Quality = 1;
                    break;
                }

                imageBytes = ImageExtensions.ToByteArray(cropped, Quality);
                //  Trace.WriteLine("Qutlity 2");
            }

            UdpHelper.Send(m_address, m_port, imageBytes);
            return cropped;
        }
    }

    public class WindowClip
    {
        public int TopClip = 31;
        public int BottomClip = 8;
        public int LClip = 8;
        public int RClip = 8;
    }

    enum WindowFlags
    {
        CDU,
        Full,
        TopFull,
    }

}
