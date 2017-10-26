using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LogicLayer.Payloads
{
    public class ApplicationContextMenuPayload
    {
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public bool isAction { get; set; }
        public bool isCategory { get; set; }
        public string ActionString { get; set; }
        public string Format { get; set; }
        public List<ApplicationContextMenuPayload> innerList { get; set; }

        public string GetXml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<ApplicationContextMenuPayload>");
            sb.AppendFormat("<Category>{0}</Category>", this.Category);
            sb.AppendFormat("<DisplayName>{0}</DisplayName>", this.DisplayName);
            sb.AppendFormat("<isAction>{0}</isAction>", XmlConvert.ToString(this.isAction));
            sb.AppendFormat("<isCategory>{0}</isCategory>", XmlConvert.ToString(this.isCategory));
            sb.AppendFormat("<ActionString>{0}</ActionString>", this.ActionString);
            sb.AppendFormat("<Format>{0}</Format>", this.Format);
            if (innerList != null)
            {
                sb.Append("<innerList>");
                sb.Append("<ArrayOfApplicationContextMenuPayload>");
                foreach (var item in innerList)
                {
                    sb.Append(item.GetXml());
                }
                sb.Append("</ArrayOfApplicationContextMenuPayload>");
                sb.Append("</innerList>");
            }
            sb.Append("</ApplicationContextMenuPayload>");

            return sb.ToString();
        }
    }

   
}
