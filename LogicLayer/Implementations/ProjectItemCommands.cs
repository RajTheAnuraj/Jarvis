using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LogicLayer.Implementations
{
    public class AddProjectItemCommand : IUndoableCommand
    {
        public ProjectPayload Project = null;
        public PayloadBase ProjectItemToBeAdded { get; set; }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();
        IProjectPayloadProvider CommandProvider = null;

        private AddProjectItemCommand()
        {
            CommandProvider = ProviderFactory.GetCurrentProvider();
        }

        public AddProjectItemCommand(ProjectPayload Project):this()
        {
            this.Project = Project;
        }

        public AddProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItemToBeAdded):this(Project)
        {
            this.ProjectItemToBeAdded = ProjectItemToBeAdded;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (ProjectItemToBeAdded == null) throw new ArgumentNullException("DocumentToBeAdded");
            if (ProjectItemToBeAdded.NeedFileManipulation)
            {
                string RootFolder = Project.ProjectFolder;
                string thisDocumentsFolder = RootFolder + "\\" + ProjectItemToBeAdded.ProjectItemType;
                if (!Directory.Exists(thisDocumentsFolder))
                {
                    IUndoableCommand createDocDirectory = CommandProvider.GetDirectoryCreateRecursiveCommand(thisDocumentsFolder);
                    History.Push(createDocDirectory);
                    createDocDirectory.Execute();
                }

                if (ProjectItemToBeAdded.NeedsUpload)
                {
                    if (!String.IsNullOrWhiteSpace(ProjectItemToBeAdded.UploadPath))
                    {
                        ProjectItemToBeAdded.FileName = Path.GetFileName(ProjectItemToBeAdded.UploadPath);
                        string DocumentToBeAddedDocumentPath = thisDocumentsFolder + "\\" + ProjectItemToBeAdded.FileName;

                        IUndoableCommand copyCommand = CommandProvider.GetFileCopyCommand(ProjectItemToBeAdded.UploadPath, DocumentToBeAddedDocumentPath);
                        History.Push(copyCommand);
                        copyCommand.Execute();
                    }

                    string documentPath = thisDocumentsFolder + "\\" + ProjectItemToBeAdded.FileName;
                }
                else
                {
                    string DocumentToBeAddedDocumentPath = thisDocumentsFolder + "\\" + ProjectItemToBeAdded.FileName;
                    if (!File.Exists(DocumentToBeAddedDocumentPath))
                    {
                        IUndoableCommand createCommand = CommandProvider.GetFileCreateCommand(DocumentToBeAddedDocumentPath, ProjectItemToBeAdded.FileContent);
                        History.Push(createCommand);
                        createCommand.Execute();
                    }
                }
            }
            Project.AddProjectItem(ProjectItemToBeAdded);
        }

        public void Undo()
        {
            while (true)
            {
                if (History.Count == 0) break;
                History.Pop().Undo();
            }
            ProjectItemToBeAdded.DeleteItem();
        }
    }

    public class ReadContentToProjectItem : ICustomCommand
    {
        public ProjectPayload Project = null;
        public PayloadBase ProjectItem { get; set; }

        IProjectPayloadProvider CommandProvider = null;

        private ReadContentToProjectItem()
        {
            CommandProvider = ProviderFactory.GetCurrentProvider();
        }

        public ReadContentToProjectItem(ProjectPayload Project):this ()
        {
            this.Project = Project;
        }

        public ReadContentToProjectItem(ProjectPayload Project, PayloadBase ProjectItem):this(Project)
        {
            this.ProjectItem = ProjectItem;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (ProjectItem == null) throw new ArgumentNullException("DocumentToBeAdded");
            if (ProjectItem.NeedFileManipulation)
            {
                string RootFolder = Project.ProjectFolder;
                string thisDocumentsFolder = RootFolder + "\\" + ProjectItem.ProjectItemType;

                if (!ProjectItem.NeedsUpload)
                {
                    string DocumentPath = thisDocumentsFolder + "\\" + ProjectItem.FileName;
                    if (File.Exists(DocumentPath))
                    {
                        ICustomCommand createCommand =  CommandProvider.GetFileReadAsStringCommand(DocumentPath);
                        createCommand.Execute();
                        ProjectItem.FileContent = ((IReadTillEndAsString)createCommand).ReadTillEndAsString;
                    }
                } 
            }
        }
    }

    public class ModifyProjectItemCommand : IUndoableCommand
    {
        public ProjectPayload Project = null;
        public PayloadBase ProjectItem { get; set; }
        string OldProjectItem { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
         IProjectPayloadProvider CommandProvider = null;

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        private ModifyProjectItemCommand()
        {
            CommandProvider = ProviderFactory.GetCurrentProvider();
        }

        public ModifyProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem, string FieldName, string FieldValue)
            : this()
        {
            this.Project = Project;
            this.ProjectItem = ProjectItem;
            this.FieldName = FieldName;
            this.FieldValue = FieldValue;
        }

        public void Execute()
        {
            //Check for nulls
            if (Project == null) throw new ArgumentNullException("Project");
            if (ProjectItem == null) throw new ArgumentNullException("ProjectItem");

            //Copy the old payloads state for undo
            OldProjectItem = ProjectItem.ReadToString();

            //See if File manipulation is needed
            if (ProjectItem.NeedFileManipulation)
            {
                string RootFolder = Project.ProjectFolder;
                string thisDocumentsFolder = RootFolder + "\\" + ProjectItem.ProjectItemType;
                string DocumentPath = thisDocumentsFolder + "\\" + ProjectItem.FileName;

                if (ProjectItem.NeedsUpload)
                {
                    if (FieldName == "FileName")
                    {
                        IUndoableCommand fileMoveCommand = CommandProvider.GetFileMoveCommand(DocumentPath, Project.TrashFolder + "\\" + ProjectItem.FileName);
                        History.Push(fileMoveCommand);
                        fileMoveCommand.Execute();

                        
                        DocumentPath = thisDocumentsFolder + "\\" + FieldValue;
                        IUndoableCommand fileCopyCommand = CommandProvider.GetFileCopyCommand(FieldValue, DocumentPath);
                        History.Push(fileCopyCommand);
                        fileCopyCommand.Execute();
                    }
                }

                if(FieldName == "FileContent"){
                    IUndoableCommand fileModifyCommand = CommandProvider.GetFileModifyContentCommand(DocumentPath, FieldValue);
                    History.Push(fileModifyCommand);
                    fileModifyCommand.Execute();
                }
            }

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(ProjectItem.ReadToString());
            var field = xdoc.DocumentElement.SelectSingleNode(FieldName);
            if (field != null)
                field.InnerText = FieldValue;
            ProjectItem.UpdateFromXml(xdoc.OuterXml);

        }

        public void Undo()
        {
            while (true)
            {
                if (History.Count == 0) break;
                History.Pop().Undo();
            }
            ProjectItem.UpdateFromXml(OldProjectItem);
        }
    }

    public class DeleteProjectItemCommand : IUndoableCommand
    {
         public ProjectPayload Project = null;
         public PayloadBase ProjectItem { get; set; }
         IProjectPayloadProvider CommandProvider = null;

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        private DeleteProjectItemCommand()
        {
            CommandProvider = ProviderFactory.GetCurrentProvider();
        }

        public DeleteProjectItemCommand(ProjectPayload Project):this()
        {
            this.Project = Project;
        }

        public DeleteProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem):this(Project)
        {
            this.ProjectItem = ProjectItem;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (ProjectItem == null) throw new ArgumentNullException("DocumentToBeAdded");
            if (ProjectItem.NeedFileManipulation)
            {
                string RootFolder = Project.ProjectFolder;
                string TrashFolder = Project.TrashFolder;
                string thisDocumentsFolder = RootFolder + "\\" + ProjectItem.ProjectItemType;
                string DocumentPath = thisDocumentsFolder + "\\" + ProjectItem.FileName;
                if (File.Exists(DocumentPath))
                {
                    if (!Directory.Exists(TrashFolder))
                    {
                        IUndoableCommand dirCreateCommand = CommandProvider.GetDirectoryCreateRecursiveCommand(TrashFolder);
                        History.Push(dirCreateCommand);
                        dirCreateCommand.Execute();
                    }

                    IUndoableCommand filedeleteCommand = CommandProvider.GetFileDeleteWithHistoryCommand(DocumentPath, TrashFolder);
                    History.Push(filedeleteCommand);
                    filedeleteCommand.Execute();
                } 
            }
            ProjectItem.DeleteItem();
        }

        public void Undo()
        {
            while (true)
            {
                if (History.Count == 0) break;
                History.Pop().Undo();
            }
            Project.AddProjectItem(ProjectItem);
        }
    }
}
