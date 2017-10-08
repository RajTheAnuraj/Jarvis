using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LogicLayer.Payloads
{
    public abstract class PayloadBase : IProjectItem
    {
        protected void LoadFromXml(PayloadBase child)
        {
            this.Id = child.Id;
            this.DisplayString = child.DisplayString;
            this.FileName = child.FileName;
            this.NeedFileManipulation = child.NeedFileManipulation;
            this.NeedsUpload = child.NeedsUpload;
            this.ProjectItemSubType = child.ProjectItemSubType;
        }

        public abstract string ProjectItemType { get; set; }

        public string ProjectItemClassName
        {
            get
            {
                return this.GetType().AssemblyQualifiedName;
            }
            set
            {
                
            }
        }

        public string Id { get; set; }

        [XmlIgnore]
        public virtual bool NeedFileManipulation { get; set; }

        public virtual bool NeedsUpload { get; set; }

        [XmlIgnore]
        public virtual string UploadPath { get; set; }

        public virtual string FileName { get; set; }

        [XmlIgnore]
        public virtual string FileContent { get; set; }

        public virtual string DisplayString { get; set; }

        public virtual string ProjectItemSubType { get; set; }



        public abstract void CreateFromXml(string xml);
        public abstract string ReadToString();
        public abstract void UpdateFromXml(string xml);
        public abstract void DeleteItem();

        public event ProjectItemDeleted onProjectItemDeleted = delegate { };

        public virtual bool Equals(IProjectItem other)
        {
            return other.ProjectItemType == this.ProjectItemType && other.Id == this.Id;
        }

        protected void InvokeDeleteItem(IProjectItem item)
        {
            onProjectItemDeleted.Invoke(item);
        }
    }
}
