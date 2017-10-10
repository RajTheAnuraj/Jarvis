using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class ProjectInitializeCommand: IUndoableCommand
    {
        public ProjectPayload Project { get; set; }
        public IProjectPayloadProvider CommandProvider { get; set; }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        private ProjectInitializeCommand()
        {
            this.CommandProvider = ProviderFactory.GetCurrentProvider();
        }

        public ProjectInitializeCommand(ProjectPayload Project):this()
        {
            this.Project = Project;
            
        }
       

        public void Execute()
        {
            //Check for null
            if (Project == null) throw new ArgumentNullException("Project");

            //Set the ProjectFolder
            Project.ProjectFolder = CommandProvider.GetProjectsRootFolder() + "\\" + Project.ProjectName;

            //Create Folders if necesary
            //Project Root Folder
            if (!Directory.Exists(Project.ProjectFolder))
            {
                IUndoableCommand createProjDirectory = CommandProvider.GetDirectoryCreateRecursiveCommand(Project.ProjectFolder);
                History.Push(createProjDirectory);
                createProjDirectory.Execute();
            }

            //Project Trash Directory
            if (!Directory.Exists(Project.TrashFolder))
            {
                IUndoableCommand createProjTrashDirectory = CommandProvider.GetDirectoryCreateRecursiveCommand(Project.TrashFolder);
                History.Push(createProjTrashDirectory);
                createProjTrashDirectory.Execute();
            }

            string projectXml = Project.ProjectFolder + "\\Project.Jarvis";
            //if project xml doesnt exisit Create it
            if (!File.Exists(projectXml))
            {
                string Content = Project.ReadToString();
                IUndoableCommand createProjectFile = CommandProvider.GetFileCreateCommand(Project.ProjectFolder + "\\Project.Jarvis", Content);
                History.Push(createProjectFile);
                createProjectFile.Execute();
            }else //if file exists load it to project
            {
                ICustomCommand fileReadCommand = CommandProvider.GetFileReadAsStringCommand(projectXml);
                fileReadCommand.Execute();
                string fileContent = ((IReadTillEndAsString)fileReadCommand).ReadTillEndAsString;
                Project.UpdateFromXml(fileContent);
            }
            
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


    public class ProjectSaveCommand : IUndoableCommand
    {
        public ProjectPayload Project { get; set; }
        public IProjectPayloadProvider CommandProvider { get; set; }
        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        private ProjectSaveCommand()
        {
            this.CommandProvider = ProviderFactory.GetCurrentProvider();
        }

        public ProjectSaveCommand(ProjectPayload Project)
            : this()
        {
            this.Project = Project;
            
        }

        public void Execute()
        {
            //Check for null
            if (Project == null) throw new ArgumentNullException("Project");

            //Save the current content in Project.Jarvis to the state variable OldContent
            //Read the xml on payload and save it back to Project.Jarvis
            string ProjectXmlPath = Project.ProjectFolder + "\\Project.Jarvis";
            string NewContent = Project.ReadToString();
            if (File.Exists(ProjectXmlPath))
            {
                IUndoableCommand fileModifyCommand = CommandProvider.GetFileModifyContentCommand(ProjectXmlPath, NewContent);
                History.Push(fileModifyCommand);
                fileModifyCommand.Execute();
            }
            else
            {
                IUndoableCommand fileCreateCommand = CommandProvider.GetFileCreateCommand(ProjectXmlPath, NewContent);
                History.Push(fileCreateCommand);
                fileCreateCommand.Execute();
            }
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
