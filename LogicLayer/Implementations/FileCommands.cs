using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class FileCreateCommand:IUndoableCommand, ICommand
    {
        public string FilePath { get; set; }
        public FileStream fileStream { get; set; }
        public bool CloseStream { get; set; }
        public string FileContent { get; set; }

        public FileCreateCommand()
        {

        }

        public FileCreateCommand(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public FileCreateCommand(string FilePath, bool CloseStream)
        {
            this.FilePath = FilePath;
            this.CloseStream = CloseStream;
        }

        public FileCreateCommand(string FilePath,string Content):this(FilePath,true)
        {
            this.FileContent = Content;
        }

        public void Execute()
        {
            if (File.Exists(FilePath))
            {
                throw new IOException("The file exists and Cannot be overwritten by this command. Use FileOverWriteCommand");
            }
            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                throw new IOException("The File Path is not valid. Directory Doesnt exist");

            fileStream = File.Create(FilePath);

            if (!String.IsNullOrWhiteSpace(FileContent))
            {
                StreamWriter sw = new StreamWriter(fileStream);
                sw.Write(FileContent);
                sw.Flush();
            }

            if (CloseStream)
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        public void Undo()
        {
            if (fileStream != null)
                fileStream.Close();
            fileStream.Dispose();
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }


    public class FileModifyContentCommand : IUndoableCommand, ICommand
    {
        public string FilePath { get; set; }
        public string Content { get; set; }
        string OldContent;


        public FileModifyContentCommand()
        {

        }

        public FileModifyContentCommand(string FilePath, string Content)
        {
            this.FilePath = FilePath;
            this.Content = Content;
            if (File.Exists(FilePath))
            {
                OldContent = File.ReadAllText(FilePath);
            }
        }

        public void Execute()
        {
            if (!File.Exists(FilePath))
            {
                throw new IOException("The file doesnt exist");
            }

            StreamWriter sw = new StreamWriter(FilePath);
            sw.Write(Content);
            sw.Close();
            sw.Dispose();
        }

        public void Undo()
        {
            if (!File.Exists(FilePath))
            {
                throw new IOException("The file doesnt exist");
            }

            StreamWriter sw = new StreamWriter(FilePath);
            sw.Write(OldContent);
            sw.Close();
            sw.Dispose();
        }
    }

    public class FileDeleteWithHistoryCommand:IUndoableCommand,ICommand
    {
        public string FilePath { get; set; }
        public string TrashDirectoryPath { get; set; }

        public FileDeleteWithHistoryCommand()
        {

        }


        public FileDeleteWithHistoryCommand(string FilePath, string TrashDirectory)
        {
            this.FilePath = FilePath;
            this.TrashDirectoryPath = TrashDirectory;
        }

        string TrashedFileName
        {
            get
            {
                return String.Format(@"{0}\{1}", TrashDirectoryPath, Path.GetFileName(FilePath));
            }
        }

        public void Execute()
        {
            CheckTrashDirectoryIsGood();
            File.Move(FilePath, TrashedFileName);
        }

        private void CheckTrashDirectoryIsGood()
        {
            if (String.IsNullOrEmpty(TrashDirectoryPath) || !Directory.Exists(TrashDirectoryPath))
                throw new IOException("ThrashDirectory Path has to be valid. The file will be moved there for Undo");
        }

        public void Undo()
        {
            CheckTrashDirectoryIsGood();
            File.Move(TrashedFileName, FilePath);
        }
    }

    public class FileMoveCommand : IUndoableCommand, ICommand
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }

        public string directorySrc { get { return Path.GetDirectoryName(SourceFilePath); } }
        public string directoryDest { get { return Path.GetDirectoryName(DestinationFilePath); } }

        public FileMoveCommand()
        {

        }

        public FileMoveCommand(string SourceFilePath, string DestinationFilePath)
        {
            this.SourceFilePath = SourceFilePath;
            this.DestinationFilePath = DestinationFilePath;
        }

        public void Execute()
        {
            if (!Directory.Exists(directoryDest))
                throw new IOException("The Destination Folder is not valid");
            File.Move(SourceFilePath, DestinationFilePath);
        }

        public void Undo()
        {
            if (!Directory.Exists(directorySrc))
                throw new IOException("The directory where the file resided is not available. Aborting Undo");
            File.Move(DestinationFilePath, SourceFilePath);
        }
    }

    public class FileCopyCommand : IUndoableCommand, ICommand
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }

        public string directorySrc { get { return Path.GetDirectoryName(SourceFilePath); } }
        public string directoryDest { get { return Path.GetDirectoryName(DestinationFilePath); } }

        public FileCopyCommand()
        {

        }

        public FileCopyCommand(string SourceFilePath, string DestinationFilePath)
        {
            this.SourceFilePath = SourceFilePath;
            this.DestinationFilePath = DestinationFilePath;
        }

        public void Execute()
        {
            if (!Directory.Exists(directoryDest))
                throw new IOException("The Destination Folder is not valid");
            File.Copy(SourceFilePath, DestinationFilePath);
        }

        public void Undo()
        {
            if (!File.Exists(DestinationFilePath))
                throw new IOException("The file not available. Aborting Undo");
            File.Delete(DestinationFilePath);
        }
    }
}
