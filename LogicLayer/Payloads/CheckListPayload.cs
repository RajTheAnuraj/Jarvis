using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;
using System.Xml.Serialization;

namespace LogicLayer.Payloads
{
    public class ChecklistPayload: PayloadBase
    {
        public override string DisplayString { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime ChecklistAlertDate { get; set; }
        public string ChecklistPhase { get; set; }

        public ChecklistPayload()
        {
            NeedFileManipulation = false;
        }

        public override string ProjectItemType
        {
            get
            {
                return "Checklist";
            }
            set
            {
                //Do nothing. Leaving here for serialization
            }
        }

        

        public override void CreateFromXml(string xml)
        {
            ChecklistPayload Retval = XmlOperations.DeserializeFromXml<ChecklistPayload>(xml);
            this.Id = Retval.Id;
            this.DisplayString = Retval.DisplayString;
            this.ChecklistAlertDate = Retval.ChecklistAlertDate;
            this.ChecklistPhase = Retval.ChecklistPhase;

        }

        public override string ReadToString()
        {
            return XmlOperations.SerializeToXml<ChecklistPayload>(this);
        }

        public override void UpdateFromXml(string xml)
        {
            ChecklistPayload Retval = XmlOperations.DeserializeFromXml<ChecklistPayload>(xml);
            this.DisplayString = Retval.DisplayString;
            this.ChecklistAlertDate = Retval.ChecklistAlertDate;
            this.ChecklistPhase = Retval.ChecklistPhase;
        }

        public override void DeleteItem()
        {
            base.InvokeDeleteItem(this);
        }

       
    }
}
