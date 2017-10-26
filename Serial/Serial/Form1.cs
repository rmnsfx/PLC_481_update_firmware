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

            //serial.PortName = "COM5";
            //serial.BaudRate = 115200;
            //serial.Handshake = System.IO.Ports.Handshake.None;
            //serial.Parity = Parity.None;
            //serial.DataBits = 8;
            //serial.StopBits = StopBits.One;
            //serial.ReadTimeout = 1000;
            //serial.WriteTimeout = 5000;

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

                byte[] bytestosend = new byte[file_lenght];
 
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();

                status = fs.Read(bytestosend, 0, file_lenght);
                if (status == 0)
                {
                    MessageBox.Show("Ошибка чтения файла прошивки");
                }
                else 
                { 
                    for (i = 0; i < bytestosend.Length; i += 8)
                    {
                        serial.DiscardOutBuffer();
                        serial.DiscardInBuffer();
                        serial.Write(bytestosend, i, 8);

                        System.Threading.Thread.Sleep(3);                    
                    }                    
                }

                fs.Position = 0;

                serial.Close();
                //serial.Dispose();
                //serial = null;
                
            }
            catch
            {
                MessageBox.Show("Ошибка открытия COM порта");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            thread = new Thread(new ThreadStart(SerialOpen));
            thread.IsBackground = true;
            thread.Start();

            progressBar1.Value = 0;
            progressBar1.Maximum = file_lenght;

            
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
                textBox1.Text += openFileDialog1.FileName;
                file_path = openFileDialog1.FileName;
                fs = new FileStream(@file_path, FileMode.Open);
                file_lenght = (int)fs.Length;
                label1.Text = file_lenght.ToString();
                
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

        }


    }
}
