using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace WindowHooks
{
    public class KeyboardLLHook : IDisposable
    {
        private bool disposed = false;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x104;
        

        private IntPtr hookID = IntPtr.Zero;

        private delegate IntPtr KeyboardDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        private KeyboardDelegate keyboardDelegateProc;

        public delegate void EventDelegate(KeyEventArgs e);

        //public event EventDelegate keyPressDownEvent;
        public event KeyEventHandler keyPressDownEvent;

        private void ShowMessageBox(string text, string caption)
        {
            Thread t = new Thread(() => MyMessageBox(text, caption));
            t.Start();
        }

        private void MyMessageBox(object text, object caption)
        {
            MessageBox.Show((string)text, (string)caption);
        }

        public KeyboardLLHook()
        {
            Console.WriteLine("In KeyboardHook class constructor: hookID={0}", hookID.ToInt32());
            keyboardDelegateProc = new KeyboardDelegate(HookedKeyboardCallbackProc);
            hookID = SetHook();

            Console.WriteLine("In KeyboardHook class constructor: hookID={0}", hookID.ToInt32());

            
        }

        private IntPtr SetHook()
        {
            //set hook keyboard
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                //MessageBox.Show(curModule.ModuleName);
                return SetWindowsHookEx(WH_KEYBOARD_LL, keyboardDelegateProc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        public IntPtr HookedKeyboardCallbackProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //trigger keyboard events
            if ((nCode >= 0) && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                Keys vkCode = (Keys)Marshal.ReadInt32(lParam);

                //Console.WriteLine("In KeyboardHook {0}", vkCode.ToString());

                //ShowMessageBox(vkCode.ToString(), vkCode.ToString());

                KeyEventArgs keyEvent = new KeyEventArgs(vkCode);
                keyPressDownEvent(this, keyEvent);
                
            }
            //pass on hook parameters
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        public void Dispose()
        {

            CleanUp(true);

            GC.SuppressFinalize(this);
        }

        private void CleanUp(bool disposing)
        {
            if (!this.disposed)
            {

                if (disposing)
                {
                    //dispose managed resources
                }

                //dispose unmanaged resources
                UnhookWindowsHookEx(hookID);
                //unhook keyboard
            }
            this.disposed = true;
        }

        ~KeyboardLLHook()
        {

            CleanUp(false);
        }



        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            KeyboardDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }

}
