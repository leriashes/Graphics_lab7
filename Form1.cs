using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics_lab7
{
    public partial class MainForm : Form
    {
        private double coef = 2;
        private int k = 1;

        public MainForm()
        {
            InitializeComponent();
        }

        private Bitmap MakeImgWithBordersCopy(PictureBox picture, int bordersize)
        {
            Bitmap imgStart = new Bitmap(picture.Image);

            Bitmap img = new Bitmap(imgStart.Width + bordersize * 2, imgStart.Height + bordersize * 2);

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    int x, y;

                    if (i < bordersize)
                    {
                        x = 0;
                    }
                    else if (i >= bordersize + imgStart.Width)
                    {
                        x = imgStart.Width - 1;
                    }
                    else
                    {
                        x = i - bordersize;
                    }

                    if (j < bordersize)
                    {
                        y = 0;
                    }
                    else if (j >= bordersize + imgStart.Height)
                    {
                        y = imgStart.Height - 1;
                    }
                    else
                    {
                        y = j - bordersize;
                    }

                    img.SetPixel(i, j, imgStart.GetPixel(x, y));
                }
            }

            return img;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            coef = Convert.ToDouble(textBox1.Text);

            Bitmap imgStart = MakeImgWithBordersCopy(pictureBox1, 1);

            int halfsize = 25;
            int size = halfsize * 2;
            double hs = halfsize / coef;
            double s = hs * 2;


            Bitmap imgRes = new Bitmap(Convert.ToInt32(Math.Round(pictureBox1.Image.Width * coef)), Convert.ToInt32(Math.Round(pictureBox1.Image.Height * coef)));

            for (int i = 0; i < imgRes.Width; i++) 
            {
                double x = (i * 2 + 1) * hs;

                int sx = Convert.ToInt32(Math.Truncate(x / halfsize));

                if (sx % 2 == 0)
                {
                    sx -= 1;
                }

                sx *= halfsize;

                int si = (sx / halfsize + 1) / 2;

                for (int j = 0; j < imgRes.Height; j++)
                {
                    double y = (j * 2 + 1) * hs;

                    int sy = Convert.ToInt32(Math.Truncate(y / halfsize));

                    if (sy % 2 == 0)
                    {
                        sy -= 1;
                    }

                    sy *= halfsize;

                    int sj = (sy / halfsize + 1) / 2;


                    Color pixel11 = imgStart.GetPixel(si, sj);
                    Color pixel12 = imgStart.GetPixel(si, sj + 1);
                    Color pixel21 = imgStart.GetPixel(si + 1, sj);
                    Color pixel22 = imgStart.GetPixel(si + 1, sj + 1);



                    int R, G, B;

                    double f1 = (sx + size - x) / size * pixel11.R + (x - sx) / size * pixel21.R;
                    double f2 = (sx + size - x) / size * pixel12.R + (x - sx) / size * pixel22.R;

                    R = Convert.ToInt32(Math.Round((sy + size - y) / size * f1 + (y - sy) / size * f2));

                    f1 = (sx + size - x) / size * pixel11.G + (x - sx) / size * pixel21.G;
                    f2 = (sx + size - x) / size * pixel12.G + (x - sx) / size * pixel22.G;

                    G = Convert.ToInt32(Math.Round((sy + size - y) / size * f1 + (y - sy) / size * f2));

                    f1 = (sx + size - x) / size * pixel11.B + (x - sx) / size * pixel21.B;
                    f2 = (sx + size - x) / size * pixel12.B + (x - sx) / size * pixel22.B;

                    B = Convert.ToInt32(Math.Round((sy + size - y) / size * f1 + (y - sy) / size * f2));

                    imgRes.SetPixel(i, j, Color.FromArgb(255, R, G, B));
                }
            }

            string st = k++ + ".png";

            imgRes.Save(st, ImageFormat.Png);
            pictureBox2.Image = imgRes;
        }

        private void открытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }
    }
}
