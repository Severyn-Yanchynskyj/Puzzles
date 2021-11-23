using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace pazz
{
    public partial class Form1 : Form
    {

        private Size initSize;

        int X_parts_number = 0;
        int Y_parts_number = 0;
        int X_part = 0;
        int Y_part = 0;
        int Box_x_part = 0;
        int Box_y_part = 0;
        const int Up_bound = 625;
        const int Low_bound = 16;
        PictureBox Fixed_image = null;
        Bitmap Input_image = null;
        readonly DirectoryInfo Directory = new(@"puzzles\");

        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {  
            initSize = Size;  
        }

        //Scale all controls of form and panel
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            foreach (Control cnt in Controls)
                Scale(cnt, Size);
            foreach (Control cnt in panel1.Controls)
                Scale(cnt, Size);
            initSize = Size;
            
        }

        //Scale control
        private void Scale(Control control, Size newSize)
        {        
            int width = newSize.Width - initSize.Width;
            control.Left += (control.Left * width) / initSize.Width;
            control.Width += (control.Width * width) / initSize.Width;
            int height = newSize.Height - initSize.Height;
            control.Top += (control.Top * height) / initSize.Height;
            control.Height += (control.Height * height) / initSize.Height;
        }

        private void PartSetter()
        {
            X_parts_number = int.Parse(textBox1.Text);
            Y_parts_number = int.Parse(textBox2.Text);

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

        private void ComposionStateBegin()
        {
            composing_label.Visible = true;
            composing_label.Refresh();
        }
        private void ComposionStateEnd()
        {
            composing_label.Visible = false;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = true;
            MaximizeBox = true;
        }
        private void SplitedMode()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;
        }
        private List<KeyValuePair<int, int>> PairsCreate()
        {
            List<KeyValuePair<int, int>> pairs = new List<KeyValuePair<int, int>>();
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

        //Handle click on puzzle
        protected void Puzzle_Click(object sender, MouseEventArgs e)
        {
            PictureBox second_fixed_image = sender as PictureBox;
            if (e.Button == MouseButtons.Left)
            {//Swap selected puzzle with remembered one after second leftclick 
                second_fixed_image.BorderStyle = BorderStyle.Fixed3D;
                if (Fixed_image != null)
                {
                    if (Fixed_image == second_fixed_image)
                    {
                        Fixed_image.BorderStyle = BorderStyle.None;
                        Fixed_image = null;                       
                        return;
                    }
                    
                    ImageSwap( Fixed_image,second_fixed_image);
                   
                    Fixed_image = null;
                }
                else
                {//Remember selected puzzle after first leftclick
                    Fixed_image = second_fixed_image;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {//Spin selected puzzle after rightclick              
                second_fixed_image.Image = SpinImage(second_fixed_image.Image);
            }
        }

        //Spin puzzle
        public static Image SpinImage(Image im)
        {
            im.RotateFlip(RotateFlipType.Rotate180FlipX);
            return im;
        }

        //Swap selected puzzles
        public static void ImageSwap( PictureBox ob1, PictureBox ob2)
        {
            Point buf = ob1.Location;
            ob1.Location = ob2.Location;
            ob2.Location = buf;
            ob2.BorderStyle = BorderStyle.None;
            ob1.BorderStyle = BorderStyle.None;           
        }

        //Compare 2 images
        public static bool ImageComparison(Bitmap i1, Bitmap i2) 
        {
            if (i1.Size == i2.Size)
            {
                for (int jj = 0; jj < i1.Width; jj++)
                {
                    for (int ii = 0; ii < i1.Height; ii++)
                    {

                        if (i1.GetPixel(jj, ii) != i2.GetPixel(jj, ii))
                        {
                            return false;
                        }
                    }
                }
            }
            else return false;
            
            return true;
        }


        //Save puzzles to folder
        public static void SavePuzzles(int x_parts_number, int y_parts_number, int box_x_part, int box_y_part, Panel panel1, DirectoryInfo directory)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            directory.Delete(true);
            bool exists = System.IO.Directory.Exists(directory.ToString());
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(directory.ToString());
            }              
            Random rand = new();
            for (int i = 0; i < x_parts_number; i++)
            {
                for (int j = 0; j < y_parts_number; j++)
                {
                    PictureBox pointim = (PictureBox)panel1.GetChildAtPoint(new Point(i * box_x_part, j * box_y_part));
                    if (pointim != null)
                    {
                        int name_random_bound = 1000;
                        string imagePath = @"puzzles\" + rand.Next(0, name_random_bound).ToString() + i.ToString() + rand.Next(0, name_random_bound).ToString() + j.ToString() + ".png";
                        pointim.Image.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }                    
                }
            }           
        }

        //Split image into puzzles and displays them
        private void Split_button_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            if (!int.TryParse(textBox1.Text, out int int_check) || !int.TryParse(textBox2.Text, out int_check))
            {
                MessageBox.Show("This is a number only field");
            }
            else // split into methods
            {
                if (Up_bound >= int.Parse(textBox1.Text) * int.Parse(textBox2.Text) & int.Parse(textBox1.Text) * int.Parse(textBox2.Text) >= Low_bound & int.Parse(textBox1.Text) > 0 & int.Parse(textBox2.Text) > 0)
                {
                    PartSetter();
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
                                Image img = pazzle.Image;
                                img.RotateFlip(RotateFlipType.Rotate180FlipX);
                                pazzle.Image = img;
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
                    SavePuzzles(X_parts_number, Y_parts_number, Box_x_part, Box_y_part, panel1, Directory);
                    SplitedMode();
                }
                else
                {
                    MessageBox.Show("Quantity of puzzles must be between 16 and 625");
                }
            }
        }

        //Open file dialog for image selection
        private void Select_button_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select image folder";
            op.Filter = "Image Files|*.png;";
            DialogResult fol = op.ShowDialog();
            if (fol == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(op.FileName);
            }
        }

        //Check if puzzle order is correct
        private void Check_button_Click(object sender, EventArgs e)
        {
            PartSetter();
            for (int i = 0; i < X_parts_number; i++)
            {
                for (int j = 0; j < Y_parts_number; j++)
                {
                    PictureBox pointim = (PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part+ Box_x_part/2, j * Box_y_part + Box_y_part / 2));
                    if (pointim == null || !ImageComparison(new Bitmap(Input_image.Clone(new Rectangle(i * X_part, j * Y_part, Input_image.Width / X_parts_number, Input_image.Height / Y_parts_number), Input_image.PixelFormat)), new Bitmap(pointim.Image)))
                    {
                        MessageBox.Show("Wrong");
                        return;
                    }                   
                } 
            }
            MessageBox.Show("Correct");            
        }
        
        //Autocompose puzzles
        private void Compose_button_Click(object sender, EventArgs e)
        {
            ComposionStateBegin();
            List<FileInfo> dirlist = new(Directory.GetFiles().ToList());
            PartSetter();
            for (int i = 0; i < X_parts_number; i++)
            {
                for (int j = 0; j < Y_parts_number; j++)
                {
                    Bitmap f = new(Input_image.Clone(new Rectangle(i * X_part, j * Y_part, Input_image.Width / X_parts_number, Input_image.Height / Y_parts_number), Input_image.PixelFormat));
                    
                    foreach (FileInfo file in dirlist)
                    {
                        if ((PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part, j * Box_y_part)) == null)
                        {
                            ComposionStateEnd();
                            MessageBox.Show("Too small size of puzzles");                           
                            return;
                        }
                        if (ImageComparison(f, new Bitmap(Image.FromFile(file.ToString()))))
                        {
                            PictureBox pointim = (PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part, j * Box_y_part));
                            pointim.Image = Image.FromFile(file.ToString());
                            dirlist.Remove(file);
                            pointim.Refresh();
                            break;
                        }
                        else if (ImageComparison(f, new Bitmap(SpinImage(Image.FromFile(file.ToString())))))
                        {
                            PictureBox pointim = (PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part, j * Box_y_part));
                            pointim.Image = SpinImage(Image.FromFile(file.ToString()));
                            dirlist.Remove(file);
                            pointim.Refresh();
                            break;
                        }
                    }
                }                
            }
            ComposionStateEnd();            
        }
    }
}
