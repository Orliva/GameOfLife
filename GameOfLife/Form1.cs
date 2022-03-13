using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        static CellAutomat cellAutomat = null!;
        private Action action = null!;
        private readonly Color activeColor = Color.FromArgb(192, 185, 221);
        private readonly Color passiveColor = Color.FromArgb(128, 161, 212);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 10, 7));
            action = new Action(GO);
            cellAutomat = new CellAutomat(pictureBox1.Image.Width, pictureBox1.Image.Height);
            toolStripButton6.Image = Properties.Resources.icon_Black;
            toolStripButton7.Image = Properties.Resources.icon_Blue;
            toolStripButton8.Image = Properties.Resources.icon_Green;
            toolStripButton9.Image = Properties.Resources.icon_Yelow;
            toolStripButton10.Image = Properties.Resources.icon_Red;

            toolStripMenuItem16.Text += cellAutomat.ProcentGeneralCell + "%";
            toolStripMenuItem15.Text += cellAutomat.ProcentDestroyerCell + "%";
            toolStripMenuItem3.Text += cellAutomat.ProcentProducerCell + "%";
            toolStripMenuItem4.Text += cellAutomat.ProcentGooderCell + "%";

            toolStripTextBox1.LostFocus += ToolStripTextBox1_LostFocus;
            textBox1.LostFocus += TextBox1_LostFocus;
            textBox2.LostFocus += TextBox2_LostFocus;
            textBox3.LostFocus += TextBox3_LostFocus;
            textBox1.KeyDown += toolStripTextBox1_KeyDown;
            textBox2.KeyDown += toolStripTextBox1_KeyDown;
            textBox3.KeyDown += toolStripTextBox1_KeyDown;
        }

        private void toolStripMenuItem1_MouseDown(object sender, MouseEventArgs e)
        {
            var thisTsmi = (ToolStripMenuItem)sender;
            foreach (ToolStripMenuItem tsmi in thisTsmi.GetCurrentParent().Items)
            {
                tsmi.Checked = thisTsmi == tsmi;
            }
        }

        private async void button1_Click_1(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                cellAutomat.StartGame();
                button1.Text = "Стоп";
                timer1.Enabled = true;
            }
            else
            {
                button1.Text = "Старт";
                timer1.Enabled = false;
            }
        }

        private void GO()
        {
            pictureBox1.Image = cellAutomat.Bitmap;

            if (!cellAutomat.IsContinue())
            {
                timer1.Enabled = false;
                button1.Text = "Старт";
            }
        }

        private async void timer1_Tick_1(object sender, EventArgs e)
        {
            Invoke(action);
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
                return;

            if (toolStripButton6.BackColor == passiveColor)
            {
                Configs.Complication1 = true;
                toolStripButton6.BackColor = activeColor;
                toolStripButton6.Image = Properties.Resources.icon_Black_Active;
            }
            else
            {
                Configs.Complication1 = false;
                toolStripButton6.BackColor = passiveColor;
                toolStripButton6.Image = Properties.Resources.icon_Black;
            }

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
                return;

            if (toolStripButton7.BackColor == passiveColor)
            {
                Configs.Complication2 = true;
                toolStripButton7.BackColor = activeColor;
                toolStripButton7.Image = Properties.Resources.icon_Blue_Active;
            }
            else
            {
                Configs.Complication2 = false;
                toolStripButton7.BackColor = passiveColor;
                toolStripButton7.Image = Properties.Resources.icon_Blue;
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
                return;

            if (toolStripButton8.BackColor == passiveColor)
            {
                Configs.Complication3 = true;
                toolStripButton8.BackColor = activeColor;
                toolStripButton8.Image = Properties.Resources.icon_Green_Active;
            }
            else
            {
                Configs.Complication3 = false;
                toolStripButton8.BackColor = passiveColor;
                toolStripButton8.Image = Properties.Resources.icon_Green;
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
                return;

            if (toolStripButton9.BackColor == passiveColor)
            {
                Configs.Complication4 = true;
                toolStripButton9.BackColor = activeColor;
                toolStripButton9.Image = Properties.Resources.icon_Yelow_Active;
            }
            else
            {
                Configs.Complication4 = false;
                toolStripButton9.BackColor = passiveColor;
                toolStripButton9.Image = Properties.Resources.icon_Yelow;
            }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == true)
                return;

            if (toolStripButton10.BackColor == passiveColor)
            {
                Configs.Complication5 = true;
                toolStripButton10.BackColor = activeColor;
                toolStripButton10.Image = Properties.Resources.icon_Red_Active;
            }
            else
            {
                Configs.Complication5 = false;
                toolStripButton10.BackColor = passiveColor;
                toolStripButton10.Image = Properties.Resources.icon_Red;
            }
        }

        private void toolStrip1_MouseDown(object sender, MouseEventArgs e)
        {
            this.Opacity = 0.6;
            base.Capture = false;
            Message m = Message.Create(base.Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
            this.WndProc(ref m);
            this.Opacity = 1;
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
           
        }
        private void ToolStripTextBox1_LostFocus(object? sender, EventArgs e)
        {
            if (int.TryParse(((ToolStripTextBox)sender).Text, out int res))
            {
                if (res < 0)
                    res = 0;
                else if (res > 100)
                    res = 100;
                    
                cellAutomat.ProcentGeneralCell = res;
                var strTmp = toolStripMenuItem16.Text.Split(' ');
                if (strTmp.Last() != res.ToString())
                    toolStripMenuItem16.Text = strTmp[0] + " " + strTmp[1] + " " + strTmp[2] + " " + res.ToString() + "%";
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.Focus();
        }

        private void TextBox1_LostFocus(object? sender, EventArgs e)
        {
            if (int.TryParse(((ToolStripTextBox)sender).Text, out int res))
            {
                if (res < 0)
                    res = 0;
                else if (res > 100)
                    res = 100;

                cellAutomat.ProcentProducerCell = res;
                var strTmp = toolStripMenuItem3.Text.Split(' ');
                if (strTmp.Last() != res.ToString())
                    toolStripMenuItem3.Text = strTmp[0] + " " + strTmp[1] + " " + res.ToString() + "%";
            }
        }


        private void TextBox2_LostFocus(object? sender, EventArgs e)
        {
            if (int.TryParse(((ToolStripTextBox)sender).Text, out int res))
            {
                if (res < 0)
                    res = 0;
                else if (res > 100)
                    res = 100;

                cellAutomat.ProcentGooderCell = res;
                var strTmp = toolStripMenuItem4.Text.Split(' ');
                if (strTmp.Last() != res.ToString())
                    toolStripMenuItem4.Text = strTmp[0] + " " + strTmp[1] + " " + res.ToString() + "%";
            }
        }

        private void TextBox3_LostFocus(object? sender, EventArgs e)
        {
            if (int.TryParse(((ToolStripTextBox)sender).Text, out int res))
            {
                if (res < 0)
                    res = 0;
                else if (res > 100)
                    res = 100;

                cellAutomat.ProcentDestroyerCell = res;
                var strTmp = toolStripMenuItem15.Text.Split(' ');
                if (strTmp.Last() != res.ToString())
                    toolStripMenuItem15.Text = strTmp[0] + " " + strTmp[1] + " " + res.ToString() + "%";
            }
        }

    }
}
