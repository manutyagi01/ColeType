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

using System.Windows.Media.Effects;
using System.Windows.Media.Animation;

namespace OSDKeyboardMain1
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {

        TextBlock appDesc = new TextBlock();

        public AboutWindow(keyboardLayoutType layout, double radiusX, double radiusY)
        {
            InitializeComponent();

            Random random = new Random();

            for (int x = 0; x < 20; x++)
            {
                int charNum = random.Next(7);
                string[] selection;
                if (layout == keyboardLayoutType.COLEMAK)
                {
                    selection = new string[] { "C", "O", "L", "E", "M", "A", "K" };
                }
                else if (layout == keyboardLayoutType.DEVORAK)
                {
                    selection = new string[] { "D", "E", "V", "O", "R", "A", "K" };
                }
                else
                {
                    selection = new string[] { "Q", "W", "E", "R", "T", "Y", "Y" };
                }

                
                KeyControl tempKey = new KeyControl(selection[charNum]);
                tempKey.Opacity = 0.2;
                tempKey.KeyRectangleRadiusX = radiusX;
                tempKey.KeyRectangleRadiusY = radiusY;



                double scaleNum = random.NextDouble() * 10;
                ScaleTransform scaleTransform = new System.Windows.Media.ScaleTransform(scaleNum, scaleNum);

                double rotateNum = random.NextDouble() * 360;
                RotateTransform rotateTransform = new System.Windows.Media.RotateTransform(rotateNum, 15 * scaleNum, 15 * scaleNum);

                double translateWidth = random.NextDouble() * this.Width;
                double translateHeight = random.NextDouble() * this.Height;
                TranslateTransform translateTransform = new System.Windows.Media.TranslateTransform(translateWidth, translateHeight);

                


                TransformGroup transformGroup = new System.Windows.Media.TransformGroup();
                
                transformGroup.Children.Add(scaleTransform);
                transformGroup.Children.Add(rotateTransform);
                transformGroup.Children.Add(translateTransform);
                

                tempKey.RenderTransform = transformGroup;

                DoubleAnimation dblAnim = new DoubleAnimation(0, 360d, TimeSpan.FromMilliseconds(random.NextDouble()*20000 + 20000));
                dblAnim.RepeatBehavior = RepeatBehavior.Forever;
                //RotateTransform Rotatetemp = transformGroup.Children[1] as RotateTransform;
                
                transformGroup.Children[1].BeginAnimation(RotateTransform.AngleProperty, dblAnim);

                RootGrid.Children.Add(tempKey);




                
               
            }

            //grid
            StackPanel ForegroundGrid = new StackPanel();
            ForegroundGrid.Height = 300;
            ForegroundGrid.Width = 300;

            this.MouseEnter += Show_Text;
            this.MouseLeave += Hide_Text;

            RootGrid.Children.Add(ForegroundGrid);
            

            //coletrain
            Label appName = new Label();

            DropShadowEffect shadoweffect = new DropShadowEffect();
            shadoweffect.BlurRadius = 5;
            shadoweffect.ShadowDepth = 0;
            shadoweffect.Color = Colors.Black;


            appName.Content = "ColeType";
            appName.FontSize = 48;
            SolidColorBrush brush = new SolidColorBrush(Colors.White);
            appName.Foreground = brush;
            appName.Effect = shadoweffect;
            //SolidColorBrush brush2 = new SolidColorBrush(Colors.Red);
            //appName.Background = brush2;
            appName.Background = new SolidColorBrush(Colors.Transparent);
            appName.Margin = new Thickness(0);
            appName.Padding = new Thickness(0, 0, 0, 0);

            appName.HorizontalAlignment = HorizontalAlignment.Center;
            ForegroundGrid.Children.Add(appName);

            ////Catchline
            //Label appCatch = new Label();

            //appCatch.Content = "Colemak Touch Typing Trainer";
            //appCatch.FontSize = 9;
            

            //appCatch.Foreground = brush;
            //appCatch.Effect = shadoweffect;

            //appCatch.Background = new SolidColorBrush(Colors.Transparent);
            //appCatch.Margin = new Thickness(0);
            //appCatch.Padding = new Thickness(0, 0, 0, 10);

            //appCatch.HorizontalAlignment = HorizontalAlignment.Center;
            //ForegroundGrid.Children.Add(appCatch);


            //version 1.0.0.0
            Label appVersion = new Label();

            appVersion.Content = "version 1.0.3";
            appVersion.FontSize = 10;
            appVersion.Opacity = 0.7;

            appVersion.Foreground = brush;
            appVersion.Effect = shadoweffect;

            appVersion.Background = new SolidColorBrush(Colors.Transparent);
            appVersion.Margin = new Thickness(0);

            appVersion.Padding = new Thickness(0, 0, 0, 0);

            appVersion.HorizontalAlignment = HorizontalAlignment.Center;
            ForegroundGrid.Children.Add(appVersion);

            //by
            Label appBy = new Label();

            appBy.Content = "By Manu Tyagi";
            appBy.FontSize = 10;
            appBy.Opacity = 0.7;

            appBy.Foreground = brush;
            appBy.Effect = shadoweffect;

            appBy.Background = new SolidColorBrush(Colors.Transparent);
            appBy.Margin = new Thickness(0);
            appBy.HorizontalAlignment = HorizontalAlignment.Center;
            ForegroundGrid.Children.Add(appBy);

            //Description
            

            //appDesc.Content = "ColeTrain is a touch typing trainer based around the idea \nthat having a virtual on-screen keyboard allows the user\n"
            //                    +"to focus on their work as they learn to touchtype at \nthe same time.\n\n This allows the user to learn in the" 
            //                    +" most \nnatural way possible while at the same time giving the user the \nflexibility to advance at their own pace and"
            //                    +" continue to work on their computer!";

            appDesc.Text = "ColeType Copyright (c) © 2014 is a flexible touch typing training application that encourages natural touch typing"
                            +" by giving usablility in real work situations while providing crucial touch feedback and reference for beginners";
            appDesc.FontSize = 9;
            //appDesc.Opacity = 0.5;
            appDesc.TextWrapping = TextWrapping.Wrap;

            appDesc.TextAlignment = TextAlignment.Justify;

            appDesc.Foreground = brush;
            appDesc.Effect = shadoweffect;
            appDesc.Width = 150;
            //appDesc.FontStretch = FontStretches.UltraExpanded;
            //appDesc.FontWeight = FontWeights.ExtraBold;
            appDesc.LineHeight = 20;

            appDesc.Background = new SolidColorBrush(Colors.Transparent);
            appDesc.Margin = new Thickness(0);
            appDesc.Padding = new Thickness(0, 25, 0, 0);
            appDesc.HorizontalAlignment = HorizontalAlignment.Center;

            //hidden!
            appDesc.Visibility = Visibility.Hidden;

            ForegroundGrid.Children.Add(appDesc);
            //
            
        }

        private void Hide_Text(object sender, EventArgs args)
        {
            appDesc.Visibility = Visibility.Hidden;

        }

        private void Show_Text(object sender, EventArgs args)
        {
            appDesc.Visibility = Visibility.Visible;
        }
    }
}
