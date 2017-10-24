using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        private ProjectListItem ProjectListItemModel;


        public IResourceProvider ResourceProvider { get; set; }



        public AddProjectWindow()
        {
            InitializeComponent();
            ResourceProvider = ProviderFactory.GetCurrentProvider();
            this.projectPathTextBox.Text = ResourceProvider.GetProjectsRootFolder();
            ProjectType_SelectionChanged(null, null);
            isRemoteCheckBox_Click(null, null);
        }

        public AddProjectWindow(ProjectListItem ProjectListItemModel)
            : this()
        {
            this.ProjectListItemModel = ProjectListItemModel;
        }

        private void ProjectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (projectPathTextBox != null)
            {
                if (Convert.ToString(ProjectType.SelectedItem) == "System.Windows.Controls.ComboBoxItem: New")
                {
                    projectNameTextBox.IsEnabled = true;
                }
                else
                {
                    projectNameTextBox.IsEnabled = false;
                }
                isRemoteCheckBox_Click(null, null);
            }
        }


        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (this.ProjectListItemModel != null)
            {
                ProjectListItemModel.isRemoteProject = isRemoteCheckBox.IsChecked.Value;
                ProjectListItemModel.ProjectName = this.projectNameTextBox.Text;
                ProjectListItemModel.ProjectRelativePath = ProjectListItemModel.isRemoteProject ? this.projectPathTextBox.Text : this.projectNameTextBox.Text;
            }
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool isCreatingNew = (Convert.ToString(ProjectType.SelectedItem) == "System.Windows.Controls.ComboBoxItem: New");
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = ResourceProvider.GetProjectsRootFolder();

            //For Existing Projects Select the .Jarvis File
            if (isCreatingNew == false) 
                dialog.Filter = "Jarvis Project (*.Jarvis)|*.Jarvis";
            else
            {
                //For new project by default will be created in root folder
               //If Shared project Checkbox is checked the project will be created in the path which is selected by the dialog
                dialog.FileName = "Select A Folder";
                dialog.Filter = string.Empty;
                dialog.CheckFileExists = false;
                dialog.CheckPathExists = false;
            }

            if (dialog.ShowDialog() == true)
                this.projectPathTextBox.Text = dialog.FileName;

            if (!String.IsNullOrWhiteSpace(this.projectPathTextBox.Text))
            {
                string paths = "";
                if (!isCreatingNew)
                {
                    paths = this.projectPathTextBox.Text;
                    ICustomCommand<string> filereadCommand = ResourceProvider.GetFileReadAsString2Command(paths);
                    string content = filereadCommand.Execute();
                    if (!String.IsNullOrWhiteSpace(content))
                    {
                        int i = content.IndexOf("<Id>");
                        if (i > -1)
                        {
                            i = i + 4;
                            int j = content.IndexOf("</Id>");
                            string Id = content.Substring(i, j - i);
                            if (ProjectListItemModel != null)
                                ProjectListItemModel.ProjectId = Id;
                        }
                    }
                    paths = paths.Replace("\\Project.Jarvis", "");
                    this.projectPathTextBox.Text = paths;
                }
                else
                {
                    paths = this.projectPathTextBox.Text.Replace("Select A Folder", "");
                    this.projectPathTextBox.Text = paths;
                }
                string projectFolder = this.projectPathTextBox.Text;
                DirectoryInfo DI = new DirectoryInfo(projectFolder);
                var projectName = DI.Name;
                this.projectPathTextBox.Text = System.IO.Path.GetDirectoryName(projectFolder);
                bool isRemote =  this.projectPathTextBox.Text.Trim().ToLower() != ResourceProvider.GetProjectsRootFolder().ToLower();
                if (String.IsNullOrWhiteSpace(projectNameTextBox.Text))
                    projectNameTextBox.Text = projectName;
                isRemoteCheckBox.IsChecked = isRemote;
            }
            
        }

        private void isRemoteCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool isCreatingNew = (Convert.ToString(ProjectType.SelectedItem) == "System.Windows.Controls.ComboBoxItem: New");
            if (isCreatingNew)
            {
                projectPathLabel.Visibility = isRemoteCheckBox.IsChecked.Value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                projectPathPanel.Visibility = isRemoteCheckBox.IsChecked.Value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                projectPathTextBox.Visibility = isRemoteCheckBox.IsChecked.Value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            else
            {
                projectPathLabel.Visibility = System.Windows.Visibility.Visible;
                projectPathPanel.Visibility = System.Windows.Visibility.Visible;
                projectPathTextBox.Visibility = System.Windows.Visibility.Visible;
            }
        }

        
    }
}
