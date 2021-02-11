using P3DReceiver.Extensions;
using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace P3DReceiver
{
    public partial class MainWindow : Window
    {
        FpsCounter m_counter = new FpsCounter();
        int m_port;

        public MainWindow()
        {
            InitializeComponent();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (_, __) =>
            {
                m_counter.CalculateFps();

                if (m_counter.Fps == 0)
                {
                    SetStartupImage("Failed");
                }
            };
            timer.Start();

            var args = Environment.GetCommandLineArgs();
            var port = int.Parse(args[1]);
            var x = int.Parse(args[2]);
            var y = int.Parse(args[3]);
            var width = int.Parse(args[4]);
            var height = int.Parse(args[5]);

            if (args.Length > 6)
            {
                Topmost = true;
            }


            Top = y;
            Left = x;
            Width = width;
            Height = height;
            m_port = port;

            Task.Factory.StartNew(RecieveProc);
        }

        private void RecieveProc()
        {
            using (var udpClient = new UdpClient(m_port))
            {
                var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                while (true)
                {
                    var receivedResults = udpClient.Receive(ref remoteEndPoint);

                    Dispatcher.InvokeAsync(() =>
                    {
                        img.Source = receivedResults.ToBitmapImage();
                        m_counter.GotFrame();
                    });
                }
            }
        }

        private void SetStartupImage(string text = "Startup")
        {
            var brush = System.Drawing.Brushes.Red;

            var blackImage = new System.Drawing.Bitmap(500, 500);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(blackImage))
            {
                g.FillRectangle(System.Drawing.Brushes.Black, 0, 0, blackImage.Width, blackImage.Height);

                System.Drawing.RectangleF rectf = new System.Drawing.RectangleF(4, 0, blackImage.Width - 8, blackImage.Height - 8);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawString("OFF", new System.Drawing.Font("Verdana", 60), System.Drawing.Brushes.White, rectf);
                rectf.Location = new System.Drawing.PointF(rectf.X, rectf.Y + 100);
                g.DrawString(text, new System.Drawing.Font("Verdana", 40), brush, rectf);
            }

            img.Source = blackImage.ToBitmapImage();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(img);
            var x = (pt.X / img.ActualWidth);
            var y = (pt.Y / img.ActualHeight);

            var bytes = ConcatByteArrays(
                BitConverter.GetBytes(m_port),
                BitConverter.GetBytes(x),
                BitConverter.GetBytes(y));

            UdpHelper.Send("192.168.0.2", 7777, bytes);
        }

        public static byte[] ConcatByteArrays(params byte[][] arrays)
        {
            return arrays.SelectMany(x => x).ToArray();
        }
    }
}
