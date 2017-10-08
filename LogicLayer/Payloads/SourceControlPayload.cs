using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;

namespace LogicLayer.Payloads
{
    public class SourceControlPayload : IProjectItem
    {
        public string Id { get; set; }
        public string SourceControlType { get; set; }
        public string SourceControlDetail { get; set; }
        public DateTime SourceControlDateTime { get; set; }

        

        public string ProjectItemType
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
            SourceControlPayload Retval = XmlOperations.DeserializeFromXml<SourceControlPayload>(xml);
            this.Id = Retval.Id;
            this.SourceControlType = Retval.SourceControlType;
            this.SourceControlDetail = Retval.SourceControlDetail;
            this.SourceControlDateTime = Retval.SourceControlDateTime;
        }

        public string ReadToString()
        {
            return XmlOperations.SerializeToXml<SourceControlPayload>(this);
        }

        public void UpdateFromXml(string xml)
        {
            SourceControlPayload Retval = XmlOperations.DeserializeFromXml<SourceControlPayload>(xml);
            this.SourceControlType = Retval.SourceControlType;
            this.SourceControlDetail = Retval.SourceControlDetail;
            this.SourceControlDateTime = Retval.SourceControlDateTime;
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
