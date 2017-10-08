using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;
using System.Xml.Serialization;

namespace LogicLayer.Payloads
{
    public class ChecklistPayload: IProjectItem
    {
        public string Id { get; set; }
        public string ChecklistDisplayString { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime ChecklistAlertDate { get; set; }
        public string ChecklistPhase { get; set; }


        public string ProjectItemType
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

        public string ProjectItemClassName
        {
            get
            {
                return this.GetType().AssemblyQualifiedName;
            }
            set
            {
                //Do nothing. Leaving here for serialization
            }
        }

        public void CreateFromXml(string xml)
        {
            ChecklistPayload Retval = XmlOperations.DeserializeFromXml<ChecklistPayload>(xml);
            this.Id = Retval.Id;
            this.ChecklistDisplayString = Retval.ChecklistDisplayString;
            this.ChecklistAlertDate = Retval.ChecklistAlertDate;
            this.ChecklistPhase = Retval.ChecklistPhase;

        }

        public string ReadToString()
        {
            return XmlOperations.SerializeToXml<ChecklistPayload>(this);
        }

        public void UpdateFromXml(string xml)
        {
            ChecklistPayload Retval = XmlOperations.DeserializeFromXml<ChecklistPayload>(xml);
            this.ChecklistDisplayString = Retval.ChecklistDisplayString;
            this.ChecklistAlertDate = Retval.ChecklistAlertDate;
            this.ChecklistPhase = Retval.ChecklistPhase;
        }

        public void DeleteItem()
        {
            if (onProjectItemDeleted != null)
                onProjectItemDeleted.Invoke(this);
        }

        public event ProjectItemDeleted onProjectItemDeleted;

        public bool Equals(IProjectItem other)
        {
            return other.ProjectItemType == this.ProjectItemType && other.Id == this.Id;
        }
    }
}
