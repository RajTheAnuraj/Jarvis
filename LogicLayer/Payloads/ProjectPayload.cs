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
using LogicLayer.Implementations;
using LogicLayer.Factories;

namespace LogicLayer.Payloads
{
    public class ProjectPayload : IProjectItem
    {
        string _ProjectFolder;

        private ProjectPayload()
        {
            Id = Guid.NewGuid().ToString();
            ProjectItems = new List<IProjectItem>();
            CommandProvider = ProviderFactory.GetCurrentProvider();
        }

        public ProjectPayload(string ProjectName)
            : this()
        {
            this.ProjectName = ProjectName;
        }

        Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();
        IProjectPayloadProvider CommandProvider = null;

        public string Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectLogText { get; set; }
        public string ProjectSummaryText { get; set; }

        public string ProjectFolder
        {
            get
            {
                return _ProjectFolder;
            }
            set
            {
                _ProjectFolder = value;
            }
        }

        [XmlIgnore]
        List<IProjectItem> ProjectItems = new List<IProjectItem>();

        [XmlIgnore]
        public int ProjectItemCount
        {
            get { return ProjectItems.Count; }
        }

        public IProjectItem GetProjectItem(int Index)
        {
            return ProjectItems[Index];
        }

        public IProjectItem GetProjectItem(string Id)
        {
            foreach (IProjectItem item in ProjectItems)
            {
                if (item.Id == Id)
                    return item;
            }
            return null;
        }

        public IProjectItem GetProjectItem(string ProjectItemType, string Id)
        {
            foreach (IProjectItem item in ProjectItems)
            {
                if (item.Id == Id && item.ProjectItemType == ProjectItemType)
                    return item;
            }
            return null;
        }

        public IEnumerable<IProjectItem> GetProjectItems(string Id)
        {
            foreach (IProjectItem item in ProjectItems)
            {
                if (item.Id == Id)
                    yield return item;
            }
        }

        public IEnumerable<IProjectItem> GetProjectItems()
        {
            foreach (IProjectItem item in ProjectItems)
            {
                yield return item;
            }
        }

        public IEnumerable<IProjectItem> GetProjectItemsByType(string ProjectItemType)
        {
            foreach (IProjectItem item in ProjectItems)
            {
                if (item.ProjectItemType == ProjectItemType)
                    yield return item;
            }
        }

        public string TrashFolder
        {
            get
            {
                return _ProjectFolder + "\\Trash";
            }
        }

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
            this.Id = Retval.Id;
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
                    PayloadBase PI = Activator.CreateInstance(Type.GetType(typeOfProjectItem)) as PayloadBase;
                    PI.CreateFromXml(docXml);

                    IUndoableCommand addProjectItemCommand = CommandProvider.GetAddProjectItemCommand(this, PI);
                    History.Push(addProjectItemCommand);
                    addProjectItemCommand.Execute();
                    
                }

            }
        }

        public void AddProjectItem(IProjectItem item)
        {
            item.onProjectItemDeleted += PI_onProjectItemDeleted;
            this.ProjectItems.Add(item);
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

            sb.Append("<Id>");
            sb.Append(Id);
            sb.Append("</Id>");

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

        public bool Equals(IProjectItem other)
        {
            return other.ProjectItemType == this.ProjectItemType && other.Id == this.Id;
        }
    }
}
