/**
 * Copyright 2018 Jean Pascal Bellot
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 **/

namespace XEyesWPF
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            //public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

            //public static implicit operator System.Drawing.Point(POINT p)
            //{
            //    return new System.Drawing.Point(p.X, p.Y);
            //}

            //public static implicit operator POINT(System.Drawing.Point p)
            //{
            //    return new POINT(p.X, p.Y);
            //}
        }

        internal static void Jiggle(int dx, int dy)
        {
            INPUT inp = new INPUT();
            inp.type = SendInputEventType.InputMouse;
            inp.mkhi.mi.dx = dx;
            inp.mkhi.mi.dy = dy;
            inp.mkhi.mi.mouseData = 0;
            inp.mkhi.mi.time = 0;
            inp.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE;
            inp.mkhi.mi.dwExtraInfo = IntPtr.Zero;
            if (SendInput(1, ref inp, Marshal.SizeOf(inp)) != 1)
            {
                //throw new Win32Exception();
            }
        }

        internal static void Click(int x, int y)
        {
            POINT oldPos = new POINT();
            if (GetCursorPos(out oldPos))
            {
                INPUT inp = new INPUT();
                inp.type = SendInputEventType.InputMouse;
                inp.mkhi.mi.dx = CalculateAbsoluteCoordinateX(x);
                inp.mkhi.mi.dy = CalculateAbsoluteCoordinateY(y);
                inp.mkhi.mi.mouseData = 0;
                inp.mkhi.mi.time = 0;
                inp.mkhi.mi.dwExtraInfo = IntPtr.Zero;

                inp.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE | MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
                SendInput(1, ref inp, Marshal.SizeOf(inp));
                inp.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_XDOWN;
                SendInput(1, ref inp, Marshal.SizeOf(inp));
                inp.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_XUP;
                SendInput(1, ref inp, Marshal.SizeOf(inp));
                SetCursorPos(oldPos.X, oldPos.Y);

                //inp.mkhi.mi.dx = CalculateAbsoluteCoordinateX(oldPos.X);
                //inp.mkhi.mi.dy = CalculateAbsoluteCoordinateX(oldPos.Y);
                //inp.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE | MouseEventFlags.MOUSEEVENTF_ABSOLUTE;
                //SendInput(1, ref inp, Marshal.SizeOf(inp));
            }
        }


        [DllImportAttribute("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImportAttribute("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReleaseCapture();
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        internal static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SystemMetric smIndex);
        [DllImport("user32.dll")]
        internal static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out POINT lpPoint);

        internal static int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetric.SM_CXSCREEN);
        }

        internal static int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetric.SM_CYSCREEN);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }
        [StructLayout(LayoutKind.Explicit)]
        internal struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)]
            public MouseInputData mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        internal struct MouseInputData
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public MouseEventFlags dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [Flags]
        internal enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }
        internal enum SendInputEventType : int
        {
            InputMouse,
            InputKeyboard,
            InputHardware
        }

        internal enum SystemMetric
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1,
        }

    }
}
