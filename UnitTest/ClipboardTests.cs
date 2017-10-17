using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;
using LogicLayer.Payloads;
using LogicLayer.Factories;

namespace UnitTest
{
    [TestClass]
    public class ClipboardTests
    {
        [TestMethod]
        public void CanCreateFromClipboardCommandTest()
        {
            ICustomCommand<bool> command = new CanCreateFromClipboardCommand();
            var result = command.Execute();
        }

        [TestMethod]
        public void CreateFromClipboardTest()
        {
            IResourceProvider ResourceProvider = ProviderFactory.GetCurrentProvider();
            ResourceProvider.setRootFolder(@"C:\Temp");

            ProjectPayload PPload = new ProjectPayload("Raj");

            IUndoableCommand projectInitialize = ResourceProvider.GetProjectInitializeCommand(PPload);
            projectInitialize.Execute();

            IUndoableCommand command = new CreateFromClipboardCommand(PPload);
            command.Execute();

            IUndoableCommand saveProjCommand = ResourceProvider.GetProjectSaveCommand(PPload);
            saveProjCommand.Execute();
        }
    }
}
