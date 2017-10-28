using LogicLayer;
using LogicLayer.Common;
using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;
using LogicLayer.Payloads;
using JarvisWpf.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace JarvisWpf.Project
{
    public class ProjectListViewModel:BindableBase
    {
        IResourceProvider ResourceProvider = ProviderFactory.GetCurrentProvider();

        ProjectListPayload ProjectList;

        ObservableCollection<ProjectListItem> _Projects;
        public ObservableCollection<ProjectListItem> Projects { get { return _Projects; }
            set
            {
                _Projects = value;
                NotifyPropertyChanged("Projects");
            }
        }

        public RelayCommand<ProjectListItem> ProjectSelectedCommand { get; set; }
        public RelayCommand<ProjectListItem> ProjectDeletedCommand { get; set; }
        public RelayCommand<object> ProjectAddCommand { get; set; }
        public ProjectListItem NewProject { get; set; }

        public ProjectListViewModel()
        {
            ProjectSelectedCommand = new RelayCommand<ProjectListItem>(ShowSelectedProject);
            ProjectAddCommand = new RelayCommand<object>(ProjectAdd);
            ProjectDeletedCommand = new RelayCommand<ProjectListItem>(ProjectDeleted);
            UpdateStatusBarText("Project List");
        }

        private async void ProjectDeleted(ProjectListItem obj)
        {
            MessageDialogStyle ms = MessageDialogStyle.AffirmativeAndNegative;
            MetroDialogSettings mds = new MetroDialogSettings{
                AffirmativeButtonText ="Yes",
                NegativeButtonText="No"
            };
            var ww = System.Windows.Application.Current.MainWindow as MetroWindow;
            MessageDialogResult result = await ww.ShowMessageAsync("Delete Warning", "Are you sure you want to delete this project?",ms,mds);
            if (result == MessageDialogResult.Negative)
                return;

            if (obj != null)
            {
                Projects.Remove(obj);
                NotifyPropertyChanged("Projects");
                this.ProjectList.Remove(obj);
                IUndoableCommand projListSaveCommand = ResourceProvider.GetSaveProjectListCommand(this.ProjectList);
                projListSaveCommand.Execute();
                if (!obj.isRemoteProject)
                {
                    string ArchiveFolder = String.Format("{0}\\Archive\\{1}", ResourceProvider.GetProjectsRootFolder(), obj.ProjectName);
                    string ProjectFolder = String.Format("{0}\\{1}", ResourceProvider.GetProjectsRootFolder(), obj.ProjectName);
                    System.IO.Directory.Move(ProjectFolder, ArchiveFolder);
                }
            } 
        }

        private void ProjectAdd(object obj)
        {
            NewProject = new ProjectListItem();
            NewProject.ProjectId = Guid.NewGuid().ToString();
            AddProjectWindow ap = new AddProjectWindow(NewProject);
            if (ap.ShowDialog() == true)
            {
                bool isPresent = false;
                isPresent = IsProjectAlreadyPresent(NewProject);
                if (!isPresent)
                {
                    Projects.Add(NewProject);
                    this.ProjectList.Add(NewProject);
                    IUndoableCommand projListSaveCommand = ResourceProvider.GetSaveProjectListCommand(this.ProjectList);
                    projListSaveCommand.Execute();
                    NotifyPropertyChanged("Projects");
                    ShowSelectedProject(NewProject);
                }
            }
        }

        private bool IsProjectAlreadyPresent(ProjectListItem project)
        {
            foreach (var item in Projects)
            {
                if (
                    item.ProjectName.ToLower() == project.ProjectName.ToLower() &&
                    item.ProjectRelativePath.ToLower() == project.ProjectRelativePath.ToLower()
                    )
                {
                    return true;
                }
            }
            return false;
        }   

        public void LoadedMethod()
        {
            ProjectList = new ProjectListPayload();
            ICustomCommand RetrieveProjectsCommand = ResourceProvider.GetRetrieveProjectListCommand(ref ProjectList);
            RetrieveProjectsCommand.Execute();
            if (ProjectList != null)
            {
                Projects = new ObservableCollection<ProjectListItem>(ProjectList);
            }
        }

        private void ShowSelectedProject(ProjectListItem obj)
        {
            ProjectViewModel projectViewModel = new ProjectViewModel();
            projectViewModel.Id = obj.ProjectId;
            projectViewModel.ProjectName = obj.ProjectName;
            projectViewModel.RemoteServerRoot = obj.ProjectRelativePath;
            projectViewModel.isRemoteProject = obj.isRemoteProject;
            UpdateStatusBarText("Project Selected : " + projectViewModel.ProjectName);
            NavigateToView(projectViewModel as BindableBase);
        }
    }
}
