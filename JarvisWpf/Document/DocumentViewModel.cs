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
using System.ComponentModel.DataAnnotations;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace JarvisWpf.Document
{
    public class DocumentViewModel : BindableBase
    {
        #region Properties
        IResourceProvider ResourceProvider = null;
        public DocumentPayload Document { get; set; }
        public ProjectPayload project { get; set; }


        public List<string> DocumentSubTypes
        {
            get
            {
                return DocumentPayload.GetDocumentSubTypes();
            }
        }

        private string _DisplayString;
        [Required]
        public string DisplayString
        {
            get { return _DisplayString; }
            set
            {
                _DisplayString = value;
                NotifyPropertyChanged("DisplayString");
            }
        }

        private string _FileName;
        [Required]
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; NotifyPropertyChanged("FileName"); }
        }

        private string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; NotifyPropertyChanged("Id"); }
        }

        private bool _NeedFileManipulation;
        public bool NeedFileManipulation
        {
            get { return _NeedFileManipulation; }
            set { _NeedFileManipulation = value; NotifyPropertyChanged("NeedFileManipulation"); }
        }

        private bool _NeedsUpload;
        public bool NeedsUpload
        {
            get { return _NeedsUpload; }
            set { _NeedsUpload = value; NotifyPropertyChanged("NeedsUpload"); }
        }

        private string _ProjectItemSubType;
        [Required]
        public string ProjectItemSubType
        {
            get { return _ProjectItemSubType; }
            set
            {
                _ProjectItemSubType = value;
                NeedsUpload = DocumentPayload.ConvertSubTypeToNeedsUpload(value);
                NeedFileManipulation = DocumentPayload.ConvertSubTypeToNeedsFileManipulation(value);
                ShowRichTextBox = DocumentPayload.ConvertSubTypeToNeedsRtf(value);
                ShowThumbNail = DocumentPayload.ConvertSubTypeToShowThumbNail(value);
                DocumentFileNameLabelText = value == "Link" ? "Url / File Path" : "File Saved as :";
                DocumentFileNameTextboxEnabled = value == "Link";
                NotifyPropertyChanged("ProjectItemSubType");
            }
        }

        private bool _ShowRichTextBox;
        public bool ShowRichTextBox
        {
            get
            {
                return _ShowRichTextBox;
            }
            set
            {
                _ShowRichTextBox = value;
                NotifyPropertyChanged("ShowRichTextBox");
            }
        }

        private bool _ShowThumbNail;
        public bool ShowThumbNail
        {
            get
            {
                return _ShowThumbNail;
            }
            set
            {
                _ShowThumbNail = value;
                NotifyPropertyChanged("ShowThumbNail");
            }
        }

        private string _UploadPath;
        public string UploadPath
        {
            get { return _UploadPath; }
            set { _UploadPath = value; NotifyPropertyChanged("UploadPath"); }
        }

        public BitmapImage ThumbnailPath
        {
            get
            {
                if (!this.ShowThumbNail) return null;
                string path = GetProjectItemProcessArgument();
                ICustomCommand<MemoryStream> readThumbnail = ResourceProvider.GetThumbnailStream(path);
                MemoryStream ms = readThumbnail.Execute();

                BitmapImage bi = new BitmapImage();
                if (ms != null)
                {
                    if (ms.Length > 0)
                    {
                        try
                        {
                            bi.BeginInit();
                            bi.StreamSource = ms;
                            bi.EndInit();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                return bi;
            }
        }

        private string GetProjectItemProcessArgument()
        {
            string ret = this.Document.GetProcessArgument();
            if (ret != null)
            {
                if (ret.Contains("{0}"))
                {
                    ret = string.Format(ret, this.project.ProjectFolder);
                }
            }
            return ret;
        }

        private string _FileContent;
        public string FileContent
        {
            get
            {
                return _FileContent;
            }
            set { _FileContent = value; NotifyPropertyChanged("FileContent"); }
        }

        private string _DocumentFileNameLabelText;

        public string DocumentFileNameLabelText
        {
            get { return _DocumentFileNameLabelText; }
            set
            {
                _DocumentFileNameLabelText = value;
                NotifyPropertyChanged("DocumentFileNameLabelText");
            }
        }

        private bool _DocumentFileNameTextboxEnabled;

        public bool DocumentFileNameTextboxEnabled
        {
            get { return _DocumentFileNameTextboxEnabled; }
            set
            {
                _DocumentFileNameTextboxEnabled = value;
                NotifyPropertyChanged("DocumentFileNameTextboxEnabled");
            }
        }


        #endregion

        #region Constructor

        public DocumentViewModel()
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();
            SaveDocumentCommand = new RelayCommand<object>(SaveDocument, CanSaveDocument);
            DeleteDocumentCommand = new RelayCommand<object>(DeleteDocument, CanDeleteDocument);
            FileBrowseCommand = new RelayCommand<object>(FileBrowse);
            OpenDocumentCommand = new RelayCommand<DocumentViewModel>(OpenDocument, CanOpenDocument);
            IsValidationOn = true;
            CanExecuteChangedContainer += SaveDocumentCommand.RaiseCanExecuteChanged;
            CanExecuteChangedContainer += OpenDocumentCommand.RaiseCanExecuteChanged;
            CanExecuteChangedContainer += DeleteDocumentCommand.RaiseCanExecuteChanged;
        }

        #endregion

        #region Commands

        public RelayCommand<object> SaveDocumentCommand { get; set; }
        public RelayCommand<object> DeleteDocumentCommand { get; set; }
        public RelayCommand<object> FileBrowseCommand { get; set; }
        public RelayCommand<DocumentViewModel> OpenDocumentCommand { get; set; }

        #endregion

        #region Command Callbacks

        private bool CanOpenDocument()
        {
            return GetProjectItemProcessArgument() != null;
        }

        private void OpenDocument(DocumentViewModel parameter)
        {
            string Args = GetProjectItemProcessArgument();
            ICustomCommand openCommand = ResourceProvider.GetStartProcessCommand(Args);
            openCommand.Execute();
        }

        private bool CanDeleteDocument()
        {
            return isEditMode;
        }

        private void DeleteDocument(object obj)
        {
            IUndoableCommand deleteProjectItemCommand = ResourceProvider.GetDeleteProjectItemCommand(this.project, this.Document as PayloadBase);
            deleteProjectItemCommand.Execute();
            RaiseChildChangedEvent("DocumentDeleted", this);
            IUndoableCommand saveProjectCommand = ResourceProvider.GetProjectSaveCommand(this.project);
            saveProjectCommand.Execute();
        }

        private void FileBrowse(object obj)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                this.UploadPath = dialog.FileName;
            if (this.UploadPath != null)
            {
                this.FileName = Path.GetFileName(UploadPath);
            }
        }

        private bool CanSaveDocument()
        {
            return isValid;
        }

        private void SaveDocument(object obj)
        {
            string ActionToInvoke = "";
            if (isEditMode)
            {
                IUndoableCommand documentModifyCommand = null;
                string fieldName = null, fieldValue = null;

                if (Document.FileContent != this.FileContent)
                {
                    fieldName = "FileContent";
                    fieldValue = this.FileContent;
                    documentModifyCommand = ResourceProvider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                if (Document.DisplayString != this.DisplayString)
                {
                    fieldName = "DisplayString";
                    fieldValue = this.DisplayString;
                    documentModifyCommand = ResourceProvider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                if (Document.UploadPath != this.UploadPath)
                {
                    fieldName = "UploadPath";
                    fieldValue = this.UploadPath;
                    documentModifyCommand = ResourceProvider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                if (Document.FileName != this.FileName)
                {
                    fieldName = "FileName";
                    fieldValue = this.FileName;
                    documentModifyCommand = ResourceProvider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                ActionToInvoke = "DocumentModified";
            }
            else
            {
                CopyViewModelToDocument();
                IUndoableCommand AddItemToProjCommand = ResourceProvider.GetAddProjectItemCommand(this.project, (PayloadBase)this.Document);
                AddItemToProjCommand.Execute();
                ActionToInvoke = "DocumentAdded";
            }

            IUndoableCommand saveProjectCommand = ResourceProvider.GetProjectSaveCommand(this.project);
            saveProjectCommand.Execute();
            RaiseChildChangedEvent(ActionToInvoke, this);

        }
        #endregion

        #region Methods
        private void CopyViewModelToDocument()
        {
            this.Document.DisplayString = this.DisplayString;
            this.Document.FileContent = this.FileContent;
            this.Document.FileName = this.FileName;
            this.Document.NeedFileManipulation = this.NeedFileManipulation;
            this.Document.NeedsUpload = this.NeedsUpload;
            this.Document.ProjectItemSubType = this.ProjectItemSubType;
            this.Document.UploadPath = this.UploadPath;
        }


        public void setDocument(DocumentPayload doc, ProjectPayload Project)
        {
            this.DisplayString = doc.DisplayString;
            this.FileName = doc.FileName;
            this.Id = doc.Id;
            this.NeedFileManipulation = doc.NeedFileManipulation;
            this.NeedsUpload = doc.NeedsUpload;
            this.ProjectItemSubType = doc.ProjectItemSubType;
            this.UploadPath = doc.UploadPath;
            this.project = Project;
            this.Document = doc;
        }
        #endregion

        #region BindableBase Overrides

        protected override void BindBigData()
        {
            if (this.NeedFileManipulation && this.NeedsUpload == false)
            {
                ICustomCommand readCommand = ResourceProvider.GetReadContentToProjectItem(this.project, this.Document as PayloadBase);
                readCommand.Execute();
                FileContent = Document.FileContent;
            }
        }

        protected override void UnBindBigData()
        {
            this.FileContent = null;
            this.Document.FileContent = null;
            setDocument(this.Document, this.project);
        }

        #endregion
       
    }
}
