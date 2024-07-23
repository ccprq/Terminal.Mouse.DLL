using System;
using System.Runtime.InteropServices;

namespace Terminal.Mouse
{
    public static class Mouse
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEvents);

        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        private const uint ENABLE_MOUSE_INPUT = 0x0010;

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public ushort EventType;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        private static IntPtr hConsoleHandle = GetStdHandle(STD_INPUT_HANDLE);

        static Mouse()
        {
            SetConsoleMode(hConsoleHandle, ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS);
        }
        public static int X()
        {
            var mouseEvent = GetMouseEvent();
            return mouseEvent.dwMousePosition.X;
        }

        public static int Y()
        {
            var mouseEvent = GetMouseEvent();
            return mouseEvent.dwMousePosition.Y;
        }

        public static bool IsLeftClick()
        {
            var mouseEvent = GetMouseEvent();
            return (mouseEvent.dwButtonState & 0x01) != 0;
        }

        public static bool IsRightClick()
        {
            var mouseEvent = GetMouseEvent();
            return (mouseEvent.dwButtonState & 0x02) != 0;
        }

        public static bool MouseHover(int x_start, int x_end, int y_start, int y_end)
        {
            int x = X();
            int y = Y();
            return (x >= x_start && x <= x_end && y >= y_start && y <= y_end);
        }
        private static MOUSE_EVENT_RECORD GetMouseEvent()
        {
            INPUT_RECORD[] records = new INPUT_RECORD[1];
            uint numberOfEvents;
            ReadConsoleInput(hConsoleHandle, records, 1, out numberOfEvents);

            if (numberOfEvents > 0 && records[0].EventType == 0x0002)
            {
                return records[0].MouseEvent;
            }

            return new MOUSE_EVENT_RECORD();
        }
    }
}
