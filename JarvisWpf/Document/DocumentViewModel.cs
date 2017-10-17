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

namespace JarvisWpf.Document
{
    public class DocumentViewModel : BindableBase
    {
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



        private string _UploadPath;

        public string UploadPath
        {
            get { return _UploadPath; }
            set { _UploadPath = value; NotifyPropertyChanged("UploadPath"); }
        }

        private string _FileContent;

        public string FileContent
        {
            get {
                return _FileContent; 
            }
            set { _FileContent = value; NotifyPropertyChanged("FileContent"); }
        }

        public RelayCommand<object> SaveDocumentCommand { get; set; }
        public RelayCommand<object> DeleteDocumentCommand { get; set; }
        public RelayCommand<object> FileBrowseCommand { get; set; }

        public DocumentViewModel()
        {
            SaveDocumentCommand = new RelayCommand<object>(SaveDocument, CanSaveDocument);
            DeleteDocumentCommand = new RelayCommand<object>(DeleteDocument, CanDeleteDocument);
            FileBrowseCommand = new RelayCommand<object>(FileBrowse);
            IsValidationOn = true;
            CanExecuteChangedContainer += SaveDocumentCommand.RaiseCanExecuteChanged;
        }

        

        private bool CanDeleteDocument()
        {
            return true;
        }

        private void DeleteDocument(object obj)
        {
            IUndoableCommand deleteProjectItemCommand = provider.GetDeleteProjectItemCommand(this.project, this.Document as PayloadBase);
            deleteProjectItemCommand.Execute();
            RaiseChildChangedEvent("DocumentDeleted", this);
            IUndoableCommand saveProjectCommand = provider.GetProjectSaveCommand(this.project);
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
                    documentModifyCommand = provider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                if (Document.DisplayString != this.DisplayString)
                {
                    fieldName = "DisplayString";
                    fieldValue = this.DisplayString;
                    documentModifyCommand = provider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                if (Document.UploadPath != this.UploadPath)
                {
                    fieldName = "UploadPath";
                    fieldValue = this.UploadPath;
                    documentModifyCommand = provider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                if (Document.FileName != this.FileName)
                {
                    fieldName = "FileName";
                    fieldValue = this.FileName;
                    documentModifyCommand = provider.GetModifyProjectItemCommand(this.project, this.Document as PayloadBase, fieldName, fieldValue);
                    documentModifyCommand.Execute();
                }
                ActionToInvoke = "DocumentModified";
            }
            else
            {
                CopyViewModelToDocument();
                IUndoableCommand AddItemToProjCommand = provider.GetAddProjectItemCommand(this.project, (PayloadBase)this.Document);
                AddItemToProjCommand.Execute();
                ActionToInvoke = "DocumentAdded";
            }
            
            IUndoableCommand saveProjectCommand = provider.GetProjectSaveCommand(this.project);
            saveProjectCommand.Execute();
            RaiseChildChangedEvent(ActionToInvoke, this);
            
        }

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

        public DocumentPayload Document { get; set; }

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

        IResourceProvider provider = ProviderFactory.GetCurrentProvider();

        protected override void BindBigData()
        {
            if (this.NeedFileManipulation && this.NeedsUpload == false)
            {
                ICustomCommand readCommand = provider.GetReadContentToProjectItem(this.project, this.Document as PayloadBase);
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

        public void LoadedMethod()
        {
            
        }

       
    }
}
