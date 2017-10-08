using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;

namespace LogicLayer.Payloads
{
    public class SourceControlPayload : PayloadBase
    {
        public string SourceControlDetail { get; set; }
        public DateTime SourceControlDateTime { get; set; }

        public override string ProjectItemType
        {
            get
            {
                return "Source Control";
            }
            set
            {
                //Do nothing. Leaving here for serialization
            }
        }

       
        public override void CreateFromXml(string xml)
        {
            SourceControlPayload Retval = XmlOperations.DeserializeFromXml<SourceControlPayload>(xml);
            base.LoadFromXml(Retval);
            this.SourceControlDetail = Retval.SourceControlDetail;
            this.SourceControlDateTime = Retval.SourceControlDateTime;
        }

        public override string ReadToString()
        {
            return XmlOperations.SerializeToXml<SourceControlPayload>(this);
        }

        public override void UpdateFromXml(string xml)
        {
            SourceControlPayload Retval = XmlOperations.DeserializeFromXml<SourceControlPayload>(xml);
            base.LoadFromXml(Retval);
            this.SourceControlDetail = Retval.SourceControlDetail;
            this.SourceControlDateTime = Retval.SourceControlDateTime;
        }

        public override void DeleteItem()
        {
            base.InvokeDeleteItem(this);
        }

        
    }
}
