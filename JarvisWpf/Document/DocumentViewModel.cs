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

namespace JarvisWpf.Document
{
    public class DocumentViewModel : BindableBase
    {
        public ProjectPayload project { get; set; }

        private string _DisplayString;

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

        public string ProjectItemSubType
        {
            get { return _ProjectItemSubType; }
            set { _ProjectItemSubType = value; NotifyPropertyChanged("ProjectItemSubType"); }
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
            get { return _FileContent; }
            set { _FileContent = value; NotifyPropertyChanged("FileContent"); }
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

        public void LoadedMethod()
        {
            if (this.NeedFileManipulation)
            {
                ICustomCommand readCommand = provider.GetReadContentToProjectItem(this.project, this.Document as PayloadBase);
                readCommand.Execute();
                FileContent = Document.FileContent;
            }
        }
    }
}
