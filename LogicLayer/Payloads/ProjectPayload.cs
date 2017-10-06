﻿using System;
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
            CreateProjectItems(xml, this);
        }

        private void CreateProjectItems(string xml, ProjectPayload projectPayload)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);
            var rootNode = xdoc.DocumentElement;
            var projectItems = rootNode.SelectSingleNode("ProjectItems");
            foreach (XmlNode xn in projectItems.ChildNodes)
            {
                if (xn.NodeType == XmlNodeType.Element)
                {
                    string docXml = xn.OuterXml;
                    var ndtypeOfProjectItem = xn.SelectSingleNode("ProjectItemClassName");
                    if (ndtypeOfProjectItem == null) continue;
                    string typeOfProjectItem = ndtypeOfProjectItem.InnerText;
                    IProjectItem PI = Activator.CreateInstance(Type.GetType(typeOfProjectItem)) as IProjectItem;
                    PI.CreateFromXml(docXml);
                    PI.onProjectItemDeleted += PI_onProjectItemDeleted;
                    projectPayload.ProjectItems.Add(PI);
                }

            }
        }

        void PI_onProjectItemDeleted(IProjectItem item)
        {
            this.ProjectItems.Remove(item);
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
            ProjectPayload Retval = XmlOperations.DeserializeFromXml<ProjectPayload>(xml);
            this.ProjectLogText = Retval.ProjectLogText;
            this.ProjectName = Retval.ProjectName;
            this.ProjectSummaryText = Retval.ProjectSummaryText;
            this.ProjectItems.Clear();
            CreateProjectItems(xml, this);
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
