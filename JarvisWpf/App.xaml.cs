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
using System.Xml;
using System.IO;

namespace JarvisWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ICrosstalk
    {
        IResourceProvider ResourceProvider = ProviderFactory.GetCurrentProvider();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ResourceProvider.CrosstalkService = JarvisCrosstalkService.CreateInstance();
            ResourceProvider.CrosstalkService.RegisterCallback("Application", this);
            ResourceProvider.CrosstalkService.RegisterCallback("MessageBox", MessageBoxService.CreateInstance());

            string rootPath = GetRootFolder();

            if (String.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
            {
                RootFolderWindow rfw = new RootFolderWindow();
                if (!rfw.ShowDialog(out rootPath))
                {
                    ResourceProvider.CrosstalkService.Crosstalk("MessageBox", "Show", new object[] { "The app cannot start without a valid root path. Exiting the App" });
                    this.Shutdown();
                    return;
                }
                else
                {
                    SetRootFolder(rootPath);
                }
            }

            IUndoableCommand ApplicationInitializeCommand = ResourceProvider.GetApplicationInitializeCommand(rootPath);
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


        private string GetRootFolder()
        {
            string assemPath = System.Reflection.Assembly.GetCallingAssembly().Location;
            assemPath = Path.GetDirectoryName(assemPath);
            assemPath = assemPath + "\\Configuration.Jarvis";
            string rootFolder = null;
            XmlDocument xdoc = new XmlDocument();
            ICustomCommand<string> fileReadAsStringCommand = ResourceProvider.GetFileReadAsString2Command(assemPath);
            string xmlStr = fileReadAsStringCommand.Execute();
            if (!String.IsNullOrWhiteSpace(xmlStr))
            {
                xdoc.LoadXml(xmlStr);
                var node = xdoc.DocumentElement.SelectSingleNode("RootFolder");
                rootFolder = node.InnerText;
            }
            return rootFolder;
        }


        private void SetRootFolder(string RootFolder)
        {
            string assemPath = System.Reflection.Assembly.GetCallingAssembly().Location;
            assemPath = Path.GetDirectoryName(assemPath);
            assemPath = assemPath + "\\Configuration.Jarvis";
            string filContent = @"<Configurations><RootFolder>{0}</RootFolder></Configurations>";
            filContent = String.Format(filContent, RootFolder);
            XmlDocument xdoc = new XmlDocument();
            if (System.IO.File.Exists(assemPath))
            {
                IUndoableCommand fileModifyCommand = ResourceProvider.GetFileModifyContentCommand(assemPath, filContent);
                fileModifyCommand.Execute();
            }
            else
            {
                IUndoableCommand fileCreateCommand = ResourceProvider.GetFileCreateCommand(assemPath, filContent);
                fileCreateCommand.Execute();
            }
            
        }

        public object IncomingCrosstalk(string ActionName, object[] Parameters)
        {

            return null;
        }
    }
}
