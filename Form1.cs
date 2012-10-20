using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace colors
{
    public partial class Form1 : Form
    {
        public static System.Drawing.Graphics g;
        public static System.Drawing.Pen MyPen;
        //координаты
        public static int x0 = 0;
        public static int y0 = 0;
        //время
        public static System.DateTime time;
        //скорость
        public static double vel = 0, v = 0, vel_old = 0, v_old = 0;
        //ускорение
        public static double a = 0, acc_old = 0;
        //коэффициент чувствительности (для перевода скорости в цвет)
        public static int k = 200;

        public static Color prevclr = Color.Turquoise;
        Bitmap bmp;
        //бэкап для отмены
        Bitmap bmp1;
        Color pen_color_old;
        float pen_width_old;

        public static double velocity(int x, int y, System.TimeSpan t)
        {
            double s = Math.Sqrt((x - x0) * (x - x0) + (y - y0) * (y - y0));
            return s / t.TotalMilliseconds;
        }

        public static double acceleration(double v, System.TimeSpan t)
        {
            double s = v - vel;
            return s / t.TotalMilliseconds;
        }
        public static Color fromHSV(int h, int s, int v)
        {
            double t = h / 60;
            int hi = Convert.ToInt32(Math.Ceiling(t));
            double vmin = (100 - s) * v / 100;
            double a = (v - vmin) * (h % 60) / 60;
            double vinc = vmin + a;
            double vdec = v - a;
            double r;
            double g;
            double b;
            switch (hi)
            {
                case 0:
                    r = v;
                    g = vinc;
                    b = vmin;
                    break;
                case 1:
                    r = vdec;
                    g = v;
                    b = vmin;
                    break;
                case 2:
                    r = vmin;
                    g = v;
                    b = vinc;
                    break;
                case 3:
                    r = vmin;
                    g = vdec;
                    b = vdec;
                    break;
                case 4:
                    r = vinc;
                    g = vmin;
                    b = v;
                    break;
                case 5:
                    r = v;
                    g = vmin;
                    b = vdec;
                    break;
                default:
                    r = 0;
                    g = 0;
                    b = 0;
                    break;
            }
            return Color.FromArgb(Convert.ToInt32(r*255),Convert.ToInt32(g*255),Convert.ToInt32(b*255));
        }

        public static Color ColorFromVelocity(double v)
        {
            //цветовая модель HSV
            int h = Convert.ToInt32(v * k) % 360;

            int s = 1;
            int vi = 1;

            //перевод в RGB
            
            return fromHSV(h,s,vi);
        }

        public Rectangle GetScreen()
        {
            return Screen.FromControl(this).Bounds;
        }

        public Form1()
        {
            InitializeComponent();

            koef.Text = k.ToString();

            x0 = 0;
            y0 = 0; 
            bmp = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            bmp.MakeTransparent(Color.Black);
            pictureBox1.Image = bmp;

            
            MyPen = new System.Drawing.Pen(Color.Violet, 4);
            MyPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.
                Drawing2D.LineCap.Round, System.Drawing.Drawing2D.DashCap.Round);


            time = System.DateTime.Now;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить картинку как ...";
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.Filter =
                "Bitmap File(*.bmp)|*.bmp|" +
                "GIF File(*.gif)|*.gif|" +
                "JPEG File(*.jpg)|*.jpg|" +
                "TIF File(*.tif)|*.tif|" +
                "PNG File(*.png)|*.png";
            savedialog.ShowHelp = true;
            // If selected, save
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                // Get the user-selected file name
                string fileName = savedialog.FileName;
                // Get the extension
                string strFilExtn =
                    fileName.Remove(0, fileName.Length - 3);
                // Save file
                switch (strFilExtn)
                {
                    case "bmp":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case "jpg":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "gif":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case "tif":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                        break;
                    case "png":
                        bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        break;
                }

            }
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            if (e.Button == MouseButtons.Left)
            {
                System.TimeSpan timesp = System.DateTime.Now - time;
                if (timesp.TotalMilliseconds >= 1)
                {
                    //скорость
                    v = velocity(e.X, e.Y, System.DateTime.Now - time);
                    MyPen.Color = ColorFromVelocity(v);
                    
                    //ускорение
                    a = acceleration(v, System.DateTime.Now - time);

                    //толщина
                    if ((Math.Abs(a * k / 500) > 0.01) && (MyPen.Width <= 50) || (MyPen.Width == 2))
                        MyPen.Width += 1;
                    else
                        MyPen.Width = MyPen.Width - 1;

                    label1.Text = "скорость = " + v.ToString(); 
                    label2.Text = "ускорение = " + a.ToString();
                    time = System.DateTime.Now;
                }
                g.DrawLine(MyPen, x0, y0, e.X, e.Y);
                if (checkBox1.Checked)
                {
                    g.DrawLine(MyPen, bmp.Width - x0, y0, bmp.Width - e.X, e.Y);
                }
                if (checkBox2.Checked)
                {
                    g.DrawLine(MyPen, x0, bmp.Height - y0, e.X, bmp.Height - e.Y);
                }
                if (checkBox3.Checked)
                {
                    g.DrawLine(MyPen, bmp.Width - x0, bmp.Height - y0, bmp.Width - e.X, bmp.Height - e.Y);
                }
            }
            x0 = e.X;
            y0 = e.Y;
            vel = v;

            g.Dispose();

            pictureBox1.Invalidate();

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.DrawLine(MyPen, e.X, e.Y, e.X + 4, e.Y);
            if (checkBox1.Checked)
            {
                g.DrawLine(MyPen, bmp.Width - e.X, e.Y, bmp.Width - e.X + 4, e.Y);
            }
            if (checkBox2.Checked)
            {
                g.DrawLine(MyPen, e.X, bmp.Height - e.Y, e.X + 4, bmp.Height - e.Y);
            }
            if (checkBox3.Checked)
            {
                g.DrawLine(MyPen, bmp.Width - e.X, bmp.Height - e.Y, bmp.Width - e.X + 4, bmp.Height - e.Y);
            }
            g.Dispose();

            pictureBox1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bmp.Dispose();
            pictureBox1.Image.Dispose();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            //MyPen.Color = Color.Turquoise;
            MyPen.Width = 4;
            button5.Enabled = false;

        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            if ((pictureBox1.Width > 0) && (pictureBox1.Height > 0))
            {
                bmp = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
                bmp.MakeTransparent(Color.Black);
            }
            //чтобы при сворачивании не умирало. Вообще костыль.

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listBox1.SelectedIndex)
            {
            case 0:
                {
                    MyPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.
                Drawing2D.LineCap.Round, System.Drawing.Drawing2D.DashCap.Round);
                    break;
                }
            case 1:
                {
                    MyPen.SetLineCap(System.Drawing.Drawing2D.LineCap.ArrowAnchor, System.Drawing.
                Drawing2D.LineCap.ArrowAnchor, System.Drawing.Drawing2D.DashCap.Round);
                    break;
                }
            case 2:
                {
                    MyPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Square, System.Drawing.
                Drawing2D.LineCap.Square, System.Drawing.Drawing2D.DashCap.Round);
                    break;
                }
            case 3:
                {
                    MyPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Triangle, System.Drawing.
                Drawing2D.LineCap.Triangle, System.Drawing.Drawing2D.DashCap.Round);
                    break;
                }
            case 4:
                {
                    MyPen.SetLineCap(System.Drawing.Drawing2D.LineCap.DiamondAnchor, System.Drawing.
                Drawing2D.LineCap.DiamondAnchor, System.Drawing.Drawing2D.DashCap.Round);
                   
                    break;
                }
            case 5:
                {
                    System.Drawing.Drawing2D.GraphicsPath hPath = new System.Drawing.Drawing2D.GraphicsPath();

                    Rectangle rect = new Rectangle(0,0,(int)MyPen.Width,(int)MyPen.Width);
                    hPath.AddString("O", FontFamily.GenericSansSerif, (int)FontStyle.Italic, (float)0.05, rect, StringFormat.GenericDefault);

                    System.Drawing.Drawing2D.CustomLineCap HookCap = new System.Drawing.Drawing2D.CustomLineCap(null, hPath);

                    HookCap.SetStrokeCaps(System.Drawing.Drawing2D.LineCap.Round, System.Drawing.Drawing2D.LineCap.Round);

                    MyPen.CustomStartCap = HookCap;
                    MyPen.CustomEndCap = HookCap;
                    MyPen.SetLineCap(System.Drawing.Drawing2D.LineCap.Custom, System.Drawing.
                Drawing2D.LineCap.Custom, System.Drawing.Drawing2D.DashCap.Round);

                    break;
                }
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = checkBox4.Checked;
            checkBox2.Checked = checkBox4.Checked;
            checkBox3.Checked = checkBox4.Checked;
        }

        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
                if (k < 500)
                    k += 1;
                koef.Text = k.ToString();
        }

        private void button4_MouseDown(object sender, MouseEventArgs e)
        {
            if (k > 0)
                k -= 1;
            koef.Text = k.ToString();
        }

        private void koef_TextChanged(object sender, EventArgs e)
        {
            int t = 0;
            if (koef.Text == "")
            {
                k = 0;
                return;
            }
            if (Int32.TryParse(koef.Text, out t))
            {
                if (t > 0)
                {
                    if (t <= 500)
                    {
                        k = t;
                    }
                    else
                    {
                        k = 500;
                    }
                }
                else
                {
                    k = 0;
                }
            }
            koef.Text = k.ToString();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //бэкапы
            bmp1 = (Bitmap)bmp.Clone();
            vel_old = vel;
            v_old = v;
            acc_old = a;
            pen_color_old = MyPen.Color;
            pen_width_old = MyPen.Width;
            button5.Enabled = true;
        }

        private void button5_MouseClick(object sender, MouseEventArgs e)
        {
            bmp = bmp1;
            pictureBox1.Image = bmp;
            vel = vel_old;
            v = v_old;
            a = acc_old;
            MyPen.Color = pen_color_old;
            MyPen.Width = pen_width_old;
            button5.Enabled = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (Form1.ActiveForm != null)
            {
                if ((Form1.ActiveForm.Width > 1) && (Form1.ActiveForm.Height > 1))
                {
                    int new_W = Form1.ActiveForm.Width;
                    int new_H = Convert.ToInt32(new_W * (3.0 / 4.0));
                    if (new_H + 100 > Form1.ActiveForm.Height)
                    {
                        new_H = Form1.ActiveForm.Height - 100;
                        new_W = Convert.ToInt32(new_H * (4.0 / 3.0));
                    }
                    pictureBox1.Size = new Size(new_W, new_H);
                }
            }
            

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }


       
    }
}