using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;

namespace LogicLayer.Payloads
{
    public class CommunicationPayload: PayloadBase
    {

        public CommunicationPayload()
        {
            NeedFileManipulation = true;
        }

        public override string ProjectItemType
        {
            get
            {
                return "Communication";
            }
            set
            {
                //Do nothing. Leaving here for serialization
            }
        }

        

        public override void CreateFromXml(string xml)
        {
            CommunicationPayload Retval = XmlOperations.DeserializeFromXml<CommunicationPayload>(xml);
            this.Id = Retval.Id;
            this.DisplayString = Retval.DisplayString;
            this.ProjectItemSubType = Retval.ProjectItemSubType;
            this.FileContent = Retval.FileContent;
            this.FileName = Retval.FileName;
        }

        public override string ReadToString()
        {
            return XmlOperations.SerializeToXml<CommunicationPayload>(this);
        }

        public override void UpdateFromXml(string xml)
        {
            CommunicationPayload Retval = XmlOperations.DeserializeFromXml<CommunicationPayload>(xml);
            this.DisplayString = Retval.DisplayString;
            this.FileContent = Retval.FileContent;
            this.FileName = Retval.FileName;
        }

        public override void DeleteItem()
        {
            base.InvokeDeleteItem(this);
        }

        
    }
}
