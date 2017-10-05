using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;

namespace UnitTest
{
    [TestClass]
    public class ChecklistPayloadTests:ProjectItemTestsBaseClass
    {
        [TestMethod]
        public void IXmlLoadableTest()
        {

            string Input = @"
            <ChecklistPayload>
                <ChecklistId>TestId</ChecklistId>
                <ChecklistDisplayString>TestName</ChecklistDisplayString>
                <ChecklistAlertDate>2017-11-20</ChecklistAlertDate>
                <ChecklistPhase>CodeSnippet</ChecklistPhase>
                <ProjectItemType>Checklist</ProjectItemType>
                <ProjectItemClassName>LogicLayer.Payloads.ChecklistPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
            </ChecklistPayload>
            ";
            IProjectItem pload = new ChecklistPayload();
            Project.Add(pload);
            pload.onProjectItemDeleted += pload_onProjectItemDeleted;
            pload.CreateFromXml(Input);
            string str = pload.ReadToString();
            Assert.AreEqual(Helpers.rg.Replace(Input, ""), Helpers.rg.Replace(str, ""));

            string UpdateInput = @"
            <ChecklistPayload>
                <ChecklistDisplayString>TestNameUpdated</ChecklistDisplayString>
                <ChecklistAlertDate>2017-11-21</ChecklistAlertDate>
                <ChecklistPhase>Development</ChecklistPhase>
            </ChecklistPayload>
            ";

            pload.UpdateFromXml(UpdateInput);

            ChecklistPayload dPload = (ChecklistPayload)pload;
            Assert.AreEqual("TestNameUpdated", dPload.ChecklistDisplayString);
            Assert.AreEqual("11/21/2017", dPload.ChecklistAlertDate.ToString("MM/dd/yyyy"));
            Assert.AreEqual("Development", dPload.ChecklistPhase);

            int currentItemCount = Project.Count;
            pload.DeleteItem();
            Assert.AreEqual(currentItemCount - 1, Project.Count);
        }
    }
}
