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
using JarvisWpf.SourceControl;

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

        private string _DocumentFilter;

        public string DocumentFilter
        {
            get { return _DocumentFilter; }
            set
            {
                _DocumentFilter = value;
                NotifyPropertyChanged("DocumentFilter");
                NotifyPropertyChanged("Documents");
            }
        }

        public List<DocumentViewModel> _Documents { get; set; }

        public ObservableCollection<DocumentViewModel> Documents
        {
            get
            {
                if (String.IsNullOrWhiteSpace(DocumentFilter))
                    return new ObservableCollection<DocumentViewModel>(_Documents);

                return new ObservableCollection<DocumentViewModel>(_Documents
                    .Where(c=>
                        c.DisplayString.ToLower().Contains(DocumentFilter.ToLower()) ||
                        c.ProjectItemSubType.ToLower().Contains(DocumentFilter.ToLower()) 
                        ));
            }
            set
            {
                _Documents = value.ToList();
                NotifyPropertyChanged("Documents");
            }
        }

        private ObservableCollection<SourceControlViewModel> _SourceControls;

        public ObservableCollection<SourceControlViewModel> SourceControls
        {
            get { return _SourceControls; }
            set
            {
                _SourceControls = value;
                NotifyPropertyChanged("SourceControls");
                NotifyPropertyChanged("SourceControlCopyOptions");
            }
        }

        public ObservableCollection<string> SourceControlCopyOptions
        {
            get
            {
                ObservableCollection<string> retval = new ObservableCollection<string>();
                if (SourceControls != null)
                {
                    if (SourceControls.Count > 0)
                    {
                        retval = new ObservableCollection<string>(
                            SourceControls
                            .Where(c => c.ProjectItemSubType == "Merge Request")
                            .Select(c => "After Merge Request On : " + c.SourceControlDateTime));
                    }
                   
                }
                retval.Add("All");
                return retval;
            }
        }


        private string _SourceControlCopySelectedOption;

        public string SourceControlCopySelectedOption
        {
            get { return _SourceControlCopySelectedOption; }
            set
            {
                _SourceControlCopySelectedOption = value;
                NotifyPropertyChanged("SourceControlCopySelectedOption");
                CopySourceControlItemsCommand.RaiseCanExecuteChanged();
            }
        }


        private BindableBase _SelectedItemViewModel;

        public BindableBase SelectedItemViewModel
        {
            get { return _SelectedItemViewModel; }
            set
            {
                if (_SelectedItemViewModel != null)
                    _SelectedItemViewModel.UnBindBigDataNow = true;
                _SelectedItemViewModel = value;
                NotifyPropertyChanged("SelectedItemViewModel");
                if (_SelectedItemViewModel != null)
                {
                    _SelectedItemViewModel.BindBigDataNow = true;
                    _SelectedItemViewModel.isEditMode = true;
                }
            }
        }

        private int _SelectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                WindowStatusText = _ProjectName;
                SelectedItemViewModel = null;
            }
        }


        public RelayCommand<object> CloseCommand { get; set; }
        public RelayCommand<object> CreateDocumentCommand { get; set; }
        public RelayCommand<object> CreateFromClipboardCommand { get; set; }
        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand<object> CreateSourceControlCommand { get; set; }
        public RelayCommand<object> CopySourceControlItemsCommand { get; set; }

        public ProjectPayload projectPayLoad { get; set; }
        IResourceProvider CommandProvider = ProviderFactory.GetCurrentProvider();

        public ProjectViewModel()
        {
            CloseCommand = new RelayCommand<object>(CloseProject);
            CreateDocumentCommand = new RelayCommand<object>(CreateDocument);
            CreateFromClipboardCommand = new RelayCommand<object>(CreateFromClipboard);
            SaveCommand = new RelayCommand<object>(Save);
            CreateSourceControlCommand = new RelayCommand<object>(CreateSourceControl);
            CopySourceControlItemsCommand = new RelayCommand<object>(CopySourceControlItems, CanCopySourceControlItems);
           
            _Documents = new List<DocumentViewModel>();
        }

        private bool CanCopySourceControlItems()
        {
            return !String.IsNullOrWhiteSpace(SourceControlCopySelectedOption) && SourceControls.Count > 0;
        }

        private void CopySourceControlItems(object obj)
        {
            IEnumerable<string> fin = null;
            string copiedChangesets = "";
            if (SourceControlCopySelectedOption == "All")
            {
                var test = SourceControls.Where(c => c.ProjectItemSubType != "Merge Request")
                    .GroupBy(c => c.ProjectItemSubType)
                    .Select(c => new { Ids = c.Select(x => x.DisplayString), Key = c.Key });
                
                foreach (var item in test)
                {
                    copiedChangesets += item.Key;
                    copiedChangesets += String.Join(",", item.Ids);
                    copiedChangesets += Environment.NewLine;
                }
            }
            else
            {
                if (SourceControlCopySelectedOption.IndexOf("On : ") > 0)
                {
                    string DateStr = SourceControlCopySelectedOption.Substring(SourceControlCopySelectedOption.IndexOf("On : ") + 5, SourceControlCopySelectedOption.Length - SourceControlCopySelectedOption.IndexOf("On : ") - 5);
                    DateTime dt = Convert.ToDateTime(DateStr);
                    var test = SourceControls
                        .Where(c => c.ProjectItemSubType != "Merge Request" && c.SourceControlDateTime > dt)
                        .GroupBy(c => c.ProjectItemSubType)
                        .Select(c => new { Ids = c.Select(x => x.DisplayString), Key = c.Key });

                    foreach (var item in test)
                    {
                        copiedChangesets += item.Key + " : ";
                        copiedChangesets += String.Join(",", item.Ids);
                        copiedChangesets += Environment.NewLine;
                    }
                }
            }
            System.Windows.Clipboard.SetText(copiedChangesets);
        }

        void SourceControls_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("SourceControls");
            NotifyPropertyChanged("SourceControlCopyOptions");
        }

        

        private void Save(object obj)
        {
            CopyFromViewModelToProjectPayload();
            IUndoableCommand saveProjectCommand = CommandProvider.GetProjectSaveCommand(this.projectPayLoad);
            saveProjectCommand.Execute();
        }

        private void CopyFromViewModelToProjectPayload()
        {
            if (this.projectPayLoad != null)
            {
                this.projectPayLoad.ProjectSummaryText = this.ProjectSummaryText;
                this.projectPayLoad.ProjectLogText = this.ProjectLogText;
                
            }
        }
        

        private void CreateFromClipboard(object obj)
        {
            ICustomCommand<bool> command = CommandProvider.GetCanCreateFromClipboardCommand();
            if (command.Execute())
            {
                IUndoableCommand clipbrdCommand = CommandProvider.GetCreateFromClipboardCommand(this.projectPayLoad);
                History.Push(clipbrdCommand);
                clipbrdCommand.Execute();
                CopyToViewModel(this.projectPayLoad);
            }
            
            IUndoableCommand saveProjectCommand = CommandProvider.GetProjectSaveCommand(this.projectPayLoad);
            saveProjectCommand.Execute();
        }

        private void CreateDocument(object obj)
        {
            DocumentPayload doc = new DocumentPayload();
            DocumentViewModel DVM = new DocumentViewModel();
            DVM.isEditMode = false;
            DVM.setDocument(doc, projectPayLoad);
            DVM.ChildChanged += ProjectItemChanged;
            _SelectedItemViewModel = DVM;
            NotifyPropertyChanged("SelectedItemViewModel");
        }

        private void CreateSourceControl(object obj)
        {
            SourceControlPayload sc = new SourceControlPayload();
            SourceControlViewModel SVM = new SourceControlViewModel();
            sc.SourceControlDateTime = DateTime.Now;
            sc.ProjectItemSubType = "Changeset";
            SVM.isEditMode = false;
            SVM.setSourceControl(sc, projectPayLoad);
            SVM.ChildChanged += ProjectItemChanged;
            _SelectedItemViewModel = SVM;
            NotifyPropertyChanged("SelectedItemViewModel");
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
            WindowStatusText = _ProjectName;
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
            this._Documents = new List<DocumentViewModel>();
            this.Documents = new ObservableCollection<DocumentViewModel>();
            this.SourceControls = new ObservableCollection<SourceControlViewModel>();
            SourceControls.CollectionChanged += SourceControls_CollectionChanged;
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
                            DVM.ChildChanged += ProjectItemChanged;
                            DVM.statusBarData = statusBarData;
                            _Documents.Add(DVM);
                            NotifyPropertyChanged("Documents");
                            break;
                        case "Source Control":
                            SourceControlPayload sc = ((SourceControlPayload)projItem);
                            SourceControlViewModel SVM = new SourceControlViewModel();
                            SVM.setSourceControl(sc, pl);
                            SVM.ChildChanged += ProjectItemChanged;
                            SourceControls.Add(SVM);
                            break;

                    }
                }
            }
        }

        private void ProjectItemChanged(string ProjectItemName, object Parameter)
        {
            switch (ProjectItemName)
            {
                case  "DocumentAdded":
                    this._Documents.Add((DocumentViewModel)Parameter);
                    NotifyPropertyChanged("Documents");
                    SelectedItemViewModel = null;
                    break;
                case "DocumentDeleted":
                    this.Documents.Remove((DocumentViewModel)Parameter);
                    this._Documents.Remove((DocumentViewModel)Parameter);
                    NotifyPropertyChanged("Documents");
                    break;
                case "DocumentModified":
                    SelectedItemViewModel = null;
                    NotifyPropertyChanged("Documents");
                    break;
                case "SourceControlAdded":
                    this.SourceControls.Add((SourceControlViewModel)Parameter);
                    SelectedItemViewModel = null;
                    break;
                case "SourceControlDeleted":
                    this.SourceControls.Remove((SourceControlViewModel)Parameter);
                    break;
                case "SourceControlModified":
                    SelectedItemViewModel = null;
                    break;
            }
        }

        private void CloseProject(object obj)
        {
            NavigateToView(new ProjectListViewModel());
        }


       
    }
}
