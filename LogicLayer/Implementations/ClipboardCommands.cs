using LogicLayer.Factories;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogicLayer.Implementations
{
    public static class ClipboardConfigurations
    {
        public static List<string> AllowedClipboardFormats { get; set; }

        static ClipboardConfigurations()
        {
            initializeAllowedClipboardFormats();
        }

        private static void initializeAllowedClipboardFormats()
        {
            AllowedClipboardFormats = new List<string>();
            AllowedClipboardFormats.Add(DataFormats.FileDrop);
            AllowedClipboardFormats.Add(DataFormats.Bitmap);
            AllowedClipboardFormats.Add(DataFormats.Rtf);
            AllowedClipboardFormats.Add(DataFormats.Html);
            AllowedClipboardFormats.Add(DataFormats.UnicodeText);
            AllowedClipboardFormats.Add(DataFormats.Text);
        }

        public static void ClipboardClear()
        {
            Clipboard.Clear();
        }
    }

    public class CanCreateFromClipboardCommand : ICustomCommand<bool>
    {

        public bool Execute()
        {
            foreach (string key in ClipboardConfigurations.AllowedClipboardFormats)
            {
                if(Clipboard.ContainsData(key))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class CreateFromClipboardCommand : IUndoableCommand, ICustomCommand
    {

        public IResourceProvider ResourceProvider { get; set; }
        public ProjectPayload Project { get; set; }
        public string ClipboardType { get; set; }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();

        public CreateFromClipboardCommand()
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();
        }

        public CreateFromClipboardCommand(ProjectPayload Project)
            : this()
        {
            this.Project = Project;
        }

        public void Execute()
        {
            if (Project == null) throw new ArgumentNullException("Project");

            foreach (string key in ClipboardConfigurations.AllowedClipboardFormats)
            {
                if (Clipboard.ContainsData(key))
                {
                    ClipboardType = key;
                    break;
                }
            }

            switch (ClipboardType)
            {
                case "FileDrop":
                    SaveFiles();
                    break;
                case "Bitmap":
                    SaveBitmap();
                    break;
                case "HTML Format":
                    SaveHtml();
                    break;
                case "Rich Text Format":
                    SaveRtf();
                    break;
                case "UnicodeText":
                case "Text":
                    SaveText();
                    break;
                default:
                    throw new Exception("Cannot handle the item that you have copied");
                    break;
            }
           
        }

        private void SaveText()
        {
            string fileName = String.Format(@"{0}{1}.txt", Path.GetTempPath(), Guid.NewGuid().ToString());
            string FileContent = ((string)(Clipboard.GetDataObject().GetData("Text")));
            IUndoableCommand fileCreateCommand = ResourceProvider.GetFileCreateCommand(fileName, FileContent);
            fileCreateCommand.Execute();

            DocumentPayload document = new DocumentPayload();
            document.DisplayString = "Pasted -- " + Path.GetFileName(fileName);
            document.FileName = Path.GetFileName(fileName);
            document.NeedsUpload = true;
            document.ProjectItemSubType = "File";
            document.UploadPath = fileName;
            IUndoableCommand AddItemCommand = ResourceProvider.GetAddProjectItemCommand(this.Project, document as PayloadBase);
            History.Push(AddItemCommand);
            AddItemCommand.Execute();

            File.Delete(fileName);
        }

        private void SaveRtf()
        {
            string FileContent = ((string)(Clipboard.GetDataObject().GetData("Rich Text Format")));
            
            DocumentPayload document = new DocumentPayload();
            document.DisplayString = "Pasted -- " + DateTime.Now.ToString();
            document.NeedsUpload = false;
            document.ProjectItemSubType = "Code Snippet";
            document.FileContent = FileContent;
            IUndoableCommand AddItemCommand = ResourceProvider.GetAddProjectItemCommand(this.Project, document as PayloadBase);
            History.Push(AddItemCommand);
            AddItemCommand.Execute();
        }

        private void SaveHtml()
        {
            string fileName = String.Format(@"{0}{1}.html", Path.GetTempPath(), Guid.NewGuid().ToString());
            string FileContent = ((string)(Clipboard.GetDataObject().GetData("HTML Format")));
            IUndoableCommand fileCreateCommand = ResourceProvider.GetFileCreateCommand(fileName, FileContent);
            fileCreateCommand.Execute();

            DocumentPayload document = new DocumentPayload();
            document.DisplayString = "Pasted -- " + Path.GetFileName(fileName);
            document.FileName = Path.GetFileName(fileName);
            document.NeedsUpload = true;
            document.ProjectItemSubType = "File";
            document.UploadPath = fileName;
            IUndoableCommand AddItemCommand = ResourceProvider.GetAddProjectItemCommand(this.Project, document as PayloadBase);
            History.Push(AddItemCommand);
            AddItemCommand.Execute();
            
            File.Delete(fileName);
        }

        private void SaveBitmap()
        {
            string fileName = String.Format(@"{0}{1}.bmp", Path.GetTempPath(), Guid.NewGuid().ToString());
            Bitmap bitmap = ((Bitmap)(Clipboard.GetDataObject().GetData("Bitmap")));
            bitmap.Save(fileName);

            DocumentPayload document = new DocumentPayload();
            document.DisplayString = "Screenshot -- " + Path.GetFileName(fileName);
            document.FileName = Path.GetFileName(fileName);
            document.NeedsUpload = true;
            document.ProjectItemSubType = "Screenshot";
            document.UploadPath = fileName;
            IUndoableCommand AddItemCommand = ResourceProvider.GetAddProjectItemCommand(this.Project, document as PayloadBase);
            History.Push(AddItemCommand);
            AddItemCommand.Execute();

            File.Delete(fileName);
        }

        private void SaveFiles()
        {
            string[] fileNames = (string[])(Clipboard.GetDataObject().GetData(ClipboardType));
            if (fileNames != null)
            {
                foreach (string filename in fileNames)
                {
                    if (File.Exists(filename))
                    {
                        DocumentPayload document = new DocumentPayload();
                        document.DisplayString = "Pasted -- " + Path.GetFileName(filename);
                        document.FileName = Path.GetFileName(filename);
                        document.NeedsUpload = true;
                        document.ProjectItemSubType = "File";
                        document.UploadPath = filename;
                        IUndoableCommand AddItemCommand = ResourceProvider.GetAddProjectItemCommand(this.Project, document as PayloadBase);
                        History.Push(AddItemCommand);
                        AddItemCommand.Execute();
                    }
                }
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


    public class CopyTextToClipBoardCommand : ICustomCommand
    {
        public string text { get; set; }
        public string Format { get; set; }

        public CopyTextToClipBoardCommand(string text, string Format = null)
        {
            this.text = text;
            this.Format = Format;
        }

        public void Execute()
        {
            if (this.Format == null)
                this.Format = DataFormats.UnicodeText;
            Clipboard.SetData(Format, text);
        }
    }


}
