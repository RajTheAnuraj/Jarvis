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

        ICrosstalkService CrosstalkService { get; set; }

        //Commands
        IUndoableCommand GetDirectoryCreateRecursiveCommand(string DirectoryPath);
        IUndoableCommand GetFileCreateCommand(string FilePath, string FileContent);
        IUndoableCommand GetFileCreateFromStreamCommand(string FilePath, MemoryStream fileStream);
        IUndoableCommand GetFileModifyContentCommand(string FilePath, string NewContent);
        IUndoableCommand GetFileDeleteWithHistoryCommand(string FilePath, string TrashDirectory);
        IUndoableCommand GetFileMoveCommand(string SourceFilePath, string DestinationFilePath);
        IUndoableCommand GetFileCopyCommand(string SourceFilePath, string DestinationFilePath);
        ICustomCommand<MemoryStream> GetThumbnailStream(string FilePath);
        IUndoableCommand GetDirectoryCreateCommand(string DirectoryPath);
        IUndoableCommand GetAddProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItemToBeAdded);
        IUndoableCommand GetDeleteProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem);


        IUndoableCommand GetProjectInitializeCommand(ProjectPayload Project);
        IUndoableCommand GetProjectSaveCommand(ProjectPayload Project);
        IUndoableCommand GetModifyProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem, string FieldName, string FieldValue);
        ICustomCommand GetRetrieveProjectListCommand(ref ProjectListPayload projectListPayload);
        IUndoableCommand GetSaveProjectListCommand(ProjectListPayload projectListPayload);

        IUndoableCommand GetApplicationInitializeCommand(string RootFolder);
        ICustomCommand<bool> GetApplicationDontShowMeCommand(string featureName);
        ICustomCommand GetApplicationDontShowMeSetCommand(string featureName);

        ICustomCommand GetFileReadAsStringCommand(string FilePath);
        ICustomCommand<string> GetFileReadAsString2Command(string FilePath);
        ICustomCommand GetFileReadAsStreamCommand(string FilePath, ref Stream stream);
        ICustomCommand GetReadContentToProjectItem(ProjectPayload Project, PayloadBase ProjectItem);

        ICustomCommand<bool> GetCanCreateFromClipboardCommand();
        IUndoableCommand GetCreateFromClipboardCommand(ProjectPayload Project);
        ICustomCommand GetCopyTextToClipBoardCommand(string text, string Format = null);

        ICustomCommand GetStartProcessCommand(string Argument);


        IUndoableCommand GetSaveCommonItemCommand(List<ApplicationContextMenuPayload> ItemToSave);
        ICustomCommand<List<ApplicationContextMenuPayload>> GetRetrieveCommonItemCommand();

    }
}
