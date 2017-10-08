using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class AddProjectItemCommand : IUndoableCommand
    {
        public ProjectPayload Project = null;
        public PayloadBase ProjectItemToBeAdded { get; set; }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        public AddProjectItemCommand(ProjectPayload Project)
        {
            this.Project = Project;
        }

        public AddProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItemToBeAdded)
        {
            this.Project = Project;
            this.ProjectItemToBeAdded = ProjectItemToBeAdded;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (ProjectItemToBeAdded == null) throw new ArgumentNullException("DocumentToBeAdded");
            string RootFolder = Project.ProjectFolder;
            string thisDocumentsFolder = RootFolder + "\\" + ProjectItemToBeAdded.ProjectItemType + "\\" + ProjectItemToBeAdded.ProjectItemSubType;
            if (!Directory.Exists(thisDocumentsFolder))
            {
                DirectoryCreateRecursiveCommand createDocDirectory = new DirectoryCreateRecursiveCommand(thisDocumentsFolder);
                History.Push(createDocDirectory);
                createDocDirectory.Execute();
            }

            if (ProjectItemToBeAdded.NeedsUpload)
            {
                ProjectItemToBeAdded.FileName = Path.GetFileName(ProjectItemToBeAdded.UploadPath);
                string DocumentToBeAddedDocumentPath = thisDocumentsFolder + "\\" + ProjectItemToBeAdded.FileName;
                
                FileCopyCommand copyCommand = new FileCopyCommand(ProjectItemToBeAdded.UploadPath, DocumentToBeAddedDocumentPath);
                History.Push(copyCommand);
                copyCommand.Execute();
            }
            else
            {
                string DocumentToBeAddedDocumentPath = thisDocumentsFolder + "\\" + ProjectItemToBeAdded.FileName;
                if (!File.Exists(DocumentToBeAddedDocumentPath))
                {
                    FileCreateCommand createCommand = new FileCreateCommand(DocumentToBeAddedDocumentPath, ProjectItemToBeAdded.FileContent);
                    History.Push(createCommand);
                    createCommand.Execute();
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

    public class ReadContentToProjectItem : ICommand
    {
        public ProjectPayload Project = null;
        public PayloadBase ProjectItem { get; set; }

        public ReadContentToProjectItem(ProjectPayload Project)
        {
            this.Project = Project;
        }

        public ReadContentToProjectItem(ProjectPayload Project, PayloadBase ProjectItem)
        {
            this.Project = Project;
            this.ProjectItem = ProjectItem;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (ProjectItem == null) throw new ArgumentNullException("DocumentToBeAdded");
            string RootFolder = Project.ProjectFolder;
            string thisDocumentsFolder = RootFolder + "\\" + ProjectItem.ProjectItemType + "\\" + ProjectItem.ProjectItemSubType;

            if (!ProjectItem.NeedsUpload)
            {
                string DocumentPath = thisDocumentsFolder + "\\" + ProjectItem.FileName;
                if (File.Exists(DocumentPath))
                {
                    FileReadAsStringCommand createCommand = new FileReadAsStringCommand(DocumentPath);
                    createCommand.Execute();
                    ProjectItem.FileContent = createCommand.ReadContent;
                }
            }
        }
    }


    public class DeleteProjectItemCommand : IUndoableCommand
    {
         public ProjectPayload Project = null;
         public PayloadBase ProjectItem { get; set; }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        public DeleteProjectItemCommand(ProjectPayload Project)
        {
            this.Project = Project;
        }

        public DeleteProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem)
        {
            this.Project = Project;
            this.ProjectItem = ProjectItem;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (ProjectItem == null) throw new ArgumentNullException("DocumentToBeAdded");
            string RootFolder = Project.ProjectFolder;
            string TrashFolder = Project.TrashFolder;
            string thisDocumentsFolder = RootFolder + "\\" + ProjectItem.ProjectItemType + "\\" + ProjectItem.ProjectItemSubType;
            string DocumentPath = thisDocumentsFolder + "\\" + ProjectItem.FileName;
            if (File.Exists(DocumentPath))
            {
                if(!Directory.Exists(TrashFolder)){

                    DirectoryCreateRecursiveCommand dirCreateCommand = new DirectoryCreateRecursiveCommand(TrashFolder);
                    History.Push(dirCreateCommand);
                    dirCreateCommand.Execute();
                }
                FileDeleteWithHistoryCommand filedeleteCommand = new FileDeleteWithHistoryCommand(DocumentPath, TrashFolder);
                History.Push(filedeleteCommand);
                filedeleteCommand.Execute();
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
