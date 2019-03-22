using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using WindowHooks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace OSDKeyboardMain1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
   
    [Serializable]
    public enum keyboardLayoutType
    {
        QWERTY,
        DEVORAK,
        COLEMAK
    };


    [Serializable]
    public class UserPrefs
    {
        public double winHeight;
        public double winWidth;
        public double winTop;
        public double winLeft;

        public double keyRectangleOpacity;
        public double KeyLabelOpacity;

        //TODO change to viewbox not whole window
        public double windowOpacity;
        public double keyMargin;

        public double keyRectangleRadiusX;
        public double keyRectangleRadiusY;

        public byte[] backgroundColor = new byte[3];
        public byte[] foregroundColor = new byte[3];
        public byte[] borderColor = new byte[3];
        public byte[] LIndexColor = new byte[3];
        public byte[] RIndexColor = new byte[3];
        public byte[] MiddleColor = new byte[3];
        public byte[] RingColor = new byte[3];
        public byte[] LittleColor = new byte[3];
        public byte[] ThumbColor = new byte[3];

        public keyboardLayoutType layoutType;

        public bool movableWindow;
        public bool autoHideWindow;
        public double fadeDelay;

        public bool fontBold;
        public bool fontItalic;

        public bool loadOnStartup;

        public bool showGuide;


    }


    class MyColorConverter
    {
        public static Color ArrayToColor(byte[] value)
        {
            return Color.FromArgb(255, value[0], value[1], value[2]);
        }

        public static byte[] ColorToArray(Color value)
        {
            byte[] returnarray = new byte[3]{value.R,value.G,value.B};
            return returnarray;
        }

    }


	public partial class MainWindow : Window
	{
        private KeyboardLLHook KeyLLHook;
        private System.Windows.Controls.TextBox tempBox;
        private Dictionary<Keys, KeyControl> keyboardLayout;
 
        
        //private keyboardLayoutType layoutType;

        //private bool movable;

        private bool winDragging = false;
        private bool winResizing = false;
        private Point winPos;
        private Point mousePos;

        UserPrefs windowPrefs = new UserPrefs();
        SettingsWindow settingsWindow;

        DispatcherTimer fadeTimer;
        //const int msFadeDelay = 2000;

        private static double TASKBAR_HEIGHT = 35.0;

        public delegate void SetDefaultsDelegate();

        public event SetDefaultsDelegate OnSetDefaults;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);


		public MainWindow()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.

            //Set window parameters
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
            this.Background = Brushes.Transparent;
            this.Topmost = true;
            this.MinHeight = 10;
            this.MinWidth = 30;

            this.Closing += Window_Closing;
            this.Closed += Window_Closed;
            
            //this.Opacity = 0.4;
           
            //enable/disable click thorugh transparency Extended Style
            //for this window in the OnSourceInitialized method 
            //movable = movableWindow;

            
            //tempBox = new System.Windows.Controls.TextBox();
            //tempBox.Width = 450.0;
            //tempBox.Height = 100.0;
            //tempBox.TextWrapping = TextWrapping.Wrap;
            //stackRoot.Children.Add(tempBox);

            //tempBox.Visibility = System.Windows.Visibility.Hidden;

            //read user preferences



            Initialise_UserPrefs();

            Intialise_FadeTimer();
             //create KeyControl Dictionary and add to window layout
            Initialise_KeyboardLayout();
            //Initialise_KeyboardlayoutColemak();
            //Initialise_KeyboardlayoutDevorak();
            
            //init settings window
            //settingsWindow = new SettingsWindow(this, windowPrefs);


		}

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (!windowPrefs.movableWindow)
            {
                //asign transparency extended style to window 
                const int WS_EX_TRANSPARENT = 0x00000020;
                const int GWL_EXSTYLE = (-20);

                // Get this window's handle         
                IntPtr hwnd = new WindowInteropHelper(this).Handle;
                // Change the extended window style to include WS_EX_TRANSPARENT         
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
            else if (windowPrefs.movableWindow)
            {
                //asign event handles for moving / resizing window
                viewBoxRoot.PreviewMouseLeftButtonDown += ViewBoxRoot_PreviewMouseLeftButtonDown;
                viewBoxRoot.PreviewMouseLeftButtonUp += ViewBoxRoot_PreviewMouseLeftButtonUp;
                viewBoxRoot.PreviewMouseMove += ViewBoxRoot_PreviewMouseMove;

                resizeBottom.MouseLeftButtonDown += ResizeBottom_MouseLeftButtonDown;
                resizeBottom.MouseLeftButtonUp += ResizeBottom_MouseLeftButtonUp;
                resizeBottom.MouseMove += ResizeBottom_MouseMove;

                resizeRight.MouseLeftButtonDown += ResizeRight_MouseLeftButtonDown;
                resizeRight.MouseLeftButtonUp += ResizeRight_MouseLeftButtonUp;
                resizeRight.MouseMove += ResizeRight_MouseMove;

                this.Cursor = System.Windows.Input.Cursors.SizeAll;

                //show resize controls
                resizeBottom.Visibility = System.Windows.Visibility.Visible;
                resizeRight.Visibility = System.Windows.Visibility.Visible;

            }

            base.OnSourceInitialized(e);

        }

        private void Window_Closing(object sender, EventArgs e)
        {
            

            //close settings window
            if(settingsWindow != null)
            settingsWindow.Close();

            //save settings to file before closing window
            Save_UserPrefs();

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }



        private void Initialise_UserPrefs()
        {
            if (File.Exists("settings"))
            {
                //System.Windows.Forms.MessageBox.Show("File Exists!");
                Load_UserPrefs();

                this.Top = windowPrefs.winTop;
                this.Left = windowPrefs.winLeft;
                this.Width = windowPrefs.winWidth;
                this.Height = windowPrefs.winHeight;

                this.Opacity = windowPrefs.windowOpacity;


            }
            else
            {
                //System.Windows.Forms.MessageBox.Show("File not Found?");
                //TODO set window default position above taskbar here or in constructor
                //this.Left = 1;
                //this.Top = 1;

                //save program defaults 
                Save_Defaults();
            }


        }

        private void Save_Defaults()
        {
            this.Opacity = 1.0;

            windowPrefs.keyRectangleOpacity = 0.75;
            windowPrefs.KeyLabelOpacity = 1.0;
            windowPrefs.windowOpacity = 0.9;
            //windowPrefs.keyMargin = 0.9;
            //windowPrefs.keyRectangleRadiusX = 5.0;
            //windowPrefs.keyRectangleRadiusY = 10.0;
            windowPrefs.keyMargin = 2.0;
            windowPrefs.keyRectangleRadiusX = 12.0;
            windowPrefs.keyRectangleRadiusY = 2.0;
            
            windowPrefs.winWidth = 455;
            windowPrefs.winHeight = 155;
            this.Width = windowPrefs.winWidth;
            this.Height = windowPrefs.winHeight;
            //this.Top = (double)Screen.PrimaryScreen.WorkingArea.Height - (double)this.Height - (double)TASKBAR_HEIGHT;
            //this.Left = ((double)Screen.PrimaryScreen.WorkingArea.Width / (double)2.0) - ((double)this.Width / (double)2.0);

            //System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;

            //this.Top = workingRectangle.Height / 2.0;
            //this.Left = (double)Screen.PrimaryScreen.WorkingArea.Width / (double)2.0;

            //this.Top = 732.5;
            //this.Left = 890;

            this.Top = SystemParameters.VirtualScreenHeight - (double)this.Height - (double)TASKBAR_HEIGHT;
            this.Left = (SystemParameters.VirtualScreenWidth / (double)2.0) - ((double)this.Width / (double)2.0);


            windowPrefs.winTop = this.Top;
            windowPrefs.winLeft = this.Left;
            //save settings because file not found

            windowPrefs.backgroundColor = new Byte[] { Colors.Orange.R, Colors.Orange.G, Colors.Orange.B };
            windowPrefs.foregroundColor = new Byte[] { 1, 1, 1};
            windowPrefs.borderColor = new byte[] { 1, 1, 1 };

            //windowPrefs.LIndexColor = new Byte[] { Colors.Red.R, Colors.Red.G, Colors.Red.B };
            //windowPrefs.RIndexColor = new Byte[] { Colors.Pink.R, Colors.Pink.G, Colors.Pink.B };
            //windowPrefs.MiddleColor = new Byte[] { Colors.Orange.R, Colors.Orange.G, Colors.Orange.B };
            //windowPrefs.RingColor = new Byte[] { Colors.Green.R, Colors.Green.G, Colors.Green.B };
            //windowPrefs.LittleColor = new Byte[] { Colors.Blue.R, Colors.Blue.G, Colors.Blue.B };
            //windowPrefs.ThumbColor = new Byte[] { Colors.Purple.R, Colors.Purple.G, Colors.Purple.B };
            windowPrefs.LIndexColor = new Byte[] { 255, 129, 129 };
            windowPrefs.RIndexColor = new Byte[] { 246, 149, 76 };
            windowPrefs.MiddleColor = new Byte[] { 249, 208, 67 };
            windowPrefs.RingColor = new Byte[] { 210, 246, 138 };
            windowPrefs.LittleColor = new Byte[] { 163, 245, 216 };
            windowPrefs.ThumbColor = new Byte[] { 214, 154, 248 };

            windowPrefs.layoutType = keyboardLayoutType.QWERTY;

            windowPrefs.movableWindow = false;

            windowPrefs.autoHideWindow = false;

            windowPrefs.fadeDelay = 2.0;

            windowPrefs.fontBold = false;
            windowPrefs.fontItalic = false;

            windowPrefs.loadOnStartup = false;

            windowPrefs.showGuide = true;

            Set_LoadOnStartup(windowPrefs.loadOnStartup);

            Save_UserPrefs();
        }

        private void Save_UserPrefs()
        {
 
 


            BinaryFormatter myBinF = new BinaryFormatter();

            using (Stream fs = new FileStream("settings", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                myBinF.Serialize(fs, (object)windowPrefs);
                //tempBox.Text = "Saving..." + "\n" +
                //    "windowTop: " + windowPrefs.winTop.ToString() + "\n" +
                //    "windowLeft: " + windowPrefs.winLeft.ToString() + "\n";
            }
        }
        private void Load_UserPrefs()
        {
            BinaryFormatter myBinF = new BinaryFormatter();

            using (Stream fs = File.OpenRead("settings"))
            {
                UserPrefs tempPrefs = (UserPrefs)myBinF.Deserialize(fs);
                windowPrefs = tempPrefs;
                //tempBox.Text = "Reading..." + "\n" +
                //    "windowTop: " + windowPrefs.winTop.ToString() + "\n" +
                //    "windowLeft: " + windowPrefs.winLeft.ToString() + "\n";
            }

            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Initialise Keyboard hook after class is loaded correctly
            KeyLLHook = new KeyboardLLHook();
            KeyLLHook.keyPressDownEvent += new System.Windows.Forms.KeyEventHandler(KeyboardLL_PressDown);
        }

        private void KeyboardLL_PressDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //Do something when a key is pressed
            //tempBox.Text += " " + e.KeyCode.ToString() + ":" + e.KeyValue  + " " + Keys.LMenu.ToString();
            //tempBox.ScrollToEnd();

            //Animate key on press with fade effect
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                

                
                

                if (e.KeyCode.ToString() == val.Value.GetCheckKey())
                {
                    DoubleAnimation dblAnim = new DoubleAnimation();
                    //if (this.Opacity <= 0.5)
                    //{
                    //    dblAnim.From = windowPrefs.keyRectangleOpacity;
                    //    dblAnim.To = 1.0;
                    //}
                    //else
                    //{
                    //    dblAnim.From = windowPrefs.keyRectangleOpacity;
                    //    dblAnim.To = 0.0;
                    //}


                    dblAnim.From = windowPrefs.keyRectangleOpacity;
                    dblAnim.To = 1.0;

                    dblAnim.AutoReverse = true;
                    //fillbehavior allows change of property after animation completion
                    dblAnim.FillBehavior = FillBehavior.Stop;
                    dblAnim.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                    val.Value.BeginAnimation(KeyControl.KeyRectangleOpacityProperty, dblAnim);
                    
                    //change color aswell!
                    ColorAnimation clrAnim = new ColorAnimation();
                    clrAnim.From = val.Value.KeyBackgroundColor;
                    clrAnim.To = Colors.Black;

                    clrAnim.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                    clrAnim.AutoReverse = true;
                    clrAnim.FillBehavior = FillBehavior.Stop;
                    val.Value.BeginAnimation(KeyControl.KeyBackgroundColorProperty, clrAnim);

                    ColorAnimation clrAnim2 = new ColorAnimation();
                    clrAnim2.From = val.Value.KeyLabelColor;
                    clrAnim2.To = Colors.White;

                    clrAnim2.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                    clrAnim2.AutoReverse = true;
                    clrAnim2.FillBehavior = FillBehavior.Stop;
                    val.Value.BeginAnimation(KeyControl.KeyLabelColorProperty, clrAnim2);

                    //for guides
                    ColorAnimation clrAnim3 = new ColorAnimation();
                    clrAnim3.From = val.Value.KeyGuideColor;
                    clrAnim3.To = Colors.Black;

                    clrAnim3.Duration = new Duration(TimeSpan.FromMilliseconds(100));
                    clrAnim3.AutoReverse = true;
                    clrAnim3.FillBehavior = FillBehavior.Stop;
                    val.Value.BeginAnimation(KeyControl.KeyGuideColorProperty, clrAnim3);

                }
            }

            //animate autohide window fade
            if (windowPrefs.autoHideWindow)
            {
                if (fadeTimer.IsEnabled == true)
                {

                    fadeTimer.Interval = TimeSpan.FromSeconds(windowPrefs.fadeDelay);

                    //fadeTimer.Start();
                }
                else
                {
                    this.BeginAnimation(MainWindow.OpacityProperty, null);
                    this.Opacity = windowPrefs.windowOpacity;

                    fadeTimer.Interval = TimeSpan.FromSeconds(windowPrefs.fadeDelay);
                    fadeTimer.Start();
                }
            }
           
        }


        private void ViewBoxRoot_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            winDragging = true;

            winPos = new Point(this.Left, this.Top);
            mousePos = this.PointToScreen(e.GetPosition(this));
            viewBoxRoot.CaptureMouse();

        }

        private void ViewBoxRoot_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            winDragging = false;
            viewBoxRoot.ReleaseMouseCapture();
        }

        private void ViewBoxRoot_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (winDragging)
            {
                Point pos = this.PointToScreen(e.GetPosition(this));
                this.Left = winPos.X + pos.X - mousePos.X;
                this.Top = winPos.Y + pos.Y - mousePos.Y;

                //save to local settings variable
                windowPrefs.winTop = this.Top;
                windowPrefs.winLeft = this.Left;
      
            }
        }


        private void ResizeBottom_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            winResizing = true;
            mousePos = this.PointToScreen(e.GetPosition(this));
            winPos = new Point(this.Width, this.Height);
            resizeBottom.CaptureMouse();
        }

        private void ResizeBottom_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.

            winResizing = false;
            resizeBottom.ReleaseMouseCapture();
            
        }

        private void ResizeBottom_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (winResizing)
            {
                //TextBox1.Text = "Sizing...";	

                Point pos = this.PointToScreen(e.GetPosition(this));
                double tempWidth = winPos.X + pos.X - mousePos.X;
                if (tempWidth >= this.MinWidth)
                    this.Width = tempWidth;
                double tempHeight = winPos.Y + pos.Y - mousePos.Y;
                if (tempHeight >= this.MinHeight)
                    this.Height = tempHeight;

                //save to local settings variable
                windowPrefs.winWidth = this.Width;
                windowPrefs.winHeight = this.Height;
            }

        }

        private void ResizeRight_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.
            winResizing = true;
            mousePos = this.PointToScreen(e.GetPosition(this));
            winPos = new Point(this.Width, this.Height);
            resizeRight.CaptureMouse();

        }

        private void ResizeRight_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Add event handler implementation here.

            winResizing = false;
            resizeRight.ReleaseMouseCapture();
        }

        private void ResizeRight_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (winResizing)
            {
                //TextBox1.Text = "Sizing...";	

                Point pos = this.PointToScreen(e.GetPosition(this));
                double tempWidth = winPos.X + pos.X - mousePos.X;
                if (tempWidth >= this.MinWidth)
                    this.Width = tempWidth;
                double tempHeight = winPos.Y + pos.Y - mousePos.Y;
                if (tempHeight >= this.MinHeight)
                    this.Height = tempHeight;

                //save to local settings variable
                windowPrefs.winWidth = this.Width;
                windowPrefs.winHeight = this.Height;
            }

        }





        private void Initialise_KeyboardLayout()
        {
            if (windowPrefs.layoutType == keyboardLayoutType.QWERTY)
            {
                Initialise_KeyboardLayoutQwerty();

               
            }
            else if (windowPrefs.layoutType == keyboardLayoutType.COLEMAK)
            {
                Initialise_KeyboardLayoutColemak();

               
            }
            else if (windowPrefs.layoutType == keyboardLayoutType.DEVORAK)
            {
                Initialise_KeyboardLayoutDevorak();

                
            }

            Initialise_GuideColors();
        }


        private void Initialise_KeyboardLayoutQwerty()
        {
            //clear previous layout type
            if (keyboardLayout != null)
            {
                keyboardLayout.Clear();
                stackRow1.Children.Clear();
                stackRow2.Children.Clear();
                stackRow3.Children.Clear();
                stackRow4.Children.Clear();
                stackRow5.Children.Clear();
            }

            //initialise new keycontrols
            //TODO resize key widths to compensate for margins
            keyboardLayout = new Dictionary<Keys, KeyControl>()
            {
                {Keys.Oemtilde, new KeyControl("`  ~", Keys.Oemtilde.ToString())},{Keys.D1, new KeyControl("1  !", Keys.D1.ToString())},
                {Keys.D2, new KeyControl("2  @", Keys.D2.ToString())},{Keys.D3, new KeyControl("3  #", Keys.D3.ToString())},
                {Keys.D4, new KeyControl("4  $", Keys.D4.ToString())},{Keys.D5, new KeyControl("5  %", Keys.D5.ToString())},
                {Keys.D6, new KeyControl("6  ^", Keys.D6.ToString())},{Keys.D7, new KeyControl("7  &", Keys.D7.ToString())},
                {Keys.D8, new KeyControl("8  *", Keys.D8.ToString())},{Keys.D9, new KeyControl("9  (", Keys.D9.ToString())},
                {Keys.D0, new KeyControl("0  )", Keys.D0.ToString())},{Keys.OemMinus, new KeyControl("-  _", Keys.OemMinus.ToString())},
                {Keys.Oemplus, new KeyControl("=  +", Keys.Oemplus.ToString())},{Keys.Back, new KeyControl("Back", Keys.Back.ToString(), (float)60.0, 0)},

                {Keys.Tab, new KeyControl("Tab", Keys.Tab.ToString(),(float)45.0, (float)0.0)},{Keys.Q, new KeyControl("Q", Keys.Q.ToString())},
                {Keys.W, new KeyControl("W", Keys.W.ToString())},{Keys.E, new KeyControl("E", Keys.E.ToString())},
                {Keys.R, new KeyControl("R", Keys.R.ToString())},{Keys.T, new KeyControl("T", Keys.T.ToString())},
                {Keys.Y, new KeyControl("Y", Keys.Y.ToString())},{Keys.U, new KeyControl("U", Keys.U.ToString())},
                {Keys.I, new KeyControl("I", Keys.I.ToString())},{Keys.O, new KeyControl("O", Keys.O.ToString())},
                {Keys.P, new KeyControl("P", Keys.P.ToString())},{Keys.OemOpenBrackets, new KeyControl("[  {", Keys.OemOpenBrackets.ToString())},
                {Keys.Oem6, new KeyControl("]  }", Keys.Oem6.ToString())},{Keys.Oem5, new KeyControl("\\  |", Keys.Oem5.ToString(), (float)45.0,0)},

                {Keys.CapsLock, new KeyControl("CapsLk", Keys.CapsLock.ToString(),(float)52.5, 0)},{Keys.A, new KeyControl("_A", Keys.A.ToString())},
                {Keys.S, new KeyControl("_S", Keys.S.ToString())},{Keys.D, new KeyControl("_D", Keys.D.ToString())},
                {Keys.F, new KeyControl("_F", Keys.F.ToString())},{Keys.G, new KeyControl("G", Keys.G.ToString())},
                {Keys.H, new KeyControl("H", Keys.H.ToString())},{Keys.J, new KeyControl("_J", Keys.J.ToString())},
                {Keys.K, new KeyControl("_K", Keys.K.ToString())},{Keys.L, new KeyControl("_L", Keys.L.ToString())},
                {Keys.Oem1, new KeyControl("_;  :", Keys.Oem1.ToString())},{Keys.Oem7, new KeyControl("'  \"", Keys.Oem7.ToString())},
                {Keys.Enter, new KeyControl("Enter", Keys.Enter.ToString(),(float)67.5, 0)},
                
                {Keys.LShiftKey, new KeyControl("Shift", Keys.LShiftKey.ToString(), (float)67.5, 0)},
                {Keys.Z, new KeyControl("Z", Keys.Z.ToString())},{Keys.X, new KeyControl("X", Keys.X.ToString())},
                {Keys.C, new KeyControl("C", Keys.C.ToString())},{Keys.V, new KeyControl("V", Keys.V.ToString())},
                {Keys.B, new KeyControl("B", Keys.B.ToString())},{Keys.N, new KeyControl("N", Keys.N.ToString())},
                {Keys.M, new KeyControl("M", Keys.M.ToString())},{Keys.Oemcomma, new KeyControl(",  <", Keys.Oemcomma.ToString())},
                {Keys.OemPeriod, new KeyControl(".  >", Keys.OemPeriod.ToString())},{Keys.OemQuestion, new KeyControl("/  ?", Keys.OemQuestion.ToString())},
                {Keys.RShiftKey, new KeyControl("Shift", Keys.RShiftKey.ToString(),(float)82.5, 0)},
                
                
                {Keys.LControlKey, new KeyControl("Ctrl", Keys.LControlKey.ToString(),(float)45.0,0)},
                {Keys.LWin, new KeyControl("Win", Keys.LWin.ToString())},{Keys.LMenu, new KeyControl("Alt", Keys.LMenu.ToString(),(float)45.0,0)},
                {Keys.Space, new KeyControl("_Space", Keys.Space.ToString(), (float)180.0,0)},{Keys.RMenu, new KeyControl("Alt", Keys.RMenu.ToString(),(float)45.0,0)},
                {Keys.RWin, new KeyControl("Win", Keys.RWin.ToString())},{Keys.Apps, new KeyControl("Mu", Keys.Apps.ToString())},
                {Keys.RControlKey, new KeyControl("Ctrl", Keys.RControlKey.ToString(),(float)45.0,0)}
            };

            //add keys to window layout
            int count = 0;
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                //add opacity settings
                val.Value.KeyRectangleOpacity = windowPrefs.keyRectangleOpacity;
                val.Value.KeyLabelOpacity = windowPrefs.KeyLabelOpacity;

                //add margin to keys
                val.Value.Margin = new Thickness(windowPrefs.keyMargin);
                
                //add radiusX radiusY
                val.Value.KeyRectangleRadiusX = windowPrefs.keyRectangleRadiusX;
                val.Value.KeyRectangleRadiusY = windowPrefs.keyRectangleRadiusY;

                //add key colors
                val.Value.KeyBackgroundColor = MyColorConverter.ArrayToColor(windowPrefs.backgroundColor);
                val.Value.KeyLabelColor = MyColorConverter.ArrayToColor(windowPrefs.foregroundColor);
                val.Value.KeyBorderColor = MyColorConverter.ArrayToColor(windowPrefs.borderColor);

                //set font styles
                val.Value.KeyboardLabelFontBold(windowPrefs.fontBold);

                val.Value.KeyboardLabelFontItalic(windowPrefs.fontItalic);

                //set show guides
                val.Value.KeyboardGuideVisibility(windowPrefs.showGuide);

                //add to layout
                if (count >= 0 && count <= 13)
                {
                    stackRow1.Children.Add(val.Value);
                }
                else if (count >= 14 && count <= 27)
                {
                    stackRow2.Children.Add(val.Value);
                }
                else if (count >= 28 && count <= 40)
                {
                    stackRow3.Children.Add(val.Value);
                }
                else if (count >= 41 && count <= 52)
                {
                    stackRow4.Children.Add(val.Value);
                }
                else if (count >= 53 && count <= 60)
                {
                    stackRow5.Children.Add(val.Value);
                }
                count++;
            }

            windowPrefs.layoutType = keyboardLayoutType.QWERTY;
            
        }

        private void Initialise_KeyboardLayoutColemak()
        {
             //clear previous layout type
            if (keyboardLayout != null)
            {
                keyboardLayout.Clear();
                stackRow1.Children.Clear();
                stackRow2.Children.Clear();
                stackRow3.Children.Clear();
                stackRow4.Children.Clear();
                stackRow5.Children.Clear();
            }

            //initialise new keycontrols
            //TODO resize key widths to compensate for margins
            keyboardLayout = new Dictionary<Keys, KeyControl>()
            {
                {Keys.Oemtilde, new KeyControl("`  ~", Keys.Oemtilde.ToString())},{Keys.D1, new KeyControl("1  !", Keys.D1.ToString())},
                {Keys.D2, new KeyControl("2  @", Keys.D2.ToString())},{Keys.D3, new KeyControl("3  #", Keys.D3.ToString())},
                {Keys.D4, new KeyControl("4  $", Keys.D4.ToString())},{Keys.D5, new KeyControl("5  %", Keys.D5.ToString())},
                {Keys.D6, new KeyControl("6  ^", Keys.D6.ToString())},{Keys.D7, new KeyControl("7  &", Keys.D7.ToString())},
                {Keys.D8, new KeyControl("8  *", Keys.D8.ToString())},{Keys.D9, new KeyControl("9  (", Keys.D9.ToString())},
                {Keys.D0, new KeyControl("0 )", Keys.D0.ToString())},{Keys.OemMinus, new KeyControl("-  _", Keys.OemMinus.ToString())},
                {Keys.Oemplus, new KeyControl("=  +", Keys.Oemplus.ToString())},{Keys.Back, new KeyControl("Back", Keys.Back.ToString(), (float)60.0, 0)},

                {Keys.Tab, new KeyControl("Tab", Keys.Tab.ToString(),(float)45.0, (float)0.0)},{Keys.Q, new KeyControl("Q", Keys.Q.ToString())},
                {Keys.W, new KeyControl("W", Keys.W.ToString())},{Keys.E, new KeyControl("F", Keys.F.ToString())},
                {Keys.R, new KeyControl("P", Keys.P.ToString())},{Keys.T, new KeyControl("G", Keys.G.ToString())},
                {Keys.Y, new KeyControl("J", Keys.J.ToString())},{Keys.U, new KeyControl("L", Keys.L.ToString())},
                {Keys.I, new KeyControl("U", Keys.U.ToString())},{Keys.O, new KeyControl("Y", Keys.Y.ToString())},
                {Keys.P, new KeyControl(";  :", Keys.Oem1.ToString())},{Keys.OemOpenBrackets, new KeyControl("[  {", Keys.OemOpenBrackets.ToString())},
                {Keys.Oem6, new KeyControl("]  }", Keys.Oem6.ToString())},{Keys.Oem5, new KeyControl("\\  |", Keys.Oem5.ToString(), (float)45.0,0)},

                {Keys.CapsLock, new KeyControl("Back", Keys.Back.ToString(),(float)52.5, 0)},{Keys.A, new KeyControl("_A", Keys.A.ToString())},
                {Keys.S, new KeyControl("_R", Keys.R.ToString())},{Keys.D, new KeyControl("_S", Keys.S.ToString())},
                {Keys.F, new KeyControl("_T", Keys.T.ToString())},{Keys.G, new KeyControl("D", Keys.D.ToString())},
                {Keys.H, new KeyControl("H", Keys.H.ToString())},{Keys.J, new KeyControl("_N", Keys.N.ToString())},
                {Keys.K, new KeyControl("_E", Keys.E.ToString())},{Keys.L, new KeyControl("_I", Keys.I.ToString())},
                {Keys.Oem1, new KeyControl("_O", Keys.O.ToString())},{Keys.Oem7, new KeyControl("'  \"", Keys.Oem7.ToString())},
                {Keys.Enter, new KeyControl("Enter", Keys.Enter.ToString(),(float)67.5, 0)},
                
                {Keys.LShiftKey, new KeyControl("Shift", Keys.LShiftKey.ToString(), (float)67.5, 0)},
                {Keys.Z, new KeyControl("Z", Keys.Z.ToString())},{Keys.X, new KeyControl("X", Keys.X.ToString())},
                {Keys.C, new KeyControl("C", Keys.C.ToString())},{Keys.V, new KeyControl("V", Keys.V.ToString())},
                {Keys.B, new KeyControl("B", Keys.B.ToString())},{Keys.N, new KeyControl("K", Keys.K.ToString())},
                {Keys.M, new KeyControl("M", Keys.M.ToString())},{Keys.Oemcomma, new KeyControl(",  <", Keys.Oemcomma.ToString())},
                {Keys.OemPeriod, new KeyControl(".  >", Keys.OemPeriod.ToString())},{Keys.OemQuestion, new KeyControl("/  ?", Keys.OemQuestion.ToString())},
                {Keys.RShiftKey, new KeyControl("Shift", Keys.RShiftKey.ToString(),(float)82.5, 0)},
                
                
                {Keys.LControlKey, new KeyControl("Ctrl", Keys.LControlKey.ToString(),(float)45.0,0)},
                {Keys.LWin, new KeyControl("Win", Keys.LWin.ToString())},{Keys.LMenu, new KeyControl("Alt", Keys.LMenu.ToString(),(float)45.0,0)},
                {Keys.Space, new KeyControl("_Space", Keys.Space.ToString(), (float)180.0,0)},{Keys.RMenu, new KeyControl("Alt", Keys.RMenu.ToString(),(float)45.0,0)},
                {Keys.RWin, new KeyControl("Win", Keys.RWin.ToString())},{Keys.Apps, new KeyControl("Mu", Keys.Apps.ToString())},
                {Keys.RControlKey, new KeyControl("Ctrl", Keys.RControlKey.ToString(),(float)45.0,0)}
            };

            //add keys to window layout
            int count = 0;
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                //add opacity settings
                val.Value.KeyRectangleOpacity = windowPrefs.keyRectangleOpacity;
                val.Value.KeyLabelOpacity = windowPrefs.KeyLabelOpacity;

                //add margin to keys
                val.Value.Margin = new Thickness(windowPrefs.keyMargin);
                
                //add radiusX radiusY
                val.Value.KeyRectangleRadiusX = windowPrefs.keyRectangleRadiusX;
                val.Value.KeyRectangleRadiusY = windowPrefs.keyRectangleRadiusY;

                //add key colors
                val.Value.KeyBackgroundColor = MyColorConverter.ArrayToColor(windowPrefs.backgroundColor);
                val.Value.KeyLabelColor = MyColorConverter.ArrayToColor(windowPrefs.foregroundColor);
                val.Value.KeyBorderColor = MyColorConverter.ArrayToColor(windowPrefs.borderColor);

                //set font styles
                val.Value.KeyboardLabelFontBold(windowPrefs.fontBold);

                val.Value.KeyboardLabelFontItalic(windowPrefs.fontItalic);

                //set show guides
                val.Value.KeyboardGuideVisibility(windowPrefs.showGuide);

                //add to layout
                if (count >= 0 && count <= 13)
                {
                    stackRow1.Children.Add(val.Value);
                }
                else if (count >= 14 && count <= 27)
                {
                    stackRow2.Children.Add(val.Value);
                }
                else if (count >= 28 && count <= 40)
                {
                    stackRow3.Children.Add(val.Value);
                }
                else if (count >= 41 && count <= 52)
                {
                    stackRow4.Children.Add(val.Value);
                }
                else if (count >= 53 && count <= 60)
                {
                    stackRow5.Children.Add(val.Value);
                }
                count++;
            }


            windowPrefs.layoutType = keyboardLayoutType.COLEMAK;
        }
        private void Initialise_KeyboardLayoutDevorak()
        {
           //TODO
            //clear previous layout type
            if (keyboardLayout != null)
            {
                keyboardLayout.Clear();
                stackRow1.Children.Clear();
                stackRow2.Children.Clear();
                stackRow3.Children.Clear();
                stackRow4.Children.Clear();
                stackRow5.Children.Clear();
            }

            //initialise new keycontrols
            //TODO resize key widths to compensate for margins
            keyboardLayout = new Dictionary<Keys, KeyControl>()
            {
                {Keys.Oemtilde, new KeyControl("`  ~", Keys.Oemtilde.ToString())},{Keys.D1, new KeyControl("1  !", Keys.D1.ToString())},
                {Keys.D2, new KeyControl("2  @", Keys.D2.ToString())},{Keys.D3, new KeyControl("3  #", Keys.D3.ToString())},
                {Keys.D4, new KeyControl("4  $", Keys.D4.ToString())},{Keys.D5, new KeyControl("5  %", Keys.D5.ToString())},
                {Keys.D6, new KeyControl("6  ^", Keys.D6.ToString())},{Keys.D7, new KeyControl("7  &", Keys.D7.ToString())},
                {Keys.D8, new KeyControl("8  *", Keys.D8.ToString())},{Keys.D9, new KeyControl("9  (", Keys.D9.ToString())},
                {Keys.D0, new KeyControl("0  )", Keys.D0.ToString())},{Keys.OemMinus, new KeyControl("[  {", Keys.OemOpenBrackets.ToString())},
                {Keys.Oemplus, new KeyControl("]  }", Keys.Oem6.ToString())},{Keys.Back, new KeyControl("Back", Keys.Back.ToString(), (float)60.0, 0)},


                {Keys.Tab, new KeyControl("Tab", Keys.Tab.ToString(),(float)45.0, (float)0.0)},{Keys.Q, new KeyControl("'  \"", Keys.Oem7.ToString())},
                {Keys.W, new KeyControl(",  <", Keys.Oemcomma.ToString())},{Keys.E, new KeyControl(".  >", Keys.OemPeriod.ToString())},
                {Keys.R, new KeyControl("P", Keys.P.ToString())},{Keys.T, new KeyControl("Y", Keys.Y.ToString())},
                {Keys.Y, new KeyControl("F", Keys.F.ToString())},{Keys.U, new KeyControl("G", Keys.G.ToString())},
                {Keys.I, new KeyControl("C", Keys.C.ToString())},{Keys.O, new KeyControl("R", Keys.R.ToString())},
                {Keys.P, new KeyControl("L", Keys.L.ToString())},{Keys.OemOpenBrackets, new KeyControl("/  ?", Keys.OemQuestion.ToString())},
                {Keys.Oem6, new KeyControl("=  +", Keys.Oemplus.ToString())},{Keys.Oem5, new KeyControl("\\  |", Keys.Oem5.ToString(), (float)45.0,0)},

                {Keys.CapsLock, new KeyControl("CapsLk", Keys.CapsLock.ToString(),(float)52.5, 0)},{Keys.A, new KeyControl("_A", Keys.A.ToString())},
                {Keys.S, new KeyControl("_O", Keys.O.ToString())},{Keys.D, new KeyControl("_E", Keys.E.ToString())},
                {Keys.F, new KeyControl("_U", Keys.U.ToString())},{Keys.G, new KeyControl("I", Keys.I.ToString())},
                {Keys.H, new KeyControl("D", Keys.D.ToString())},{Keys.J, new KeyControl("_H", Keys.H.ToString())},
                {Keys.K, new KeyControl("_T", Keys.T.ToString())},{Keys.L, new KeyControl("_N", Keys.N.ToString())},
                {Keys.Oem1, new KeyControl("_S", Keys.S.ToString())},{Keys.Oem7, new KeyControl("-  _", Keys.OemMinus.ToString())},
                {Keys.Enter, new KeyControl("Enter", Keys.Enter.ToString(),(float)67.5, 0)},
                
                {Keys.LShiftKey, new KeyControl("Shift", Keys.LShiftKey.ToString(), (float)67.5, 0)},
                {Keys.Z, new KeyControl(";  :", Keys.Oem1.ToString())},{Keys.X, new KeyControl("Q", Keys.Q.ToString())},
                {Keys.C, new KeyControl("J", Keys.J.ToString())},{Keys.V, new KeyControl("K", Keys.K.ToString())},
                {Keys.B, new KeyControl("X", Keys.X.ToString())},{Keys.N, new KeyControl("B", Keys.B.ToString())},
                {Keys.M, new KeyControl("M", Keys.M.ToString())},{Keys.Oemcomma, new KeyControl("W", Keys.W.ToString())},
                {Keys.OemPeriod, new KeyControl("V", Keys.V.ToString())},{Keys.OemQuestion, new KeyControl("Z", Keys.Z.ToString())},
                {Keys.RShiftKey, new KeyControl("Shift", Keys.RShiftKey.ToString(),(float)82.5, 0)},
                
                
                {Keys.LControlKey, new KeyControl("Ctrl", Keys.LControlKey.ToString(),(float)45.0,0)},
                {Keys.LWin, new KeyControl("Win", Keys.LWin.ToString())},{Keys.LMenu, new KeyControl("Alt", Keys.LMenu.ToString(),(float)45.0,0)},
                {Keys.Space, new KeyControl("_Space", Keys.Space.ToString(), (float)180.0,0)},{Keys.RMenu, new KeyControl("Alt", Keys.RMenu.ToString(),(float)45.0,0)},
                {Keys.RWin, new KeyControl("Win", Keys.RWin.ToString())},{Keys.Apps, new KeyControl("Mu", Keys.Apps.ToString())},
                {Keys.RControlKey, new KeyControl("Ctrl", Keys.RControlKey.ToString(),(float)45.0,0)}
            };

            //add keys to window layout
            int count = 0;
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                //add opacity settings
                val.Value.KeyRectangleOpacity = windowPrefs.keyRectangleOpacity;
                val.Value.KeyLabelOpacity = windowPrefs.KeyLabelOpacity;

                //add margin to keys
                val.Value.Margin = new Thickness(windowPrefs.keyMargin);

                //add radiusX radiusY
                val.Value.KeyRectangleRadiusX = windowPrefs.keyRectangleRadiusX;
                val.Value.KeyRectangleRadiusY = windowPrefs.keyRectangleRadiusY;

                //add key colors
                val.Value.KeyBackgroundColor = MyColorConverter.ArrayToColor(windowPrefs.backgroundColor);
                val.Value.KeyLabelColor = MyColorConverter.ArrayToColor(windowPrefs.foregroundColor);
                val.Value.KeyBorderColor = MyColorConverter.ArrayToColor(windowPrefs.borderColor);

                //set font styles
                val.Value.KeyboardLabelFontBold(windowPrefs.fontBold);

                val.Value.KeyboardLabelFontItalic(windowPrefs.fontItalic);

                //set show guides
                val.Value.KeyboardGuideVisibility(windowPrefs.showGuide);

                //add to layout
                if (count >= 0 && count <= 13)
                {
                    stackRow1.Children.Add(val.Value);
                }
                else if (count >= 14 && count <= 27)
                {
                    stackRow2.Children.Add(val.Value);
                }
                else if (count >= 28 && count <= 40)
                {
                    stackRow3.Children.Add(val.Value);
                }
                else if (count >= 41 && count <= 52)
                {
                    stackRow4.Children.Add(val.Value);
                }
                else if (count >= 53 && count <= 60)
                {
                    stackRow5.Children.Add(val.Value);
                }
                count++;
            }

            windowPrefs.layoutType = keyboardLayoutType.DEVORAK;
        }


        public void Show_SettingWindow()
        {
            //System.Windows.Forms.MessageBox.Show("Options window Started");

            //if (settingsWindow.Visibility == Visibility.Hidden)
            //{

            //    settingsWindow.Visibility = Visibility.Visible;

            //}

            if(settingsWindow != null)
                settingsWindow.Close();

            settingsWindow = new SettingsWindow(this, windowPrefs);
            settingsWindow.Show();
                

        }

        public void Set_WindowOpacity(double value)
        {
            this.Opacity = value;

            windowPrefs.windowOpacity = value;
        }

        public void Set_KeyRectangleOpacity(double value)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyRectangleOpacity = value;
            }

            windowPrefs.keyRectangleOpacity = value;
        }

        public void Set_KeyLabelOpacity(double value)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyLabelOpacity = value;
            }

            windowPrefs.KeyLabelOpacity = value;
        }

        public void Set_KeyMargin(double value)
        {
            //windowPrefs.keyMargin = value;

            //Initialise_KeyboardLayoutUSInternational();
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.Margin = new Thickness(value);
            }

            windowPrefs.keyMargin = value;

        }

        public void Set_KeyRectangleRadiusX(double value)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyRectangleRadiusX = value;
            }

            windowPrefs.keyRectangleRadiusX = value;
        }

        public void Set_KeyRectangleRadiusY(double value)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyRectangleRadiusY = value;
            }

            windowPrefs.keyRectangleRadiusY = value;
        }

        public void Set_KeyForegroundColor(Color color)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyLabelColor = color;
            }

        //    //windowPrefs.foregroundColor = color;
            windowPrefs.foregroundColor = MyColorConverter.ColorToArray(color);
        }

        public void Set_KeyBackgroundColor(Color color)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyBackgroundColor = color;
            }

            //    //windowPrefs.foregroundColor = color;
            windowPrefs.backgroundColor = MyColorConverter.ColorToArray(color);
        }

        public void Set_KeyBorderColor(Color color)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyBorderColor = color;
            }

            //    //windowPrefs.foregroundColor = color;
            windowPrefs.borderColor = MyColorConverter.ColorToArray(color);
        }

        public void Set_KeyFontBold(bool value)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyboardLabelFontBold(value);
            }

            windowPrefs.fontBold = value;
        }

        public void Set_KeyFontItalic(bool value)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyboardLabelFontItalic(value);
            }

            windowPrefs.fontItalic = value;
        }

        public void Set_KeyboardLayout(keyboardLayoutType type)
        {
            if (type == keyboardLayoutType.QWERTY)
            {
                Initialise_KeyboardLayoutQwerty();

                
            }
            else if (type == keyboardLayoutType.COLEMAK)
            {
                Initialise_KeyboardLayoutColemak();

                
            }
            else if (type == keyboardLayoutType.DEVORAK)
            {
                Initialise_KeyboardLayoutDevorak();

                
            }

            Initialise_GuideColors();
       }

        public keyboardLayoutType Get_Keyboardlayout()
        {

            return windowPrefs.layoutType;
        }

        public void Set_MovableFlag(bool value)
        {
            windowPrefs.movableWindow = value;
        }

        public bool Get_MovableFlag()
        {
            return windowPrefs.movableWindow;
        }

        public void Start_AutoHideWindow()
        {

            fadeTimer.Start();

            windowPrefs.autoHideWindow = true;
        }

        public void Stop_AutohideWindow()
        {
            fadeTimer.Stop();

            this.BeginAnimation(MainWindow.OpacityProperty, null);
            this.Opacity = windowPrefs.windowOpacity;

            windowPrefs.autoHideWindow = false;
        }

        private void TimerCallback(object sender, EventArgs e)
        {

            DoubleAnimation doubleAnim = new DoubleAnimation();
            doubleAnim.From = windowPrefs.windowOpacity;
            doubleAnim.To = 0.0;
            //doubleAnim.FillBehavior = FillBehavior.Stop;
            this.BeginAnimation(MainWindow.OpacityProperty, doubleAnim);
            fadeTimer.Stop();

        }

        public void Intialise_FadeTimer()
        {
            if (fadeTimer != null)
            {
                //fadeTimer.Tick -= TimerCallback;
                fadeTimer = null; //dispose instead....
            }

            fadeTimer = new DispatcherTimer();
            fadeTimer.Interval = TimeSpan.FromSeconds(windowPrefs.fadeDelay);
            fadeTimer.Tick += TimerCallback;

            if (windowPrefs.autoHideWindow)
            {
                Start_AutoHideWindow();
            }
            else
            {
                Stop_AutohideWindow();
            }

        }

        public void Set_FadeDelay(double value)
        {
            windowPrefs.fadeDelay = value;
            
            //stop timer
            fadeTimer.Stop();

            this.BeginAnimation(MainWindow.OpacityProperty, null);
            this.Opacity = windowPrefs.windowOpacity;
            
            //set new delay and start timer
            Intialise_FadeTimer();

            
        }

        
        public bool Get_AutoHideFlag()
        {
            return windowPrefs.autoHideWindow;
        }

        public void Set_WindowDefaults()
        {
            keyboardLayoutType tempLayout = windowPrefs.layoutType;

            Save_Defaults();



            //Intialise_FadeTimer();

            //Initialise_KeyboardLayout();

            //if (settingsWindow != null)
            //    settingsWindow.Close();

            //settingsWindow = new SettingsWindow(this, windowPrefs);
            //settingsWindow.Show();

            //event to update notifyicon / autohide item
            OnSetDefaults();
        }


        ///http://dotnet-snippets.com/dns/c-addremove-registry-entries-for-windows-startup-SID438.aspx
        private void SetStartupReg(string AppName, bool enable)
        {
            string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

            Microsoft.Win32.RegistryKey startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey);

            if (enable)
            {
                if (startupKey.GetValue(AppName) == null)
                {
                    startupKey.Close();
                    startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey, true);
                    // Add startup reg key
                    startupKey.SetValue(AppName, System.Windows.Forms.Application.ExecutablePath.ToString());
                    startupKey.Close();
                }


            }
            else
            {
                // remove startup
                startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey, true);
                startupKey.DeleteValue(AppName, false);
                startupKey.Close();
            }
        }

        public void setStartupShortcut(bool value)
        {
            if (value == true)
            {
                //copy lik
                

                if (File.Exists(Environment.CurrentDirectory + "\\ColeType.lnk"))
                {
                    File.Copy(Environment.CurrentDirectory + "\\ColeType.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\ColeType.lnk");  
                }

            }
            else
            {
                //delete lik
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\ColeType.lnk"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\ColeType.lnk");
                }
            }
           
        }

        public void Set_LoadOnStartup( bool value)
        {
            //SetStartupReg("ColeType", (bool)value);
            setStartupShortcut(value);
            windowPrefs.loadOnStartup = value;
        }

        public double Get_CurrentRadiusX()
        {
            return windowPrefs.keyRectangleRadiusX;
        }

        public double Get_CurrentRadiusY()
        {
            return windowPrefs.keyRectangleRadiusY;
        }

        public void Show_KeyboardGuide(bool value)
        {
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                val.Value.KeyboardGuideVisibility(value);
            }

            windowPrefs.showGuide = value;

        }

        public bool Get_ShowGuide()
        {
            return windowPrefs.showGuide;
        }

        public void Set_GuideColors(Color color, SelectFor finger)
        {
            if (finger == SelectFor.LINDEX)
            {
                
                //save color to windowPrefs
                windowPrefs.LIndexColor = MyColorConverter.ColorToArray(color);

                
            }
            else if (finger == SelectFor.RINDEX)
            {
                windowPrefs.RIndexColor = MyColorConverter.ColorToArray(color);
            }
            else if (finger == SelectFor.MIDDLE)
            {

                windowPrefs.MiddleColor = MyColorConverter.ColorToArray(color);

            }
            else if (finger == SelectFor.RING)
            {

                windowPrefs.RingColor = MyColorConverter.ColorToArray(color);

            }
            else if (finger == SelectFor.LITTLE)
            {

                windowPrefs.LittleColor = MyColorConverter.ColorToArray(color);

            }
            else if (finger == SelectFor.THUMB)
            {

                windowPrefs.ThumbColor = MyColorConverter.ColorToArray(color);

            }

            //change color for specific keys
            Initialise_GuideColors();
        }

        public void Initialise_GuideColors()
        {
          
            foreach (KeyValuePair<Keys, KeyControl> val in keyboardLayout)
            {
                if (val.Key == Keys.D4 || val.Key == Keys.D5 ||
                        val.Key == Keys.R || val.Key == Keys.T || 
                        val.Key == Keys.F || val.Key == Keys.G || 
                        val.Key == Keys.V || val.Key == Keys.B)
                {

                    val.Value.KeyGuideColor = MyColorConverter.ArrayToColor(windowPrefs.LIndexColor);
                }
                else if(val.Key == Keys.D6 || val.Key == Keys.D7 || 
                            val.Key == Keys.Y || val.Key == Keys.U ||
                            val.Key == Keys.H || val.Key == Keys.J ||
                            val.Key == Keys.N || val.Key == Keys.M)
                {

                    val.Value.KeyGuideColor = MyColorConverter.ArrayToColor(windowPrefs.RIndexColor);
                }
                else if (val.Key == Keys.D3 || val.Key == Keys.D8 ||
                           val.Key == Keys.E || val.Key == Keys.I ||
                           val.Key == Keys.D || val.Key == Keys.K ||
                           val.Key == Keys.C || val.Key == Keys.Oemcomma)
                {

                    val.Value.KeyGuideColor = MyColorConverter.ArrayToColor(windowPrefs.MiddleColor);
                }
                else if (val.Key == Keys.D2 || val.Key == Keys.D9 ||
                           val.Key == Keys.W || val.Key == Keys.O ||
                           val.Key == Keys.S || val.Key == Keys.L ||
                           val.Key == Keys.X || val.Key == Keys.OemPeriod)
                {

                    val.Value.KeyGuideColor = MyColorConverter.ArrayToColor(windowPrefs.RingColor);
                }
                else if (val.Key == Keys.Oemtilde || val.Key == Keys.D1 || val.Key == Keys.D0 || val.Key == Keys.OemMinus || val.Key == Keys.Oemplus || val.Key == Keys.Back ||
                           val.Key == Keys.Q || val.Key == Keys.P || val.Key == Keys.OemOpenBrackets || val.Key == Keys.Oem5 || val.Key == Keys.Oem6 ||
                           val.Key == Keys.A || val.Key == Keys.Oem1 || val.Key == Keys.Oem7 ||
                           val.Key == Keys.Z || val.Key == Keys.OemQuestion || 
                    val.Key == Keys.Tab || val.Key == Keys.CapsLock || val.Key == Keys.LShiftKey || val.Key == Keys.LControlKey || val.Key == Keys.LWin ||
                    val.Key == Keys.Enter || val.Key == Keys.RShiftKey || val.Key == Keys.RControlKey || val.Key == Keys.Apps || val.Key == Keys.RWin )
                {

                    val.Value.KeyGuideColor = MyColorConverter.ArrayToColor(windowPrefs.LittleColor);
                }
                else
                {
                    val.Value.KeyGuideColor = MyColorConverter.ArrayToColor(windowPrefs.ThumbColor);
                }

            }
        }

 	}

    

}



