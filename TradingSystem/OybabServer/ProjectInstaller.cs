using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace Oybab.Server
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {

        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += ProjectInstaller_AfterInstall;
            this.BeforeUninstall += ProjectInstaller_BeforeUninstall;
        }

        void ProjectInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {

            try
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "OybabTradingSystemService");
                if (ctl != null)
                {
                    ctl.Stop();
                    ctl.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));
                }
            }
            catch
            {
            }
        }

        void ProjectInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

            try
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "OybabTradingSystemService");
                if (ctl != null)
                {
                    ctl.Start();
                }
            }
            catch
            {
            }
        }

          
    

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }
    }
}
