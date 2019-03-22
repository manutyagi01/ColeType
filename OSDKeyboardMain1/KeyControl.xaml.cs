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
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace OSDKeyboardMain1
{
    /// <summary>
    /// Interaction logic for KeyControl.xaml
    /// </summary>
    public partial class KeyControl : UserControl
    {
        private Rectangle backgroundRectangle;
        private Rectangle backgroundGuide;
        private Label foregroundLabel;

        private Ellipse backgroundEllipse;
        //private Label foregroundLabelUnderline;

        private Grid rootGrid;

        private String checkKey;

        public KeyControl(string label) : this(label, "") { }
        public KeyControl(string label, string check) : this(label, check, 0, 0) { }
        public KeyControl(float width, float height) : this("", "", width, height) { }
        public KeyControl(string label, string check, float width, float height)
        {
            InitializeComponent();

            if(width > 0)
                this.Width = width;
            if(height > 0)
                this.Height = height;

            checkKey = check;
                              
            backgroundRectangle = new Rectangle();
            backgroundRectangle.Fill = Brushes.Orange;
            //backgroundRectangle.RadiusX = 5;
            //backgroundRectangle.RadiusY = 5;
            backgroundRectangle.Stroke = Brushes.Black;
            //backgroundRectangle.Width = 30;
            //backgroundRectangle.Height = 30;
            backgroundEllipse = new Ellipse();
            backgroundEllipse.Fill = Brushes.Black;
            backgroundEllipse.Stroke = Brushes.Black;
            backgroundEllipse.Width = 3.0;
            backgroundEllipse.Height = 3.0;
            backgroundEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            backgroundEllipse.VerticalAlignment = VerticalAlignment.Bottom;
            backgroundEllipse.Margin = new Thickness(4);
            backgroundEllipse.Visibility = System.Windows.Visibility.Hidden;


            Color tempColor = Colors.Orange;
            //SolidColorBrush opacitymask = new SolidColorBrush(Color.FromArgb(255, (byte)(tempColor.R - 20d), (byte)(tempColor.G - 200d), (byte)(tempColor.B - 20d)));

         
            foregroundLabel = new Label();
            foregroundLabel.Content = label;
            //foregroundLabel.FontWeight = FontWeights.Bold;
            foregroundLabel.VerticalAlignment = VerticalAlignment.Top;
            //foregroundLabel.HorizontalAlignment = HorizontalAlignment.Right;
            //backgroundRectangle.Opacity = 0.6;
            //foregroundLabel.Opacity = 0.1;
            //foregroundLabel.Foreground = Brushes.Red;

            //foregroundLabel.FontStyle = FontStyles.Italic;
            foregroundLabel.FontSize = 10.0;

            //foregroundLabelUnderline = new Label();

            if (label.Contains("_") && !label.Contains("-"))
            {
                //foregroundLabelUnderline.Content = "_____";
                backgroundEllipse.Visibility = System.Windows.Visibility.Visible;
            }
            //foregroundLabelUnderline.VerticalAlignment = VerticalAlignment.Bottom;
            //foregroundLabelUnderline.HorizontalAlignment = HorizontalAlignment.Center;
            //foregroundLabelUnderline.FontSize = 10.0;



            backgroundGuide = new Rectangle();
            backgroundGuide.Fill = Brushes.Black;
            backgroundGuide.Stroke = Brushes.Black;
            

            rootGrid = new Grid();
            rootGrid.Children.Add(backgroundRectangle);
            rootGrid.Children.Add(backgroundGuide);
            backgroundGuide.Visibility = System.Windows.Visibility.Hidden;

            rootGrid.Children.Add(backgroundEllipse);

            rootGrid.Children.Add(foregroundLabel);

            //rootGrid.Children.Add(foregroundLabelUnderline);

            this.AddChild(rootGrid);

           
            
        }

 

        public double KeyRectangleOpacity
        {
            get { return (double)GetValue(KeyRectangleOpacityProperty); }
            set { SetValue(KeyRectangleOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyRectangleOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyRectangleOpacityProperty = 
            DependencyProperty.Register("KeyRectangleOpacity", typeof(double), typeof(KeyControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(KeyRectangleOpacityChanged)), new ValidateValueCallback(ValidateKeyRectangleOpacity));

        public static bool ValidateKeyRectangleOpacity(object value)
        {
            double propValue = (double)value;
            if (propValue >= 0.0 && propValue <= 1.0)
                return true;
            return false;
        }

        public static void KeyRectangleOpacityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Rectangle thisRect = thisKey.backgroundRectangle;
            thisRect.Opacity = Convert.ToDouble(args.NewValue);

            Rectangle thisGuide = thisKey.backgroundGuide;
            thisGuide.Opacity = Convert.ToDouble(args.NewValue); 
            
        }



        public double KeyRectangleRadiusX
        {
            get { return (double)GetValue(KeyRectangleRadiusXProperty); }
            set { SetValue(KeyRectangleRadiusXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyRectangleRadiusX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyRectangleRadiusXProperty =
            DependencyProperty.Register("KeyRectangleRadiusX", typeof(double), typeof(KeyControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(KeyRectangleRadiusXChanged)), new ValidateValueCallback(ValidateKeyRectangleRadiusX));

        public static bool ValidateKeyRectangleRadiusX(object value)
        {
            double propValue = (double)value;
            if (propValue >= 0.0 && propValue <= 15.0)
                return true;
            return false;
        }

        public static void KeyRectangleRadiusXChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Rectangle thisRect = thisKey.backgroundRectangle;
            thisRect.RadiusX = Convert.ToDouble(args.NewValue);

            Rectangle thisGuide = thisKey.backgroundGuide;
            thisGuide.RadiusX = Convert.ToDouble(args.NewValue); 
        }

        public double KeyRectangleRadiusY
        {
            get { return (double)GetValue(KeyRectangleRadiusYProperty); }
            set { SetValue(KeyRectangleRadiusYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyRectangleRadiusX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyRectangleRadiusYProperty =
            DependencyProperty.Register("KeyRectangleRadiusY", typeof(double), typeof(KeyControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(KeyRectangleRadiusYChanged)), new ValidateValueCallback(ValidateKeyRectangleRadiusY));

        public static bool ValidateKeyRectangleRadiusY(object value)
        {
            double propValue = (double)value;
            if (propValue >= 0.0 && propValue <= 15.0)
                return true;
            return false;
        }

        public static void KeyRectangleRadiusYChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Rectangle thisRect = thisKey.backgroundRectangle;
            thisRect.RadiusY = Convert.ToDouble(args.NewValue);

            Rectangle thisGuide = thisKey.backgroundGuide;
            thisGuide.RadiusY = Convert.ToDouble(args.NewValue); 
        }

 



        public double KeyLabelOpacity
        {
            get { return (double)GetValue(KeyLabelOpacityProperty); }
            set { SetValue(KeyLabelOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyFontOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyLabelOpacityProperty =
            DependencyProperty.Register("KeyLabelOpacity", typeof(double), typeof(KeyControl), new UIPropertyMetadata(0.0, new PropertyChangedCallback(KeyLabelOpacityChanged)), new ValidateValueCallback(ValidateKeyLabelOpacity));

        public static bool ValidateKeyLabelOpacity(object value)
        {
            double propValue = (double)value;
            if (propValue >= 0.0 && propValue <= 1.0)
                return true;
            return false;
        }

        public static void KeyLabelOpacityChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Label thisFont = thisKey.foregroundLabel;
            thisFont.Opacity = Convert.ToDouble(args.NewValue);


            //KeyControl thisKey2 = (KeyControl)depObj;
            //Label thisFont2 = thisKey2.foregroundLabelUnderline;
            //thisFont2.Opacity = Convert.ToDouble(args.NewValue);


            Ellipse thisElli = thisKey.backgroundEllipse;
            thisElli.Opacity = Convert.ToDouble(args.NewValue);

        }







        public Color KeyLabelColor
        {
            get { return (Color)GetValue(KeyLabelColorProperty); }
            set { SetValue(KeyLabelColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyBrushColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyLabelColorProperty =
            DependencyProperty.Register("KeyLabelColorColor", typeof(Color), typeof(KeyControl), new UIPropertyMetadata(Color.FromArgb(255, 0, 0, 0), new PropertyChangedCallback(KeyLabelColorChanged)), new ValidateValueCallback(ValidateKeyLabelColor));

        public static bool ValidateKeyLabelColor(object value)
        {
            if (value is Color && value != null)
                return true;
            return false;
        }

        public static void KeyLabelColorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Label thisLabel = thisKey.foregroundLabel;
            SolidColorBrush brush1 = new SolidColorBrush((Color)args.NewValue);
            thisLabel.Foreground = brush1;

            //KeyControl thisKey2 = (KeyControl)depObj;
            //Label thisLabel2 = thisKey2.foregroundLabelUnderline;
            //SolidColorBrush brush2 = new SolidColorBrush((Color)args.NewValue);
            //thisLabel2.Foreground = brush2;

            Ellipse thisElli = thisKey.backgroundEllipse;
            SolidColorBrush brush3 = new SolidColorBrush((Color)args.NewValue);
            thisElli.Fill = brush3;
            thisElli.Stroke = brush3;
        }



        public Color KeyBackgroundColor
        {
            get { return (Color)GetValue(KeyBackgroundColorProperty); }
            set { SetValue(KeyBackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyBackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyBackgroundColorProperty =
            DependencyProperty.Register("KeyBackgroundColor", typeof(Color), typeof(KeyControl), new UIPropertyMetadata(Color.FromArgb(255,0,0,0), new PropertyChangedCallback(KeyBackgroundColorChanged)), new ValidateValueCallback(ValidateKeyBackgroundColor));

        public static bool ValidateKeyBackgroundColor(object value)
        {
            if (value is Color && value != null)
                return true;
            return false;
        }

        public static void KeyBackgroundColorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Rectangle thisRect = thisKey.backgroundRectangle;
            SolidColorBrush brush1 = new SolidColorBrush((Color)args.NewValue);
            thisRect.Fill = brush1;

            //thisRect.Stroke = new SolidColorBrush(Colors.Black);
            //thisRect.StrokeThickness = 4;
        }



        public Color KeyBorderColor
        {
            get { return (Color)GetValue(KeyBorderColorProperty); }
            set { SetValue(KeyBorderColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyBorderColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyBorderColorProperty =
            DependencyProperty.Register("KeyBorderColor", typeof(Color), typeof(KeyControl), new UIPropertyMetadata(Color.FromArgb(255,0,0,0), new PropertyChangedCallback(KeyBorderColorChanged)),new ValidateValueCallback(ValidateKeyBorderColor));

        
        public static bool ValidateKeyBorderColor(object value)
        {
            if (value is Color && value != null)
                return true;
            return false;
        }

        public static void KeyBorderColorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Rectangle thisRect = thisKey.backgroundRectangle;
            SolidColorBrush brush1 = new SolidColorBrush((Color)args.NewValue);
            thisRect.Stroke = brush1;

            Rectangle thisGuide = thisKey.backgroundGuide;
            thisGuide.Stroke = brush1;

        }

        public void KeyboardLabelFontBold(bool enable)
        {

            if (enable)
            {
                foregroundLabel.FontWeight = FontWeights.Bold;
                
            }
            else
            {
                foregroundLabel.FontWeight = FontWeights.Normal;
            }   

        }

        public void KeyboardLabelFontItalic(bool enable)
        {

            if (enable)
            {
                foregroundLabel.FontStyle = FontStyles.Italic;
                
            }
            else
            {
                foregroundLabel.FontStyle = FontStyles.Normal;
            }

        }

        public void KeyboardGuideVisibility(bool value)
        {
            if (value)
            {
                backgroundGuide.Visibility = System.Windows.Visibility.Visible;

                //hide background rect
                backgroundRectangle.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                backgroundGuide.Visibility = System.Windows.Visibility.Hidden;

                //show background rect
                backgroundRectangle.Visibility = System.Windows.Visibility.Visible;
            }

        }

        //add guide rectangle dependent property for fade effects
        public Color KeyGuideColor
        {
            get { return (Color)GetValue(KeyGuideColorProperty); }
            set { SetValue(KeyGuideColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyBackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyGuideColorProperty =
            DependencyProperty.Register("KeyGuideColor", typeof(Color), typeof(KeyControl), new UIPropertyMetadata(Color.FromArgb(255, 0, 0, 0), new PropertyChangedCallback(KeyGuideColorChanged)), new ValidateValueCallback(ValidateKeyGuideColor));

        public static bool ValidateKeyGuideColor(object value)
        {
            if (value is Color && value != null)
                return true;
            return false;
        }

        public static void KeyGuideColorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            KeyControl thisKey = (KeyControl)depObj;
            Rectangle thisGuide = thisKey.backgroundGuide;
            SolidColorBrush brush1 = new SolidColorBrush((Color)args.NewValue);
            thisGuide.Fill = brush1;

            //thisRect.Stroke = new SolidColorBrush(Colors.Black);
            //thisRect.StrokeThickness = 4;
        }

        public string GetCheckKey()
        {

            return checkKey;

        }
    }
}
