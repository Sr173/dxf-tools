using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

//10195613    FF15 44B33310   call dword ptr ds:[<&KERNEL32.GetTickCou>; kernel32.GetTickCount

namespace dxf_tools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int WM_START = 0x1001;
        const int WM_ADD = 0x1002;
        const int WM_END = 0x1003;
        const int WM_GETPACKAGE = 0x1004;
        const int WM_STRING = 0x1005;

        const int WM_44_START = 0x1006;
        const int WM_44_ADD = 0x1007;
        const int WM_44_END = 0x1008;

        bool s = false;
        Int32 lenth = 0;

        protected override void WndProc(ref Message m) {
            try {
                //按快捷键 

                switch (m.Msg) {
                    case WM_START: {
                            bool flag = true;

                            string data = textBox10.Text;
                            if (data != "") {
                                string[] s_data = data.Split(' ');
                                Int32[] i_data = new Int32[s_data.Length];
                                for (int i = 0; i < i_data.Length; i++)
                                    i_data[i] = Convert.ToInt32(s_data[i], 16);
                                if (comboBox1.SelectedIndex == 0) {
                                    flag = true;
                                    foreach(Int32 d in i_data) {
                                        if (d == (Int32)m.LParam)
                                            flag = false;
                                    }
                                } else {
                                    flag = false;
                                    foreach (Int32 d in i_data) {
                                        if (d == (Int32)m.LParam)
                                            flag = true;
                                    }
                                }
                            }
                            if (flag) {
                                textBox9.AppendText("------包结构开始-------" + "\r\n");
                                textBox9.AppendText("包头id:0x" + Convert.ToString((Int32)m.LParam, 16) + ",返回地址:0x" + Convert.ToString((Int32)m.WParam, 16) + "\r\n");
                                s = true;
                                lenth = 0;
                            }
                            break;
                        }
                    case WM_ADD: {
                            if (s) {
                                switch ((Int32)m.WParam) {
                                    case 1: {
                                            lenth += 1;
                                            textBox9.AppendText("      Int8:" + m.LParam + "\r\n");
                                            break;
                                        }
                                    case 2: {
                                            lenth += 2;
                                            textBox9.AppendText("      Int16:" + m.LParam + "\r\n");
                                            break;
                                        }
                                    case 4: {
                                            lenth += 4;
                                            textBox9.AppendText("      Int32:" + m.LParam + "\r\n");
                                            break;
                                        }
                                    default: {
                                            lenth += 8;
                                            textBox9.AppendText("      Int64:(" + m.LParam + "," + m.WParam + ")" + "\r\n");
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case WM_STRING:
                        {
                            for (int i = 0; i < 0x30;i += 4) {
                                textBox9.AppendText("      String:" + Convert.ToString(gMrw.readInt32((int)m.LParam + i),16) + "\r\n");
                            }
                            break;
                        }
                    case WM_END: {
                            if (s) {
                                textBox9.AppendText("包长:" + lenth + ",包尾返回地址:0x" + Convert.ToString((Int32)m.LParam, 16)  + "\r\n");
                                textBox9.AppendText("------包结构结束-------" + "\r\n\r\n");
                                s = false;
                            }
                            break;
                        }

                    case WM_44_START:
                        {
                            bool flag = true;

                            string data = textBox13.Text;
                            if (data != "")
                            {
                                string[] s_data = data.Split(' ');
                                Int32[] i_data = new Int32[s_data.Length];
                                for (int i = 0; i < i_data.Length; i++)
                                    i_data[i] = Convert.ToInt32(s_data[i], 16);
                                if (comboBox2.SelectedIndex == 0)
                                {
                                    flag = true;
                                    foreach (Int32 d in i_data)
                                    {
                                        if (d == (Int32)m.LParam)
                                            flag = false;
                                    }
                                }
                                else
                                {
                                    flag = false;
                                    foreach (Int32 d in i_data)
                                    {
                                        if (d == (Int32)m.LParam)
                                            flag = true;
                                    }
                                }
                            }
                            if (flag)
                            {
                                textBox12.AppendText("------包结构开始-------" + "\r\n");
                                textBox12.AppendText("包头id:0x" + Convert.ToString((Int32)m.LParam, 16) + ",返回地址:0x" + Convert.ToString((Int32)m.WParam, 16) + "\r\n");
                                s = true;
                                lenth = 0;
                            }
                            break;
                        }
                    case WM_44_ADD:
                        {
                            if (s)
                            {
                                switch ((Int32)m.WParam)
                                {
                                    case 1:
                                        {
                                            lenth += 1;
                                            textBox12.AppendText("      Int8:" + m.LParam + "\r\n");
                                            break;
                                        }
                                    case 2:
                                        {
                                            lenth += 2;
                                            textBox12.AppendText("      Int16:" + m.LParam + "\r\n");
                                            break;
                                        }
                                    case 4:
                                        {
                                            lenth += 4;
                                            textBox12.AppendText("      Int32:" + m.LParam + "\r\n");
                                            break;
                                        }
                                    case 8:
                                        {
                                            lenth += 8;
                                            textBox12.AppendText("      Int64:(" + gMrw.readInt64((int)m.LParam) + ")" + "\r\n");
                                            break;
                                        }
                                    case 0xC:
                                        {
                                            lenth += 4;
                                            textBox12.AppendText("      Int32(1):" + m.LParam + "\r\n");
                                            break;
                                        }
                                    default:
                                        {
                                            lenth += (int)m.WParam;
                                            textBox12.AppendText("      IntXX:(" + m.LParam + "," + m.WParam + ")" + "\r\n");
                                            break;
                                        }
                                }
                            }
                            break;
                        }
                    case WM_44_END:
                        {
                            if (s)
                            {
                                textBox12.AppendText("包长:" + lenth + ",包尾返回地址:0x" + Convert.ToString((Int32)m.LParam, 16) + "\r\n");
                                textBox12.AppendText("------包结构结束-------" + "\r\n\r\n");
                                s = false;
                            }
                            break;
                        }
                    case WM_GETPACKAGE:
                        {
                            //work((int)m.LParam);
                            work(gMrw.readInt32(0x57F9AD8));
                            //57F9AD0
                            break;
                        }
                }

                base.WndProc(ref m);
            } catch {

            }
        }

        public static AssemblyTools at;
        int GameRpcs = 0;


        private void button1_Click(object sender, EventArgs e)
        {
            gMrw.mhProcess = (uint)Process.OpenProcessByWindowName("地下城与勇士", "地下城与勇士");
            at = new AssemblyTools((int)gMrw.mhProcess, 0, writeLogLine);

            at.clear();
            at.pushad();
            gMrw.writeString(at.GetVirtualAddr() + 0xE00, "GameRpcs.dll");
            at.push_Int32(at.GetVirtualAddr() + 0xE00);
            at.mov_eax(at.GetModuleHandleW);
            at.call_eax();
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xE00);
            at.popad();
            at.retn();

            at.RunRemoteThread();
            Thread.Sleep(1000);
            GameRpcs = gMrw.readInt32(at.GetVirtualAddr() + 0xE00);
            writeLogLine("GameRpcs = 0x" + Convert.ToString(GameRpcs, 16));
            writeLogLine("VirtualAddr = 0x" + Convert.ToString(at.GetVirtualAddr(), 16));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int base_dnf = 0x401000;
            int result = 0;
            int result_1;
            
            //人物基址
            result = gMrw.SearchSignature(new byte[] { 0x56, 0x33, 0xF6, 0x89, 0x4D, 0xF4, 0x89, 0x45, 0xF8, 0x3B, 0xC6, 0x0F, 0x84, 0x9E, 0x00, 0x00, 0x00 }, "xxxxxxxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Character = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("人物基址 = " + result + "*" + "\r\n");

            //商店基址
            result = gMrw.SearchSignature(new byte[] { 0x53, 0x85, 0xC9, 0x0F, 0x84, 0x81, 0x00, 0x00, 0x00 }, "xxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0x4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Shop = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("商店基址 = " + result + "*" + "\r\n");

            //解密基址
            result = gMrw.SearchSignature(new byte[] { 0x85, 0xC0, 0x75, 0x2B, 0x68, 0x60, 0x08, 0x00, 0x00 }, "xxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0x4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Decryption = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("解密基址 = " + result + "*" + "\r\n");


            //背包基址
            result = gMrw.SearchSignature(new byte[] {0x68,0xCC ,0xD8 ,0x34 ,0x1D}, "xxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0x4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Bag = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("背包基址 = " + result + "*" + "\r\n");

            //任务基址
            result = gMrw.SearchSignature(new byte[] { 0x8B, 0x0D, 0x88, 0x3C, 0x1D, 0x04, 0x6A, 0x01, 0x6A, 0x01, 0x6A, 0x00, 0x6A, 0x00, 0x6A, 0x05 }, "xx????xxxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result + 0x2);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Quest = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("任务基址 = " + result + "*" + "\r\n");

            //评分基址 call
            result = gMrw.SearchSignature(new byte[] { 0x50, 0x6A, 0x06, 0x6A, 0xFF }, "xxxxx", base_dnf, 0);
            result_1 = result;
            result = gMrw.readInt32(result - 0x4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_SSS = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("评分基址 = " + result + "*" + "\r\n");
            result = gMrw.GetCallAddress(result_1 + 5);
            TB_Base_Cs.AppendText("public static Int32 dwCall_SSS = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("评分call = " + result + "*" + "\r\n");

            //制裁基址
            result = gMrw.SearchSignature(new byte[] { 0x6A, 0x00, 0x68, 0x98, 0xEB, 0x00, 0x00, 0xE9 }, "xxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result + 0xD);
            TB_Base_Boss.AppendText("制裁基址 = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            //卖修基址
            result = gMrw.SearchSignature(new byte[] { 0x75, 0x05, 0xB8, 0x67, 0x00, 0x00, 0x00, 0xC3, 0xCC }, "xxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0xF);
            TB_Base_Boss.AppendText("卖修基址 = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            //时间基址
            TB_Base_Cs.AppendText("public static Int32 dwBase_Time = 0x" + Convert.ToString(0x20A028, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("时间基址 = " + Convert.ToString(0x20A028, 16).ToUpper() + "\r\n");


            //挑战基址
            result = gMrw.SearchSignature(new byte[] { 0x8B,0xF8,0x6A,0x01,0x6A,0x05,0x89,0xBD }, "xxxxxxxx", base_dnf, 0);
            result = gMrw.GetCallAddress(result - 0x5);
            result = gMrw.readInt32(result + 0x36) + 0x2EC;
            TB_Base_Cs.AppendText("public static Int32 dwBase_TiaoZhan = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("挑战基址 = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            ////分解call
            //result = gMrw.SearchSignature(new byte[] { 0x53, 0x56, 0x57, 0x8B, 0xF1, 0x6A, 0x02, 0x89, 0x75, 0xE0, 0x89, 0x45, 0xDC }, "xxxxxxxxxxxxx", base_dnf, 0);
            //result = result - 0x13;
            //TB_Base_Boss.AppendText("分解call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            //无敌call
            result = gMrw.SearchSignature(new byte[] { 0x5F,0x75,0x39,0x6A,0xFF,0x6A,0x00,0x6A,0x01,0x6A,0x00,0x8B,0xCE }, "xxxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.GetCallAddress(result + 0xD);
            TB_Base_Boss.AppendText("无敌call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            ////城镇call
            //result = gMrw.SearchSignature(new byte[] { 0x55, 0x8B, 0xEC, 0x53, 0x56, 0x57, 0x6A, 0x00, 0x6A, 0x24 }, "xxxxxxxxxx", base_dnf, 0);
            //result = result + 0;
            //TB_Base_Boss.AppendText("城镇call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            ////完成call 以及发包一套
            result = gMrw.SearchSignature(new byte[] { 0x0F, 0x8C, 0xCA, 0x00, 0x00, 0x00, 0x6A, 0x48 }, "xxxxxxxx", base_dnf, 0);//0053C6D0    55              push ebp

            //result = result - 8;
            result_1 = result;
            //TB_Base_Boss.AppendText("完成call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            result = gMrw.GetCallAddress(result_1 + 8);
            result = gMrw.readInt32(result + 1);
            TB_Base_Cs.AppendText("public static Int32 dwBase_SEND = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("发包基址 = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            result = gMrw.GetCallAddress(result_1 + 0x16 - 7);
            TB_Base_Cs.AppendText("public static Int32 dwCall_HANDLE = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("缓冲call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            result = gMrw.GetCallAddress(result_1 + 0x24 + 7);
            TB_Base_Cs.AppendText("public static Int32 dwCall_ADD = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("加密包call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            result = gMrw.GetCallAddress(result + 0x1E);
            TB_Base_Boss.AppendText("密钥call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            result = gMrw.GetCallAddress(result_1 + 0x56);
            TB_Base_Cs.AppendText("public static Int32 dwCall_SEND = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Boss.AppendText("发包call = " + Convert.ToString(result, 16).ToUpper() + "\r\n");

            //加密全套
            result = gMrw.SearchSignature(new byte[] {0x8B,0x45,0x08,0x66,0x8B,0x00,0x0F,0xB7,0xD2,0x66,0x33,0xC2,0x56,0x57}, "xxxxxxxxxxxxxx", base_dnf, 0);
            result_1 = result;
            result = gMrw.readInt32(result_1 - 0x1D + 1);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Encryption = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            result = gMrw.readInt32(result_1 - 0x15 + 4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Encryption_Param1 = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            result = gMrw.readInt32(result_1 - 0x8 + 4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Encryption_Param2 = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");



            //角色基址
            result = gMrw.SearchSignature(new byte[] { 0x8B,0xC3,0xEB,0x94 }, "xxxx", base_dnf, 0);
            result = gMrw.readInt32(result + 0x6F + 1);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Role = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            //最大当前pl
            result = gMrw.SearchSignature(new byte[] { 0x75, 0x37, 0x6A, 0x00, 0x6A, 0xFF, 0x6A, 0xFF, 0x6A, 0x00, 0x6A, 0x00, 0x6A, 0xFF }, "xxxxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.GetCallAddress(result + 0x34);
            result = gMrw.readInt32(result + 4 + 1);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Max_Pl = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Cs.AppendText("public static Int32 dwBase_Cur_Pl = 0x" + Convert.ToString(result - 8, 16).ToUpper() + ";\r\n");

            //鼠标基址
            result = gMrw.SearchSignature(new byte[] { 0x85, 0xC9, 0x75, 0x03, 0x32, 0xC0, 0xC3, 0x8B, 0x01, 0x8B, 0x10 }, "xxxxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Mouse = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            //剧情编号
            result = gMrw.SearchSignature(new byte[] { 0x84, 0xDB, 0x74, 0x0B, 0x84, 0xC0, 0x74, 0x07, 0xB8, 0x01, 0x00, 0x00, 0x00, 0xEB, 0x02}, "xxxxxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.GetCallAddress(result - 0xC);
            result = gMrw.readInt32(result + 0x3C + 1) + 0x34;
            TB_Base_Cs.AppendText("public static Int32 dwBase_Quest_Instace_Id = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            //角色等级
            result = gMrw.SearchSignature(new byte[] { 0x8B, 0x56, 0x0C, 0x8B, 0x46, 0x08, 0x51, 0x8B, 0x4E, 0x04, 0x52, 0x50, 0x51 }, "xxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0x4);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Chara_Level = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            //地图id
            result = gMrw.SearchSignature(new byte[] { 0x8B, 0x56, 0x0C, 0x8B, 0x46, 0x08, 0x51, 0x8B, 0x4E, 0x04, 0x52, 0x50, 0x51 }, "xxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 4) - 0x50;
            TB_Base_Cs.AppendText("public static Int32 dwBase_Map_ID = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");


            //创建装备
            result = gMrw.SearchSignature(new byte[] { 0x85, 0xC0, 0x74, 0x13, 0x8B, 0x45, 0x14, 0x8B, 0x5D, 0x0C, 0x50, 0x53, 0x57 }, "xxxxxxxxxxxxx", base_dnf, 0);
            result = result - 0x38;
            TB_Base_Cs.AppendText("public static Int32 dwCall_CreateEqui = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            //购买call
            result = gMrw.SearchSignature(new byte[] { 0x84, 0xC0, 0x0F, 0x84, 0x10, 0x01, 0x00, 0x00, 0x68, 0x52, 0x02, 0x00, 0x00 }, "xxxxxxxxxxxxx", base_dnf, 0);
            result = result - 0x8E;
            TB_Base_Cs.AppendText("public static Int32 dwCall_Buy = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            //角色等级
            result = gMrw.SearchSignature(new byte[] { 0x83, 0xC4, 0x04, 0x50, 0x6A, 0x03, 0xE8, 0xF7, 0x01, 0x55, 0x01, 0x8B, 0x15, 0x44, 0x60, 0x1B, 0x04, 0xA1}, "xxxxxxx????xx????x", base_dnf, 0);
            result = gMrw.readInt32(result + 0x16 + 2);
            TB_Base_Cs.AppendText("public static Int32 dwBase_Role_Id = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = gMrw.SearchSignature(new byte[] { 0x8D,0x0C,0x9B,0x8D,0x0C,0x8E,0x8B,0x91,0x04,0x01,0x00,0x00 }, "xxxxxxxxxxxx", base_dnf, 0);
            result = result - 0x5F;
            
            TB_Base_Cs.AppendText("public static Int32 dwCall_UI = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");


            result = gMrw.SearchSignature(new byte[] { 0x52, 0x8D, 0x55, 0xA0, 0x52, 0xE8, 0xDC, 0x39, 0x8B, 0x02, 0xEB, 0x2E }, "xxxxxx????xx", base_dnf, 0);
            result_1 = result;
            result = result_1 + 5;
            result = gMrw.GetCallAddress(result);
            TB_Base_Cs.AppendText("public static Int32 dwCall_Encryption1 = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = result_1 + 0x11;
            result = gMrw.GetCallAddress(result);
            TB_Base_Cs.AppendText("public static Int32 dwCall_Encryption2 = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = result_1 + 0x1D;
            result = gMrw.GetCallAddress(result);
            TB_Base_Cs.AppendText("public static Int32 dwCall_Encryption3 = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = result_1 + 0x29;
            result = gMrw.GetCallAddress(result);
            TB_Base_Cs.AppendText("public static Int32 dwCall_Encryption4 = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = result_1 + 0x35;
            result = gMrw.GetCallAddress(result);
            TB_Base_Cs.AppendText("public static Int32 dwCall_Encryption5 = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = gMrw.SearchSignature(new byte[] { 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x58, 0x53, 0x56, 0x57, 0x6A, 0x00, 0x8B, 0xF1, 0xC7, 0x45, 0xFC, 0x00, 0x00, 0x00, 0x00 }, "xxxxxxxxxxxxxxxxxxxx", base_dnf, 0);
            TB_Base_Cs.AppendText("public static Int32 dwCall_CreateEmery = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = gMrw.SearchSignature(new byte[] { 0x3D ,0x86 ,0x00 ,0x00 ,0x00 ,0x74 ,0x0E }, "xxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result + 0x9);
            TB_Base_Cs.AppendText("public static Int32 dwBase_KeyPress = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = gMrw.SearchSignature(new byte[] { 0x55, 0x8B, 0xEC, 0x56, 0x57, 0x8B, 0x7D, 0x08, 0x8B, 0xF1, 0x85, 0xFF, 0x75, 0x08, 0x5F, 0x33, 0xC0, 0x5E, 0x5D, 0xC2, 0x08, 0x00 }, "xxxxxxxxxxxxxxxxxxxxxx", base_dnf, 0);
            TB_Base_Cs.AppendText("public static Int32 dwCall_LoadCall = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            result = gMrw.SearchSignature(new byte[] { 0x55, 0x8B, 0xEC, 0x8B, 0x45, 0x08, 0x83, 0xE8, 0x00, 0x74, 0x6C }, "xxxxxxxxxxx", base_dnf, 0);
            TB_Base_Cs.AppendText("public static Int32 dwCall_CompleteAllQuest = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");

            TB_Base_Cs.AppendText("public struct Index\r\n{\r\n");
            result = gMrw.SearchSignature(new byte[] { 0x85, 0xC0, 0x7E, 0x35, 0x50, 0xB9 }, "xxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result + 0x6);
            TB_Base_Cs.AppendText("public static Int32 Monster = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            result = gMrw.SearchSignature(new byte[] { 0x85, 0xC0, 0x74, 0x13, 0x8B, 0x45, 0x14, 0x8B, 0x5D, 0x0C, 0x50, 0x53, 0x57 }, "xxxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0xB);
            TB_Base_Cs.AppendText("public static Int32 Equip = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            result = gMrw.SearchSignature(new byte[] { 0x8B, 0xF0, 0x85, 0xF6, 0x74, 0x68, 0x33, 0xD2, 0x68, 0x06, 0x02, 0x00, 0x00, 0x52 }, "xxxxxxxxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0x9);
            TB_Base_Cs.AppendText("public static Int32 Code = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            result = gMrw.SearchSignature(new byte[] { 0x89, 0x7D, 0xA4, 0x89, 0x5D, 0xAC }, "xxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0x4);
            TB_Base_Cs.AppendText("public static Int32 Character = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Cs.AppendText("}\r\n");


            TB_Base_Cs.AppendText("public struct GetIndexObj\r\n{\r\n");
            result = gMrw.SearchSignature(new byte[] { 0xF3, 0x0F, 0x2C, 0x70, 0x14, 0x6A, 0x00, 0x52 }, "xxxxxxxx", base_dnf, 0);
            result = gMrw.readInt32(result - 0x4);
            TB_Base_Cs.AppendText("public static Int32 Monster = 0x" + Convert.ToString(result, 16).ToUpper() + ";\r\n");
            TB_Base_Cs.AppendText("public static Int32 Equip = 0x" + Convert.ToString(result + 8, 16).ToUpper() + ";\r\n");
            TB_Base_Cs.AppendText("public static Int32 Code = 0x" + Convert.ToString(result + 4, 16).ToUpper() + ";\r\n");
            TB_Base_Cs.AppendText("public static Int32 Atk = 0x" + Convert.ToString(result + 0x18, 16).ToUpper() + ";\r\n");
            TB_Base_Cs.AppendText("public static Int32 Character = 0x" + Convert.ToString(result - 4, 16).ToUpper() + ";\r\n");
            TB_Base_Cs.AppendText("}\r\n");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Win32.Kernel.CloseHandle((IntPtr)gMrw.mhProcess);
        }

        void writeLogLine (string text) {
            textBox11.AppendText(text + "\r\n");
        }

        Int32 hWnd;

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            hWnd = (Int32)base.Handle;
            writeLogLine(Convert.ToString(hWnd, 16));
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox4.Text = textBox3.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox5.Text = "";
            int start = int.Parse(textBox3.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
            int end  = int.Parse(textBox4.Text, System.Globalization.NumberStyles.AllowHexSpecifier);

            for (int i = start;i <= end;i += int.Parse(textBox6.Text))
            {
                int result = gMrw.Decryption(i);
                textBox5.AppendText(Convert.ToString(i,16) + " = " + result + "\r\n");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            gMrw.EncryptionCall(int.Parse(textBox7.Text, System.Globalization.NumberStyles.AllowHexSpecifier), int.Parse(textBox8.Text));

        }

        private void button5_Click(object sender, EventArgs e)
        {
            gMrw.Encryption1(int.Parse(textBox7.Text, System.Globalization.NumberStyles.AllowHexSpecifier), int.Parse(textBox8.Text));

        }

        private void button6_Click(object sender, EventArgs e) {
            #region
            at.clear();
            at.push_eax();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x04);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF00);//900存放返回地址
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x08);//mov eax,[esp + 4]
                               //获取包头
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF04);//900存放返回地址
            at.writeInt8(0x58);//pop eax
            at.pushad();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF04);//获取返回地址
            at.push_eax();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF00);//获取返回地址
            at.push_eax();
            at.push_Int32(WM_START);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_ebp();
            at.mov_ebp_esp();
            at.push_Int8(0xFF);
            at.push_Int32(baseAddr.dwCall_HANDLE + 5);
            at.retn();
            Int32 m = 0;
            foreach (Byte a in at.Code) {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x400 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.HookJmp(baseAddr.dwCall_HANDLE, at.GetVirtualAddr() + 0x400);

            at.clear();
            at.push_eax();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x08);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF00);//900存放包数据
            at.writeInt8(0x58);//pop eax
            at.pushad();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF00);//获取返回地址
            at.push_eax();
            at.push_Int32(1); ;
            at.push_Int32(WM_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_ebp();
            at.mov_ebp_esp();
            at.writeInt8(0x80);
            at.writeInt8(0x79);
            at.writeInt8(0x08);
            at.writeInt8(0x00);
            at.push_Int32(baseAddr.dwCall_ADD + 0x7);
            at.retn();
            m = 0;
            foreach (Byte a in at.Code) {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x450 + m++), a);
            }
            gMrw.HookJmp(baseAddr.dwCall_ADD, at.GetVirtualAddr() + 0x450);//Int8

            at.clear();
            at.push_eax();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x08);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF00);//900存放包数据
            at.writeInt8(0x58);//pop eax
            at.pushad();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF00);//获取返回地址
            at.push_eax();
            at.push_Int32(2);
            at.push_Int32(WM_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_ebp();
            at.mov_ebp_esp();
            at.writeInt8(0x80);
            at.writeInt8(0x79);
            at.writeInt8(0x08);
            at.writeInt8(0x00);
            at.push_Int32(baseAddr.dwCall_ADD + 0x30 + 0x7);
            at.retn();
            m = 0;
            foreach (Byte a in at.Code) {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x500 + m++), a);
            }
            gMrw.HookJmp(baseAddr.dwCall_ADD + 0x30 , at.GetVirtualAddr() + 0x500);//Int32

            at.clear();
            at.push_eax();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x08);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF00);//900存放包数据
            at.writeInt8(0x58);//pop eax
            at.pushad();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF00);//获取返回地址
            at.push_eax();
            at.push_Int32(4);
            at.push_Int32(WM_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_ebp();
            at.mov_ebp_esp();
            at.writeInt8(0x80);
            at.writeInt8(0x79);
            at.writeInt8(0x08);
            at.writeInt8(0x00);
            at.push_Int32(baseAddr.dwCall_ADD + 0x60 + 0x7);
            at.retn();
            m = 0;
            foreach (Byte a in at.Code) {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x550 + m++), a);
            }
            gMrw.HookJmp(baseAddr.dwCall_ADD + 0x60 , at.GetVirtualAddr() + 0x550);//Int32

            at.clear();
            at.push_eax();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x08);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF00);//900存放包数据
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x0C);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF04);//900存放包数据
            at.writeInt8(0x58);//pop eax
            at.pushad();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF00);//获取返回地址
            at.push_eax();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF04);//获取返回地址
            at.push_eax();
            at.push_Int32(WM_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_ebp();
            at.mov_ebp_esp();
            at.writeInt8(0x80);
            at.writeInt8(0x79);
            at.writeInt8(0x08);
            at.writeInt8(0x00);
            at.push_Int32(baseAddr.dwCall_ADD + 0x90 + 0x7);
            at.retn();
            m = 0;
            foreach (Byte a in at.Code) {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x650 + m++), a);
            }
            gMrw.HookJmp(baseAddr.dwCall_ADD + 0x90, at.GetVirtualAddr() + 0x650);//Int64

            at.clear();
            at.push_eax();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x04);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF00);//900存放包数据
            at.writeInt8(0x58);//pop eax
            at.pushad();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF00);//获取返回地址
            at.push_eax();
            at.push_Int32(0);
            at.push_Int32(WM_END);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_ebp();
            at.mov_ebp_esp();
            at.mov_eax(0x40050);
            at.push_Int32(baseAddr.dwCall_SEND + 0x8);
            at.retn();
            m = 0;
            foreach (Byte a in at.Code) {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x700 + m++), a);
            }
            gMrw.HookJmp(baseAddr.dwCall_SEND, at.GetVirtualAddr() + 0x700);//End

            at.clear();
            at.push_eax();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);//mov eax,[esp]
            at.writeInt8(0x08);
            at.mov_xxx_eax(at.GetVirtualAddr() + 0xF00);//900存放包数据
            at.writeInt8(0x58);//pop eax
            at.pushad();
            at.writeInt8(0xA1);
            at.writeInt32(at.GetVirtualAddr() + 0xF00);//获取返回地址
            at.push_eax();
            at.push_Int32(4);
            at.push_Int32(WM_STRING);
            at.push_Int32(hWnd);
            at.mov_eax(at.SendMessage);
            at.call_eax();
            at.popad();
            at.push_ebp();
            at.mov_ebp_esp();
            at.push_Int8(0xFF);
            at.push_Int32(0x0345B5D0 + 0x5);
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x750 + m++), a);
            }
            gMrw.HookJmp(0x0345B5D0, at.GetVirtualAddr() + 0x750);//Int32
            #endregion
        }

        private void button9_Click(object sender, EventArgs e) {
            at.clear();
            at.writeInt8(0x8B);
            at.writeInt8(0x54);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov edx,[esp+8]
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov eax,[esp+4]
            at.pushad();
            at.push_eax(); //
            at.push_edx(); //
            at.push_Int32(WM_44_START);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x8));
            at.retn();
            Int32 m = 0;
            foreach (Byte a in at.Code) {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x400 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x08, at.GetVirtualAddr() + 0x400, true);

            at.clear();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov eax,[esp+4]

            at.pushad();
            at.push_eax(); //
            at.push_Int32(1); //
            at.push_Int32(WM_44_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x24));
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x450 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x24, at.GetVirtualAddr() + 0x450, true);

            at.clear();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov eax,[esp+4]

            at.pushad();
            at.push_eax(); //
            at.push_Int32(2); //
            at.push_Int32(WM_44_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x28));
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x500 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x28, at.GetVirtualAddr() + 0x500, true);
            at.clear();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov eax,[esp+4]

            at.pushad();
            at.push_eax(); //
            at.push_Int32(4); //
            at.push_Int32(WM_44_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x2C));
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x550 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x2C, at.GetVirtualAddr() + 0x550, true);
            at.clear();
            at.pushad();
            at.push_eax(); //
            at.push_Int32(4); //
            at.push_Int32(WM_44_END);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x48));
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x600 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x48, at.GetVirtualAddr() + 0x600, true);

            at.clear();
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov eax,[esp+4]

            at.pushad();
            at.push_eax(); //
            at.push_Int32(0xC); //
            at.push_Int32(WM_44_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.PostMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x30));
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x650 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x30, at.GetVirtualAddr() + 0x650, true);

            at.clear();
            at.writeInt8(0x8D);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov eax,[esp+4]
            //at.writeInt8(0x8B);
            //at.writeInt8(0x54);
            //at.writeInt8(0x24);
            //at.writeInt8(0x08);//mov edx,[esp+8]
            at.pushad();
            at.push_eax(); //
            at.push_Int8(8); //
            at.push_Int32(WM_44_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.SendMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x34));
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x700 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x34, at.GetVirtualAddr() + 0x700, true);

            at.clear();
            at.writeInt8(0x8B);
            at.writeInt8(0x54);
            at.writeInt8(0x24);
            at.writeInt8(0x08);//mov edx,[esp+8]
            at.writeInt8(0x8B);
            at.writeInt8(0x44);
            at.writeInt8(0x24);
            at.writeInt8(0x04);//mov eax,[esp+4]
            at.writeInt8(0x83);
            at.writeInt8(0xFA);
            at.writeInt8(0x04);//cmp eax,8
            at.writeInt8(0x7F);
            at.writeInt8(0x02);
            at.mov_eax_ptr_eax();

            at.pushad();
            at.push_eax(); //
            at.push_edx(); //
            at.push_Int32(WM_44_ADD);
            at.push_Int32(hWnd);
            at.mov_eax(at.SendMessage);
            at.call_eax();
            at.popad();
            at.push_Int32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0, 0x38));
            at.retn();
            m = 0;
            foreach (Byte a in at.Code)
            {
                gMrw.writeInt8((at.GetVirtualAddr() + 0x750 + m++), a);
                //gMrw.writeInt8(0x400400 + m++, a);
            }
            gMrw.writeInt32(gMrw.readInt32(baseAddr.dwBase_Shop - 8, 0) + 0x38, at.GetVirtualAddr() + 0x750, true);
        }


        void work(int ss)
        {
            int old_addr = ss;

            int id = gMrw.readInt16(old_addr + 1);
            int lenth = gMrw.readInt16(old_addr + 3);
            bool flag = false;

            string data = textBox14.Text;
            if (data != "")
            {
                string[] s_data = data.Split(' ');
                Int32[] i_data = new Int32[s_data.Length];
                for (int i = 0; i < i_data.Length; i++)
                    i_data[i] = Convert.ToInt32(s_data[i], 16);
                if (comboBox3.SelectedIndex == 0)
                {
                    flag = true;
                    foreach (Int32 d in i_data)
                    {
                        if (d == (Int32)id)
                            flag = false;
                    }
                }
                else
                {
                    flag = false;
                    foreach (Int32 d in i_data)
                    {
                        if (d == (Int32)id)
                            flag = true;
                    }
                }
            }

            if (flag)
            {

                textBox15.AppendText("[包头:" + Convert.ToString(id, 16) + ",包长:" + Convert.ToString(lenth, 16) + "]");
                byte[] package = gMrw.readData((uint)old_addr, (uint)gMrw.readInt16(old_addr + 3));
                string s = "";



                foreach (var b in package)
                {
                    s += Convert.ToString(b, 16) + " ";
                }
                textBox15.AppendText(s + "\r\n");
            }
        }

        void getPackage()
        {
            int old_addr = gMrw.readInt32(0x52EF3C0);
            while (true)
            {
 

            }
        }

        Thread getPackageThread ;
        bool IsLoad = false;
        Int32 HookAddr = 0;

        private void button10_Click(object sender, EventArgs e)
        {
            //if (!IsLoad)
            //{
                
            //    getPackageThread = new Thread(getPackage);
            //    IsLoad = true;
            //}
            if (button10.Text == "开始拦截")
            {
                at.clear();
                at.cmp_esp(GameRpcs + 0x195F63 + 6);
                at.je_Int8(6);
                at.push_Int32(at.GetTickCount);
                at.retn();
                at.pushad();
                at.push_ebp_addx(0x34);
                at.push_ebp_addx(0x34);
                at.push_Int32(WM_GETPACKAGE);
                at.push_Int32(hWnd);
                at.mov_eax(at.SendMessage);
                at.call_eax();

                at.popad();
                at.push_Int32(at.GetTickCount);
                at.retn();

                button10.Text = "结束拦截";

                int i = 0;
                //10195F63    FF15 44E33310   call dword ptr ds:[<&KERNEL32.GetTickCount>]               ; kernel32.GetTickCount
                foreach (Byte b in at.Code)
                {
                    gMrw.writeInt8(at.GetVirtualAddr() + 0xE50 + i++, b);
                }
                writeLogLine(Convert.ToString(gMrw.readInt32(GameRpcs + 0x195F63 + 2, 0), 16));
                gMrw.writeInt32(gMrw.readInt32(GameRpcs + 0x195F63 + 2), at.GetVirtualAddr() + 0xE50,true);


            }
            else
            {
                gMrw.writeInt32(gMrw.readInt32(GameRpcs + 0x195F63 + 2), at.GetTickCount, true);

                button10.Text = "开始拦截";
                IsLoad = false;

            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show(gMrw.writeInt32(GameRpcs,0).ToString());
        }
    }
}
