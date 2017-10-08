using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Payloads;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;
using System.Collections;
using System.Collections.Generic;
using LogicLayer.Factories;

namespace UnitTest
{
    [TestClass]
    public class ProjectPayloadTests: ProjectItemTestsBaseClass
    {
        [TestMethod]
        public void TestInterfaceCollection()
        {
            ProjectPayload pload = new ProjectPayload("Raj");
            pload.AddProjectItem(new DocumentPayload {  ProjectItemSubType = "Screenshot" });
            pload.AddProjectItem(new CommunicationPayload { Id = "comin", ProjectItemSubType = "Email" });

            string str = pload.ReadToString();
        }

        [TestMethod]
        public void TestCreateFromXmlTest()
        {
            string Input = @"
            <ProjectPayload>
              <Id>c05f7a07-8d4d-4507-af2d-f44abf4dfb5b</Id>
              <ProjectItemClassName>LogicLayer.Payloads.ProjectPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
              <ProjectItemType>Project</ProjectItemType>
              <ProjectLogText>Projects Log Text</ProjectLogText>
              <ProjectName>Anuraj</ProjectName>
              <ProjectSummaryText> Goes live in November</ProjectSummaryText>
              <ProjectItems>
                <DocumentPayload>
                  <ProjectItemType>Document</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>adc90465-1c96-4131-b784-5410d435a3d7</Id>
                  <NeedsUpload>false</NeedsUpload>
                  <FileName>ADD.txt</FileName>
                  <DisplayString>Application Design Document</DisplayString>
                  <ProjectItemSubType>Document</ProjectItemSubType>
                </DocumentPayload>
                <CommunicationPayload>
                  <ProjectItemType>Communication</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>dd2dd169-351d-4af8-88c9-7281f780890f</Id>
                  <NeedsUpload>false</NeedsUpload>
                  <FileName>MagaChat.txt</FileName>
                  <DisplayString>Chat with Manager</DisplayString>
                  <ProjectItemSubType>Chat</ProjectItemSubType>
                </CommunicationPayload>
              </ProjectItems>
            </ProjectPayload>";


            IProjectItem pload = new ProjectPayload("Anuraj");
            pload.CreateFromXml(Input);
            string str = pload.ReadToString();
            Assert.AreEqual(Helpers.rg.Replace(Input, ""), Helpers.rg.Replace(str, ""));

            Input = @"
            <ProjectPayload>
              <Id>c05f7a07-8d4d-4507-af2d-f44abf4dfb5b</Id>
              <ProjectItemClassName>LogicLayer.Payloads.ProjectPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
              <ProjectItemType>Project</ProjectItemType>
              <ProjectLogText>Projects Log Text</ProjectLogText>
              <ProjectName>Anuraj</ProjectName>
              <ProjectSummaryText> Goes live in November</ProjectSummaryText>
              <ProjectItems>
                <DocumentPayload>
                  <ProjectItemType>Document</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>adc90465-1c96-4131-b784-5410d435a3d7</Id>
                  <NeedsUpload>false</NeedsUpload>
                  <FileName>ADD.txt</FileName>
                  <DisplayString>Application Design Document</DisplayString>
                  <ProjectItemSubType>Document</ProjectItemSubType>
                </DocumentPayload>
                <CommunicationPayload>
                  <ProjectItemType>Communication</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>dd2dd169-351d-4af8-88c9-7281f780890f</Id>
                  <NeedsUpload>false</NeedsUpload>
                  <FileName>MagaChat.txt</FileName>
                  <DisplayString>Chat with Manager</DisplayString>
                  <ProjectItemSubType>Chat</ProjectItemSubType>
                </CommunicationPayload>
                <CommunicationPayload>
                  <ProjectItemType>Communication</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>dd2dd169-351d-4af8-88c9-7281f780890a</Id>
                  <NeedsUpload>true</NeedsUpload>
                  <FileName>Em.msg</FileName>
                  <DisplayString>Email to Jose</DisplayString>
                  <ProjectItemSubType>Email</ProjectItemSubType>
                </CommunicationPayload>
              </ProjectItems>
            </ProjectPayload>";
            pload.UpdateFromXml(Input);

            int cnt = ((ProjectPayload)pload).ProjectItemCount;
            Assert.AreEqual(3, cnt);
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

        [TestMethod]
        public void ProjectInitializeTest()
        {
            ProjectPayload pload = new ProjectPayload("Anuraj");

            ProjectInitializeCommand com = new ProjectInitializeCommand(pload);
            com.Execute();

            PayloadBase pb = ((PayloadBase)(pload.GetProjectItem(0)));
            ICommand bb = new ReadContentToProjectItem(pload, pb);
            bb.Execute();


            //com.Undo();
        }

        [TestMethod]
        public void ProjectSaveTest()
        {
            ProjectPayload pload = new ProjectPayload("Raj");

            ProjectInitializeCommand com = new ProjectInitializeCommand(pload);
            com.Execute();


            string Input = @"
            <ProjectPayload>
              <Id>c05f7a07-8d4d-4507-af2d-f44abf4dfb5b</Id>
              <ProjectItemClassName>LogicLayer.Payloads.ProjectPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
              <ProjectItemType>Project</ProjectItemType>
              <ProjectLogText>Projects Log Text</ProjectLogText>
              <ProjectName>Anuraj</ProjectName>
              <ProjectSummaryText> Goes live in November</ProjectSummaryText>
              <ProjectItems>
                <DocumentPayload>
                  <ProjectItemType>Document</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>adc90465-1c96-4131-b784-5410d435a3d7</Id>
                  <NeedsUpload>false</NeedsUpload>
                  <FileName>ADD.txt</FileName>
                  <DisplayString>Application Design Document</DisplayString>
                  <ProjectItemSubType>Document</ProjectItemSubType>
                </DocumentPayload>
                <CommunicationPayload>
                  <ProjectItemType>Communication</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>dd2dd169-351d-4af8-88c9-7281f780890f</Id>
                  <NeedsUpload>false</NeedsUpload>
                  <FileName>MagaChat.txt</FileName>
                  <DisplayString>Chat with Manager</DisplayString>
                  <ProjectItemSubType>Chat</ProjectItemSubType>
                </CommunicationPayload>
                <CommunicationPayload>
                  <ProjectItemType>Communication</ProjectItemType>
                  <ProjectItemClassName>LogicLayer.Payloads.CommunicationPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
                  <Id>dd2dd169-351d-4af8-88c9-7281f780890a</Id>
                  <NeedsUpload>true</NeedsUpload>
                  <FileName>Em.msg</FileName>
                  <DisplayString>Email to Jose</DisplayString>
                  <ProjectItemSubType>Email</ProjectItemSubType>
                </CommunicationPayload>
              </ProjectItems>
            </ProjectPayload>";
            pload.UpdateFromXml(Input);


            ProjectSaveCommand save = new ProjectSaveCommand(pload);
            save.Execute();


            save.Undo();

            com.Undo();
        }


        [TestMethod]
        public void ModifyProjectItemTest()
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

            ModifyProjectItemCommand modDoc = new ModifyProjectItemCommand(pload, dpload, "FileContent", "Doctor Deena");
            modDoc.Execute();

            ModifyProjectItemCommand modDoc1 = new ModifyProjectItemCommand(pload, dpload, "FileContent", "Doctor Deena Modified the plan");
            modDoc1.Execute();

            modDoc1.Undo();
            modDoc.Undo();
        }


        [TestMethod]
        public void FullConsoleTest()
        {
            Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();
            IProjectPayloadProvider CommandProvider = ProviderFactory.GetCurrentProvider();

            try
            {
                ProjectPayload PPload = new ProjectPayload("Manjari");
                
                IUndoableCommand projectInitialize = CommandProvider.GetProjectInitializeCommand(PPload);
                History.Push(projectInitialize);
                projectInitialize.Execute();

                DocumentPayload Dpayload = new DocumentPayload();
                Dpayload.DisplayString = "Application Config";
                Dpayload.NeedsUpload = true;
                Dpayload.UploadPath = @"C:\Users\Anuraj\Documents\IISExpress\config\applicationhost.config";
                Dpayload.ProjectItemSubType = "File";
                IUndoableCommand addDocCommand = CommandProvider.GetAddProjectItemCommand(PPload, Dpayload);
                History.Push(addDocCommand);
                addDocCommand.Execute();

                DocumentPayload DplCodeSnippet = new DocumentPayload();
                DplCodeSnippet.DisplayString = "Queries For Rebates";
                DplCodeSnippet.NeedsUpload = false;
                DplCodeSnippet.FileContent = "Select * from table";
                DplCodeSnippet.ProjectItemSubType = "Code Snippet";
                DplCodeSnippet.FileName = "Queries For Rebates.txt";
                IUndoableCommand addDocSnippetCommand = CommandProvider.GetAddProjectItemCommand(PPload, DplCodeSnippet);
                History.Push(addDocSnippetCommand);
                addDocSnippetCommand.Execute();

                CommunicationPayload cpl = new CommunicationPayload();
                cpl.DisplayString = "Email With Peopls";
                cpl.FileContent = "Hi How are you";
                cpl.FileName = "EmailWithPeeps.txt";
                cpl.NeedsUpload = false;
                cpl.ProjectItemSubType = "Email";
                IUndoableCommand addComCommand = CommandProvider.GetAddProjectItemCommand(PPload, cpl);
                History.Push(addComCommand);
                addComCommand.Execute();

                ToDoPayload tdpl = new ToDoPayload();
                tdpl.DisplayString = "Get the stuff done man";
                tdpl.DueDate = DateTime.Now;
                IUndoableCommand addToDoCommand = CommandProvider.GetAddProjectItemCommand(PPload, tdpl);
                History.Push(addToDoCommand);
                addToDoCommand.Execute();

                SourceControlPayload scpl = new SourceControlPayload();
                scpl.DisplayString = "12345";
                scpl.ProjectItemSubType = "Changeset";
                scpl.SourceControlDateTime = DateTime.Now;
                scpl.SourceControlDetail = "Assembly control";
                IUndoableCommand addSCCommand = CommandProvider.GetAddProjectItemCommand(PPload, scpl);
                History.Push(addSCCommand);
                addSCCommand.Execute();

                IUndoableCommand saveProjCommand = CommandProvider.GetProjectSaveCommand(PPload);
                History.Push(saveProjCommand);
                saveProjCommand.Execute();

                while (true)
                {
                    if (History.Count == 0) break;
                    History.Pop().Undo();
                }
            }
            catch (Exception)
            {
                while (true)
                {
                    if (History.Count == 0) break;
                    History.Pop().Undo();
                }
                throw;
            }


        }

        [TestMethod]
        public void ProjectInitializeFullTest()
        {
            Stack<IUndoableCommand> History = new Stack<IUndoableCommand>();
            IProjectPayloadProvider CommandProvider = ProviderFactory.GetCurrentProvider();

            ProjectPayload pl = new ProjectPayload("Manjari");
            IUndoableCommand initProjCommand = CommandProvider.GetProjectInitializeCommand(pl);
            History.Push(initProjCommand);
            initProjCommand.Execute();
        }


    }
}
