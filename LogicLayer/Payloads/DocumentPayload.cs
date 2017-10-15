using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;
using System.Xml.Serialization;
using LogicLayer.Implementations;

namespace LogicLayer.Payloads
{
    public class DocumentPayload : PayloadBase
    {
        public DocumentPayload()
        {
            NeedFileManipulation = true;
        }

        public override string ProjectItemType
        {
            get
            {
                return "Document";
            }
            set{ }
        }

        public override void CreateFromXml(string xml)
        {
            DocumentPayload Retval = XmlOperations.DeserializeFromXml<DocumentPayload>(xml);
            base.LoadFromXml(Retval);
        }

        public override string ReadToString()
        {
            return XmlOperations.SerializeToXml<DocumentPayload>(this);
        }

        public override void UpdateFromXml(string xml)
        {
            DocumentPayload Retval = XmlOperations.DeserializeFromXml<DocumentPayload>(xml);
            base.LoadFromXml(Retval);
        }

        public override void DeleteItem()
        {
            InvokeDeleteItem(this);
        }

        static List<string> _DocumentSubTypes = new List<string>{
            "File",
            "Code Snippet",
            "Link"
        };

        public static List<string> GetDocumentSubTypes()
        {
            return _DocumentSubTypes;
        }

        public static bool ConvertSubTypeToNeedsUpload(string DocumentSubType)
        {
            if (DocumentSubType == "File")
                return true;
            return false;
        }

        public static bool ConvertSubTypeToNeedsFileManipulation(string DocumentSubType)
        {
            if (DocumentSubType == "File" || DocumentSubType == "Code Snippet")
                return true;
            return false;
        }
    }
}
