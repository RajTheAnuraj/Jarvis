using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class AddDocumentCommand : IUndoableCommand
    {
        public ProjectPayload Project = null;
        public DocumentPayload DocumentToBeAdded { get; set; }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        public AddDocumentCommand(ProjectPayload Project)
        {
            this.Project = Project;
        }

        public AddDocumentCommand(ProjectPayload Project,DocumentPayload DocumentToBeAdded)
        {
            this.Project = Project;
            this.DocumentToBeAdded = DocumentToBeAdded;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (DocumentToBeAdded == null) throw new ArgumentNullException("DocumentToBeAdded");
            string RootFolder = Project.ProjectFolder;
            string thisDocumentsFolder = RootFolder + "\\" + DocumentToBeAdded.ProjectItemType + "\\" + DocumentToBeAdded.DocumentType;
            if (!Directory.Exists(thisDocumentsFolder))
            {
                DirectoryCreateRecursiveCommand createDocDirectory = new DirectoryCreateRecursiveCommand(thisDocumentsFolder);
                History.Push(createDocDirectory);
                createDocDirectory.Execute();
            }

            if (DocumentToBeAdded.NeedsUpload)
            {
                DocumentToBeAdded.DocumentPath = Path.GetFileName(DocumentToBeAdded.DocumentUploadFromPath);
                string DocumentToBeAddedDocumentPath = thisDocumentsFolder + "\\" + DocumentToBeAdded.DocumentPath;
                
                FileCopyCommand copyCommand = new FileCopyCommand(DocumentToBeAdded.DocumentUploadFromPath, DocumentToBeAddedDocumentPath);
                History.Push(copyCommand);
                copyCommand.Execute();
            }
            else
            {
                string DocumentToBeAddedDocumentPath = thisDocumentsFolder + "\\" + DocumentToBeAdded.DocumentPath;
                if (!File.Exists(DocumentToBeAddedDocumentPath))
                {
                    FileCreateCommand createCommand = new FileCreateCommand(DocumentToBeAddedDocumentPath, DocumentToBeAdded.DocumentContent);
                    History.Push(createCommand);
                    createCommand.Execute();
                }
            }
            Project.AddProjectItem(DocumentToBeAdded);
        }

        public void Undo()
        {
            while (true)
            {
                if (History.Count == 0) break;
                History.Pop().Undo();
            }
            DocumentToBeAdded.DeleteItem();
        }
    }

    public class ReadContentToDocument : ICommand
    {
        public ProjectPayload Project = null;
        public DocumentPayload Document { get; set; }

        public ReadContentToDocument(ProjectPayload Project)
        {
            this.Project = Project;
        }

        public ReadContentToDocument(ProjectPayload Project, DocumentPayload Document)
        {
            this.Project = Project;
            this.Document = Document;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (Document == null) throw new ArgumentNullException("DocumentToBeAdded");
            string RootFolder = Project.ProjectFolder;
            string thisDocumentsFolder = RootFolder + "\\" + Document.ProjectItemType + "\\" + Document.DocumentType;

            if (!Document.NeedsUpload)
            {
                string DocumentPath = thisDocumentsFolder + "\\" + Document.DocumentPath;
                if (File.Exists(DocumentPath))
                {
                    FileReadAsStringCommand createCommand = new FileReadAsStringCommand(DocumentPath);
                    createCommand.Execute();
                    Document.DocumentContent = createCommand.ReadContent;
                }
            }
        }
    }


    public class DeleteDocumentCommand : IUndoableCommand
    {
         public ProjectPayload Project = null;
         public DocumentPayload Document { get; set; }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        public DeleteDocumentCommand(ProjectPayload Project)
        {
            this.Project = Project;
        }

        public DeleteDocumentCommand(ProjectPayload Project, DocumentPayload Document)
        {
            this.Project = Project;
            this.Document = Document;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");
            if (Document == null) throw new ArgumentNullException("DocumentToBeAdded");
            string RootFolder = Project.ProjectFolder;
            string TrashFolder = Project.TrashFolder;
            string thisDocumentsFolder = RootFolder + "\\" + Document.ProjectItemType + "\\" + Document.DocumentType;
            string DocumentPath = thisDocumentsFolder + "\\" + Document.DocumentPath;
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
            Document.DeleteItem();
        }

        public void Undo()
        {
            while (true)
            {
                if (History.Count == 0) break;
                History.Pop().Undo();
            }
            Project.AddProjectItem(Document);
        }
    }
}
