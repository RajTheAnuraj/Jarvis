using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LogicLayer.Payloads
{
    public class ApplicationContextMenuPayload
    {
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public bool isAction { get; set; }
        public bool isCategory { get; set; }
        public string Format { get; set; }
        public List<ApplicationContextMenuPayload> innerList { get; set; }
        
        [XmlIgnore]
        public string ActionStringText { get; set; }

        string _ActionString;
        public string ActionString
        {
            get
            {
                return _ActionString;
            }
            set
            {
                _ActionString = value;
                SetTextIfNeeded();
            }
        }

        private void SetTextIfNeeded()
        {
            if (this.isAction == true || this.Format != "Rich Text Format")
            {
                if(!String.IsNullOrWhiteSpace(ActionString))
                    if (ActionString.StartsWith(@"{\rtf"))
                    {
                        System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
                        rtb.Rtf = ActionString;
                        ActionStringText = rtb.Text;
                    }
                    else
                    {
                        ActionStringText = ActionString;
                    }
            }
            else
            {
                ActionStringText = ActionString;
            }
        }

        
    }

   
}
