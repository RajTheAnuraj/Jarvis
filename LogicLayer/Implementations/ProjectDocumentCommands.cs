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
        ProjectPayload Project = null;
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
                string DocumentToBeAddedDocumentPath = thisDocumentsFolder + "\\" + Path.GetFileName(DocumentToBeAdded.DocumentUploadFromPath);
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
            Project.ProjectItems.Add(DocumentToBeAdded);
        }

        public void Undo()
        {
            while (true)
            {
                if (History.Count == 0) break;
                History.Pop().Undo();
            }
        }
    }
}
