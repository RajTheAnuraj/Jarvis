using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LogicLayer.Payloads
{
    public class ProjectListPayload : IEnumerable<ProjectListItem>
    {
        List<ProjectListItem> projectListItems = new List<ProjectListItem>();



        public ProjectListItem this[int index]
        {
            get
            {
                if (index > projectListItems.Count - 1) return null;
                return projectListItems[index];
            }
        }

        public void Add(ProjectListItem projectListItem)
        {
            projectListItems.Add(projectListItem);
        }

        public void Remove(ProjectListItem projectListItem)
        {
            projectListItems.Remove(projectListItem);
        }

        public void Add(XmlNode projectListItem)
        {
            if (projectListItem != null)
            {
                var newItem = new ProjectListItem();

                if (projectListItem.SelectSingleNode("isRemoteProject") != null)
                    newItem.isRemoteProject = Convert.ToBoolean(projectListItem.SelectSingleNode("isRemoteProject").InnerText);

                if (projectListItem.SelectSingleNode("ProjectId") != null)
                    newItem.ProjectId = Convert.ToString(projectListItem.SelectSingleNode("ProjectId").InnerText);

                if (projectListItem.SelectSingleNode("ProjectRelativePath") != null)
                    newItem.ProjectRelativePath = Convert.ToString(projectListItem.SelectSingleNode("ProjectRelativePath").InnerText);

                if (projectListItem.SelectSingleNode("ProjectName") != null)
                    newItem.ProjectName = Convert.ToString(projectListItem.SelectSingleNode("ProjectName").InnerText);
                
                projectListItems.Add(newItem);
            }
        }


        public IEnumerator GetEnumerator()
        {
            foreach (var item in projectListItems)
            {
                yield return item;
            }
        }

        IEnumerator<ProjectListItem> IEnumerable<ProjectListItem>.GetEnumerator()
        {
            foreach (var item in projectListItems)
            {
                yield return item;
            }
        }

        public string ReadToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<Projects>");
            foreach (ProjectListItem item in this)
            {
                sb.Append("<Project>");
                sb.AppendFormat("<ProjectId>{0}</ProjectId>", item.ProjectId);
                sb.AppendFormat("<ProjectRelativePath>{0}</ProjectRelativePath>", item.ProjectRelativePath);
                sb.AppendFormat("<isRemoteProject>{0}</isRemoteProject>", item.isRemoteProject);
                sb.AppendFormat("<ProjectName>{0}</ProjectName>", item.ProjectName);
                sb.Append("</Project>");
            }
            sb.Append("</Projects>");
            return sb.ToString();
        }
    }


    public class ProjectListItem
    {
        public string ProjectId { get; set; }
        public string ProjectRelativePath { get; set; }
        public bool isRemoteProject { get; set; }
        public string ProjectName { get; set; }
    }
}
