using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dxf_tools {
    public class AssemblyTools {

        [DllImport("kernel32.dll")]
        public static extern Int32 VirtualAllocEx(
        Int32 hprocess,
        Int32 lpaddress,
        Int32 dwsize,
        Int32 flallocationtype,
        Int32 flprotect
        );

        [DllImport("User32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("kernel32", EntryPoint = "CreateRemoteThread")]
        public static extern int CreateRemoteThread(
            int hprocess,
            int lpthreadattributes,
            int dwstacksize,
            int lpstartaddress,
            int lpparameter,
            int dwcreationflags,
            int lpthreadid
             );

        [DllImport("kernel32", EntryPoint = "VirtualProtectEx")]
        public static extern int VirtualProtectEx(
    int hprocess,
    int address,
    int size,
    int newprotect,
    int old
     );

        [DllImport("kernel32.dll")]
        public static extern Int32 WaitForSingleObject(
            Int32 hHandle,
            uint Second
            );

        [DllImport("kernel32.dll ")]
        public static extern bool CloseHandle(int hProcess);

        [DllImport("kernel32.dll ")]
        public static extern Int32 GetProcAddress(IntPtr hMouddle, string name);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandleA(string name);

        [DllImport("kernel32.dll")]
        public static extern int LoadLibary(string na);


        //私有变量
        public string Code = "";
        private Int32 mhProcess;
        private Int32 virtualAddr = 0;
        private IntPtr hModule = IntPtr.Zero;
        private Int32 CallProcAddress = 0;
        private Int32 GetWindowLongW = 0;
        private Int32 SetWindowLongW = 0;
        private Int32 SetTimer = 0;
        private Int32 KillTimer = 0;
        private Int32 MyGetModuleHandleA = 0;
        public Int32 PostMessage = 0;
        public Int32 GetTickCount = 0;
        public Int32 GetModuleHandleW = 0;
        public Int32 memcpy = 0;
        public Int32 SendMessage = 0;


        //常量

        public AssemblyTools(Int32 hProcess, Int32 Lenth , Action<string> writeLogLine) {
            if (Lenth == 0)
                Lenth = 0x1000;
            // gMrw = new MemRWer((uint)hProcess);
            mhProcess = hProcess;
            int twice = 0;

            while (virtualAddr == 0) {
                virtualAddr = VirtualAllocEx(hProcess, 0, 0x1000, 0x103000, 0x40);
                //virtualAddr = GetProcAddress(GetModuleHandleA("ntdll.dll"), "RtlFreeMemoryBlockLookaside");
                //writeLogLine("virtualAddr = " + Convert.ToString(virtualAddr, 16));
                Int32 eid = (Int32)GetLastError();

                if (virtualAddr == 0) {
                    if (eid == 8L) {
                        writeLogLine("由于客户端工作集内存不足，无法分配内存 重试第 " + twice++ + "次" + "期间最好进行选择角色等操作");
                        Thread.Sleep(500);
                    } else {
                        writeLogLine("289行 错误代码 : " + eid);
                        return;
                    }
                }
                if (twice >= 99) {
                    writeLogLine("等待次数过多 终止重试");
                    return;
                }
            }

            hModule = GetModuleHandleA("User32.dll");

            if (hModule == IntPtr.Zero)
                MessageBox.Show("303 行 句柄错误");
            CallProcAddress = GetProcAddress(hModule, "CallWindowProcW");
            GetWindowLongW = GetProcAddress(hModule, "GetWindowLongW");
            SetWindowLongW = GetProcAddress(hModule, "SetWindowLongW");
            SetTimer = GetProcAddress(hModule, "SetTimer");
            PostMessage = GetProcAddress(hModule, "PostMessageW");
            SendMessage = GetProcAddress(hModule, "SendMessageW");

            hModule = GetModuleHandleA("kernel32.dll");
            GetTickCount = GetProcAddress(hModule, "GetTickCount");
            writeLogLine("GetTickCount = " + PostMessage);

            hModule = GetModuleHandleA("ntdll.dll");
            memcpy = GetProcAddress(hModule, "memcpy");
            writeLogLine("memcpy = " + memcpy);

            //MyGetModuleHandleA = GetProcAddress(hModule, "KillTimer");

            hModule = GetModuleHandleA("Kernel32.dll");
            MyGetModuleHandleA = GetProcAddress(hModule, "GetModuleHandleA");
            GetModuleHandleW = GetProcAddress(hModule, "GetModuleHandleW");
            

            if (CallProcAddress == 0)
                MessageBox.Show("306 行 句柄错误");
        }
        public Int32 GetVirtualAddr() {
            return virtualAddr;
        }
        public void MessageBoxText() {
            clear();
            pushad();
            push_Int8(0);
            push_Int8(0);
            push_Int8(0);
            push_Int8(0);
            mov_eax(GetProcAddress(hModule, "MessageBoxA"));
            call_eax();
            popad();
            retn();
            RunRempteThreadWithMainThread();

        }

        public void asm_KillTimer() {
            Int32 hWnd = (Int32)FindWindow("地下城与勇士", "地下城与勇士");

            clear();
            pushad();
            push_Int32(0x100);
            push_Int32(hWnd);
            mov_eax(KillTimer);
            call_eax();
            popad();
            retn();

            RunRemoteThread();
        }

        public void RunRemoteThread() {
            Int32 m = 0;
            foreach (Byte a in Code)
                gMrw.writeInt8((virtualAddr + m++), a);
            Int32 hThread = CreateRemoteThread(gMrw.GetHandle(), 0, 0, CallProcAddress, virtualAddr, 0, 0);

            if (hThread == 0) {
                int eid = (Int32)GetLastError();
                if (eid == 8L)
                    MessageBox.Show("客户端进程预定的内存空间不足，无法创建远程线程。\r\n这可能是因为你的计算机可用内存不足\r\n请重启游戏后再试");
                else if (eid == 193L)
                    MessageBox.Show("远程线程的代码区不是可执行代码");
                else if (eid == 5L)
                    MessageBox.Show("访问被拒绝");
                else
                    MessageBox.Show("CreateRemoteThread Failed，Error code：" + eid);

            }

            WaitForSingleObject(hThread, 0xFFFFFFFF);
            CloseHandle(hThread);

            m = 0;
            foreach (Byte a in Code)
                gMrw.writeInt8((virtualAddr + m++), 0);
        }


        public void RunRempteThreadWithMainThread() {
            while (gMrw.readInt32(virtualAddr + 0x990) == virtualAddr) Thread.Sleep(0);
            Int32 m = 0;
            foreach (Byte a in Code)
                gMrw.writeInt8((virtualAddr + m++), a);
            gMrw.writeInt32(virtualAddr + 0x990, virtualAddr);
            while (gMrw.readInt32(virtualAddr + 0x990) == virtualAddr)
                Thread.Sleep(0);
            m = 0;
            foreach (Byte a in Code)
                gMrw.writeInt8((virtualAddr + m++), 0);
        }
        public void clear() {
            Code = "";
        }
        public void writeInt8(Byte value) {
            Code += (Char)value;
        }
        public void writeInt16(Int16 value) {
            Byte[] buffer = new Byte[2];
            IntPtr addr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            Marshal.WriteInt16(addr, value);
            Code += (Char)buffer[0];
            Code += (Char)buffer[1];
        }
        public void writeInt32(Int32 value) {
            Byte[] buffer = new Byte[4];
            IntPtr addr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            Marshal.WriteInt32(addr, value);
            Code += (Char)buffer[0];
            Code += (Char)buffer[1];
            Code += (Char)buffer[2];
            Code += (Char)buffer[3];
        }

        public void writeIntData(string value) {
            Code += value;
        }
        public void pushad() {
            writeInt8(0x60);
        }
        public void pushfd() {
            writeInt8(0x9C);
        }

        public void push_ebp_addx(byte value)
        {
            writeInt8(0xFF);
            writeInt8(0x75);
            writeInt8(value);
        }


        public void popfd() {
            writeInt8(0x9D);
        }


        public void mov_100100_eax() {
            writeInt8(0xA3);
            writeInt32(0x100100);
        }

        public void mov_addr_eax() {
            writeInt8(0xA3);
            writeInt32(0x100100);
        }

        public void mov_virtualaddr_c3() {
            writeInt8(0xC7);
            writeInt8(0x05);
            writeInt32(virtualAddr + 0x990);
            writeInt32(virtualAddr + 0x9A0);

        }


        public void mov_xxx_eax(Int32 address) {
            writeInt8(0x89);
            writeInt8(0x05);
            writeInt32(address);
        }

        public void popad() {
            writeInt8(0x61);
        }
        public void mov_eax(Int32 value) {
            writeInt8(0xB8);
            writeInt32(value);
        }
        public void cmp_esp(Int32 value)
        {
            writeInt8(0x81);
            writeInt8(0x3C);
            writeInt8(0x24);
            writeInt32(value);
        }
        public void jnz_Int8(byte value)
        {
            writeInt8(0x75);
            writeInt8(value);
        }
        public void je_Int8(byte value)
        {
            writeInt8(0x74);
            writeInt8(value);

        }

        public void mov_eax_edi()
        {
            writeInt8(0x8B);
            writeInt8(0xC7);
        }

        public void call_addr(Int32 addr) {
            writeInt8(0xFF);
            writeInt8(0x15);

            writeInt32(addr);
        }

        public void mov_ebx(Int32 value) {
            writeInt8(0xBB);
            writeInt32(value);
        }
        public void mov_ecx(Int32 value) {
            writeInt8(0xB9);
            writeInt32(value);
        }
        public void push_ebp() {
            writeInt8(0x55);
        }

        public void mov_ebp_esp() {
            writeInt8(0x8B);
            writeInt8(0xEC);

        }

        public void mov_esp_ebp() {
            writeInt8(0x8B);
            writeInt8(0xE5);

        }

        public void sub_esp(byte value) {
            writeInt8(0x83);
            writeInt8(0xEC);
            writeInt8(value);
        }

        public void mov_edx(Int32 value) {
            writeInt8(0xBA);
            writeInt32(value);
        }
        public void mov_ebp(Int32 value) {
            writeInt8(0xBD);
            writeInt32(value);
        }
        public void mov_esp(Int32 value) {
            writeInt8(0xB8);
            writeInt32(value);
        }
        public void mov_edi(Int32 value) {
            writeInt8(0xBF);
            writeInt32(value);
        }
        public void mov_esi(Int32 value) {
            writeInt8(0xBE);
            writeInt32(value);
        }
        public void mov_eax_ptr_esi() {
            writeInt8(0x8B);
            writeInt8(0x06);
        }
        public void mov_eax_edx() {
            writeInt8(0x8B);
            writeInt8(0xC2);
        }
        public void call_eax() {
            writeInt8(0xFF);
            writeInt8(0xD0);
        }
        public void call_ebx() {
            writeInt8(0xFF);
            writeInt8(0xD3);
        }
        public void call_edx() {
            writeInt8(0xFF);
            writeInt8(0xD2);
        }
        public void call_edi() {
            writeInt8(0xFF);
            writeInt8(0xD7);
        }
        public void mov_ecx_ptr_ecx() {
            writeInt8(0x8B);
            writeInt8(0x09);
        }
        public void lea_esi_edi_addx(Int32 value) {
            writeInt8(0x8D);
            writeInt8(0xB7);
            writeInt32(value);
        }
        public void lea_eax_edi_addx(Int32 value) {
            writeInt8(0x8D);
            writeInt8(0x87);
            writeInt32(value);
        }
        public void lea_edx_ebp_4() {

            writeInt8(0x8D);
            writeInt8(0x55);
            writeInt8(0xFC);

        }
        public void mov_ptr_edx(Int32 value) {
            writeInt8(0xC7);
            writeInt8(0x02);
            writeInt32(value);
        }
        public void mov_edx_ptr_ecx() {
            writeInt8(0x8B);
            writeInt8(0x11);
        }

        public void push_eax() {
            writeInt8(0x50);
        }
        public void push_ebx() {
            writeInt8(0x53);
        }
        public void push_ecx() {
            writeInt8(0x51);
        }
        public void push_edx() {
            writeInt8(0x52);
        }

        public void push_esi() {
            writeInt8(0x56);
        }
        public void push_edi() {
            writeInt8(0x57);
        }
        public void retn() {
            writeInt8(0xC3);
        }

        public void add_esp(Byte value) {
            writeInt8(0x83);
            writeInt8(0xC4);
            writeInt8(value);
        }

        public void push_Int8(Byte value) {
            writeInt8(0x6A);
            writeInt8(value);
        }

        public void push_Int32(Int32 value) {
            writeInt8(0x68);
            writeInt32(value);
        }

        public void mov_edi_eax() {
            writeInt8(0x8B);
            writeInt8(0xF8);
        }

        public void mov_eax_ptr_eax() {
            writeInt8(0x8B);
            writeInt8(0x00);
        }
    }

}
