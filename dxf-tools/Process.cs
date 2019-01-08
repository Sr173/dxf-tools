using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dxf_tools
{
    class Process
    {
        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll")]
        public static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out int id);
        [DllImport("kernel32.dll")]
        public static extern Int32 OpenProcess(Int32 dwAccessFlag, bool handle, Int32 dwProcessID);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);
        static public Byte[] temp = new Byte[0x10000000];

        [DllImport("Dll.dll")]
        public static extern int promoteHandle(int handle, int access);
        [DllImport("Dll.dll")]
        public static extern int unlinkHandleTable();

        static public Int32 OpenProcessByWindowName(string lpClassName, string lpWindowName)
        {


            IntPtr hWnd = FindWindow(lpClassName, lpWindowName);

            if (hWnd == IntPtr.Zero)
                MessageBox.Show("找不到窗口，打开进程失败");

            if (hWnd == IntPtr.Zero)
                return -1;
            Int32 ID;
            GetWindowThreadProcessId(hWnd, out ID);
            Int32 handle = OpenProcess(0x1, false, ID);
            promoteHandle(handle, 0x7FFFFFFF);
            unlinkHandleTable();
            return handle;
        }

    }
}
