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
            base.LoadFromXml(Retval);
        }

        public override string ReadToString()
        {
            return XmlOperations.SerializeToXml<CommunicationPayload>(this);
        }

        public override void UpdateFromXml(string xml)
        {
            CommunicationPayload Retval = XmlOperations.DeserializeFromXml<CommunicationPayload>(xml);
            base.LoadFromXml(Retval);
        }

        public override void DeleteItem()
        {
            base.InvokeDeleteItem(this);
        }

        
    }
}
