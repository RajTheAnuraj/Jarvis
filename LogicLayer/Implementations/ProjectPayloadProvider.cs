using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class ProjectPayloadProvider: IProjectPayloadProvider
    {
        public string GetProjectsRootFolder()
        {
            return @"C:\Temp";
        }

        public string GetSystemFolder()
        {
            return GetProjectsRootFolder() + "\\System";
        }


        public IUndoableCommand GetDirectoryCreateRecursiveCommand(string DirectoryPath)
        {
            return new DirectoryCreateRecursiveCommand(DirectoryPath);
        }


        public IUndoableCommand GetFileCreateCommand(string FilePath, string FileContent)
        {
            return new FileCreateCommand(FilePath, FileContent);
        }


        public IUndoableCommand GetFileModifyContentCommand(string FilePath, string NewContent)
        {
            return new FileModifyContentCommand(FilePath, NewContent);
        }


        public IUndoableCommand GetFileDeleteWithHistoryCommand(string FilePath, string TrashDirectory)
        {
            return new FileDeleteWithHistoryCommand(FilePath, TrashDirectory);
        }


        public IUndoableCommand GetFileMoveCommand(string SourceFilePath, string DestinationFilePath)
        {
            return new FileMoveCommand(SourceFilePath, DestinationFilePath);
        }


        public IUndoableCommand GetFileCopyCommand(string SourceFilePath, string DestinationFilePath)
        {
            return new FileCopyCommand(SourceFilePath, DestinationFilePath);
        }

        public IUndoableCommand GetDirectoryCreateCommand(string DirectoryPath)
        {
            return new DirectoryCreateCommand(DirectoryPath);
        }

        public IUndoableCommand GetAddProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItemToBeAdded)
        {
            return new AddProjectItemCommand(Project, ProjectItemToBeAdded);
        }

        public IUndoableCommand GetDeleteProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem)
        {
            return new DeleteProjectItemCommand(Project, ProjectItem);
        }

        public IUndoableCommand GetProjectInitializeCommand(ProjectPayload Project)
        {
            return new ProjectInitializeCommand(Project);
        }

        public IUndoableCommand GetProjectSaveCommand(ProjectPayload Project)
        {
            return new ProjectSaveCommand(Project);
        }

        public ICustomCommand GetFileReadAsStringCommand(string FilePath)
        {
            return new FileReadAsStringCommand(FilePath);
        }

        public ICustomCommand GetReadContentToProjectItem(ProjectPayload Project, PayloadBase ProjectItem)
        {
            return new ReadContentToProjectItem(Project, ProjectItem);
        }


       
    }
}
