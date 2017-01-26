using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApplication1
{
    public class Crc16
    {
        const ushort polynomial = 0xA001;
        ushort[] table = new ushort[256];

        public ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public Crc16()
        {
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
    class Program
    {
        public static void GetCRC16(byte[] message, ref byte[] CRC16)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length); i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);
                    if (CRCLSB == 1)
                    {
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                    }
                }
                CRC16[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
                CRC16[0] = CRCLow = (byte)(CRCFull & 0xFF);
            }
        }
        static void Main(string[] args)
        {
            byte[] otvet = new byte[2];
            GetCRC16(new byte[] {1,3, 2, 0, 4},ref otvet);
            //Crc16 a = new Crc16();
            //byte[] msg = a.ComputeChecksumBytes(new byte[] { 1,3,0,41,0,1 });
            // Мы дождались клиента, пытающегося с нами соединиться
            //ReadHoldingRegister(1,41,1);

            //data += Encoding.UTF8.GetString(bytes, 0, bytesRec);


            //byte[] msg = Encoding.UTF8.GetBytes(Str);

            Console.Write(0);

        }
        public static byte[] ReadHoldingRegister(int id, int startAddress, int length)
        {
            byte[] data = new byte[8];

            byte High, Low;
            data[0] = Convert.ToByte(id);
            data[1] = Convert.ToByte(3);
            byte[] _adr = BitConverter.GetBytes(startAddress - 1);
            data[2] = _adr[1];
            data[3] = _adr[0];
            byte[] _length = BitConverter.GetBytes(length);
            data[4] = _length[1];
            data[5] = _length[0];
            myCRC(data, 6, out High, out Low);
            data[6] = Low;
            data[7] = High;
            return data;
        }

        public static void myCRC(byte[] message, int length, out byte CRCHigh, out byte CRCLow)
        {
            ushort CRCFull = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((CRCFull & 0x0001) == 0)
                        CRCFull = (ushort)(CRCFull >> 1);
                    else
                    {
                        CRCFull = (ushort)((CRCFull >> 1) ^ 0xA001);
                    }
                }
            }
            CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRCLow = (byte)(CRCFull & 0xFF);
        }
    }
}
