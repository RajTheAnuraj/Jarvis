using LogicLayer;
using LogicLayer.Common;
using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;
using LogicLayer.Payloads;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using JarvisWpf.Common;

namespace JarvisWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ICrosstalk
    {


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IResourceProvider provider = ProviderFactory.GetCurrentProvider();
            provider.CrosstalkService = JarvisCrosstalkService.CreateInstance();
            provider.CrosstalkService.RegisterCallback("Application", this);
            provider.CrosstalkService.RegisterCallback("MessageBox", MessageBoxService.CreateInstance());

            string rootPath = null;

            RootFolderWindow rfw = new RootFolderWindow();
            if (!rfw.ShowDialog(out rootPath))
            {
                provider.CrosstalkService.Crosstalk("MessageBox", "Show", new object[] { "The app cannot start without a valid root path. Exiting the App" });
                this.Shutdown();
                return;
            }

            IUndoableCommand ApplicationInitializeCommand = provider.GetApplicationInitializeCommand(rootPath);
            try
            {
                ApplicationInitializeCommand.Execute();
            }
            catch (Exception)
            {
                ApplicationInitializeCommand.Undo();
                throw;
            }
        }

        public object IncomingCrosstalk(string ActionName, object[] Parameters)
        {

            return null;
        }
    }
}
