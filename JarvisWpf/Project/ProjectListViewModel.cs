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

namespace JarvisWpf.Project
{
    public class ProjectListViewModel:BindableBase
    {
        IResourceProvider ResourceProvider = ProviderFactory.GetCurrentProvider();

        ObservableCollection<ProjectListItem> _Projects;
        public ObservableCollection<ProjectListItem> Projects { get { return _Projects; }
            set
            {
                _Projects = value;
                NotifyPropertyChanged("Projects");
            }
        }

        public RelayCommand<ProjectListItem> ProjectSelectedCommand { get; set; }

        public ProjectListViewModel()
        {
            ProjectSelectedCommand = new RelayCommand<ProjectListItem>(ShowSelectedProject);
            
        }

        public void LoadedMethod()
        {
            ProjectListPayload plPload = new ProjectListPayload();
            ICustomCommand RetrieveProjectsCommand = ResourceProvider.GetRetrieveProjectListCommand(ref plPload);
            RetrieveProjectsCommand.Execute();
            if (plPload != null)
            {
                Projects = new ObservableCollection<ProjectListItem>(plPload);
            }
        }

        private void ShowSelectedProject(ProjectListItem obj)
        {
            ProjectViewModel projectViewModel = new ProjectViewModel();
            projectViewModel.Id = obj.ProjectId;
            projectViewModel.ProjectName = obj.ProjectName;
            projectViewModel.RemoteServerRoot = obj.ProjectRelativePath;
            projectViewModel.isRemoteProject = obj.isRemoteProject;
            NavigateToView(projectViewModel as BindableBase);
        }
    }
}
