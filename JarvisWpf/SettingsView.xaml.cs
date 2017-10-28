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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Threading.Tasks;
using LogicLayer.Interfaces;
using LogicLayer.Factories;

namespace JarvisWpf
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            var metroWindow = Application.Current.MainWindow as MetroWindow;
            //metroWindow.ShowMessageAsync("Root Folder", "Please Set Root Folder");
            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.RootPath))
            {
                Properties.Settings.Default.RootPath = "C:\\Temp";
                Properties.Settings.Default.Save();
            }
            else
            {
                txtRootPath.Text = Properties.Settings.Default.RootPath;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnAppQuit_Click(object sender, RoutedEventArgs e)
        {
            IResourceProvider ResourceProvider = ProviderFactory.GetCurrentProvider();
            ICrosstalkService crosstalkservice = ResourceProvider.CrosstalkService;
            crosstalkservice.Crosstalk("Application", "AppShutDown", null);
            ICustomCommand DontShowCloseBaloonCommand = ResourceProvider.GetApplicationDontShowMeSetCommand("CloseNotification");
            DontShowCloseBaloonCommand.Execute();
            Application.Current.Shutdown();
        }
    }
}
