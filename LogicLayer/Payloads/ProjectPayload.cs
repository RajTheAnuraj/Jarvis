using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicLayer.Interfaces;
using LogicLayer.Common;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace LogicLayer.Payloads
{
    public class ProjectPayload : IProjectItem
    {
        public ProjectPayload()
        {
            ProjectId = String.Format("Proj-{0}", new Guid());
            ProjectItems = new List<IProjectItem>();

            
        }

        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLogText { get; set; }
        public string ProjectSummaryText { get; set; }

        [XmlIgnore]
        public List<IProjectItem> ProjectItems { get; set; }


        public string ProjectItemType
        {
            get
            {
                return "Project";
            }
            set
            {
                
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
                
            }
        }

        public void CreateFromXml(string xml)
        {
            ProjectPayload Retval = XmlOperations.DeserializeFromXml<ProjectPayload>(xml);
            this.ProjectId = Retval.ProjectId;
            this.ProjectLogText = Retval.ProjectLogText;
            this.ProjectName = Retval.ProjectName;
            this.ProjectSummaryText = Retval.ProjectSummaryText;
        }

        public string ReadToString()
        {
            string retval = "";

            StringBuilder sb = new StringBuilder();

            sb.Append("<ProjectPayload>");

            sb.Append("<ProjectId>");
            sb.Append(ProjectId);
            sb.Append("</ProjectId>");

            sb.Append("<ProjectItemClassName>");
            sb.Append(ProjectItemClassName);
            sb.Append("</ProjectItemClassName>");


            sb.Append("<ProjectItemType>");
            sb.Append(ProjectItemType);
            sb.Append("</ProjectItemType>");

            sb.Append("<ProjectLogText>");
            sb.Append(ProjectLogText);
            sb.Append("</ProjectLogText>");

            sb.Append("<ProjectName>");
            sb.Append(ProjectName);
            sb.Append("</ProjectName>");

            sb.Append("<ProjectSummaryText>");
            sb.Append(ProjectSummaryText);
            sb.Append("</ProjectSummaryText>");

            sb.Append("<ProjectItems>");
            foreach (IProjectItem projectItem in ProjectItems)
            {
                sb.Append(projectItem.ReadToString());
            }

            sb.Append("</ProjectItems>");

            sb.Append("</ProjectPayload>");
            retval = FormatXml(sb.ToString());

            return retval;
        }

        public void UpdateFromXml(string xml)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem()
        {
            if (onProjectItemDeleted != null)
                onProjectItemDeleted.Invoke(this);
        }

        public event ProjectItemDeleted onProjectItemDeleted;


        string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }
    }
}
