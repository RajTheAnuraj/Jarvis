﻿using LogicLayer.Factories;
using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace LogicLayer.Implementations
{
    public class ApplicationInitializeCommand:IUndoableCommand, ICustomCommand
    {
        public IResourceProvider ResourceProvider { get; set; }
        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();
        public string RootFolder { get; set; }

        public ApplicationInitializeCommand(string RootFolder)
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();
            this.RootFolder = RootFolder;
        }

        public void Execute()
        {
            //if root folder is null get it from config
            ResourceProvider.setRootFolder(this.RootFolder);

            //Get the root folder
            string RootFolder = ResourceProvider.GetProjectsRootFolder();
            if (RootFolder == null)
                throw new ArgumentNullException("Rootfolder");

            //Check whether the root folder exists
            if (!Directory.Exists(RootFolder))
            {
                IUndoableCommand dirCreateCommand = ResourceProvider.GetDirectoryCreateRecursiveCommand(RootFolder);
                History.Push(dirCreateCommand);
                dirCreateCommand.Execute();
            }

            DirectoryInfo rootFolderDI = new DirectoryInfo(RootFolder);
            rootFolderDI.Attributes &= ~FileAttributes.ReadOnly;

            //Get system Folder
            string SystemsFolder = ResourceProvider.GetSystemFolder();

            //If System Folder doesnt exist then create it
            if (!Directory.Exists(SystemsFolder))
            {
                IUndoableCommand dirCreateCommand = ResourceProvider.GetDirectoryCreateRecursiveCommand(SystemsFolder);
                History.Push(dirCreateCommand);
                dirCreateCommand.Execute();
            }

            DirectoryInfo systemsFolderDI = new DirectoryInfo(SystemsFolder);
            systemsFolderDI.Attributes &= ~FileAttributes.ReadOnly;

            //See if Projects File exists in System Folder
            //If project file doesnt exist create the base
            string ProjectsFilePath = String.Format("{0}\\Projects.Jarvis", SystemsFolder);
            if (!File.Exists(ProjectsFilePath))
            {
                IUndoableCommand filCreateCommand = ResourceProvider.GetFileCreateCommand(ProjectsFilePath, "<Projects></Projects>");
                History.Push(filCreateCommand);
                filCreateCommand.Execute();
            }

            //Get ArchiveFolder
            //If Archive folder doesnt exist create it
            string ArchiveFolder = String.Format("{0}\\Archive", RootFolder);
            if (!Directory.Exists(ArchiveFolder))
            {
                IUndoableCommand dirCreateCommand = ResourceProvider.GetDirectoryCreateRecursiveCommand(ArchiveFolder);
                History.Push(dirCreateCommand);
                dirCreateCommand.Execute();
            }

            DirectoryInfo ArchiveFolderDI = new DirectoryInfo(ArchiveFolder);
            ArchiveFolderDI.Attributes &= ~FileAttributes.ReadOnly;
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

    public class ApplicationDontShowMeCommand:ICustomCommand<bool>
    {
        public ApplicationDontShowMeCommand(string featureName)
        {

        }

        public bool Execute()
        {
            IResourceProvider resourceProvider = ProviderFactory.GetCurrentProvider();
            string pathOfMarker = resourceProvider.GetSystemFolder() + "\\marker.Jarvis";
            return File.Exists(pathOfMarker);
            
        }
    }

    public class ApplicationDontShowMeSetCommand : ICustomCommand
    {
        public ApplicationDontShowMeSetCommand(string featureName)
        {

        }

        public void Execute()
        {
            IResourceProvider resourceProvider = ProviderFactory.GetCurrentProvider();
            string pathOfMarker = resourceProvider.GetSystemFolder() + "\\marker.Jarvis";
            if(!File.Exists(pathOfMarker))
            {
                IUndoableCommand fileCreationCommand = resourceProvider.GetFileCreateCommand(pathOfMarker, "");
                fileCreationCommand.Execute();
            }

        }
    }
}
