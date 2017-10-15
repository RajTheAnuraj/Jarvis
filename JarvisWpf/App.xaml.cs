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

namespace JarvisWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IResourceProvider provider = ProviderFactory.GetCurrentProvider();
            IUndoableCommand ApplicationInitializeCommand = provider.GetApplicationInitializeCommand();
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
    }
}
