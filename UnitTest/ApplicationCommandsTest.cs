using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;
using LogicLayer.Payloads;

namespace UnitTest
{
    [TestClass]
    public class ApplicationCommandsTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            IUndoableCommand appInit = new ApplicationInitializeCommand("");
            appInit.Execute();

            ProjectListPayload plist = new ProjectListPayload();
            ProjectPayloadProvider pp = new ProjectPayloadProvider();
            ICustomCommand projectlistCommand = pp.GetRetrieveProjectListCommand(ref plist);
            projectlistCommand.Execute();
            if (plist != null)
            {
                foreach (ProjectListItem item in plist)
                {

                }
            }

            appInit.Undo();
        }
    }
}
