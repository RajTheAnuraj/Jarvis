using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class ProjectToDoTest:ProjectItemTestsBaseClass
    {
        

        [TestMethod]
        public void LoadFromXmlTest()
        {
            string Input = @"
            <ToDoPayload>
                <ToDoId>TestId</ToDoId>
                <ToDoDisplayName>TestName</ToDoDisplayName>
                <DueDate>2017-11-20</DueDate>
                <ProjectItemType>ToDo</ProjectItemType>
                <ProjectItemClassName>LogicLayer.Payloads.ToDoPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
            </ToDoPayload>
            ";
            
            IProjectItem pload = new ToDoPayload();
            Project.Add(pload);
            pload.onProjectItemDeleted += pload_onProjectItemDeleted;
            pload.CreateFromXml(Input);
            string str = pload.ReadToString();
            Assert.AreEqual(Helpers.rg.Replace(Input, ""), Helpers.rg.Replace(str, ""));

            string UpdateInput = @"
            <ToDoPayload>
                <ToDoDisplayName>TestNameUpdated</ToDoDisplayName>
                <DueDate>2017-11-21</DueDate>
            </ToDoPayload>
            ";

            string ExpectedOutputAfterUpdate = @"
            <ToDoPayload>
                <ToDoId>TestId</ToDoId>
                <ToDoDisplayName>TestNameUpdated</ToDoDisplayName>
                <DueDate>2017-11-21</DueDate>
                <ProjectItemType>ToDo</ProjectItemType>
                <ProjectItemClassName>LogicLayer.Payloads.ToDoPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
            </ToDoPayload>
            ";

            pload.UpdateFromXml(UpdateInput);
            str = pload.ReadToString();

            Assert.AreEqual(Helpers.rg.Replace(ExpectedOutputAfterUpdate, ""), Helpers.rg.Replace(str, ""));

            int currentItemCount = Project.Count;
            pload.DeleteItem();
            Assert.AreEqual(currentItemCount - 1, Project.Count);
        }



       
    }
}
