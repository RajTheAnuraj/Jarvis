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
            this.Id = Retval.Id??Guid.NewGuid().ToString();
            this.DisplayString = Retval.DisplayString;
            this.FileName = Retval.FileName;
            this.ProjectItemSubType = Retval.ProjectItemSubType;
        }

        public override string ReadToString()
        {
            return XmlOperations.SerializeToXml<DocumentPayload>(this);
        }

        public override void UpdateFromXml(string xml)
        {
            DocumentPayload Retval = XmlOperations.DeserializeFromXml<DocumentPayload>(xml);
            this.DisplayString = Retval.DisplayString;
            this.FileName = Retval.FileName;
        }

        public override void DeleteItem()
        {
            InvokeDeleteItem(this);
        }

    }
}
