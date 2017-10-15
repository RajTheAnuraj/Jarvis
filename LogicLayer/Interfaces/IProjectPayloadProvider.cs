using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Interfaces
{
    public interface IResourceProvider
    {
        string GetProjectsRootFolder();
        string GetSystemFolder();
        void setRootFolder(string rootFolder);


        //Commands
        IUndoableCommand GetDirectoryCreateRecursiveCommand(string DirectoryPath);
        IUndoableCommand GetFileCreateCommand(string FilePath, string FileContent);
        IUndoableCommand GetFileModifyContentCommand(string FilePath, string NewContent);
        IUndoableCommand GetFileDeleteWithHistoryCommand(string FilePath, string TrashDirectory);
        IUndoableCommand GetFileMoveCommand(string SourceFilePath, string DestinationFilePath);
        IUndoableCommand GetFileCopyCommand(string SourceFilePath, string DestinationFilePath);
        IUndoableCommand GetDirectoryCreateCommand(string DirectoryPath);
        IUndoableCommand GetAddProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItemToBeAdded);
        IUndoableCommand GetDeleteProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem);

        IUndoableCommand GetProjectInitializeCommand(ProjectPayload Project);
        IUndoableCommand GetProjectSaveCommand(ProjectPayload Project);
        ICustomCommand GetRetrieveProjectListCommand(ref ProjectListPayload projectListPayload);

        IUndoableCommand GetApplicationInitializeCommand();


        ICustomCommand GetFileReadAsStringCommand(string FilePath);
        ICustomCommand GetFileReadAsStreamCommand(string FilePath, ref Stream stream);
        ICustomCommand GetReadContentToProjectItem(ProjectPayload Project, PayloadBase ProjectItem);
        
    }
}
