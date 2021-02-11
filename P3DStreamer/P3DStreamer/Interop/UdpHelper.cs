using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace P3DStreamer.Interop
{
    public static class UdpHelper
    {
        public static void Send(string dstIp, int dstPort, byte[] data)
        {
            try
            {
                using (UdpClient c = new UdpClient(AddressFamily.InterNetwork))
                    c.SendAsync(data, data.Length, dstIp, dstPort);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(data.Length + " MX: " + ex.Message);
            }
        }
    }
}
