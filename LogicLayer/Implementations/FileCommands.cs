using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class FileCreateCommand:IUndoableCommand, ICustomCommand
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

    public class FileCreateFromStreamCommand : IUndoableCommand, ICustomCommand
    {
        public string FilePath { get; set; }
        public MemoryStream fileStream { get; set; }

        public FileCreateFromStreamCommand()
        {

        }

        public FileCreateFromStreamCommand(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public FileCreateFromStreamCommand(string FilePath, MemoryStream fileStream)
        {
            this.FilePath = FilePath;
            this.fileStream = fileStream;
        }


        public void Execute()
        {
            if (File.Exists(FilePath))
            {
                throw new IOException("The file exists and Cannot be overwritten by this command. Use FileOverWriteCommand");
            }
            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                throw new IOException("The File Path is not valid. Directory Doesnt exist");

            if (fileStream == null)
                throw new IOException("The Stream is empty");

            FileStream fs = new FileStream(FilePath, FileMode.Create);
            fileStream.Position = 0;
            fileStream.CopyTo(fs);

            fs.Flush();
            fs.Close();
            fs.Dispose();

            fileStream.Close();
            fileStream.Dispose();
        }

        public void Undo()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }

    public class FileModifyContentCommand : IUndoableCommand, ICustomCommand
    {
        public string FilePath { get; set; }
        public string Content { get; set; }
        string OldContent;


        private FileModifyContentCommand()
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

    public class FileDeleteWithHistoryCommand:IUndoableCommand,ICustomCommand
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
                return String.Format(@"{0}\{1}.{2}", TrashDirectoryPath, Path.GetFileName(FilePath),Guid.NewGuid());
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

    public class FileMoveCommand : IUndoableCommand, ICustomCommand
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

    public class FileCopyCommand : IUndoableCommand, ICustomCommand
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
            File.Copy(SourceFilePath, DestinationFilePath,true);
        }

        public void Undo()
        {
            if (!File.Exists(DestinationFilePath))
                throw new IOException("The file not available. Aborting Undo");
            File.Delete(DestinationFilePath);
        }
    }

    public class FileReadAsStringCommand : ICustomCommand, IReadTillEndAsString, ICustomCommand<string>
    {

        public string FilePath { get; set; }
        public string ReadTillEndAsString { get; set; }

        public FileReadAsStringCommand()
        {

        }

        public FileReadAsStringCommand(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public void Execute()
        {
            if (File.Exists(FilePath))
            {
                ReadTillEndAsString = File.ReadAllText(FilePath);
            }
        }

        string ICustomCommand<string>.Execute()
        {
            string retval = null;
            if (File.Exists(FilePath))
            {
                using (StreamReader Sr = new StreamReader(FilePath))
                {
                    retval = Sr.ReadToEnd();
                    Sr.Close();
                }

            }
            return retval;
        }
    }

    public class FileReadAsStreamCommand : ICustomCommand
    {

        public string FilePath { get; set; }
        public string ReadTillEndAsString { get; set; }
        public Stream fileStream { get; set; }

        public FileReadAsStreamCommand()
        {

        }

        public FileReadAsStreamCommand(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public FileReadAsStreamCommand(string FilePath, ref Stream stream)
        {
            this.FilePath = FilePath;
            this.fileStream = stream;
        }

        public void Execute()
        {
            if (File.Exists(FilePath))
            {
                this.fileStream = new StreamReader(FilePath).BaseStream;
            }
        }
    }

    public class ThumbnailStreamCommand : ICustomCommand<MemoryStream>
    {
        string FilePath;

        public ThumbnailStreamCommand(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public MemoryStream Execute()
        {
            MemoryStream st = new MemoryStream();
            if (!String.IsNullOrWhiteSpace(FilePath))
            {
                if (File.Exists(FilePath))
                {
                    byte[] by = File.ReadAllBytes(FilePath);
                    st.Write(by, 0, by.Length);
                    st.Position = 0;
                }
            }
            return st;
        }
    }
}
