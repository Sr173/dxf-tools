﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32;

namespace dxf_tools
{
    class gMrw
    {
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(uint hProcess, uint lpBaseAddr, IntPtr lpBuffer, uint size, uint lpNumber);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(uint hProcess, uint lpBaseAddr, float[] lpBuffer, uint size, uint lpNumber);
        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory", SetLastError = true)]
        static extern bool WriteProcessMemory(uint hProcess, uint lpBaseAddr, IntPtr lpBuffer, uint size, uint lpNumber);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(uint hProcess, uint lpBaseAddr, float[] lpBuffer, uint size, uint lpNumber);

        static public uint mhProcess;

        static public byte readInt8(uint lpBaseAddr)
        {
            Byte result = 0;
            Byte[] buffer = new Byte[1];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            ReadProcessMemory(mhProcess, lpBaseAddr, vBytesAddress, 1, 0);
            result = Marshal.ReadByte(vBytesAddress);
            return result;
        }
        static public Int16 readInt16(int lpBaseAddr)
        {
            Int16 result = 0;
            Byte[] buffer = new Byte[2];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            ReadProcessMemory(mhProcess, (uint)lpBaseAddr, vBytesAddress, 2, 0);
            result = Marshal.ReadInt16(vBytesAddress);
            return result;
        }
        static public Int32 readInt32(int lpBaseAddr)
        {
            Int32 result = 0;
            Byte[] buffer = new Byte[4];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            ReadProcessMemory(mhProcess, (uint)lpBaseAddr, vBytesAddress, 4, 0);
            result = Marshal.ReadInt32(vBytesAddress);
            return result;
        }
        static public float readFloat(int lpBaseAddr)
        {
            float[] buffer = new float[] { 0 };
            ReadProcessMemory(mhProcess, (uint)lpBaseAddr, buffer, 4, 0);
            return buffer[0];
        }
        static public Int64 readInt64(int lpBaseAddr)
        {
            Int64 result = 0;
            Byte[] buffer = new Byte[8];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            ReadProcessMemory(mhProcess, (uint)lpBaseAddr, vBytesAddress, 8, 0);
            result = Marshal.ReadInt64(vBytesAddress);
            return result;
        }
        static public Int32 readInt32(int lpBaseAddr, params int[] ofe)
        {
            int result = readInt32(lpBaseAddr);

            foreach (Int32 offset in ofe)
            {
                result = readInt32(result + offset);
            }
            return result;
        }
        static public byte[] readData(uint lpBaseAddr, uint lenth, byte[] temp = null)
        {

            byte[] result;
            if (temp == null)
                result = new byte[lenth];
            else
                result = temp;
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(result, 0);
            ReadProcessMemory(mhProcess, lpBaseAddr, vBytesAddress, lenth, 0);
            return result;
        }
        static public void writeInt8(int lpBaseAddr, Byte Data)
        {
            byte[] buffer = new Byte[1];
            buffer[0] = Data;
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            WriteProcessMemory(mhProcess, (uint)lpBaseAddr, vBytesAddress, 1, 0);
        }
        static public void writeInt64(int lpBaseAddr, Int64 Data)
        {
            Byte[] buffer = new Byte[8];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            Marshal.WriteInt64(vBytesAddress, Data);
            WriteProcessMemory(mhProcess, (uint)lpBaseAddr, vBytesAddress, 8, 0);
        }
        static public void writedData(uint lpBaseAddr, Byte[] Data, uint lenth)
        {
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);
            WriteProcessMemory(mhProcess, lpBaseAddr, vBytesAddress, lenth, 0);
        }

        static public void writeString(int addr, string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                writeInt16(addr + i * 2, (short)s[i]);
            }
            writeInt16(addr + s.Length * 2, 0);
        }
        static public bool writeInt32(int lpBaseAddr, Int32 Data, bool IsCheckProtect = false)
        {
            Byte[] buffer = new Byte[4];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            Marshal.WriteInt32(vBytesAddress, Data);
            int oldProtect = 0;
            if (IsCheckProtect)
                Win32.Kernel.VirtualProtectEx((IntPtr)mhProcess, (IntPtr)lpBaseAddr, 4, 4, ref oldProtect);
            bool result = WriteProcessMemory(mhProcess, (uint)lpBaseAddr, vBytesAddress, 4, 0);
            if (IsCheckProtect)
                Win32.Kernel.VirtualProtectEx((IntPtr)mhProcess, (IntPtr)lpBaseAddr, 4, oldProtect, ref oldProtect);

            return result;

        }
        static public void writeInt16(int lpBaseAddr, Int16 Data)
        {
            Byte[] buffer = new Byte[2];
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            Marshal.WriteInt16(vBytesAddress, Data);
            WriteProcessMemory(mhProcess, (uint)lpBaseAddr, vBytesAddress, 2, 0);
        }
        static public int GetHandle()
        {
            return (int)mhProcess;
        }
        static public void writeFloat(int lpBaseAddr, float value)
        {
            WriteProcessMemory(mhProcess, (uint)lpBaseAddr, new float[] { value }, 4, 0);
        }
        static public string readString(int LPWCHAR)
        {
            string result = "";
            for (int i = 0; i < 200; i++)
            {
                Char t = (Char)readInt16(LPWCHAR + i * 2);
                if (t == 0)
                    break;
                result += t;
            }
            return result;
        }
        static public Int32 Decryption(Int32 address)
        {
            Int32 a = readInt32(address);
            if (a == -1)
                return -1;
            Int32 b = readInt32(readInt32(baseAddr.dwBase_Decryption) + (a >> 16) * 4 + 36);
            if (b == -1)
                return -1;
            a = readInt32(((a & 0xFFFF) * 4) + b + 8468);
            if (a == -1)
                return -1;
            a &= 0xFFFF;

            return ((a << 16) | a) ^ readInt32(address + 4);
        }
        static public void Encryption1(Int32 addr, Int32 value)
        {
            Int32 ecx, eax, esi, edx, over;
            eax = readInt32(baseAddr.dwBase_Encryption);
            eax = eax + 1;
            writeInt32(baseAddr.dwBase_Encryption, eax);
            edx = (eax >> 8) & 255;
            ecx = readInt32(baseAddr.dwBase_Encryption_Param1 + edx * 2) & 65535;
            eax = eax & 255;
            eax = readInt32(baseAddr.dwBase_Encryption_Param2 + eax * 2) & 65535;
            eax = (ecx ^ eax) & 65535;
            over = addr & 15;
            if (over == 0)
            {
                ecx = value >> 16;
                ecx = ecx - eax;
                ecx += value & 65535;
            }
            else if (over == 4)
            {
                ecx = value & 65535;
                ecx = ecx - value >> 16;
            }
            else if (over == 8)
            {
                ecx = value >> 16;
                ecx = ecx * value & 65535;
            }
            else if (over == 12)
            {
                ecx = value >> 16;
                ecx = ecx + value & 65535;
                ecx = ecx + eax;
            }
            else
            {
                ecx = value >> 16;
                ecx = ecx + value & 65535;
            }

            esi = (ecx ^ eax) & 65535;
            ecx = eax;

            eax = (eax << 16) | ecx;
            eax = eax ^ value;
            writeInt32(addr + 4, eax);
            eax = readInt32(addr);
            edx = esi << 16;
            esi = readInt32(baseAddr.dwBase_Decryption);
            edx = edx | ecx;
            ecx = eax >> 16;
            ecx = readInt32(esi + ecx * 4 + 36);
            eax = eax & 65535;
            writeInt32(ecx + eax * 4 + 8468, edx);
        }
        static public void writeDnfType(Int32 lpBaseAddress, Int32 addr, Int32 types)
        {
            Byte[] a = new Byte[12];
            Byte[] temp;
            temp = BitConverter.GetBytes(addr);
            for (int i = 0; i < 4; i++)
            {
                a[i] = temp[i];
            }
            temp = BitConverter.GetBytes(addr + types);

            for (int i = 4; i < 12; i++)
            {
                a[i] = temp[i % 4];
            }
            IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(a, 0);
            WriteProcessMemory((uint)lpBaseAddress, (uint)addr, vBytesAddress, 12, 0);
        }

        static public void EncryptionCall(int addr, int data, bool IsMainThread = false)
        {
            int eax = addr & 0xF;
            int ebp_c = Form1.at.GetVirtualAddr() + 0xD00;
            gMrw.writeInt32(ebp_c, data);
            Form1.at.clear();
            Form1.at.pushad();
            Form1.at.mov_ecx(addr);
            Form1.at.push_Int32(addr + 4);
            Form1.at.push_Int32(ebp_c);
            if (eax == 0)
                eax = baseAddr.dwCall_Encryption1;
            else if (eax == 4)
                eax = baseAddr.dwCall_Encryption2;
            else if (eax == 8)
                eax = baseAddr.dwCall_Encryption3;
            else if (eax == 0xC)
                eax = baseAddr.dwCall_Encryption4;
            else
                eax = baseAddr.dwCall_Encryption5;
            Form1. at.mov_eax(eax);
            Form1.at.call_eax();
            if (IsMainThread)
                Form1.at.mov_virtualaddr_c3();
            Form1.at.popad();
            Form1.at.retn();
            if (IsMainThread)
                Form1.at.RunRempteThreadWithMainThread();
            else
                Form1.at.RunRemoteThread();
        }

        static public void Encryption(Int32 addr, Int32 value)
        {


            Int32 ID = readInt32(addr);

            Int32 para = readInt32(readInt32(baseAddr.dwBase_Decryption) + (ID >> 16) * 4 + 36);
            Int32 Paddress = para + (65535 & ID) * 4 + 8468;
            para = readInt32(Paddress);
            Int32 si = 0;

            Int32 data = (para & 65535);
            data = data + (data << 16);
            Int16 ax = (Int16)(para & 65535);

            Int32 addr_over = (addr & 15);
            if (addr_over == 0)
            {
                si = (data >> 16);
                si = si - ax;
                si += value;
            }

            if (addr_over == 4)
            {
                si = ((value & 65535) - (value >> 16));
            }

            if (addr_over == 8)
            {
                si = (value >> 16);
                si = si * value;
            }

            if (addr_over == 12)
            {
                si = (value >> 16);
                si = si + value;
                si = si + ax;
            }

            ax = (Int16)(si ^ ax);
            writeInt16(Paddress + 2, ax);
            writeInt32(addr + 4, (data ^ value));
        }

        static public Int32 GetCallAddress(Int32 addr)
        {
            return readInt32(addr + 0x1) + addr + 5;
        }

        static public void HookJmp(Int32 addr,Int32 newAddr) {
            Int32 code = newAddr - addr - 5;
            writeInt8(addr, 0xE9);
            writeInt32(addr + 1, code);
        }

        static public Int32 SearchSignature(Byte[] code, string szMask, int dwStartAddress, int ID)
        {

            int dwResult = 0;
            int lpAddress = dwStartAddress;
            int dwBestResult = 0;
            MEMORY_BASIC_INFORMATION lpBuffer = new MEMORY_BASIC_INFORMATION();
            Byte[] temp = Process.temp;

            while (Win32.Kernel.VirtualQueryEx(mhProcess, (uint)lpAddress, ref lpBuffer, System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))) != 0)
            {

                if (lpBuffer.Protect != 1)
                {
                    readData((uint)lpBuffer.BaseAddress, (uint)lpBuffer.RegionSize, temp);
                    IntPtr vBytesAddress = Marshal.UnsafeAddrOfPinnedArrayElement(temp, 0);

                    for (int i = 0; i < lpBuffer.RegionSize - szMask.Length; i++)
                    {
                        IntPtr t = vBytesAddress + i;
                        bool flag = true;
                        for (int j = 0; j < szMask.Length; j++)
                        {
                            if (szMask[j] != 'x')
                                continue;
                            if ( Marshal.ReadByte(t + j) != code[j])
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag == true)
                        {
                            return lpAddress + i;
                        }
                    }

                    if (dwResult > dwBestResult)
                    {
                        dwBestResult = dwResult;
                        break;
                    }
                    lpAddress = lpAddress + lpBuffer.RegionSize;
                    if (lpAddress > 0x6000000)
                        return 0;
                }
            }
            return dwBestResult;
        }
    }

    class test
    {
        static public int gg()
        {
            return 1;
        }
        public test()
        {
            MessageBox.Show("构造");
        }

        ~test()
        {
            MessageBox.Show("析构");
        }
    }
}
