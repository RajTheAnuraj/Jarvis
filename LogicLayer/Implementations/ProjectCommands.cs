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
    public class ProjectInitializeCommand: IUndoableCommand
    {
        public ProjectPayload Project { get; set; }
        public IResourceProvider CommandProvider { get; set; }

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
            if (!Project.isRemoteProject)
                Project.ProjectFolder = CommandProvider.GetProjectsRootFolder() + "\\" + Project.ProjectName;
            else
                Project.ProjectFolder = Project.RemoteServerRoot + "\\" + Project.ProjectName;

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
            }
            else //if file exists load it to project
            {
                ICustomCommand fileReadCommand = CommandProvider.GetFileReadAsStringCommand(projectXml);
                fileReadCommand.Execute();
                string fileContent = ((IReadTillEndAsString)fileReadCommand).ReadTillEndAsString;
                Project.UpdateFromXml(fileContent);
            }

            string projectLogPath = Project.ProjectFolder + "\\Log.rtf";
            if (File.Exists(projectLogPath))
            {
                ICustomCommand fileReadLogCommand = CommandProvider.GetFileReadAsStringCommand(projectLogPath);
                fileReadLogCommand.Execute();
                Project.ProjectLogText = ((IReadTillEndAsString)fileReadLogCommand).ReadTillEndAsString;
            }


            string projectSummaryPath = Project.ProjectFolder + "\\Summary.rtf";
            if (File.Exists(projectSummaryPath))
            {
                ICustomCommand fileReadSummaryCommand = CommandProvider.GetFileReadAsStringCommand(projectSummaryPath);
                fileReadSummaryCommand.Execute();
                Project.ProjectSummaryText = ((IReadTillEndAsString)fileReadSummaryCommand).ReadTillEndAsString;
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
        public IResourceProvider CommandProvider { get; set; }
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


            //Save the log
            string ProjectLogPath = Project.ProjectFolder + "\\Log.rtf";
            string NewContent = Project.ProjectLogText;
            if (File.Exists(ProjectLogPath))
            {
                IUndoableCommand fileModifyCommand = CommandProvider.GetFileModifyContentCommand(ProjectLogPath, NewContent);
                History.Push(fileModifyCommand);
                fileModifyCommand.Execute();
            }
            else
            {
                IUndoableCommand fileCreateCommand = CommandProvider.GetFileCreateCommand(ProjectLogPath, NewContent);
                History.Push(fileCreateCommand);
                fileCreateCommand.Execute();
            }

            //Save the Summary
            string ProjectSummaryPath = Project.ProjectFolder + "\\Summary.rtf";
            NewContent = Project.ProjectSummaryText;
            if (File.Exists(ProjectSummaryPath))
            {
                IUndoableCommand fileModifyCommand = CommandProvider.GetFileModifyContentCommand(ProjectSummaryPath, NewContent);
                History.Push(fileModifyCommand);
                fileModifyCommand.Execute();
            }
            else
            {
                IUndoableCommand fileCreateCommand = CommandProvider.GetFileCreateCommand(ProjectSummaryPath, NewContent);
                History.Push(fileCreateCommand);
                fileCreateCommand.Execute();
            }

            //Save the current content in Project.Jarvis to the state variable OldContent
            //Read the xml on payload and save it back to Project.Jarvis
            string ProjectXmlPath = Project.ProjectFolder + "\\Project.Jarvis";
            NewContent = Project.ReadToString();
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


    public class RetrieveProjectListCommand:ICustomCommand
    {
        IResourceProvider ResourceProvider = null;
        public ProjectListPayload projectList { get; set; }

        public RetrieveProjectListCommand()
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();
        }

        public RetrieveProjectListCommand(ref ProjectListPayload ProjectList)
            : this()
        {
            projectList = ProjectList;
        }

        public void Execute()
        {
           //The assumption is this call wil be made only after applicationInitialize is called
            //else this will error out
            if (projectList == null)
                projectList = new ProjectListPayload();
            string ProjectsFilePath = ResourceProvider.GetSystemFolder() + "\\Projects.Jarvis";
            ICustomCommand fileReadCommand = ResourceProvider.GetFileReadAsStringCommand(ProjectsFilePath);
            fileReadCommand.Execute();
            string xml = ((IReadTillEndAsString)fileReadCommand).ReadTillEndAsString;
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            foreach (XmlNode Xn in xdoc.DocumentElement.SelectNodes("Project"))
            {
                projectList.Add(Xn);
            }
        }
    }
}
