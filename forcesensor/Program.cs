using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace CRC
{
    class Program
    {

        public static SerialPort port2 = new SerialPort("COM32", 19200, Parity.None, 8, StopBits.One);

        private static void Main(string[] args)
        {
            byte[] values = new byte[14];
            port2.Open();

            System.Threading.Thread.Sleep(1000);
            port2.DiscardInBuffer();
            port2.Write(new byte[] { 0x09, 0x10, 0x01, 0x9A, 0x00, 0x01, 0x02, 0x02, 0x00, 0xCD, 0xCA }, 0, 11);  // asking for a value
            System.Threading.Thread.Sleep(10);
            int line;
            byte lines;
            int[] forces = new int[6];
            byte[] temporal = new byte[2];
            line = port2.ReadByte();
            Console.WriteLine(line);

            while (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                System.Threading.Thread.Sleep(10);
                line = port2.ReadByte();
                if (line == 32)
                {
                    Console.WriteLine(line);
                    line = port2.ReadByte();
                    if (line == 78)
                    {
                        Console.WriteLine(line);
                        for (int i = 0; i <= 13; i++)
                        {
                            line = port2.ReadByte();
                            lines = (byte)line;
                            Console.WriteLine(line);
                            values[i] = lines;
                        }
                        Console.WriteLine("Results");

                        Array.Copy(values, 0, temporal, 0, 2);
                        forces[0] = BitConverter.ToInt16(temporal, 0);
                        Array.Copy(values, 2, temporal, 0, 2);
                        forces[1] = BitConverter.ToInt16(temporal, 0);
                        Array.Copy(values, 4, temporal, 0, 2);
                        forces[2] = BitConverter.ToInt16(temporal, 0);
                        Array.Copy(values, 6, temporal, 0, 2);
                        forces[3] = BitConverter.ToInt16(temporal, 0);
                        Array.Copy(values, 8, temporal, 0, 2);
                        forces[4] = BitConverter.ToInt16(temporal, 0);
                        Array.Copy(values, 10, temporal, 0, 2);
                        forces[5] = BitConverter.ToInt16(temporal, 0);
                        Console.WriteLine(forces[0]/1000);   //fx divided by 1000  N
                        Console.WriteLine(forces[1] / 1000);   //fy divided by 1000  N
                        Console.WriteLine(forces[2] / 1000);   //fz divided by 1000  N
                        Console.WriteLine(forces[3] / 100);   //Tx divided by 100  Nm
                        Console.WriteLine(forces[4] / 100);   //Ty divided by 100  Nm
                        Console.WriteLine(forces[5] / 100);   //Tz divided by 100  Nm

                    }
                    else
                    {
                        port2.DiscardInBuffer();
                    }

                }

            }
        }



        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();

        }


        public byte[] CalculateCrc16Modbus(byte[] bytes)
        {
            CrcStdParams.StandartParameters.TryGetValue(CrcAlgorithms.Crc16Modbus, out Parameters crc_p);
            Crc crc = new Crc(crc_p);
            crc.Initialize();
            byte[] crc_bytes = crc.ComputeHash(bytes);
            return crc_bytes;
        }


    }


    
}



