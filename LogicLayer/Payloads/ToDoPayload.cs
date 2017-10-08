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
    public class ToDoPayload : PayloadBase
    {

        [XmlElement(DataType = "date")]
        public DateTime DueDate { get; set; }
    
        public override string ProjectItemType
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

       

        public override void CreateFromXml(string xml)
        {
            ToDoPayload Retval = XmlOperations.DeserializeFromXml<ToDoPayload>(xml);
            base.LoadFromXml(Retval);
            this.DueDate = Retval.DueDate;
        }

        public override string ReadToString()
        {
            return XmlOperations.SerializeToXml<ToDoPayload>(this);
        }

        public override void UpdateFromXml(string xml)
        {
            ToDoPayload Retval = XmlOperations.DeserializeFromXml<ToDoPayload>(xml);
            base.LoadFromXml(Retval);
            this.DueDate = Retval.DueDate;
        }

        public override void DeleteItem()
        {
            base.InvokeDeleteItem(this);
        }

        
    }
}
