using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace P3DStreamer.Interop
{
    struct POINT { public int x; public int y; }

    class User32
    {
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public static uint MF_BYPOSITION = 0x400;
        public static uint MF_REMOVE = 0x1000;

        public static int GWL_STYLE = -16;
        public static int GWL_EXSTYLE = -20;

        public static uint WS_CHILD = 0x40000000;
        public static uint WS_BORDER = 0x00800000;
        public static uint WS_DLGFRAME = 0x00400000;
        public static uint WS_CAPTION = WS_BORDER | WS_DLGFRAME;
        public static uint WS_SYSMENU = 0x00080000;
        public static uint WS_VISIBLE = 0x10000000;
        public static uint WS_CLIPSIBLINGS = 0x04000000;
        public static uint WS_POPUP = 0x80000000;

        public const short SWP_NOMOVE = 0X2;
        public const short SWP_NOSIZE = 1;
        public const short SWP_NOZORDER = 0X4;
        public const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hwnd, uint cmd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT pt);
        [DllImport("user32.dll")]
        public static extern int ShowCursor([MarshalAs(UnmanagedType.Bool)] bool fShow);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("USER32.DLL")]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string className, string caption);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow_ByCaption(IntPtr zero, string captionTitle);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string captionTitle);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        public static void LeftMouseClick(int xpos, int ypos)
        {
            POINT savedCursorPos;
            GetCursorPos(out savedCursorPos);

            SetCursorPos(xpos, ypos);

            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            Thread.Sleep(50);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
            Thread.Sleep(50);

            SetCursorPos(savedCursorPos.x, savedCursorPos.y);
        }
    }
}
