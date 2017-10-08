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
    public class DocumentPayload: IProjectItem
    {
        public string Id { get; set; }
        public string DocumentDisplayString { get; set; }
        public string DocumentPath { get; set; }
        public string DocumentType { get; set; }
        public bool NeedsUpload { get; set; }

        [XmlIgnore]
        public string DocumentUploadFromPath { get; set; }
        
        [XmlIgnore]
        public string DocumentContent { get; set; }

        public string ProjectItemType
        {
            get
            {
                return "Document";
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
            DocumentPayload Retval = XmlOperations.DeserializeFromXml<DocumentPayload>(xml);
            this.Id = Retval.Id??Guid.NewGuid().ToString();
            this.DocumentDisplayString = Retval.DocumentDisplayString;
            this.DocumentPath = Retval.DocumentPath;
            this.DocumentType = Retval.DocumentType;
        }

        public string ReadToString()
        {
            return XmlOperations.SerializeToXml<DocumentPayload>(this);
        }

        public void UpdateFromXml(string xml)
        {
            DocumentPayload Retval = XmlOperations.DeserializeFromXml<DocumentPayload>(xml);
            this.DocumentDisplayString = Retval.DocumentDisplayString;
            this.DocumentPath = Retval.DocumentPath;
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
