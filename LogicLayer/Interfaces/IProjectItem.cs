using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer.Interfaces
{
    public delegate void ProjectItemDeleted(IProjectItem item);

    public interface IProjectItem: IEquatable<IProjectItem>
    {
        string ProjectItemType { get; set; }
        string ProjectItemClassName { get; set; }
        string Id { get; set; }


        string GetProcessArgument();

        void CreateFromXml(string xml);
        string ReadToString();
        void UpdateFromXml(string xml);
        void DeleteItem();

        event ProjectItemDeleted onProjectItemDeleted;
    }
}
