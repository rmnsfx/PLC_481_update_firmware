using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Windows;
using System.IO.Ports;
using System.Threading;
using System.ComponentModel;

namespace Serial
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SerialPort serial = new SerialPort();
        FileStream fs = new FileStream(@"D:\Repos\PLC_1xx\plc_1xx_L433\MDK-ARM\PLC_1xx.bin", FileMode.Open);
        

        bool SerialOpen()
        {
            try
            {
                serial.PortName = "COM5";
                serial.BaudRate = 115200;
                serial.Handshake = System.IO.Ports.Handshake.None;
                serial.Parity = Parity.None;
                serial.DataBits = 8;
                serial.StopBits = StopBits.One;
                serial.ReadTimeout = 1000;
                serial.WriteTimeout = 5000;

                serial.Open();

                //int len = (int)fs.Length;
                
                byte[] bytestosend = new byte[45808];

                //serial.Write(erase_command, 0, 2);
                //serial.DiscardOutBuffer();
                //serial.DiscardInBuffer();
                //System.Threading.Thread.Sleep(5);
                
                fs.Read(bytestosend, 0, 45808);
                                
                for (int i = 0; i < bytestosend.Length; i += 8)
                {
                    serial.DiscardOutBuffer();
                    serial.DiscardInBuffer();
                    serial.Write(bytestosend, i, 8);

                    System.Threading.Thread.Sleep(3); 
                }

                serial.Close();
                //serial.Dispose();
                //serial = null;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            SerialOpen();

        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            serial.PortName = "COM5";
            serial.BaudRate = 115200;
            serial.Handshake = System.IO.Ports.Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.ReadTimeout = 1000;
            serial.WriteTimeout = 5000;

            byte[] erase_command = new byte[]{1, 2};
            serial.Open();
            serial.DiscardOutBuffer();
            serial.DiscardInBuffer();
            serial.Write(erase_command, 0, erase_command.Length);
            serial.Close();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            serial.PortName = "COM5";
            serial.BaudRate = 115200;
            serial.Handshake = System.IO.Ports.Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.ReadTimeout = 1000;
            serial.WriteTimeout = 5000;

            byte[] erase_command = new byte[] {10, 98, 111, 111,  116};
            serial.Open();
            serial.DiscardOutBuffer();
            serial.DiscardInBuffer();
            serial.Write(erase_command, 0, erase_command.Length);
            serial.Close();
        }
    }
}
