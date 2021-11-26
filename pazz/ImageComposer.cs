using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static pazz.Form1;

namespace pazz
{
    public static class ImageComposer
    {
        static List<FileInfo> dirlist = new(MyDirectory.GetFiles().ToList());

        //This method allow to autocompose puzzles without knowledge of data textboxes
        public static void ComposionPartSetter()
        {
            dirlist = new(MyDirectory.GetFiles().ToList());
            if (dirlist.Count != 0)
            {
                X_parts_number = Input_image.Width / Image.FromFile(dirlist[0].ToString()).Width;
                Y_parts_number = Input_image.Height / Image.FromFile(dirlist[0].ToString()).Height;
            }
            else { MessageBox.Show("No puzzles"); return; }

            while (X_parts_number * Y_parts_number != dirlist.Count)
            {
                if (X_parts_number > Y_parts_number) { X_parts_number--; }
                else if (X_parts_number < Y_parts_number) { Y_parts_number--; }
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
        
        public static void ComposionAlgo(Panel panel1, Label composion_label, Form1 formex )
        {
            dirlist= new(MyDirectory.GetFiles().OrderBy(f => f.LastWriteTime).ToList());
            for (int i = 0; i < X_parts_number; i++)
            {
                for (int j = 0; j < Y_parts_number; j++)
                {
                    PictureBox pointim = (PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part, j * Box_y_part));
                    if ((PictureBox)panel1.GetChildAtPoint(new Point(i * Box_x_part, j * Box_y_part)) == null || pointim==null || dirlist[0]==null)
                    {
                        ComposionStateEnd(composion_label, formex);
                        MessageBox.Show("Too small size of puzzles");
                        return;
                    }
                    
                    pointim.Image = Image.FromFile(dirlist[0].ToString());
                    dirlist.Remove(dirlist[0]);
                    pointim.Refresh();
                }
            }
        }
    }
}
