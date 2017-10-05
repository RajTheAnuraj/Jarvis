using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;

namespace UnitTest
{
    [TestClass]
    public class CommunicationPayloadTests: ProjectItemTestsBaseClass
    {
        [TestMethod]
        public void IXmlLoadableTest()
        {
            string Input = @"
            <CommunicationPayload>
                <CommunicationId>TestId</CommunicationId>
                <CommunicationDisplayName>TestName</CommunicationDisplayName>
                <CommunicationType>Clamper.doc</CommunicationType>
                <CommunicationDetails>CodeSnippet</CommunicationDetails>
                <CommunicationPath>CodeSnippet</CommunicationPath>
                <ProjectItemType>Communication</ProjectItemType>
                <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
            </CommunicationPayload>
            ";
            IProjectItem pload = new CommunicationPayload();
            Project.Add(pload);
            pload.onProjectItemDeleted += pload_onProjectItemDeleted;
            pload.CreateFromXml(Input);
            string str = pload.ReadToString();
            Assert.AreEqual(Helpers.rg.Replace(Input, ""), Helpers.rg.Replace(str, ""));

            string UpdateInput = @"
            <CommunicationPayload>
                <CommunicationDisplayName>TestNameUpdated</CommunicationDisplayName>
                <CommunicationDetails>Chaged Details</CommunicationDetails>
                <CommunicationPath>Some Path at local</CommunicationPath>
            </CommunicationPayload>
            ";

            pload.UpdateFromXml(UpdateInput);

            CommunicationPayload dPload = (CommunicationPayload)pload;
            Assert.AreEqual("TestNameUpdated", dPload.CommunicationDisplayName);
            Assert.AreEqual("Chaged Details", dPload.CommunicationDetails);
            Assert.AreEqual("Some Path at local", dPload.CommunicationPath);

            int currentItemCount = Project.Count;
            pload.DeleteItem();
            Assert.AreEqual(currentItemCount - 1, Project.Count);
        }
    }
}
