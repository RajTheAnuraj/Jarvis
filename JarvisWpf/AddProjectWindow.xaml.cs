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

namespace JarvisWpf
{
    /// <summary>
    /// Interaction logic for AddProjectWindow.xaml
    /// </summary>
    public partial class AddProjectWindow 
    {
        public AddProjectWindow()
        {
            InitializeComponent();
        }

        private void ProjectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (projectPathTextBox != null)
            {
                if (Convert.ToString(ProjectType.SelectedValue) == "System.Windows.Controls.ComboBoxItem: New")
                {
                    projectPathTextBox.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    projectPathTextBox.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
    }
}
