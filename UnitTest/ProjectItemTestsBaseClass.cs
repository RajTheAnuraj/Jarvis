using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest
{
    public class ProjectItemTestsBaseClass
    {
        public List<IProjectItem> Project = new List<IProjectItem>();

        #region Testing Harness
        public void pload_onProjectItemDeleted(IProjectItem item)
        {
            Project.Remove(item);
            item = null;
        }
        #endregion
    }
}
