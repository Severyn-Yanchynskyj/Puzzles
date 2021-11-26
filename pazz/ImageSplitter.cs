using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static pazz.Form1;
using static pazz.ImageHandler;

namespace pazz
{
    public static class ImageSplitter
    {

        public static List<KeyValuePair<int, int>> PairsCreate()
        {
            List<KeyValuePair<int, int>> pairs = new();
            for (int i = 0; i < X_parts_number; i++)
            {
                for (int j = 0; j < Y_parts_number; j++)
                {
                    var el = new KeyValuePair<int, int>(i, j);
                    pairs.Add(el);
                }
            }
            return pairs;

        }
        public static void ClearFolder()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            
            bool exists = Directory.Exists(MyDirectory.ToString());
            if (exists)
            {
                foreach (FileInfo file in MyDirectory.GetFiles())
                {
                    file.Delete();
                }
                Directory.CreateDirectory(MyDirectory.ToString());
            }
            else if (!exists)
            {
                Directory.CreateDirectory(MyDirectory.ToString());
            }
        }
        
        public static void PartSetter(TextBox textBox1, TextBox textBox2)
        {
            X_parts_number = int.Parse(textBox1.Text);
            Y_parts_number = int.Parse(textBox2.Text);
        }
        public static void SecondPartSetter(PictureBox pictureBox1)
        {
            Input_image = new(pictureBox1.Image);
            if (X_parts_number != 0 & Y_parts_number != 0)
            {
                X_part = Input_image.Width / X_parts_number;
                Y_part = Input_image.Height / Y_parts_number;

            }
            if (X_part != 0 & Y_part != 0)
            {
                Box_x_part = pictureBox1.Width / X_parts_number;
                Box_y_part = pictureBox1.Height / Y_parts_number;
            }
        }
        
        public static void SplitedMode(Form1 f)
        {
            f.FormBorderStyle = FormBorderStyle.FixedSingle;
            f.MinimizeBox = false;
            f.MaximizeBox = false;
        }
        public static void Splitter(Panel panel1, TextBox textBox1, TextBox textBox2, PictureBox pictureBox1, Form1 f)
        {
            if (Fixed_image != null)
            {

                Fixed_image.BorderStyle = BorderStyle.None;
                Fixed_image = null;

            }
            panel1.Controls.Clear();
            if (!int.TryParse(textBox1.Text, out int int_check) || !int.TryParse(textBox2.Text, out int_check))
            {
                MessageBox.Show("This is a number only field");
            }
            else
            {
                if (Up_bound >= int.Parse(textBox1.Text) * int.Parse(textBox2.Text) & int.Parse(textBox1.Text) * int.Parse(textBox2.Text) >= Low_bound & int.Parse(textBox1.Text) > 0 & int.Parse(textBox2.Text) > 0)
                {
                    ClearFolder();
                    PartSetter(textBox1, textBox2);
                    SecondPartSetter(pictureBox1);
                    List<KeyValuePair<int, int>> pairs = PairsCreate();
                    Random r = new();
                    for (int i = 0; i < X_parts_number; i++)
                    {
                        for (int j = 0; j < Y_parts_number; j++)
                        {
                            Bitmap iter_image = Input_image.Clone(new Rectangle(i * X_part, j * Y_part, Input_image.Width / X_parts_number, Input_image.Height / Y_parts_number), Input_image.PixelFormat);
                            PictureBox pazzle = new();
                            pazzle.Image = iter_image;

                            int name_random_bound = 1000;
                            string imagePath = MyDirectory + r.Next(0, name_random_bound).ToString() + i.ToString() + r.Next(0, name_random_bound).ToString() + j.ToString() + ".png";
                            iter_image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);

                            int random_bound = 10;
                            int ro = r.Next(0, random_bound);
                            if (ro % 2 == 0)
                            {
                                SpinImage(pazzle.Image);
                            }
                            pazzle.MouseClick += new MouseEventHandler(Puzzle_Click);
                            panel1.Controls.Add(pazzle);
                            int ri = r.Next(0, pairs.Count);
                            pazzle.Location = new Point(pairs[ri].Key * Box_x_part, pairs[ri].Value * Box_y_part);
                            pazzle.SizeMode = PictureBoxSizeMode.StretchImage;
                            pazzle.Size = new Size(pictureBox1.Width / X_parts_number, pictureBox1.Height / Y_parts_number);
                            pairs.RemoveAt(ri);

                        }
                    }
                    SplitedMode(f);
                }
                else
                {
                    MessageBox.Show("Quantity of puzzles must be between 16 and 625");
                }
            }
        }
    }
}
