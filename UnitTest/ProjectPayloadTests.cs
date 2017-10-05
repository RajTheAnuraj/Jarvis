using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Payloads;

namespace UnitTest
{
    [TestClass]
    public class ProjectPayloadTests: ProjectItemTestsBaseClass
    {
        [TestMethod]
        public void TestInterfaceCollection()
        {
            ProjectPayload pload = new ProjectPayload();
            pload.ProjectItems.Add(new DocumentPayload {  DocumentType = "Screenshot" });
            pload.ProjectItems.Add(new CommunicationPayload { CommunicationId = "comin", CommunicationType = "Email" });

            string str = pload.ReadToString();
        }
    }
}
