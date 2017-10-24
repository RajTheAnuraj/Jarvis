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
    public class SaveCommonItemCommand: IUndoableCommand, ICustomCommand
    {
        public List<ApplicationContextMenuPayload> ItemToSave { get; set; }
        IResourceProvider ResourceProvider = null;

        public SaveCommonItemCommand(List<ApplicationContextMenuPayload> ItemToSave)
        {
            this.ItemToSave = ItemToSave;
            ResourceProvider = ProviderFactory.GetCurrentProvider();
        }

        public void Execute()
        {
            string SystemFolder = ResourceProvider.GetSystemFolder();
            string CommonItemFile = String.Format("{0}\\CommonItems.Jarvis", SystemFolder);

            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<ApplicationContextMenuPayload>));
            MemoryStream ms = new MemoryStream();
            xs.Serialize(ms, ItemToSave);
            ms.Position = 0;
            string Content = System.Text.Encoding.Default.GetString(ms.ToArray());

            if (File.Exists(CommonItemFile))
            {
                IUndoableCommand fileModifyCommand = ResourceProvider.GetFileModifyContentCommand(CommonItemFile, Content);
                fileModifyCommand.Execute();
            }
            else
            {
                IUndoableCommand fileCreateCommand = ResourceProvider.GetFileCreateCommand(CommonItemFile, Content);
                fileCreateCommand.Execute();
            }
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }


    public class RetrieveCommonItemCommand: ICustomCommand<List<ApplicationContextMenuPayload>>{

        IResourceProvider ResourceProvider = null;

        public RetrieveCommonItemCommand()
        {
            ResourceProvider = ProviderFactory.GetCurrentProvider();

        }

        public List<ApplicationContextMenuPayload> Execute()
        {
            List<ApplicationContextMenuPayload> retval = new List<ApplicationContextMenuPayload>();
            string SystemFolder = ResourceProvider.GetSystemFolder();
            string CommonItemFile = String.Format("{0}\\CommonItems.Jarvis", SystemFolder);
            if (File.Exists(CommonItemFile))
            {
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(List<ApplicationContextMenuPayload>));
                ICustomCommand<string> xmlCommand = ResourceProvider.GetFileReadAsString2Command(CommonItemFile);
                string xml = xmlCommand.Execute();
                object obj  = xs.Deserialize(new StringReader(xml));
                if (obj != null)
                {
                    retval = (List<ApplicationContextMenuPayload>)obj;
                    retval = retval.OrderBy(c => c.Category).ToList();
                }
            }

            ApplicationContextMenuPayload ExternalToolMenuItem = new ApplicationContextMenuPayload();
            ExternalToolMenuItem.DisplayName = "External Tools";
            ExternalToolMenuItem.isCategory = true;

            string ExternalToolsFolder = String.Format("{0}\\External Tools", ResourceProvider.GetProjectsRootFolder());
            if (Directory.Exists(ExternalToolsFolder))
            {
                ExternalToolMenuItem.innerList = new List<ApplicationContextMenuPayload>();
                DirectoryInfo externalTools = new DirectoryInfo(ExternalToolsFolder);
                foreach (DirectoryInfo ToolCategories in externalTools.GetDirectories())
                {
                    ApplicationContextMenuPayload ExternalToolMenuCategory = new ApplicationContextMenuPayload();
                    ExternalToolMenuCategory.DisplayName = ToolCategories.Name;
                    ExternalToolMenuCategory.isCategory = true;
                    FileInfo[] fi = ToolCategories.GetFiles();
                    if (fi != null)
                    {
                        if (fi.Length > 0)
                        {
                            ExternalToolMenuCategory.innerList = new List<ApplicationContextMenuPayload>();
                            foreach (FileInfo toolItem in fi)
                            {
                                ApplicationContextMenuPayload ExternalToolItem = new ApplicationContextMenuPayload();
                                ExternalToolItem.DisplayName = toolItem.Name;
                                ExternalToolItem.isCategory = false;
                                ExternalToolItem.isAction = true;
                                ExternalToolItem.ActionString = toolItem.FullName;
                                ExternalToolMenuCategory.innerList.Add(ExternalToolItem);
                            }
                        }
                    }
                    ExternalToolMenuItem.innerList.Add(ExternalToolMenuCategory);
                }
                retval.Insert(0,ExternalToolMenuItem);
            }

            return retval;
        }
    }
    
}
