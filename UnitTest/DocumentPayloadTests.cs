using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Payloads;

namespace UnitTest
{
    [TestClass]
    public class DocumentPayloadTests:ProjectItemTestsBaseClass
    {
        [TestMethod]
        public void IXmlLoadableTest()
        {
            string Input = @"
            <DocumentPayload>
                <DocumentId>TestId</DocumentId>
                <DocumentDisplayString>TestName</DocumentDisplayString>
                <DocumentPath>Clamper.doc</DocumentPath>
                <DocumentType>CodeSnippet</DocumentType>
                <ProjectItemType>Document</ProjectItemType>
                <ProjectItemClassName>LogicLayer.Payloads.DocumentPayload, LogicLayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</ProjectItemClassName>
            </DocumentPayload>
            ";
            IProjectItem pload = new DocumentPayload();
            Project.Add(pload);
            pload.onProjectItemDeleted += pload_onProjectItemDeleted;
            pload.CreateFromXml(Input);
            string str = pload.ReadToString();
            Assert.AreEqual(Helpers.rg.Replace(Input, ""), Helpers.rg.Replace(str, ""));

            string UpdateInput = @"
            <DocumentPayload>
                <DocumentDisplayString>TestNameUpdated</DocumentDisplayString>
                <DocumentPath>Clamper.docx</DocumentPath>
            </DocumentPayload>
            ";

            pload.UpdateFromXml(UpdateInput);

            DocumentPayload dPload = (DocumentPayload)pload;
            Assert.AreEqual("TestNameUpdated", dPload.DocumentDisplayString);
            Assert.AreEqual("Clamper.docx", dPload.DocumentPath);

            int currentItemCount = Project.Count;
            pload.DeleteItem();
            Assert.AreEqual(currentItemCount - 1, Project.Count);
        }
    }
}
