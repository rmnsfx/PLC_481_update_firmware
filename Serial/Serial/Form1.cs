using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows;
using System.IO.Ports;
using System.Threading;


namespace Serial
{
    public partial class Form1 : Form
    {
        private delegate void SetProgressBarValue(int value);


        int i = 0;
        SerialPort serial = new SerialPort();
        
        Thread thread = null;
        Thread thread_2 =  null;

        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        string file_path = null;
        int file_lenght = 0;
        int file_lenght_short = 0;
        int file_lenght_full = 0;
        int remain = 0;
        FileStream fs;

        string selectedValue_comPort = "COM5";
        int selectedValue_baudRate = 115200;

        int modbus_adr = 10; 

        public Form1()
        {
            InitializeComponent();

            var ports = SerialPort.GetPortNames();
            comboBox1.DataSource = ports;
            comboBox1.SelectedIndex = 1;
            comboBox2.Items.Add("9600"); 
            comboBox2.Items.Add("115200");
            comboBox2.SelectedIndex = 1;

            textBox2.Text += Convert.ToString(modbus_adr);
        }

        private void prog_bar()
        {
            while (progressBar1.Value < progressBar1.Maximum)
            {
                if (InvokeRequired)
                {
                    this.Invoke(new SetProgressBarValue(SetProgressValue), i);
                }            
            }

            System.Threading.Thread.Sleep(800);
            MessageBox.Show("Прошивка завершена");

        }

        public void SerialOpen()
        {
            i = 0;
            int status = 0;
            

            try
            {

                serial.PortName = selectedValue_comPort;
                serial.BaudRate = selectedValue_baudRate;
                serial.Handshake = System.IO.Ports.Handshake.None;
                serial.Parity = Parity.None;
                serial.DataBits = 8;
                serial.StopBits = StopBits.One;
                serial.ReadTimeout = 1000;
                serial.WriteTimeout = 5000;
                
                serial.Open();
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();

                byte[] size_send = new byte[4];
                byte[] crc_send = new byte[4];
                byte[] bytestosend = new byte[file_lenght_full];

                size_send = BitConverter.GetBytes(file_lenght);
                serial.Write(size_send, 0, 4);

                System.Threading.Thread.Sleep(10);   

                status = fs.Read(bytestosend, 0, file_lenght_full);

                GetCRC(bytestosend, file_lenght, ref crc_send);
                //crc_send[4] = Convert.ToByte(0);
                //crc_send[5] = Convert.ToByte(0);
                //crc_send[6] = Convert.ToByte(0);
                //crc_send[7] = Convert.ToByte(0);

                //serial.Write(crc_send, 0, 4);

                
                if (status == 0)
                {
                    MessageBox.Show("Ошибка чтения файла прошивки");
                }
                else 
                {
                    for (i = 0; i < file_lenght_full; i += 8)
                    {
                        serial.DiscardOutBuffer();
                        serial.DiscardInBuffer();
                        serial.Write(bytestosend, i, 8);

                        System.Threading.Thread.Sleep(5);                    
                    }                    
                }

                fs.Position = 0;

                serial.Close();
                //serial.Dispose();
                //serial = null;
                
            }
            catch
            {
                MessageBox.Show("Ошибка SERIAL");
            }
        }

        public void GetCRC(byte[] message, int length, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (length); i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        //private int crc16(byte *adr_buffer, int byte_cnt)
        //{
        //    int crc = 0xFFFF;
        //    int[] table = 	        
        //    {
        //        0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
        //        0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
        //        0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
        //        0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
        //        0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
        //        0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
        //        0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
        //        0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
        //        0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
        //        0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
        //        0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
        //        0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
        //        0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
        //        0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
        //        0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
        //        0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
        //        0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
        //        0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
        //        0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
        //        0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
        //        0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
        //        0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
        //        0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
        //        0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
        //        0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
        //        0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
        //        0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
        //        0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
        //        0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
        //        0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
        //        0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
        //        0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040
        //    };

        //    int lut;
        //    /* CRC Generation Function */
        //    while( byte_cnt-- == 0) /* pass through message buffer */
        //    {
        //        lut = crc^ *adr_buffer++;
        //        crc  = (crc >> 8) ^ table[lut];
        //    }
        //    return crc;
        //}
        private void button1_Click(object sender, EventArgs e)
        {

            thread = new Thread(new ThreadStart(SerialOpen));
            thread.IsBackground = true;
            thread.Start();

            file_lenght_short = ((int)file_lenght / 8) * 8;
            remain = file_lenght - file_lenght_short;
            file_lenght_full = file_lenght_short + 8;
            
            progressBar1.Value = 0;
            progressBar1.Maximum = file_lenght_full;
            
            thread_2 = new Thread(new ThreadStart(prog_bar));
            thread_2.IsBackground = true;
            thread_2.Start();
            
        }

        private void button2_Click(object sender, System.EventArgs e)
        {

            serial.PortName = selectedValue_comPort;
            serial.BaudRate = selectedValue_baudRate;
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

            serial.PortName = selectedValue_comPort;
            serial.BaudRate = selectedValue_baudRate;
            serial.Handshake = System.IO.Ports.Handshake.None;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.StopBits = StopBits.One;
            serial.ReadTimeout = 1000;
            serial.WriteTimeout = 5000;

            byte[] erase_command = new byte[] {Convert.ToByte(modbus_adr), 98, 111, 111,  116};
            serial.Open();
            serial.DiscardOutBuffer();
            serial.DiscardInBuffer();
            serial.Write(erase_command, 0, erase_command.Length);
            serial.Close();
        }

        private void progressBar1_Click(object sender, System.EventArgs e)
        {

        }

        private void SetProgressValue(int value)
        {            
            progressBar1.Value = value;
        }

        private void button4_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try 
                { 
                    textBox1.Text += openFileDialog1.FileName;
                    file_path = openFileDialog1.FileName;
                    fs = new FileStream(@file_path, FileMode.Open);
                    file_lenght = (int)fs.Length;
                    label1.Text = "[" + file_lenght.ToString() + " байт]";
                }
                catch
                {
                    MessageBox.Show("Файл уже открыт");
                }
                
            }
        }

        private void textBox1_TextChanged(object sender, System.EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                selectedValue_comPort = (string)comboBox1.SelectedValue;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, System.EventArgs e)
        {

            if (comboBox2.SelectedItem != null)
            {
                Int32.TryParse(comboBox2.SelectedItem.ToString(), out selectedValue_baudRate);
            }            
                       
        }

        private void textBox2_TextChanged(object sender, System.EventArgs e)
        {
            modbus_adr = Convert.ToInt32(textBox2.Text.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
