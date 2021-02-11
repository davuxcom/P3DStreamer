using System;
using System.Runtime.InteropServices;

namespace P3DStreamer.Interop
{
    class DwmApi
    {
        internal const int DWMA_CLOAK = 13;

        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(
            IntPtr hwnd,
            int attr,
            ref int attrValue,
            int attrSize);

        public static void CloakWindow(IntPtr handle, bool hide = true)
        {
            int attributeValue = hide ? 1 : 0;
            try
            {
                DwmApi.DwmSetWindowAttribute(handle, DwmApi.DWMA_CLOAK, ref attributeValue, Marshal.SizeOf(attributeValue));
            }
            catch (Exception) { } // access denied
        }
    }
}
