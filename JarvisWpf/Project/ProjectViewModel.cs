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
using JarvisWpf.Document;
using JarvisWpf.Communication;

namespace JarvisWpf.Project
{
    public class ProjectViewModel: BindableBase
    {
        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();
        private string _Id;

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _RemoteServerRoot;

        public string RemoteServerRoot
        {
            get { return _RemoteServerRoot; }
            set { _RemoteServerRoot = value; NotifyPropertyChanged("RemoteServerRoot"); }
        }

        private bool _isRemoteProject;

        public bool isRemoteProject
        {
            get { return _isRemoteProject; }
            set { _isRemoteProject = value; NotifyPropertyChanged("isRemoteProject"); }
        }

        private string _ProjectName;

        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; NotifyPropertyChanged("ProjectName"); }
        }

        private string _ProjectId;

        public string ProjectId
        {
            get { return _ProjectId; }
            set { _ProjectId = value; NotifyPropertyChanged("ProjectId"); }
        }

        string _ProjectFolder;
        public string ProjectFolder
        {
            get { return _ProjectFolder; }
            set { _ProjectFolder = value; NotifyPropertyChanged("ProjectFolder"); }
        }

        private string _ProjectLogText;

        public string ProjectLogText
        {

            get { return _ProjectLogText; }
            set { _ProjectLogText = value; NotifyPropertyChanged("ProjectLogText"); }
        }

        private string _ProjectSummaryText;

        public string ProjectSummaryText
        {
            get { return _ProjectSummaryText; }
            set { _ProjectSummaryText = value; NotifyPropertyChanged("ProjectSummaryText"); }
        }


        private ObservableCollection<DocumentViewModel> _Documents;

        public ObservableCollection<DocumentViewModel> Documents
        {
            get { return _Documents; }
            set { _Documents = value; NotifyPropertyChanged("Documents"); }
        }

        private ObservableCollection<CommunicationViewModel> _Communications;

        public ObservableCollection<CommunicationViewModel> Communications
        {
            get { return _Communications; }
            set { _Communications = value; NotifyPropertyChanged("Communications"); }
        }


        private BindableBase _SelectedItemViewModel;

        public BindableBase SelectedItemViewModel
        {
            get { return _SelectedItemViewModel; }
            set
            {
                _SelectedItemViewModel = value;
                NotifyPropertyChanged("SelectedItemViewModel");
            }
        }

        private int _SelectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                SelectedItemViewModel = null;
            }
        }


        public RelayCommand<object> CloseCommand { get; set; }

        public ProjectPayload projectPayLoad { get; set; }
        IResourceProvider CommandProvider = ProviderFactory.GetCurrentProvider();

        public ProjectViewModel()
        {
            CloseCommand = new RelayCommand<object>(CloseProject);
            
        }

        public void LoadedMethod()
        {
            ProjectPayload pl = new ProjectPayload(_ProjectName);
            pl.isRemoteProject = isRemoteProject;
            pl.RemoteServerRoot = RemoteServerRoot;
            IUndoableCommand initProjCommand = CommandProvider.GetProjectInitializeCommand(pl);
            History.Push(initProjCommand);
            initProjCommand.Execute();
            CopyToViewModel(pl);
        }

        private void CopyToViewModel(ProjectPayload pl)
        {
            this.Id = pl.Id;
            this.isRemoteProject = pl.isRemoteProject;
            this.ProjectFolder = pl.ProjectFolder;
            this.ProjectId= pl.ProjectIdentifier;
            this.ProjectLogText = pl.ProjectLogText;
            this.ProjectName = pl.ProjectName;
            this.ProjectSummaryText = pl.ProjectSummaryText;
            this.RemoteServerRoot = pl.RemoteServerRoot;
            this.projectPayLoad = pl;
            this.Documents = new ObservableCollection<DocumentViewModel>();
            this.Communications = new ObservableCollection<CommunicationViewModel>();
            CopyProjectItems(pl);
        }

        private void CopyProjectItems(ProjectPayload pl)
        {
            if (pl.ProjectItemCount > 0)
            {
                foreach (IProjectItem projItem in pl.GetProjectItems())
                {
                    switch (projItem.ProjectItemType)
                    {
                        case "Document":
                            DocumentPayload doc = ((DocumentPayload)projItem);
                            DocumentViewModel DVM = new DocumentViewModel();
                            DVM.setDocument(doc,pl);
                            Documents.Add(DVM);
                            break;
                        case "Communication":
                            CommunicationPayload com = ((CommunicationPayload)projItem);
                            CommunicationViewModel CVM = new CommunicationViewModel();
                            CVM.setCommunication(com);
                            Communications.Add(CVM);
                            break;
                    }
                }
            }
        }

        private void CloseProject(object obj)
        {
            NavigateToView(new ProjectListViewModel());
        }


       
    }
}
