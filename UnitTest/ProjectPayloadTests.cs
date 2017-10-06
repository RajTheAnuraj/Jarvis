using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Payloads;
using LogicLayer.Interfaces;

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

        [TestMethod]
        public void TestCreateFromXmlTest()
        {
            string Input = @"
            <ProjectPayload>
              <ProjectId>Proj-00000000-0000-0000-0000-000000000000</ProjectId>
              <ProjectItemClassName>LogicLayer.Payloads.ProjectPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
              <ProjectItemType>Project</ProjectItemType>
              <ProjectLogText>What a log</ProjectLogText>
              <ProjectName>Shakunthala</ProjectName>
              <ProjectSummaryText>Summary</ProjectSummaryText>
              <ProjectItems>
                <DocumentPayload>
                  <DocumentId>ff95a455-7412-4fa9-b393-b9f8a4644e18</DocumentId>
                  <DocumentDisplayString>Jingalala</DocumentDisplayString>
                  <DocumentPath>Doc path .asl</DocumentPath>
                  <DocumentType>Screenshot</DocumentType>
                  <ProjectItemType>Document</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                </DocumentPayload>
                <CommunicationPayload>
                  <CommunicationId>comin</CommunicationId>
                  <CommunicationType>Email</CommunicationType>
                  <ProjectItemType>Communication</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                </CommunicationPayload>
              </ProjectItems>
            </ProjectPayload>";


            IProjectItem pload = new ProjectPayload();
            pload.CreateFromXml(Input);
            string str = pload.ReadToString();
            Assert.AreEqual(Helpers.rg.Replace(Input, ""), Helpers.rg.Replace(str, ""));

            Input = @"
            <ProjectPayload>
              <ProjectId>Proj-00000000-0000-0000-0000-000000000000</ProjectId>
              <ProjectItemClassName>LogicLayer.Payloads.ProjectPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
              <ProjectItemType>Project Modified</ProjectItemType>
              <ProjectLogText>What a log Modified</ProjectLogText>
              <ProjectName>Shakunthala Modified</ProjectName>
              <ProjectSummaryText>Summary Modified</ProjectSummaryText>
              <ProjectItems>
                <DocumentPayload>
                  <DocumentId>ff95a455-7412-4fa9-b393-b9f8a4644e18</DocumentId>
                  <DocumentDisplayString>Jingalala Modified</DocumentDisplayString>
                  <DocumentPath>Doc path .asl Modified</DocumentPath>
                  <DocumentType>Screenshot Modified</DocumentType>
                  <ProjectItemType>Document</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                </DocumentPayload>
                <CommunicationPayload>
                  <CommunicationId>comin</CommunicationId>
                  <CommunicationType>Email</CommunicationType>
                  <ProjectItemType>Communication</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                </CommunicationPayload>
              </ProjectItems>
            </ProjectPayload>";
            pload.UpdateFromXml(Input);

            Input=@"
            <DocumentPayload>
                <DocumentDisplayString>TestNameUpdated</DocumentDisplayString>
                <DocumentPath>Clamper.docx</DocumentPath>
            </DocumentPayload>
            ";
            IProjectItem nPload = ((ProjectPayload)pload).ProjectItems[0];
            nPload.UpdateFromXml(Input);

            ((ProjectPayload)pload).ProjectItems[0].DeleteItem();
        }
    }
}
