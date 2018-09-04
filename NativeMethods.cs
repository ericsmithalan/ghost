using System;
using System.Runtime.InteropServices;

namespace Ghost
{
    internal class NativeMethods
    {
        public const int CONNECTIONCENTER_BROADCAST = 0xffff;
        public static readonly int CC = RegisterWindowMessage("Ghost");

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
    }
}