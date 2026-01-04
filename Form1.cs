using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
 
 

 

namespace CRC32_calculate
{
  
    public partial class Form1 : Form
    {
        
        //================================================================================================================================
        byte[] byteArray = new byte[1024];
        byte[] byteread = new byte[1024];
        char[] charread = new char[1024];
        byte[] read_code = new byte[24];

        int my_write_length = 0;
        long my_file_length = 0;
        UInt32 my_CRC32 = 0;
        UInt32 my_POLY = 0;
        UInt32 crc_xor = 0;
        UInt32 Read_ADDR = 0;
        //================================================================================================================================
      
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setup_combos();
        }

        //================================================================================================================================
        //================================================================================================================================
        //================================================================================================================================
        private void button1_Click(object sender, EventArgs e)
        { 
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if(File.Exists(openFileDialog1.FileName) == false)
                {
                    MessageBox.Show("File not found!");
                    return;
                }
                my_file_length = new System.IO.FileInfo(openFileDialog1.FileName).Length;
                label1.Text = openFileDialog1.FileName;
                label1.Visible = true;
                label2.Text = "File Length = " + my_file_length.ToString() + " bytes";
                label2.Visible = true;
                long readlength = my_file_length;
                setup_combos();
                long bytes_read = 0;
                System.IO.FileStream fsr = new System.IO.FileStream((openFileDialog1.FileName), System.IO.FileMode.Open, System.IO.FileAccess.Read);
                while (readlength > 0)
                {
                    
                    if (readlength >= 1024)
                    {
                        bytes_read = fsr.Read(byteArray, 0, 1024);
                    }
                    else
                    {
                        bytes_read = fsr.Read(byteArray, 0, (int)readlength);
                    }
                    readlength -= bytes_read;

                    //--- CRC32 calculate ---
                    for(int i = 0; i < bytes_read; i++)
                    {
                        my_CRC32 ^= byteArray[i];
                        for(int j = 8; j > 0; j--)
                        {
                            if((my_CRC32 & 0x01) == 0x01)
                            {
                                my_CRC32 = (my_CRC32 >> 1) ^ my_POLY;
                            }
                            else
                            {
                                my_CRC32 >>= 1;
                            }
                        }
                    }
                    
                    //--- CRC32 calculate ---
                }
                fsr.Close();
                fsr.Dispose();
                my_CRC32 = my_CRC32 ^ crc_xor;
                label3.Text = "CRC32: 0x" + my_CRC32.ToString("X8");
                label3.Visible = true;
            }
        }
        //================================================================================================================================
        //================================================================================================================================
        //================================================================================================================================

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //================================================================================================================================
        private void makefile()
        {
            System.IO.FileStream fsw = new System.IO.FileStream((saveFileDialog1.FileName), System.IO.FileMode.Create, System.IO.FileAccess.Write);
            fsw.Close();
            fsw.Dispose();
        }

        private void append_to_file()
        {
            System.IO.FileStream fsw = new System.IO.FileStream((saveFileDialog1.FileName), System.IO.FileMode.Append, System.IO.FileAccess.Write);
            if (fsw.CanWrite) { fsw.Write(byteArray, 0, my_write_length); }
            fsw.Flush();
            fsw.Close();
            fsw.Dispose();
        }
        //================================================================================================================================
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = checkBox1.Checked;
            if (checkBox1.Checked == false)
            {
                comboBox1.Text = "0xFFFFFFFF";
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = checkBox1.Checked;
            if (checkBox1.Checked == false)
            {
                comboBox1.Text = "0xFFFFFFFF";
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = checkBox2.Checked;
            if (checkBox2.Checked == false)
            {
                comboBox2.Text = "0xFFFFFFFF";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            comboBox4.Enabled = checkBox3.Checked;
            if (checkBox3.Checked == false)
            {
                comboBox4.Text = "0xEDB88320";
            }
        }

        private void CONVERT_HEX_ADDRESS(string my_txt)
        {
            comboBox3.Text = my_txt;
            char[] read_data = new char[24];
            comboBox3.Text = comboBox3.Text.Trim();
            var str = comboBox3.Text;


            UInt32 dana;
            int i, datalen;

            if (str.Length < 3)
            {
                comboBox3.Text = "0x00000000";
                str = comboBox3.Text;
            }
            if (str.Length > 10)
            {
                comboBox3.Text = "0x00000000";
                str = comboBox3.Text;
            }
            read_data[0] = str[0];
            read_data[1] = str[1];
            if (read_data[0] != '0')
            {
                comboBox3.Text = "0x00000000";
                str = comboBox3.Text;
            }
            if (read_data[1] != 'x')
            {
                comboBox3.Text = "0x00000000";
                str = comboBox3.Text;
            }

            for (i = 2; i < str.Length; i++)
            {
                read_data[i - 2] = str[i];
            }
            UInt32 cnv1;
            datalen = str.Length - 2;
            dana = 0;
            for (i = 0; i < datalen; i++)
            {
                cnv1 = (UInt32)read_data[i];
                if ((cnv1 > 0x2F) && (cnv1 < 0x3A))
                {
                    cnv1 = cnv1 & 0x0f;
                }
                else
                {
                    cnv1 = cnv1 & 0x0f;
                    cnv1 = cnv1 + 9;
                }
                dana = dana << 4;
                dana = dana + (UInt32)cnv1;
            }
            Read_ADDR = dana;
            CONVERT_ADDRESS_TO_HEX(dana);
        }
        private void CONVERT_ADDRESS_TO_HEX(UInt32 addy)
        {
            int i, i2;
            char[] read_data = new char[24];
            UInt32 dana, storage;
            var str = "0x";
            i2 = 7;
            dana = addy;
            for (i = 0; i < 8; i++)
            {
                storage = dana;
                dana = dana >> 4;
                storage = storage & 0x0f;
                storage = storage + 0x30;
                if (storage > 0x39)
                {
                    storage = storage + 7;
                }
                read_data[i2] = (char)storage;
                i2 = i2 - 1;
            }

            for (i = 0; i < 8; i++)
            {
                str = str + read_data[i];
            }
            comboBox3.Text = str;
        }



        private void setup_combos()
        {
            CONVERT_HEX_ADDRESS(comboBox4.Text);
            comboBox4.Text = comboBox3.Text;
            my_POLY = Read_ADDR;

            CONVERT_HEX_ADDRESS(comboBox1.Text);
            comboBox1.Text = comboBox3.Text;
            my_CRC32 = Read_ADDR;

            CONVERT_HEX_ADDRESS(comboBox2.Text);
            comboBox2.Text = comboBox3.Text;
            crc_xor = Read_ADDR;
        }
        private void comboBox4_Leave(object sender, EventArgs e)
        {
            CONVERT_HEX_ADDRESS(comboBox4.Text);
            comboBox4.Text = comboBox3.Text;
        }
        private void comboBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char) Keys.Enter)
            {
                CONVERT_HEX_ADDRESS(comboBox4.Text);
                comboBox4.Text = comboBox3.Text;
            }
        }

        private void comboBox2_Leave(object sender, EventArgs e)
        {
            CONVERT_HEX_ADDRESS(comboBox2.Text);
            comboBox2.Text = comboBox3.Text;
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CONVERT_HEX_ADDRESS(comboBox2.Text);
                comboBox2.Text = comboBox3.Text;
            }
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            CONVERT_HEX_ADDRESS(comboBox1.Text);
            comboBox1.Text = comboBox3.Text;
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CONVERT_HEX_ADDRESS(comboBox1.Text);
                comboBox1.Text = comboBox3.Text;
            }
        }

     
        //================================================================================================================================
    }

}
