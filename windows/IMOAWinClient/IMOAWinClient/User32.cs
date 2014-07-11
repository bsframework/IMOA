using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IMOAWinClient
{
    class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public enum Enu_SystemParametersInfo_Action
        {
            SPI_GETWORKAREA = 0x0030
        }

        [DllImport("User32.dll")]
        public static extern bool GetCursorPos(ref POINT lpPoint);

        [DllImport("User32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref RECT lpRect, uint fWinIni);

        [DllImport("User32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);
    }
}

