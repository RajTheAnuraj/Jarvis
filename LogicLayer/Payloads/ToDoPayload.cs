using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using LogicLayer.Common;

namespace LogicLayer.Payloads
{
    public class ToDoPayload : IProjectItem
    {
        public string ToDoId { get; set; }
        public string ToDoDisplayName { get; set; }

        [XmlElement(DataType = "date")]
        public DateTime DueDate { get; set; }
    
        public string ProjectItemType
        {
            get
            {
                return "ToDo";
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
            ToDoPayload Retval = XmlOperations.DeserializeFromXml<ToDoPayload>(xml);
            this.ToDoId = Retval.ToDoId;
            this.ToDoDisplayName = Retval.ToDoDisplayName;
            this.DueDate = Retval.DueDate;
        }

        public string ReadToString()
        {
            return XmlOperations.SerializeToXml<ToDoPayload>(this);
        }

        public void UpdateFromXml(string xml)
        {
            ToDoPayload Retval = XmlOperations.DeserializeFromXml<ToDoPayload>(xml);
            this.ToDoDisplayName = Retval.ToDoDisplayName;
            this.DueDate = Retval.DueDate;
        }

        public void DeleteItem()
        {
            if (onProjectItemDeleted != null)
                onProjectItemDeleted.Invoke(this);
        }

        public event ProjectItemDeleted onProjectItemDeleted;
    }
}
