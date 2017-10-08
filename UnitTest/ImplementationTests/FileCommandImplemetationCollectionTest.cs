using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicLayer.Interfaces;
using LogicLayer.Implementations;

namespace UnitTest.ImplementationTests
{
    /// <summary>
    /// Summary description for FileCommandImplemetationCollectionTest
    /// </summary>
    [TestClass]
    public class FileCommandImplemetationCollectionTest
    {
        public Stack<IUndoableCommand> CommandHistory = new Stack<IUndoableCommand>();

        [TestMethod]
        public void Test1()
        {
            FileCreateCommand createCommand = new FileCreateCommand(@"C:\Temp\Dishum.txt", true);
            CommandHistory.Push(createCommand);
            createCommand.Execute();

            FileModifyContentCommand modifyCommand = new FileModifyContentCommand(@"C:\Temp\Dishum.txt", "New Content");
            CommandHistory.Push(modifyCommand);
            modifyCommand.Execute();
            

            FileMoveCommand moveCommand = new FileMoveCommand(@"C:\Temp\Dishum.txt", @"C:\Temp\Dishum1.txt");
            CommandHistory.Push(moveCommand);
            moveCommand.Execute();

            FileModifyContentCommand modifyCommand2 = new FileModifyContentCommand(@"C:\Temp\Dishum1.txt", "Changing the content");
            CommandHistory.Push(modifyCommand2);
            modifyCommand2.Execute();

            FileDeleteWithHistoryCommand deleteCommand = new FileDeleteWithHistoryCommand(@"C:\Temp\Dishum1.txt", @"C:\Temp\Trashed");
            CommandHistory.Push(deleteCommand);
            deleteCommand.Execute();

            while (true)
            {
                if (CommandHistory.Count == 0) break;
                IUndoableCommand command = CommandHistory.Pop();
                command.Undo();
            }
        }


        [TestMethod]
        public void Test2()
        {
            DirectoryCreateRecursiveCommand command = new DirectoryCreateRecursiveCommand(@"C:\Temp\Trashed\Lonko\Mlono\Kickback\Stuffun");
            command.Execute();
            command.Undo();
        }
    }
}
