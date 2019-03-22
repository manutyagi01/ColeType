using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Windows.Forms;

namespace OSDKeyboardMain1
{
    /// <summary>
    /// Interaction logic for ColorSelectWindow.xaml
    /// </summary>
    /// 

    public enum SelectFor
    {
        NONE,
        FONT,
        BACKGROUND,
        BORDER,
        LINDEX,
        RINDEX,
        MIDDLE,
        RING,
        LITTLE,
        THUMB
    }

    public partial class ColorSelectWindow : Window
    {
        Color currentcolor = Color.FromArgb(255, 255, 0, 0);
        Color currentGradient = Color.FromArgb(255, 255, 0, 0);
        bool colorDragging = false;
        bool gradientDragging = false;
        

        SettingsWindow parentWindow;
        public SelectFor selectFor;

        private static double TASKBAR_HEIGHT = 35;

        public ColorSelectWindow(SettingsWindow parentWin, SelectFor select)
        {
            InitializeComponent();

            parentWindow = parentWin;
            selectFor = select;

            RedTb.Text = "255";
            GreenTb.Text = "0";
            BlueTb.Text = "0";

            gradientDragging = false;
            SolidColorBrush brush1 = new SolidColorBrush(currentGradient);
            CurrentRect.Fill = brush1;

            Draw_ColorSelector();

            Draw_GradientSelector(255D, 0D, 0D);

            Color1.MouseLeave += Color1_MouseLeave;
            Color1.MouseMove += Color1_MouseMove;
            Color1.MouseLeftButtonDown += Color1_MouseLeftButtonDown;
            Color1.MouseLeftButtonUp += Color1_MouseLeftButtonUp;

            Gradient1.MouseLeave += Gradient1_MouseLeave;
            Gradient1.MouseMove += Gradient1_MouseMove;
            Gradient1.MouseLeftButtonDown += Gradient1_MouseLeftButtonDown;
            Gradient1.MouseLeftButtonUp += Gradient1_MouseLeftButtonUp;

            this.Top = SystemParameters.VirtualScreenHeight - (double)this.Height - ((double)TASKBAR_HEIGHT * 3.0d);
            this.Left = SystemParameters.VirtualScreenWidth - (double)this.Width - ((double)TASKBAR_HEIGHT * 3.0d);
           
        }

        public void Gradient1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            gradientDragging = false;
            SolidColorBrush brush1 = new SolidColorBrush(currentGradient);
            MoveRect.Fill = brush1;

        }

        public void Gradient1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gradientDragging = true;

            Gradient1_MouseMove(sender, (System.Windows.Input.MouseEventArgs)e);

        }

        public void Gradient1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            gradientDragging = false;
        }


        public void Gradient1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Color tempGradientColor = currentcolor;

            Point curPoint = e.GetPosition(Gradient1);

            double temprx = tempGradientColor.R + ((255D - tempGradientColor.R) * ((256D - curPoint.X) / 256D));
            double tempry = temprx - (temprx * (curPoint.Y / 256D));
            double tempgx = tempGradientColor.G + ((255D - tempGradientColor.G) * ((256D - curPoint.X) / 256D));
            double tempgy = tempgx - (tempgx * (curPoint.Y / 256D));
            double tempbx = tempGradientColor.B + ((255D - tempGradientColor.B) * ((256D - curPoint.X) / 256D));
            double tempby = tempbx - (tempbx * (curPoint.Y / 256D));

            tempGradientColor = Color.FromArgb(255, (byte)tempry, (byte)tempgy, (byte)tempby);

            textBlock1.Text = "GradientColor: " + tempGradientColor.R + "," + tempGradientColor.G + "," + tempGradientColor.B;

            if (gradientDragging)
            {
                SolidColorBrush brush1 = new SolidColorBrush(tempGradientColor);
                CurrentRect.Fill = brush1;
                currentGradient = tempGradientColor;

                RedTb.Text = currentGradient.R.ToString();
                GreenTb.Text = currentGradient.G.ToString();
                BlueTb.Text = currentGradient.B.ToString();


            }
            else
            {
                SolidColorBrush brush1 = new SolidColorBrush(tempGradientColor);
                MoveRect.Fill = brush1;
            }
            //textBlock1.Text = curPoint.X + "," + curPoint.Y;
        }

        public void Color1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            colorDragging = false;
            SolidColorBrush brush1 = new SolidColorBrush(currentGradient);
            MoveRect.Fill = brush1;

        }

        public void Color1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            textBlock1.Text = e.GetPosition(Color1).ToString();
            Point curPoint = e.GetPosition(Color1);
            Color tempFinalColor = currentcolor;

            if (curPoint.Y > 0 && curPoint.Y <= 43.666)
            {

                tempFinalColor = Color.FromArgb(255, 255, (byte)((curPoint.Y / 43.666D) * 255D), 0);

                textBlock1.Text += "\n" + "Region 1/6: RGB=" + tempFinalColor.R + "," + tempFinalColor.G + "," + tempFinalColor.B;
            }
            else if (curPoint.Y > 43.666 && curPoint.Y <= 85.333)
            {
                tempFinalColor = Color.FromArgb(255, (byte)(255 - (((curPoint.Y - 43.666D) / 43.666D) * 255D)), 255, 0);

                textBlock1.Text += "\n" + "Region 2/6: RGB=" + tempFinalColor.R + "," + tempFinalColor.G + "," + tempFinalColor.B;
            }
            else if (curPoint.Y > 85.333 && curPoint.Y <= 128)
            {
                tempFinalColor = Color.FromArgb(255, 0, 255, (byte)(((curPoint.Y - 85.333D) / 43.666D) * 255D));


                textBlock1.Text += "\n" + "Region 3/6: RGB=" + tempFinalColor.R + "," + tempFinalColor.G + "," + tempFinalColor.B;
            }
            else if (curPoint.Y > 128 && curPoint.Y <= 170.666)
            {
                tempFinalColor = Color.FromArgb(255, 0, (byte)(255 - (((curPoint.Y - 128D) / 43.666D) * 255D)), 255);


                textBlock1.Text += "\n" + "Region 4/6: RGB=" + tempFinalColor.R + "," + tempFinalColor.G + "," + tempFinalColor.B;
            }
            else if (curPoint.Y > 170.666 && curPoint.Y <= 213.333)
            {
                tempFinalColor = Color.FromArgb(255, (byte)(((curPoint.Y - 170.666D) / 43.666D) * 255D), 0, 255);


                textBlock1.Text += "\n" + "Region 5/6: RGB=" + tempFinalColor.R + "," + tempFinalColor.G + "," + tempFinalColor.B;
            }
            else if (curPoint.Y > 213.333 && curPoint.Y <= 256)
            {
                tempFinalColor = Color.FromArgb(255, 255, 0, (byte)(255 - (((curPoint.Y - 213.333D) / 43.666D) * 255D)));



                textBlock1.Text += "\n" + "Region 6/6: RGB=" + tempFinalColor.R + "," + tempFinalColor.G + "," + tempFinalColor.B;
            }

            if (colorDragging)
            {
                SolidColorBrush brush1 = new SolidColorBrush(tempFinalColor);
                CurrentRect.Fill = brush1;
                currentcolor = tempFinalColor;
                currentGradient = tempFinalColor;

                Draw_GradientSelector(tempFinalColor.R, tempFinalColor.G, tempFinalColor.B);

                RedTb.Text = currentcolor.R.ToString();
                GreenTb.Text = currentcolor.G.ToString();
                BlueTb.Text = currentcolor.B.ToString();

            }
            else
            {
                SolidColorBrush brush1 = new SolidColorBrush(tempFinalColor);
                MoveRect.Fill = brush1;
            }

        }



        public void Color1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            colorDragging = true;

            Color1_MouseMove(sender, (System.Windows.Input.MouseEventArgs)e);

        }

        public void Color1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            colorDragging = false;
        }


        public void Draw_ColorSelector()
        {
            //TODO - Use lineargradientBursh with gradientStops instead of solid Color

            DrawingGroup prepGroup = new DrawingGroup();

            LineGeometry line1 = new LineGeometry(new System.Windows.Point(0.0, 0.0), new System.Windows.Point(0.0, 40.0));

            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 0, 0), 0.0));
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 255, 0), 1.0 / 6.0));
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 0, 255, 0), 2.0 / 6.0));
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 0, 255, 255), 3.0 / 6.0));
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 0, 0, 255), 4.0 / 6.0));
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 0, 255), 5.0 / 6.0));
            gradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 0, 0), 1.0));

            GeometryDrawing myDrawing = new GeometryDrawing();
            myDrawing.Pen = new System.Windows.Media.Pen(gradientBrush, 1.0);

            myDrawing.Geometry = line1;


            prepGroup.Children.Add(myDrawing);

            DrawingBrush rectBrush = new DrawingBrush();
            rectBrush.Drawing = prepGroup;


            rectBrush.TileMode = TileMode.None;

            Rect tempRect = rectBrush.Viewbox;


            rectBrush.Viewbox = new Rect(new System.Windows.Point(0, 0), new System.Windows.Point(1, 1));
            Color1.Fill = rectBrush;


        }


        public void Draw_GradientSelector(double r, double g, double b)
        {
            Gradient1.Height = 256;
            Gradient1.Width = 256;

            //create back color gradient DrawingGroup
            DrawingGroup prepGroup = new DrawingGroup();

            System.Windows.Media.LinearGradientBrush solidBrush = new System.Windows.Media.LinearGradientBrush();
            solidBrush.StartPoint = new System.Windows.Point(1.0, 0.0);
            solidBrush.EndPoint = new System.Windows.Point(0.0, 0.0);

            solidBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, (byte)r, (byte)g, (byte)b), 0.0));
            solidBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 255, 255), 1.0));

            LineGeometry line1 = new LineGeometry(new System.Windows.Point(0.0, 0.0), new System.Windows.Point(0.0, 255D));

            GeometryDrawing myDrawing = new GeometryDrawing();
            myDrawing.Pen = new System.Windows.Media.Pen(solidBrush, 1.0);
            myDrawing.Geometry = line1;
            prepGroup.Children.Add(myDrawing);


            //create opacity mask black gradient DrawingGroup
            DrawingGroup prepGroup2 = new DrawingGroup();
            System.Windows.Media.LinearGradientBrush solidBrushMask = new System.Windows.Media.LinearGradientBrush();
            solidBrushMask.StartPoint = new System.Windows.Point(0.0, 1.0);
            solidBrushMask.EndPoint = new System.Windows.Point(0.0, 0.0);

            solidBrushMask.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromArgb(255, 0, 0, 0), 0.0));
            solidBrushMask.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0));

            LineGeometry line2 = new LineGeometry(new System.Windows.Point(0.0, 0.0), new System.Windows.Point(0.0, 255D));

            GeometryDrawing myDrawing2 = new GeometryDrawing();
            myDrawing2.Pen = new System.Windows.Media.Pen(new SolidColorBrush(Colors.Black), 1.0);
            myDrawing2.Geometry = line2;


            prepGroup2.Children.Add(myDrawing2);
            prepGroup2.OpacityMask = solidBrushMask;

            //add two DrawingGroups together
            prepGroup.Children.Add(prepGroup2);


            DrawingBrush rectBrush = new DrawingBrush();
            rectBrush.Drawing = prepGroup;


            rectBrush.TileMode = TileMode.None;

            Gradient1.Fill = rectBrush;


        }



        private void RedTb_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // TODO: Add event handler implementation here.


            int testValue = 0;
            if (int.TryParse(RedTb.Text, out testValue) == false)
            {
                RedTb.Text = "0";
            }
            else
            {
                if (testValue < 0 || testValue > 255)
                    RedTb.Text = "0";
            }

            int tempGreen = 0;
            int.TryParse(GreenTb.Text, out tempGreen);
            int tempBlue = 0;
            int.TryParse(BlueTb.Text, out tempBlue);

            Color tempColor = Color.FromArgb(255, (byte)testValue, (byte)tempGreen, (byte)tempBlue);
            SolidColorBrush brush1 = new SolidColorBrush(tempColor);
            CurrentRect.Fill = brush1;
        }

        private void GreenTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            int testValue = 0;
            if (int.TryParse(GreenTb.Text, out testValue) == false)
            {
                GreenTb.Text = "0";
            }
            else
            {
                if (testValue < 0 || testValue > 255)
                    GreenTb.Text = "0";
            }

            int tempRed = 0;
            int.TryParse(RedTb.Text, out tempRed);
            int tempBlue = 0;
            int.TryParse(BlueTb.Text, out tempBlue);

            Color tempColor = Color.FromArgb(255, (byte)tempRed, (byte)testValue, (byte)tempBlue);
            SolidColorBrush brush1 = new SolidColorBrush(tempColor);
            CurrentRect.Fill = brush1;
        }

        private void BlueTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            int testValue = 0;
            if (int.TryParse(BlueTb.Text, out testValue) == false)
            {
                BlueTb.Text = "0";
            }
            else
            {
                if (testValue < 0 || testValue > 255)
                    BlueTb.Text = "0";
            }

            int tempGreen = 0;
            int.TryParse(GreenTb.Text, out tempGreen);
            int tempRed = 0;
            int.TryParse(RedTb.Text, out tempRed);

            Color tempColor = Color.FromArgb(255, (byte)tempRed, (byte)tempGreen, (byte)testValue);
            SolidColorBrush brush1 = new SolidColorBrush(tempColor);
            CurrentRect.Fill = brush1;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (selectFor == SelectFor.FONT)
            {
                if (parentWindow != null)
                    parentWindow.ForegroundColorPick_ChangeColor(currentGradient);
            }
            else if (selectFor == SelectFor.BACKGROUND)
            {
                if (parentWindow != null)
                    parentWindow.BackgroundColorPick_ChangeColor(currentGradient);
                 
            }
            else if (selectFor == SelectFor.BORDER)
            {
                if (parentWindow != null)
                    parentWindow.BorderColorPick_ChangeColor(currentGradient);

            }
            else if (selectFor == SelectFor.LINDEX || selectFor == SelectFor.RINDEX ||
                        selectFor == SelectFor.MIDDLE ||
                         selectFor == SelectFor.RING ||
                         selectFor == SelectFor.LITTLE ||
                         selectFor == SelectFor.THUMB)
            {
                if (parentWindow != null)
                    parentWindow.GuideColorPick_ChangeColor(currentGradient, selectFor);
            }

            this.Close();
        }
    }
}
