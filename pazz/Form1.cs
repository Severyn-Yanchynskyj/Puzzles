using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static pazz.ImageComposer;
using static pazz.ImageSplitter;
using static pazz.ImageHandler;

namespace pazz
{
    public partial class Form1 : Form
    {

        public static Size initSize;
        
        public static int X_parts_number = 0;
        public static int Y_parts_number = 0;
        public static int X_part = 0;
        public static int Y_part = 0;
        public static int Box_x_part = 0;
        public static int Box_y_part = 0;
        public const int Up_bound = 625;
        public const int Low_bound = 16;
        public static PictureBox Fixed_image = null;
        public static Bitmap Input_image = null;
        public static readonly DirectoryInfo MyDirectory = new(@"puzzles\");

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
        private static void Scale(Control control, Size newSize)
        {        
            int width = newSize.Width - initSize.Width;
            control.Left += (control.Left * width) / initSize.Width;
            control.Width += (control.Width * width) / initSize.Width;
            int height = newSize.Height - initSize.Height;
            control.Top += (control.Top * height) / initSize.Height;
            control.Height += (control.Height * height) / initSize.Height;
        }
        
        
        //Split image into puzzles and displays them
        private void Split_button_Click(object sender, EventArgs e)
        {
            Splitter(panel1, textBox1, textBox2, pictureBox1, this);            
        }

        //Open file dialog for image selection
        private void Select_button_Click(object sender, EventArgs e)
        {
            FileSelect(pictureBox1);
        }

        //Check if puzzle order is correct
        private void Check_button_Click(object sender, EventArgs e)
        {
            CheckPuzzles(pictureBox1, textBox1, textBox2, panel1);            
        }
        
        //Autocompose puzzles
        private void Compose_button_Click(object sender, EventArgs e)
        {
            ComposionStateBegin(composing_label);
            PartSetter(pictureBox1, textBox1, textBox2);
            ComposionAlgo(panel1, composing_label, this);
            ComposionStateEnd(composing_label, this);            
        }
    }
}
