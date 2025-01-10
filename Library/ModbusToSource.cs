using System;
using System.Collections.Generic;

using System.IO.Ports;

using System.Threading;

namespace TestAvalonia
{ 
    public class ModbusSourceInterface: SerialPort
    {
        public List<ModbusSourceClient> Clients=new List<ModbusSourceClient>();
        public ModbusSourceInterface():base("/dev/serial0", 9600, Parity.None, 8, StopBits.One)
        {
            this.Handshake = Handshake.None;
            this.RtsEnable = false;
            this.DtrEnable = false;
            
            this.Open();
        }

        /// <summary>
        /// Add a new client to your Modbus RTU network 
        /// </summary>
        /// <param name="clientAdress">Adress code</param>
        public void newClient(byte clientAdress) {
            ModbusSourceClient client = new(this,clientAdress);
            Clients.Add(client);
        }
    }

    /// <summary>
    /// Datasheet from the Buckmodule: https://basicpi.org/Archive/DPS5020%20CNC%20power%20communication%20protocol.pdf
    /// </summary>
    public class ModbusSourceClient
     {
        private ModbusRegisterAdressModel RegisterAdress =new();

        private ModbusSourceInterface ModbusInterface;

        private byte clientAdress;

        private float u_set=-1;//--> not defined
        public float U_set
        {
            get {
               
                u_set=(float)ReadRegisterData(RegisterAdress.U_SET)  /100;
                
                return u_set;
            }

            set {
               
                WriteSingleRegister(RegisterAdress.U_SET, value);
                u_set = value;   
            } 
        }

        private float i_set=-1;
        public float I_set
        {
            get
            {
                
                i_set = (float)ReadRegisterData(RegisterAdress.I_SET) / 100;
                
               
                return i_set;
            }

            set
            {
                WriteSingleRegister(RegisterAdress.I_SET, value);
                i_set = value;
            }
        }

        private float uout;
        public float Uout
        {
            get
            {
                uout= (float)ReadRegisterData(RegisterAdress.UOUT)/100;
                return uout;
            }
           
            //only read
        }


        private float iout;
        public float Iout
        {
            get
            {
                iout= (float)ReadRegisterData(RegisterAdress.IOUT) / 100;
                return iout;
            }
            //only read
        }

        private float power;
        public float Power
        {
            get
            {
                power = (float)ReadRegisterData(RegisterAdress.POWER)/100;
                return power;
            }
            //only read
        }



        private float uin;
        public float Uin
        {
            get
            {
                uin = (float)ReadRegisterData(RegisterAdress.UIN)/100;
                return uin;
            }
            //only read
        }

        private int _lock=-1;
        public int Lock
        {
            get
            {
                if (_lock== -1)
                {
                    _lock= ReadRegisterData(RegisterAdress.LOCK);
                }
               
                return _lock;
            }

            set
            {
                WriteSingleRegister(RegisterAdress.LOCK, value);
                _lock = value;
            }
        }


        private int protect;
        public int Protect
        {
            get
            {
                protect=ReadRegisterData(RegisterAdress.PROTECT);
                return protect;
            }
            //only read
        }

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modbusInterface"></param>
        /// <param name="adress">Client Adress from the Buckmodule</param>
        public ModbusSourceClient(ModbusSourceInterface modbusInterface, byte adress)
        {
            this.ModbusInterface = modbusInterface;
            this.clientAdress = adress;
        }

        /// <summary>
        /// Function Code 0x03
        /// </summary>
        /// <param name="Register"></param>
        /// <returns>Content from requestet Register (-1 --> Error)</returns>
        public int ReadRegisterData(int Register)
        {
            //Function Code 0x03

            //adress (0-255)    |   Function Code   |   Register Higher Byte    |   Register Lower Byte |   number of Register Higher byte  |   number of Register Lower byte 
            byte[] data = [clientAdress, 0x03, BitConverter.GetBytes(Register)[1], BitConverter.GetBytes(Register)[0],0,2];
            
            var crc_sum = Crc16.ComputeChecksum(data);//calculate crc 16

            //Add crc summe to Array
            byte[] send_data = [data[0], data[1], data[2], data[3], data[4], data[5], BitConverter.GetBytes(crc_sum)[0], BitConverter.GetBytes(crc_sum)[1]];
            

            //clear the Uart Buffer
            ModbusInterface.DiscardOutBuffer();
            ModbusInterface.DiscardInBuffer();

            Thread.Sleep(10);
            ModbusInterface.Write(send_data, 0, send_data.Length);//Send Data with Uart
            Thread.Sleep(600);
            

            byte[] received = new byte[6];
            int readValue=-1;



            int searchSlave=-1;
            int timeout = 1000;             //in ms
            
            int value = -1;
            while (ModbusInterface.BytesToRead > 0)
            {
                searchSlave = ModbusInterface.ReadByte();
                if (searchSlave == clientAdress)
                {

                    ModbusInterface.Read(received, 0, received.Length);
                    value = received[2] << 8 | received[3];
                    return value;
                }
            }


            byte[] backCalculationCRC = [clientAdress, received[0], received[1], received[2], received[3], received[4]];
            
            crc_sum = Crc16.ComputeChecksum(backCalculationCRC);

            if (crc_sum == (received[6] << 8 | received[5]))
            {
                return value;
            }
            else
            {
                return -1;
            }
        }


        public bool WriteSingleRegister(int Register, int value)
        {
            //Function Code is 0x06

            //adress (0-255)    |   Function Code   |   Register Higher Byte    |   Register Lower Byte |   Register Content Higher byte  |  Register Content Lower byte 
            byte[] data = [clientAdress, 0x06, BitConverter.GetBytes(Register)[1], BitConverter.GetBytes(Register)[0], BitConverter.GetBytes(value)[1], BitConverter.GetBytes(value)[0]];

                var crc_sum = Crc16.ComputeChecksum(data);

            //Add crc summe to Array
            byte[] send_data = [data[0], data[1], data[2], data[3], data[4], data[5], BitConverter.GetBytes(crc_sum)[0], BitConverter.GetBytes(crc_sum)[1]];

            ModbusInterface.Write(send_data, 0, send_data.Length);//Send Data with Uart
            Thread.Sleep(200);

            for (int i = 0; ModbusInterface.BytesToRead == 0; i++)//wait
            {
                Thread.Sleep(200);
                if (i == 20)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Only for float Register
        /// Function Code is 0x06
        /// </summary>
        /// <param name="Register"></param>
        /// <param name="value">New Content from the Register</param>
        public bool WriteSingleRegister(int Register, float value)
        {
            //float to interpretable int value for the Buckmodule
            int int_value = (int)(value * 100);

            //adress (0-255)    |   Function Code   |   Register Higher Byte    |   Register Lower Byte |   Register Content Higher byte  |  Register Content Lower byte 
            byte[] data = [clientAdress, 0x06, BitConverter.GetBytes(Register)[1], BitConverter.GetBytes(Register)[0], BitConverter.GetBytes(int_value)[1], BitConverter.GetBytes(int_value)[0]];

            var crc_sum = Crc16.ComputeChecksum(data);

            //Add crc summe to Array
            byte[] send_data = [data[0], data[1], data[2], data[3], data[4], data[5], BitConverter.GetBytes(crc_sum)[0], BitConverter.GetBytes(crc_sum)[1]];

            ModbusInterface.Write(send_data, 0, send_data.Length);//Send Data with Uart
            Thread.Sleep(200);

            return true;
        }


     }

    public static class Crc16
    {
        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        static Crc16()
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


    class ModbusRegisterAdressModel
    {
        public readonly int U_SET;
        public readonly int I_SET;
        public readonly int UOUT;
        public readonly int IOUT;
        public readonly int POWER;
        public readonly int UIN;
        public readonly int LOCK;
        public readonly int PROTECT;
        public readonly int CVCC;
        public readonly int ONOFF;
        public readonly int B_LED;
        public readonly int MODEL;
        public readonly int VERSON;
        public readonly int EXTRACT_M;
        public ModbusRegisterAdressModel() {
            U_SET = 0x0000;
            I_SET = 0x0001;
            UOUT = 0x0002;
            IOUT = 0x0003;
            POWER = 0x0004;
            UIN = 0x0005;
            LOCK = 0x0006;
            PROTECT = 0x0007;
            CVCC = 0x0008;
            ONOFF = 0x0009;
            B_LED = 0x000A;
            MODEL = 0x000B;
            VERSON = 0x000C;
            EXTRACT_M = 0x0023;
        }
    }
}
