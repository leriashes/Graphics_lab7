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

        private void MakeBilinear() 
        {
            Bitmap imgStart = MakeImgWithBordersCopy(pictureBox1, 1);

            int halfsize = 25;
            int size = halfsize * 2;
            double hs = halfsize / coef;


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

        private void MakeBicubic()
        {
            Bitmap imgStart = MakeImgWithBordersCopy(pictureBox1, 2);

            int halfsize = 25;
            int size = halfsize * 2;
            double hs = halfsize / coef;


            Bitmap imgRes = new Bitmap(Convert.ToInt32(Math.Round(pictureBox1.Image.Width * coef)), Convert.ToInt32(Math.Round(pictureBox1.Image.Height * coef)));

            for (int i = 0; i < imgRes.Width; i++)
            {
                double x = (i * 2 + 1) * hs;

                int sx = Convert.ToInt32(Math.Truncate(x / halfsize));

                //x = (x - Math.Floor(x)) / 2;

                if (sx % 2 == 0)
                {
                    sx -= 1;
                }

                sx *= halfsize;

                x = (x - sx) / size;

                int si = (sx / halfsize + 1) / 2;

                for (int j = 0; j < imgRes.Height; j++)
                {
                    double y = (j * 2 + 1) * hs;

                    int sy = Convert.ToInt32(Math.Truncate(y / halfsize));

                    //y = (y - Math.Floor(y)) / 2;

                    if (sy % 2 == 0)
                    {
                        sy -= 1;
                        //y += 0.5;
                    }

                    sy *= halfsize;

                    y = (y - sy) / size;

                    int sj = (sy / halfsize + 1) / 2;

                    Color[] pixels = new Color[16];

                    for (int n = 0; n < 4; n++)
                    {
                        for (int m = 0; m < 4; m++) 
                        {
                            pixels[n * 4 + m] = imgStart.GetPixel(si + n, sj + m);
                        }
                    }

                    double R = 0, G = 0, B = 0;

                    double[] b = new double[16];
                    double sign = 1.0 / 4;

                    for (int n = 0; n < 4; n++)
                    {
                        int d = 1;

                        if (n % 3 == 0)
                        {
                            d *= 3;
                        }

                        for (int m = 0; m < 4; m++)
                        {
                            b[n * 4 + m] = sign / d;

                            for (int l = -1; l <= 2; l++) 
                            {
                                if (l + 1 != n)
                                {
                                    b[n * 4 + m] *= x - l;
                                }

                                if (l + 1 != m)
                                {
                                    b[n * 4 + m] *= y - l;
                                }
                            }

                            if (m % 3 == 0) 
                            {
                                b[n * 4 + m] /= 3;
                            }

                            sign *= -1;
                        }

                        sign *= -1;
                    }

                    //b[5] = (x - 1) * (x - 2) * (x + 1) * (y - 1) * (y - 2) * (y + 1) / 4;
                    //b[6] = x * (x + 1) * (x - 2) * (y - 1) * (y - 2) * (y + 1) / (-4);
                    //b[9] = y * (x - 1) * (x - 2) * (x + 1) * (y + 1) * (y - 2) / (-4);
                    //b[10] = x * y * (x + 1) * (x - 2) * (y + 1) * (y - 2) / 4;

                    //b[4] = x * (x - 1) * (x - 2) * (y - 1) * (y - 2) * (y + 1) / (-12);
                    //b[1] = y * (x - 1) * (x - 2) * (x + 1) * (y - 1) * (y - 2) / (-12);
                    //b[8] = x * y * (x - 1) * (x - 2) * (y + 1) * (y - 2) / 12;
                    //b[2] = x * y * (x + 1) * (x - 2) * (y - 1) * (y - 2) / 12;

                    //b[7] = x * (x - 1) * (x + 1) * (y - 1) * (y - 2) * (y + 1) / 12;
                    //b[13] = y * (x - 1) * (x - 2) * (x + 1) * (y - 1) * (y + 1) / 12;
                    //b[0] = x * y * (x - 1) * (x - 2) * (y - 1) * (y - 2) / 36;
                    //b[11] = x * y * (x - 1) * (x + 1) * (y + 1) * (y - 2) / (-12);

                    //double b1 = (x - 1) * (x - 2) * (x + 1) * (y - 1) * (y - 2) * (y + 1) / 4;
                    //double b2 = x * (x + 1) * (x - 2) * (y - 1) * (y - 2) * (y + 1) / (-4);
                    //double b3 = y * (x - 1) * (x - 2) * (x + 1) * (y + 1) * (y - 2) / (-4);
                    //double b4 = x * y * (x + 1) * (x - 2) * (y + 1) * (y - 2) / 4;

                    //double b5 = x * (x - 1) * (x - 2) * (y - 1) * (y - 2) * (y + 1) / (-12);
                    //double b6 = y * (x - 1) * (x - 2) * (x + 1) * (y - 1) * (y - 2) / (-12);
                    //double b7 = x * y * (x - 1) * (x - 2) * (y + 1) * (y - 2) / 12;
                    //double b8 = x * y * (x + 1) * (x - 2) * (y - 1) * (y - 2) / 12;

                    //double b9 = x * (x - 1) * (x + 1) * (y - 1) * (y - 2) * (y + 1) / 12;
                    //double b10 = y * (x - 1) * (x - 2) * (x + 1) * (y - 1) * (y + 1) / 12;
                    //double b11 = x * y * (x - 1) * (x - 2) * (y - 1) * (y - 2) / 36;
                    //double b12 = x * y * (x - 1) * (x + 1) * (y + 1) * (y - 2) / (-12);

                    //double b13 = x * y * (x + 1) * (x - 2) * (y - 1) * (y + 1) / (-12);
                    //double b14 = x * y * (x - 1) * (x + 1) * (y - 1) * (y - 2) / (-36);
                    //double b15 = x * y * (x - 1) * (x - 2) * (y - 1) * (y + 1) / (-36);
                    //double b16 = x * y * (x - 1) * (x + 1) * (y - 1) * (y + 1) / 36;

                    for (int n = 0; n < 16; n++)
                    {
                        R += b[n] * pixels[n].R;
                        G += b[n] * pixels[n].G;
                        B += b[n] * pixels[n].B;
                    }
                    
                    if (R < 0)
                    {
                        R = 0;
                    }
                    else if (R > 255) 
                    {
                        R = 255;
                    }

                    if (G < 0)
                    {
                        G = 0;
                    }
                    else if (G > 255)
                    {
                        G = 255;
                    }

                    if (B < 0)
                    {
                        B = 0;
                    }
                    else if (B > 255)
                    {
                        B = 255;
                    }

                    imgRes.SetPixel(i, j, Color.FromArgb(255, Convert.ToInt32(Math.Round(R)), Convert.ToInt32(Math.Round(G)), Convert.ToInt32(Math.Round(B))));
                }
            }

            string st = k++ + ".png";

            imgRes.Save(st, ImageFormat.Png);
            pictureBox3.Image = imgRes;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            coef = Convert.ToDouble(textBox1.Text);

            MakeBilinear();
            MakeBicubic();
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }
    }
}
