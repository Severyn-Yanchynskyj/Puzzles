﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static pazz.Form1;
using static pazz.ImageComposer;
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
        //Save puzzles to folder
        public static void SavePuzzles(Panel panel1)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            MyDirectory.Delete(true);
            bool exists = Directory.Exists(MyDirectory.ToString());
            if (!exists)
            {
                Directory.CreateDirectory(MyDirectory.ToString());
            }
            Random rand = new();
            for (int i = 0; i < X_parts_number; i++)
            {
                for (int j = 0; j < Y_parts_number; j++)
                {
                    PictureBox pointim = (PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part, j * Box_y_part));
                    if (pointim != null)
                    {
                        int name_random_bound = 1000;
                        string imagePath = @"puzzles\" + rand.Next(0, name_random_bound).ToString() + i.ToString() + rand.Next(0, name_random_bound).ToString() + j.ToString() + ".png";
                        pointim.Image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
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
            panel1.Controls.Clear();
            if (!int.TryParse(textBox1.Text, out int int_check) || !int.TryParse(textBox2.Text, out int_check))
            {
                MessageBox.Show("This is a number only field");
            }
            else
            {
                if (Up_bound >= int.Parse(textBox1.Text) * int.Parse(textBox2.Text) & int.Parse(textBox1.Text) * int.Parse(textBox2.Text) >= Low_bound & int.Parse(textBox1.Text) > 0 & int.Parse(textBox2.Text) > 0)
                {
                    PartSetter(pictureBox1, textBox1, textBox2);
                    List<KeyValuePair<int, int>> pairs = PairsCreate();
                    Random r = new();
                    for (int i = 0; i < X_parts_number; i++)
                    {
                        for (int j = 0; j < Y_parts_number; j++)
                        {
                            Bitmap iter_image = Input_image.Clone(new Rectangle(i * X_part, j * Y_part, Input_image.Width / X_parts_number, Input_image.Height / Y_parts_number), Input_image.PixelFormat);
                            PictureBox pazzle = new();
                            pazzle.Image = iter_image;
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
                    SavePuzzles(panel1);
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