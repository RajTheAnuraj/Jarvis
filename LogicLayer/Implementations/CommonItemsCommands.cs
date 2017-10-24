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

            //StringBuilder sb = new StringBuilder();
            //sb.Append("<ArrayOfApplicationContextMenuPayload>");
            //foreach (ApplicationContextMenuPayload item in ItemToSave)
            //{
            //    sb.Append(item.GetXml());
            //}
            //sb.Append("</ArrayOfApplicationContextMenuPayload>");

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
                }
            }

            return retval;
        }
    }
    
}
