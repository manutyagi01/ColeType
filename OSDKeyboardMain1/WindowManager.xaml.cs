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
using System.Drawing;
using System.IO;

namespace OSDKeyboardMain1
{
    /// <summary>
    /// Interaction logic for WindowManager.xaml
    /// </summary>
    public partial class WindowManager : Window
    {


        private NotifyIcon myTrayIcon;
        private System.ComponentModel.IContainer myTrayContainer;
        private System.Windows.Forms.ContextMenuStrip myTrayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ResizeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LayoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem QwertySubMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DevorakSubMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ColemakSubMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AutohideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowGuideMenuItem;

        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;

        private System.Windows.Forms.ToolStripSeparator seperatorFirst, seperatorSecond;


        MainWindow myWindow;

        AboutWindow myAboutWindow;

        public WindowManager()
        {
            InitializeComponent();

            //Initialise Tray Icon
         

            ///////////////////////////
            myTrayContainer = new System.ComponentModel.Container();
            myTrayContextMenu = new System.Windows.Forms.ContextMenuStrip();

            QwertySubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            QwertySubMenuItem.Click += QwertySubItem_Click;
            //QwertySubMenuItem.Index = 0;
            QwertySubMenuItem.Text = "&Qwerty";

            DevorakSubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            DevorakSubMenuItem.Click += DevorakSubItem_Click;
            //DevorakSubMenuItem.Index = 1;
            DevorakSubMenuItem.Text = "&Dvorak";

            ColemakSubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ColemakSubMenuItem.Click += ColemakSubItem_Click;
            //ColemakSubMenuItem.Index = 2;
            ColemakSubMenuItem.Text = "&Colemak";


            LayoutMenuItem = new System.Windows.Forms.ToolStripMenuItem(" ", null, new ToolStripItem[] { QwertySubMenuItem, DevorakSubMenuItem, ColemakSubMenuItem });
            //LayoutMenuItem.Click += SettingsMenuItem_Click;
            //LayoutMenuItem.Index = 3;
            LayoutMenuItem.Text = "&Layout";


            myTrayContextMenu.Items.Add(LayoutMenuItem);


            seperatorFirst = new ToolStripSeparator();

            myTrayContextMenu.Items.Add(seperatorFirst);




            AutohideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            AutohideMenuItem.Click += AutoHideMenuItem_Click;
            //AutohideMenuItem.Index = 4;
            AutohideMenuItem.Text = "&AutoHide";


            myTrayContextMenu.Items.Add(AutohideMenuItem);

            ShowGuideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ShowGuideMenuItem.Click += ShowGuideItem_Click;
            //ResizeMenuItem.Index = 1;
            ShowGuideMenuItem.Text = "&Guides";

            myTrayContextMenu.Items.Add(ShowGuideMenuItem);

            ResizeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ResizeMenuItem.Click += ResizeMenuItem_Click;
            //ResizeMenuItem.Index = 1;
            ResizeMenuItem.Text = "&Resize";

            myTrayContextMenu.Items.Add(ResizeMenuItem);

            SettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            SettingsMenuItem.Click += SettingsMenuItem_Click;
            //SettingsMenuItem.Index = 2;
            SettingsMenuItem.Text = "&Settings";


            myTrayContextMenu.Items.Add(SettingsMenuItem);

            AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            AboutMenuItem.Click += AboutMenuItem_Click;
            //SettingsMenuItem.Index = 2;
            AboutMenuItem.Text = "&About..";


            myTrayContextMenu.Items.Add(AboutMenuItem);

            seperatorSecond = new ToolStripSeparator();

            myTrayContextMenu.Items.Add(seperatorSecond);

            ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ExitMenuItem.Click += ExitMenuItem_Click;
            //ExitMenuItem.Index = 0;
            ExitMenuItem.Text = "E&xit";

            myTrayContextMenu.Items.Add(ExitMenuItem);

            Stream exitStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/Exit_off.png", UriKind.Absolute)).Stream;
            ExitMenuItem.Image = System.Drawing.Image.FromStream(exitStream);

            Stream SettingsStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/Settings_off.png", UriKind.Absolute)).Stream;
            SettingsMenuItem.Image = System.Drawing.Image.FromStream(SettingsStream);

            Stream AutohideStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/Autohide_off.png", UriKind.Absolute)).Stream;
            AutohideMenuItem.Image = System.Drawing.Image.FromStream(AutohideStream);

            Stream layoutStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/Layout_off.png", UriKind.Absolute)).Stream;
            LayoutMenuItem.Image = System.Drawing.Image.FromStream(layoutStream);

            Stream resizeStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/Resize_off.png", UriKind.Absolute)).Stream;
            ResizeMenuItem.Image = System.Drawing.Image.FromStream(resizeStream);

            Stream aboutStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/About_off.png", UriKind.Absolute)).Stream;
            AboutMenuItem.Image = System.Drawing.Image.FromStream(aboutStream);

            Stream guideStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/Guide_off.png", UriKind.Absolute)).Stream;
            ShowGuideMenuItem.Image = System.Drawing.Image.FromStream(guideStream);

            myTrayIcon = new NotifyIcon(myTrayContainer);
            myTrayIcon.DoubleClick += myTrayIcon_DoubleClick;

            Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Images/icon.ico", UriKind.Absolute)).Stream;

            myTrayIcon.Icon = new Icon(iconStream);
            myTrayIcon.ContextMenuStrip = myTrayContextMenu;
            myTrayIcon.Visible = true;

            //
            this.Visibility = Visibility.Hidden;

            Start_MainWindow();


        }

        private void Start_MainWindow()
        {
            if (myWindow != null)
                myWindow.Close();

            myWindow = new MainWindow();

            myWindow.OnSetDefaults += Start_MainWindow;
            myWindow.Show();

            //load saved trayicon selections
            Load_TrayIconSelection();
        }

        public void Load_TrayIconSelection()
        {
            //check layout selected
            QwertySubMenuItem.Checked = false;
            ColemakSubMenuItem.Checked = false;
            DevorakSubMenuItem.Checked = false;
            keyboardLayoutType type = myWindow.Get_Keyboardlayout();
            if (type == keyboardLayoutType.QWERTY)
            {
                QwertySubMenuItem.Checked = true;
            }
            else if (type == keyboardLayoutType.COLEMAK)
            {
                ColemakSubMenuItem.Checked = true;
            }
            else if (type == keyboardLayoutType.DEVORAK)
            {
                DevorakSubMenuItem.Checked = true;
            }

            //check autohide enabled
            if (myWindow.Get_AutoHideFlag())
            {
                AutohideMenuItem.Checked = true;
            }
            else
            {
                AutohideMenuItem.Checked = false;
            }
            

            //check movable enabled
            if (myWindow.Get_MovableFlag())
            {
                ResizeMenuItem.Checked = true;
            }
            else
            {
                ResizeMenuItem.Checked = false;
            }
            
            //check show guides
            if (myWindow.Get_ShowGuide())
            {
                ShowGuideMenuItem.Checked = true;
            }
            else
            {
                ShowGuideMenuItem.Checked = false;
            }

        }

        public void ExitMenuItem_Click(object sender, EventArgs e)
        {
  

            if (myAboutWindow != null)
                myAboutWindow.Close();

            myWindow.Close();
            this.Close();


            //hack for debuggin purposes
            //string file = @".\settings";
            //if (Directory.Exists(System.IO.Path.GetDirectoryName(file)))
            //{
            //    File.Delete(file);
            //}
        }

        public void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            myWindow.Show_SettingWindow();
        }

        public void ResizeMenuItem_Click(object sender, EventArgs e)
        {
            if (ResizeMenuItem.Checked == false)
            {
                //Start resizable window

                myWindow.Set_MovableFlag(true);

                //myWindow.Close();

                //myWindow = new MainWindow();

                //myWindow.Show();
                Start_MainWindow();

                ResizeMenuItem.Checked = true;
            }
            else if (ResizeMenuItem.Checked == true)
            {
                //Start regular window

                myWindow.Set_MovableFlag(false);

                //myWindow.Close();

                //myWindow = new MainWindow();

                //myWindow.Show();
                Start_MainWindow();

                ResizeMenuItem.Checked = false;
            }
        }

        public void AutoHideMenuItem_Click(object sender, EventArgs e)
        {
            if (AutohideMenuItem.Checked == false)
            {
                //Start autohide timer
                myWindow.Start_AutoHideWindow();



                AutohideMenuItem.Checked = true;
            }
            else if (AutohideMenuItem.Checked == true)
            {
                //Stop autohide timer
                myWindow.Stop_AutohideWindow();


                AutohideMenuItem.Checked = false;
            }
        }

        public void ShowGuideItem_Click(object sender, EventArgs e)
        {
            if (ShowGuideMenuItem.Checked == false)
            {
                //show guides
                myWindow.Show_KeyboardGuide(true);


                ShowGuideMenuItem.Checked = true;
            }
            else if (ShowGuideMenuItem.Checked == true)
            {
                //hide guides
                myWindow.Show_KeyboardGuide(false);


                ShowGuideMenuItem.Checked = false;
            }
        }

        public void myTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (myWindow.WindowState == System.Windows.WindowState.Normal)
            {
                myWindow.WindowState = System.Windows.WindowState.Minimized;
            }
            else if (myWindow.WindowState == System.Windows.WindowState.Minimized)
            {
                myWindow.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //myTrayIcon.Visible = false;
            myTrayIcon.Dispose();
        }

        public void QwertySubItem_Click(object sender, EventArgs args)
        {
            myWindow.Set_KeyboardLayout(keyboardLayoutType.QWERTY);

            QwertySubMenuItem.Checked = true;
            ColemakSubMenuItem.Checked = false;
            DevorakSubMenuItem.Checked = false;
        }

        public void ColemakSubItem_Click(object sender, EventArgs args)
        {
            myWindow.Set_KeyboardLayout(keyboardLayoutType.COLEMAK);

            QwertySubMenuItem.Checked = false;
            ColemakSubMenuItem.Checked = true;
            DevorakSubMenuItem.Checked = false;
            
        }

        public void DevorakSubItem_Click(object sender, EventArgs args)
        {
            myWindow.Set_KeyboardLayout(keyboardLayoutType.DEVORAK);

            QwertySubMenuItem.Checked = false;
            ColemakSubMenuItem.Checked = false;
            DevorakSubMenuItem.Checked = true;
        }

        public void AboutMenuItem_Click(object sender, EventArgs args)
        {
            if (myAboutWindow != null)
                myAboutWindow.Close();

            myAboutWindow = new AboutWindow(myWindow.Get_Keyboardlayout(), myWindow.Get_CurrentRadiusX(), myWindow.Get_CurrentRadiusY());
            myAboutWindow.Show();
        }

        

    }
}
