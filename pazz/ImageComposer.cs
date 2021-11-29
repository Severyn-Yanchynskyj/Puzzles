using System;
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

        //Merge 2 images
        public static Bitmap ImConc(Bitmap bo, Bitmap bt)
        {
            if ( bt == null)
            {
                return bo;
            }
            Bitmap rim = new(bo.Width, bo.Height + bt.Height);
            for (int i = 0; i < bo.Width; i++)
            {
                for (int j = 0; j < bo.Height; j++)
                {
                    
                    rim.SetPixel(i,j,bo.GetPixel(i,j));
                }
            }
            for (int i = 0; i < bo.Width; i++)
            {               
                for (int j = bo.Height; j < rim.Height; j++)
                {

                    rim.SetPixel(i, j, bt.GetPixel(i, j- bo.Height));
                }
            }
            return rim;
        }

        //Merge 2 images if their bound pixels are compatible
        public static Bitmap ImageBoundСompatibility(Bitmap bo, Bitmap bt)
        {
            int maxpixelvalue = 255;
            int colornumber = 3;
            int maxdif = bo.Width * colornumber * maxpixelvalue;
            double firstImdifsum = 0;//sum of differences of bound pixels on one side           
            for (int i = 0; i < bo.Width; i++)
            {
                firstImdifsum += Math.Abs(bo.GetPixel(i, bo.Height - 1).R - bt.GetPixel(i, 0).R);
                firstImdifsum += Math.Abs(bo.GetPixel(i, bo.Height - 1).G - bt.GetPixel(i, 0).G);
                firstImdifsum += Math.Abs(bo.GetPixel(i, bo.Height - 1).B - bt.GetPixel(i, 0).B);
            }
            double secImdifsum = 0;//sum of differences of bound pixels on the other side 
            for (int i = 0; i < bo.Width; i++)
            {
                secImdifsum += Math.Abs(bt.GetPixel(i, bt.Height - 1).R - bo.GetPixel(i, 0).R);
                secImdifsum += Math.Abs(bt.GetPixel(i, bt.Height - 1).G - bo.GetPixel(i, 0).G);
                secImdifsum += Math.Abs(bt.GetPixel(i, bt.Height - 1).B - bo.GetPixel(i, 0).B);
            }
            //the percentage of incompatibility, which must be less than 1 % for the puzzles to be compatible
            double po = firstImdifsum / maxdif;
            double pt = secImdifsum / maxdif;
            double limitpercent = 0.01;
            if (po < limitpercent)
            {
                return ImConc(bo, bt);
            }
            else if (pt < limitpercent)
            {
                return ImConc(bt, bo);
            }
            return null;
        }
        
        public static void ComposionAlgo(PictureBox p, PictureBox p2)
        {
            dirlist = new(MyDirectory.GetFiles().ToList());
            List<Bitmap> lineslist = new();
            Bitmap startpuzzle;
            //compose columns of puzzles one by one
            for (int i = 0; i < X_parts_number; i++)
            {
                startpuzzle = (Bitmap)Image.FromFile(dirlist[0].FullName);
                dirlist.RemoveAt(0);
                int imcount = 1;
                while (imcount != Y_parts_number)
                {
                    foreach (FileInfo f in dirlist)
                    {
                        Bitmap compres = ImageBoundСompatibility(startpuzzle, (Bitmap)Image.FromFile(f.FullName));
                        if (compres != null)
                        {
                            startpuzzle = compres;
                            imcount++;
                            dirlist.Remove(f);
                            p2.Image = startpuzzle;
                            p2.Refresh();
                            break;                           
                        }

                    }

                }
                //rotate composed column and store it to list
                Image rotstim = startpuzzle;
                rotstim.RotateFlip(RotateFlipType.Rotate90FlipNone);
                lineslist.Add((Bitmap)rotstim);
            }
            //compose rotated composed columns like another column
            int secimcount = 1;
            Bitmap finalpuzzle = lineslist[0];
            lineslist.RemoveAt(0);
            while (secimcount != X_parts_number)
            {
                foreach (Bitmap bl in lineslist)
                {
                    Bitmap compres = ImageBoundСompatibility(finalpuzzle, bl);
                    if (compres != null)
                    {
                        finalpuzzle = compres;
                        secimcount++;
                        lineslist.Remove(bl);
                        p2.Image = finalpuzzle;
                        p2.Refresh();
                        break;                         
                    }

                }

            }
            //rotate image to the correct position
            Image rotfim = finalpuzzle;
            rotfim.RotateFlip(RotateFlipType.Rotate270FlipNone);
            rotfim = new Bitmap(rotfim, p.Size); 
            p2.Image = rotfim;
        }
    }
}
