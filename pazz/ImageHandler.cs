using System.Drawing;
using System.Windows.Forms;
using static pazz.Form1;
using static pazz.ImageComposer;


namespace pazz
{
    public static class ImageHandler
    {
        //Swap selected puzzles
        public static void ImageSwap(PictureBox ob1, PictureBox ob2)
        {
            Point buf = ob1.Location;
            ob1.Location = ob2.Location;
            ob2.Location = buf;
            ob2.BorderStyle = BorderStyle.None;
            ob1.BorderStyle = BorderStyle.None;
        }
        //Spin puzzle
        public static Image SpinImage(Image im)
        {
            im.RotateFlip(RotateFlipType.Rotate180FlipX);
            return im;
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
        public static void Puzzle_Click(object sender, MouseEventArgs e)
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

                    ImageSwap(Fixed_image, second_fixed_image);

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

        public static void CheckPuzzles(PictureBox pictureBox1, TextBox textBox1, TextBox textBox2, Panel panel1)
        {
            PartSetter(pictureBox1, textBox1, textBox2);
            for (int i = 0; i < X_parts_number; i++)
            {
                for (int j = 0; j < Y_parts_number; j++)
                {
                    PictureBox pointim = (PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part + Box_x_part / 2, j * Box_y_part + Box_y_part / 2));
                    if (pointim == null || !ImageComparison(new Bitmap(Input_image.Clone(new Rectangle(i * X_part, j * Y_part, Input_image.Width / X_parts_number, Input_image.Height / Y_parts_number), Input_image.PixelFormat)), new Bitmap(pointim.Image)))
                    {
                        MessageBox.Show("Wrong");
                        return;
                    }
                }
            }
            MessageBox.Show("Correct");
        }

        public static void FileSelect(PictureBox pictureBox1)
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
    }
}
