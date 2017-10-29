using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class ProjectPayloadProvider: IResourceProvider
    {
        string _rootFolder;
        

        public string GetProjectsRootFolder()
        {
            return _rootFolder;
        }

        public string GetSystemFolder()
        {
            return GetProjectsRootFolder() + "\\System";
        }

        public void setRootFolder(string rootFolder)
        {
            _rootFolder = rootFolder;
        }


        public ICrosstalkService CrosstalkService { get; set; }

        public IUndoableCommand GetDirectoryCreateRecursiveCommand(string DirectoryPath)
        {
            return new DirectoryCreateRecursiveCommand(DirectoryPath);
        }


        public IUndoableCommand GetFileCreateCommand(string FilePath, string FileContent)
        {
            return new FileCreateCommand(FilePath, FileContent);
        }

        public IUndoableCommand GetFileCreateFromStreamCommand(string FilePath, MemoryStream fileStream)
        {
            return new FileCreateFromStreamCommand(FilePath, fileStream);
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

        public IUndoableCommand GetModifyProjectItemCommand(ProjectPayload Project, PayloadBase ProjectItem, string FieldName, string FieldValue)
        {
            return new ModifyProjectItemCommand(Project, ProjectItem, FieldName, FieldValue);
        }

        public IUndoableCommand GetProjectInitializeCommand(ProjectPayload Project)
        {
            return new ProjectInitializeCommand(Project);
        }

        public IUndoableCommand GetProjectSaveCommand(ProjectPayload Project)
        {
            return new ProjectSaveCommand(Project);
        }

        public ICustomCommand GetRetrieveProjectListCommand(ref ProjectListPayload projectListPayload)
        {
            return new RetrieveProjectListCommand(ref projectListPayload);
        }


        public IUndoableCommand GetApplicationInitializeCommand(string RootFolder)
        {
            return new ApplicationInitializeCommand(RootFolder);
        }


        public ICustomCommand GetFileReadAsStringCommand(string FilePath)
        {
            return new FileReadAsStringCommand(FilePath);
        }

        public ICustomCommand GetFileReadAsStreamCommand(string FilePath, ref Stream stream)
        {
            return new FileReadAsStreamCommand(FilePath,ref stream);
        }

        public ICustomCommand GetReadContentToProjectItem(ProjectPayload Project, PayloadBase ProjectItem)
        {
            return new ReadContentToProjectItem(Project, ProjectItem);
        }











        public ICustomCommand<bool> GetCanCreateFromClipboardCommand()
        {
            return new CanCreateFromClipboardCommand();
        }

        public IUndoableCommand GetCreateFromClipboardCommand(ProjectPayload Project)
        {
            return new CreateFromClipboardCommand(Project);
        }

        public ICustomCommand GetStartProcessCommand(string Argument)
        {
            return new StartProcessCommand(Argument);
        }


        public ICustomCommand<MemoryStream> GetThumbnailStream(string FilePath)
        {
            return new ThumbnailStreamCommand(FilePath);
        }


        public IUndoableCommand GetSaveProjectListCommand(ProjectListPayload projectListPayload)
        {
            return new SaveProjectListCommand(projectListPayload);
        }


        public ICustomCommand<string> GetFileReadAsString2Command(string FilePath)
        {
            return new FileReadAsStringCommand(FilePath);
        }


        public ICustomCommand GetCopyTextToClipBoardCommand(string text, string Format = null)
        {
            return new CopyTextToClipBoardCommand(text, Format);
        }


        public IUndoableCommand GetSaveCommonItemCommand(List<ApplicationContextMenuPayload> ItemToSave)
        {
            return new SaveCommonItemCommand(ItemToSave);
        }


        public ICustomCommand<List<ApplicationContextMenuPayload>> GetRetrieveCommonItemCommand()
        {
            return new RetrieveCommonItemCommand();
        }



        public ICustomCommand<bool> GetApplicationDontShowMeCommand(string featureName)
        {
            return new ApplicationDontShowMeCommand(featureName);
        }


        public ICustomCommand GetApplicationDontShowMeSetCommand(string featureName)
        {
            return new ApplicationDontShowMeSetCommand(featureName);
        }
    }
}
