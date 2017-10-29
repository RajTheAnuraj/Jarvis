using LogicLayer.Factories;
using LogicLayer.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JarvisWpf.Common
{
    /// <summary>
    /// Interaction logic for RootFolderWindow.xaml
    /// </summary>
    public partial class RootFolderWindow 
    {
        IResourceProvider ResourceProvider = ProviderFactory.GetCurrentProvider();

        public RootFolderWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
                this.Close();
        }

        public bool ShowDialog(out string SelectedPath)
        {
            this.ShowDialog();
            bool dr = Validate();
            SelectedPath = txtRootPath.Text;
            return dr;
        }


        public bool Validate()
        {
            string rootPath = txtRootPath.Text;
            if (string.IsNullOrWhiteSpace(rootPath))
            {
                ResourceProvider.CrosstalkService.Crosstalk("MessageBox", "Show", new object[] { "The path cannot be empty" });
                return false;
            }
            if (!System.IO.Directory.Exists(rootPath))
            {
                ResourceProvider.CrosstalkService.Crosstalk("MessageBox", "Show", new object[] { "The path is not valid" });
                return false;
            }

            return true;
        }

        
    }
}
