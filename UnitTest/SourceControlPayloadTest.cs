using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;

namespace UnitTest
{
    [TestClass]
    public class SourceControlPayloadTest:ProjectItemTestsBaseClass
    {
        [TestMethod]
        public void IXmlLoadableTest()
        {
            string Input = @"
            <SourceControlPayload>
                <SourceControlId>TestId</SourceControlId>
                <SourceControlType>TestName</SourceControlType>
                <SourceControlDetail>447</SourceControlDetail>
                <SourceControlDateTime>2017-11-20T00:00:00</SourceControlDateTime>
                <ProjectItemType>Source Control</ProjectItemType>
                <ProjectItemClassName>LogicLayer.Payloads.SourceControlPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
            </SourceControlPayload>
            ";
            IProjectItem pload = new SourceControlPayload();
            Project.Add(pload);
            pload.onProjectItemDeleted += pload_onProjectItemDeleted;
            pload.CreateFromXml(Input);
            string str = pload.ReadToString();
            Assert.AreEqual(Helpers.rg.Replace(Input, ""), Helpers.rg.Replace(str, ""));

            string UpdateInput = @"
            <SourceControlPayload>
                <SourceControlType>TestNameUpdated</SourceControlType>
                <SourceControlDetail>76356</SourceControlDetail>
                <SourceControlDateTime>2017-11-21T00:00:00</SourceControlDateTime>
            </SourceControlPayload>
            ";

            string ExpectedOutputAfterUpdate = @"
            <SourceControlPayload>
                <SourceControlId>TestId</SourceControlId>
                <SourceControlType>TestNameUpdated</SourceControlType>
                <SourceControlDetail>76356</SourceControlDetail>
                <SourceControlDateTime>2017-11-21T00:00:00</SourceControlDateTime>
                <ProjectItemType>Source Control</ProjectItemType>
                <ProjectItemClassName>LogicLayer.Payloads.SourceControlPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
            </SourceControlPayload>
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
