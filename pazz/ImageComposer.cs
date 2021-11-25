using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static pazz.Form1;
using static pazz.ImageHandler;

namespace pazz
{
    public static class ImageComposer
    {
        public static void PartSetter(PictureBox pictureBox1, TextBox textBox1, TextBox textBox2)
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
        public static void ComposionStateBegin(Label composing_label)
        {
            composing_label.Visible = true;
            composing_label.Refresh();
        }
        public static void ComposionStateEnd(Label composing_label, Form1 f)
        {
            composing_label.Visible = false;
            f.FormBorderStyle = FormBorderStyle.Sizable;
            f.MinimizeBox = true;
            f.MaximizeBox = true;
        }
        
        public static void ComposionAlgo(Panel panel1, Label l, Form1 formex )
        {
            List<FileInfo> dirlist = new(MyDirectory.GetFiles().ToList());
            for (int i = 0; i < X_parts_number; i++)
            {
                for (int j = 0; j < Y_parts_number; j++)
                {
                    Bitmap f = new(Input_image.Clone(new Rectangle(i * X_part, j * Y_part, Input_image.Width / X_parts_number, Input_image.Height / Y_parts_number), Input_image.PixelFormat));

                    foreach (FileInfo file in dirlist)
                    {
                        if ((PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part, j * Box_y_part)) == null)
                        {
                            ComposionStateEnd(l, formex);
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
        }
    }
}
