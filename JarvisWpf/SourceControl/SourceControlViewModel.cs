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
using System.Xml;

namespace JarvisWpf.SourceControl
{
    public class SourceControlViewModel : BindableBase
    {
        IResourceProvider ResourceProvider = null;
        public ProjectPayload project { get; set; }
        public SourceControlPayload SourceControl { get; set; }

        public List<string> SourceControlSubTypes
        {
            get
            {
                return SourceControlPayload.GetSourceControlSubTypes();
            }
        }

        private string _ProjectItemSubType;
        [Required]
        public string ProjectItemSubType
        {
            get { return _ProjectItemSubType; }
            set
            {
                _ProjectItemSubType = value;
                NotifyPropertyChanged("ProjectItemSubType");
            }
        }

        private string _SourceControlDetail;

        public string SourceControlDetail
        {
            get { return _SourceControlDetail; }
            set
            {
                _SourceControlDetail = value;
                NotifyPropertyChanged("SourceControlDetail");
            }
        }


        private DateTime _SourceControlDateTime;

        public DateTime SourceControlDateTime
        {
            get { return _SourceControlDateTime; }
            set
            {
                _SourceControlDateTime = value;
                NotifyPropertyChanged("SourceControlDateTime");
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



        public RelayCommand<object> SaveSourceControlCommand { get; set; }
        public RelayCommand<object> DeleteSourceControlCommand { get; set; }


        public SourceControlViewModel()
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();
            SaveSourceControlCommand = new RelayCommand<object>(SaveSourceControl, CanSaveSourceControl);
            CanExecuteChangedContainer += SaveSourceControlCommand.RaiseCanExecuteChanged;
            IsValidationOn = true;

            DeleteSourceControlCommand = new RelayCommand<object>(DeleteSourceControl, CanDeleteSourceControl);
        }

        private bool CanDeleteSourceControl()
        {
            return true;
        }

        private void DeleteSourceControl(object obj)
        {
            IUndoableCommand deleteProjectItemCommand = ResourceProvider.GetDeleteProjectItemCommand(this.project, this.SourceControl as PayloadBase);
            deleteProjectItemCommand.Execute();
            RaiseChildChangedEvent("SourceControlDeleted", this);
            IUndoableCommand saveProjectCommand = ResourceProvider.GetProjectSaveCommand(this.project);
            saveProjectCommand.Execute();
        }

        private bool CanSaveSourceControl()
        {
            return isValid;
        }

        private void SaveSourceControl(object obj)
        {
            string ActionToInvoke = "";
            if (isEditMode)
            {
                IUndoableCommand sourceControlModifyCommand = null;
                string fieldName = null, fieldValue = null;

                if (SourceControl.DisplayString != this.DisplayString)
                {
                    fieldName = "DisplayString";
                    fieldValue = this.DisplayString;
                    sourceControlModifyCommand = ResourceProvider.GetModifyProjectItemCommand(this.project, this.SourceControl as PayloadBase, fieldName, fieldValue);
                    sourceControlModifyCommand.Execute();
                }
                if (SourceControl.SourceControlDateTime != this.SourceControlDateTime)
                {
                    fieldName = "SourceControlDateTime";
                    fieldValue = XmlConvert.ToString(this.SourceControlDateTime);
                    sourceControlModifyCommand = ResourceProvider.GetModifyProjectItemCommand(this.project, this.SourceControl as PayloadBase, fieldName, fieldValue);
                    sourceControlModifyCommand.Execute();
                }
                if (SourceControl.SourceControlDetail != this.SourceControlDetail)
                {
                    fieldName = "SourceControlDetail";
                    fieldValue = this.SourceControlDetail;
                    sourceControlModifyCommand = ResourceProvider.GetModifyProjectItemCommand(this.project, this.SourceControl as PayloadBase, fieldName, fieldValue);
                    sourceControlModifyCommand.Execute();
                }

                ActionToInvoke = "SourceControlModified";
            }
            else
            {
                CopyViewModelToSourceControl();
                IUndoableCommand AddItemToProjCommand = ResourceProvider.GetAddProjectItemCommand(this.project, (PayloadBase)this.SourceControl);
                AddItemToProjCommand.Execute();
                ActionToInvoke = "SourceControlAdded";
            }

            IUndoableCommand saveProjectCommand = ResourceProvider.GetProjectSaveCommand(this.project);
            saveProjectCommand.Execute();
            RaiseChildChangedEvent(ActionToInvoke, this);

        }

        private void CopyViewModelToSourceControl()
        {
            this.SourceControl.DisplayString = this.DisplayString;
            this.SourceControl.SourceControlDateTime = this.SourceControlDateTime;
            this.SourceControl.SourceControlDetail = this.SourceControlDetail;
            this.SourceControl.ProjectItemSubType = this.ProjectItemSubType;
        }

        public void setSourceControl(SourceControlPayload sc, ProjectPayload pl)
        {
            this.project = pl;
            this.SourceControl = sc;
            this.DisplayString = sc.DisplayString;
            this.ProjectItemSubType = sc.ProjectItemSubType;
            this.SourceControlDetail = sc.SourceControlDetail;
            this.SourceControlDateTime = sc.SourceControlDateTime;
        }

    }
}
