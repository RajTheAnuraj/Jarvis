using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;

namespace LogicLayer.Payloads
{
    public class CommunicationPayload: IProjectItem
    {
        public string Id { get; set; }
        public string CommunicationDisplayName { get; set; }
        public string CommunicationType { get; set; }
        public string CommunicationDetails { get; set; }
        public string CommunicationPath { get; set; }

        public string ProjectItemType
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
            CommunicationPayload Retval = XmlOperations.DeserializeFromXml<CommunicationPayload>(xml);
            this.Id = Retval.Id;
            this.CommunicationDisplayName = Retval.CommunicationDisplayName;
            this.CommunicationType = Retval.CommunicationType;
            this.CommunicationDetails = Retval.CommunicationDetails;
            this.CommunicationPath = Retval.CommunicationPath;
        }

        public string ReadToString()
        {
            return XmlOperations.SerializeToXml<CommunicationPayload>(this);
        }

        public void UpdateFromXml(string xml)
        {
            CommunicationPayload Retval = XmlOperations.DeserializeFromXml<CommunicationPayload>(xml);
            this.CommunicationDisplayName = Retval.CommunicationDisplayName;
            this.CommunicationDetails = Retval.CommunicationDetails;
            this.CommunicationPath = Retval.CommunicationPath;
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
