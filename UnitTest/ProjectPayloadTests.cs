﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Payloads;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;

namespace UnitTest
{
    [TestClass]
    public class ProjectPayloadTests: ProjectItemTestsBaseClass
    {
        [TestMethod]
        public void TestInterfaceCollection()
        {
            ProjectPayload pload = new ProjectPayload();
            pload.ProjectItems.Add(new DocumentPayload {  ProjectItemSubType = "Screenshot" });
            pload.ProjectItems.Add(new CommunicationPayload { Id = "comin", ProjectItemSubType = "Email" });

            string str = pload.ReadToString();
        }

        [TestMethod]
        public void TestCreateFromXmlTest()
        {
            string Input = @"
            <ProjectPayload>
              <Id>Proj-00000000-0000-0000-0000-000000000000</Id>
              <ProjectItemClassName>LogicLayer.Payloads.ProjectPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
              <ProjectItemType>Project</ProjectItemType>
              <ProjectLogText>What a log</ProjectLogText>
              <ProjectName>Shakunthala</ProjectName>
              <ProjectSummaryText>Summary</ProjectSummaryText>
              <ProjectItems>
                <DocumentPayload>
                  <Id>ff95a455-7412-4fa9-b393-b9f8a4644e18</Id>
                  <DocumentDisplayString>Jingalala</DocumentDisplayString>
                  <DocumentPath>Doc path .asl</DocumentPath>
                  <DocumentType>Screenshot</DocumentType>
                  <ProjectItemType>Document</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                </DocumentPayload>
                <CommunicationPayload>
                  <Id>comin</Id>
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
              <Id>Proj-00000000-0000-0000-0000-000000000000</Id>
              <ProjectItemClassName>LogicLayer.Payloads.ProjectPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
              <ProjectItemType>Project Modified</ProjectItemType>
              <ProjectLogText>What a log Modified</ProjectLogText>
              <ProjectName>Shakunthala Modified</ProjectName>
              <ProjectSummaryText>Summary Modified</ProjectSummaryText>
              <ProjectItems>
                <DocumentPayload>
                  <Id>ff95a455-7412-4fa9-b393-b9f8a4644e18</Id>
                  <DocumentDisplayString>Jingalala Modified</DocumentDisplayString>
                  <DocumentPath>Doc path .asl Modified</DocumentPath>
                  <DocumentType>Screenshot Modified</DocumentType>
                  <ProjectItemType>Document</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                </DocumentPayload>
                <CommunicationPayload>
                  <Id>comin</Id>
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


        [TestMethod]
        public void ConstructorTest()
        {
            ProjectPayload pload = new ProjectPayload(@"C:\Temp\Projects\New project");
        }

        [TestMethod]
        public void DocumentAddTest()
        {
            ProjectPayload pload = new ProjectPayload(@"C:\Temp\NewProj");
            DocumentPayload dpload = new DocumentPayload();
            dpload.FileContent = "New DocumentContent";
            dpload.DisplayString = "Some stuff that I created";
            dpload.FileName = @"as.txt";
            dpload.ProjectItemSubType = "CodeSnippet";
            dpload.NeedsUpload = false;
            dpload.Id = Guid.NewGuid().ToString();

            AddProjectItemCommand addNewDoc = new AddProjectItemCommand(pload, dpload);
            addNewDoc.Execute();

            DocumentPayload dpload2 = new DocumentPayload();
            dpload2.FileContent = "New DocumentContent Modified Baba";
            dpload2.DisplayString = "Some stuff that I created";
            dpload2.FileName = @"as2.txt";
            dpload2.ProjectItemSubType = "CodeSnippet";
            dpload2.NeedsUpload = false;
            dpload2.Id = dpload.Id;

            AddProjectItemCommand addNewDoc2 = new AddProjectItemCommand(pload, dpload2);
            addNewDoc2.Execute();

        }

        [TestMethod]
        public void DocumentUploadTest()
        {
            ProjectPayload pload = new ProjectPayload(@"C:\Temp\NewProj");
            DocumentPayload dpload = new DocumentPayload();
            dpload.FileContent = "New DocumentContent";
            dpload.DisplayString = "Uploaded Doc";
            dpload.FileName = @"as.txt";
            dpload.ProjectItemSubType = "Screenshots";
            dpload.NeedsUpload = true;
            dpload.UploadPath = @"C:\Users\Anuraj\.nbi\registry.xml";
            dpload.Id = Guid.NewGuid().ToString();

            AddProjectItemCommand addNewDoc = new AddProjectItemCommand(pload, dpload);
            addNewDoc.Execute();

            
        }

        [TestMethod]
        public void DocumentReadTest()
        {
            ProjectPayload pload = new ProjectPayload(@"C:\Temp\NewProj");
            DocumentPayload dpload = new DocumentPayload();
            dpload.FileContent = "New DocumentContent";
            dpload.DisplayString = "Some stuff that I created";
            dpload.FileName = @"as.txt";
            dpload.ProjectItemSubType = "CodeSnippet";
            dpload.NeedsUpload = false;
            dpload.Id = Guid.NewGuid().ToString();

            AddProjectItemCommand addNewDoc = new AddProjectItemCommand(pload, dpload);
            addNewDoc.Execute();

            ReadContentToProjectItem readCommand = new ReadContentToProjectItem(pload, dpload);
            readCommand.Execute();
            var ss= dpload.FileContent;
        }

        [TestMethod]
        public void DocumentDeleteTest()
        {
            ProjectPayload pload = new ProjectPayload(@"C:\Temp\NewProj");
            DocumentPayload dpload = new DocumentPayload();
            dpload.FileContent = "New DocumentContent";
            dpload.DisplayString = "Some stuff that I created";
            dpload.FileName = @"as.txt";
            dpload.ProjectItemSubType = "CodeSnippet";
            dpload.NeedsUpload = false;
            dpload.Id = Guid.NewGuid().ToString();

            AddProjectItemCommand addNewDoc = new AddProjectItemCommand(pload, dpload);
            addNewDoc.Execute();

            DeleteProjectItemCommand deleteCommand = new DeleteProjectItemCommand(pload, dpload);
            deleteCommand.Execute();

            string str = pload.ReadToString();

            deleteCommand.Undo();
            str = pload.ReadToString();
        }
    }
}
