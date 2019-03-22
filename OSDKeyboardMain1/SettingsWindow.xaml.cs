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
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    /// 

    public partial class SettingsWindow : Window
    {

        MainWindow parentWindow;
        ColorSelectWindow childColorWindow;

        private static double TASKBAR_HEIGHT = 35;

        

        public SettingsWindow(MainWindow parentWin, UserPrefs currentPrefs)
        {
            InitializeComponent();

            parentWindow = parentWin;


            WindowOpacitySlider.Value = currentPrefs.windowOpacity;
            RectangleOpacitySlider.Value = currentPrefs.keyRectangleOpacity;
            LabelOpacitySlider.Value = currentPrefs.KeyLabelOpacity;
            KeyMarginSlider.Value = currentPrefs.keyMargin;

            RectangleRadiusXSlider.Value = currentPrefs.keyRectangleRadiusX;
            RectangleRadiusYSlider.Value = currentPrefs.keyRectangleRadiusY;

            LabelBoldCheck.IsChecked = currentPrefs.fontBold;
            LabelItalicCheck.IsChecked = currentPrefs.fontItalic;

            StartupCheck.IsChecked = currentPrefs.loadOnStartup;

            fadeDelaySlider.Value = currentPrefs.fadeDelay;

            SolidColorBrush brush1 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.backgroundColor));
            BackgroundColorPick.Fill = brush1;

            SolidColorBrush brush2 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.borderColor));
            BorderColorPick.Fill = brush2;
            
            SolidColorBrush brush3 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.foregroundColor));
            ForegroundColorPick.Fill = brush3;


            SolidColorBrush brush4 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.LIndexColor));
            LIndexColorPick.Fill = brush4;

            SolidColorBrush brush5 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.MiddleColor));
            MiddleColorPick.Fill = brush5;

            SolidColorBrush brush6 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.RingColor));
            RingColorPick.Fill = brush6;

            SolidColorBrush brush7 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.LittleColor));
            LittleColorPick.Fill = brush7;

            SolidColorBrush brush8 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.ThumbColor));
            ThumbColorPick.Fill = brush8;

            SolidColorBrush brush9 = new SolidColorBrush(MyColorConverter.ArrayToColor(currentPrefs.RIndexColor));
            RIndexColorPick.Fill = brush9;

            this.Closing += Window_Closing;

            this.Top = SystemParameters.VirtualScreenHeight - (double)this.Height - (double)TASKBAR_HEIGHT;
            this.Left = SystemParameters.VirtualScreenWidth - (double)this.Width - (double)TASKBAR_HEIGHT;
            //this.Top = 0;
            //this.Left = 0;
        }

        private void Window_Closing(object sender, EventArgs args)
        {

            if(childColorWindow != null)
                childColorWindow.Close();
        }


        private void WindowOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(parentWindow != null)
            parentWindow.Set_WindowOpacity(e.NewValue);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //this.Visibility = Visibility.Hidden;
            this.Close();
        }

        private void LabelOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (parentWindow != null)
                parentWindow.Set_KeyLabelOpacity(e.NewValue);
        }

        private void RectangleOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (parentWindow != null)
                parentWindow.Set_KeyRectangleOpacity(e.NewValue);
        }

        private void KeyMarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (parentWindow != null)
                parentWindow.Set_KeyMargin(e.NewValue);
        }

        private void RectangleRadiusXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (parentWindow != null)
                parentWindow.Set_KeyRectangleRadiusX(e.NewValue);
        }

        private void RectangleRadiusYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (parentWindow != null)
                parentWindow.Set_KeyRectangleRadiusY(e.NewValue);
        }

        private void ForegroundColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.FONT);
            childColorWindow.Show();

        }

        private void BackgroundColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.BACKGROUND);
            childColorWindow.Show();
        }

        private void BorderColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.BORDER);
            childColorWindow.Show();
        }

        public void ForegroundColorPick_ChangeColor(Color select)
        {
            SolidColorBrush brush1 = new SolidColorBrush(select);
            ForegroundColorPick.Fill = brush1;

            if (parentWindow != null)
                parentWindow.Set_KeyForegroundColor(select);
        }

        public void BackgroundColorPick_ChangeColor(Color select)
        {
            SolidColorBrush brush1 = new SolidColorBrush(select);
            BackgroundColorPick.Fill = brush1;

            if (parentWindow != null)
                parentWindow.Set_KeyBackgroundColor(select);
        }

        public void BorderColorPick_ChangeColor(Color select)
        {
            SolidColorBrush brush1 = new SolidColorBrush(select);
            BorderColorPick.Fill = brush1;

            if (parentWindow != null)
                parentWindow.Set_KeyBorderColor(select);
        }

        private void LabelBoldCheck_Click(object sender, RoutedEventArgs e)
        {
            if (parentWindow != null)
                parentWindow.Set_KeyFontBold((bool)LabelBoldCheck.IsChecked);
        }

        private void LabelItalicCheck_Click(object sender, RoutedEventArgs e)
        {
            if (parentWindow != null)
                parentWindow.Set_KeyFontItalic((bool)LabelItalicCheck.IsChecked);
        }

        private void DefaultsBtn_Click(object sender, RoutedEventArgs e)
        {

            DialogResult result;
            result = System.Windows.Forms.MessageBox.Show( "Are you sure you want to reset layout default settings? " , "Reset Defaults", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                if (parentWindow != null)
                    parentWindow.Set_WindowDefaults();
            }
        }

   

        private void StartupCheck_Click(object sender, RoutedEventArgs e)
        {
           // SetStartup("ColeTrain", (bool)StartupCheck.IsChecked);
            if (parentWindow != null)
                parentWindow.Set_LoadOnStartup((bool)StartupCheck.IsChecked);
        }

        public void GuideColorPick_ChangeColor(Color Select, SelectFor finger)
        {
            if (finger == SelectFor.LINDEX)
            {
                //update setting window colors
                LIndexColorPick.Fill = new SolidColorBrush(Select);
            }
            else if (finger == SelectFor.RINDEX)
            {
                RIndexColorPick.Fill = new SolidColorBrush(Select);
            }
            else if (finger == SelectFor.MIDDLE)
            {
                MiddleColorPick.Fill = new SolidColorBrush(Select);
            }
            else if (finger == SelectFor.RING)
            {
                RingColorPick.Fill = new SolidColorBrush(Select);
            }
            else if (finger == SelectFor.LITTLE)
            {
                LittleColorPick.Fill = new SolidColorBrush(Select);
            }
            else if (finger == SelectFor.THUMB)
            {
                ThumbColorPick.Fill = new SolidColorBrush(Select);
            }



            if (parentWindow != null)
                parentWindow.Set_GuideColors(Select, finger);
        }

 

        private void MiddleColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.MIDDLE);
            childColorWindow.Show();
        }

        private void RingColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.RING);
            childColorWindow.Show();
        }

        private void LittleColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.LITTLE);
            childColorWindow.Show();
        }

        private void ThumbColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.THUMB);
            childColorWindow.Show();
        }



        private void LIndexColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.LINDEX);
            childColorWindow.Show();
        }

        private void RIndexColorPick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (childColorWindow != null)
                childColorWindow.Close();

            childColorWindow = new ColorSelectWindow(this, SelectFor.RINDEX);
            childColorWindow.Show();
        }

        private void FadeDelaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (parentWindow != null)
                parentWindow.Set_FadeDelay(e.NewValue);
        }

    }
}
