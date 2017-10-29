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
        IResourceProvider ResourceProvider = ProviderFactory.GetCurrentProvider();

        public SettingsView()
        {
            InitializeComponent();
            
        }


        private void btnAppQuit_Click(object sender, RoutedEventArgs e)
        {
            ICrosstalkService crosstalkservice = ResourceProvider.CrosstalkService;
            crosstalkservice.Crosstalk("Application", "AppShutDown", null);
            ICustomCommand DontShowCloseBaloonCommand = ResourceProvider.GetApplicationDontShowMeSetCommand("CloseNotification");
            DontShowCloseBaloonCommand.Execute();
            Application.Current.Shutdown();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            object retval = ResourceProvider.CrosstalkService.Crosstalk("MessageBox", "ConfirmYesNo", new object[] { "This will restart the application. Are you sure to continue?" });
            MessageBoxResult dr = (MessageBoxResult)retval;
            if (dr == MessageBoxResult.No)
                return;

            string assemPath = System.Reflection.Assembly.GetCallingAssembly().Location;
            assemPath = System.IO.Path.GetDirectoryName(assemPath);
            assemPath = assemPath + "\\Configuration.Jarvis";
            System.IO.File.Delete(assemPath);
            ResourceProvider.CrosstalkService.Crosstalk("Application", "AppShutDown", null);
        }
    }
}
