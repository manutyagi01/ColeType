using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

using System.Windows.Forms;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;


namespace OSDKeyboardMain1
{
    [RunInstaller(true)]
    public partial class InstallerClass : System.Configuration.Install.Installer
    {
        public InstallerClass()
        {
            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            stateSaver.Add("TargetDir", Context.Parameters["DP_TargetDir"].ToString());
        }

        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);



            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            System.Security.Principal.NTAccount acct = sid.Translate(typeof(System.Security.Principal.NTAccount)) as System.Security.Principal.NTAccount;
            string strEveryoneAccount = acct.ToString();

            DirectorySecurity dirSec = Directory.GetAccessControl(savedState["TargetDir"].ToString());
            FileSystemAccessRule fsar = new FileSystemAccessRule(strEveryoneAccount
                                          , FileSystemRights.FullControl
                                          , InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit
                                          , PropagationFlags.None
                                          , AccessControlType.Allow);
            dirSec.AddAccessRule(fsar);
            Directory.SetAccessControl(savedState["TargetDir"].ToString(), dirSec);
        }


        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            //MessageBox.Show("testing my install class :)");

            //StreamWriter sw = new StreamWriter("C:\\VbCity_caic_Commit.txt", false);

            //sw.WriteLine("savedState count : " + savedState.Count.ToString());
            //sw.WriteLine("savedState keys : " + savedState.Keys.Count.ToString());
            //sw.WriteLine("savedState values : " + savedState.Values.Count.ToString());

            //foreach (string k in savedState.Keys)
            //{
            //    sw.WriteLine("savedState key[" + k + "]= " + savedState[k].ToString());
            //}

            ////writeContext(sw);

            //sw.Flush();
            //sw.Close(); 

            String _TargetDir;

            _TargetDir = savedState["TargetDir"].ToString(); 

            if (File.Exists(_TargetDir + "settings") == true)
            {
                File.Delete(_TargetDir + "settings");
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\ColeType.lnk"))
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\ColeType.lnk");
            }
        }


    }
}
