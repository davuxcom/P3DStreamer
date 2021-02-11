using P3DStreamer.Interop;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace P3DStreamer.Extensions
{
    public static class WindowExtensions
    {
        public static void Cloak(this Window window, bool hide = true)
        {
            DwmApi.CloakWindow(new WindowInteropHelper(window).Handle, hide);
        }
    }
}
