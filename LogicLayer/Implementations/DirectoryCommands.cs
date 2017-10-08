using LogicLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Implementations
{
    public class DirectoryCreateCommand: IUndoableCommand
    {
        public string DirectoryPath { get; set; }

        public DirectoryCreateCommand()
        {

        }

        public DirectoryCreateCommand(string DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
        }

        public void Execute()
        {
            Directory.CreateDirectory(DirectoryPath);
        }


        public void Undo()
        {
            Directory.Delete(DirectoryPath);
        }
    }


    public class DirectoryCreateRecursiveCommand : IUndoableCommand
    {
        public string DirectoryPath { get; set; }
        Stack<IUndoableCommand> CommandHistory = new Stack<IUndoableCommand>();

        public DirectoryCreateRecursiveCommand()
        {

        }

        public DirectoryCreateRecursiveCommand(string DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
        }

        public void Execute()
        {
            if (String.IsNullOrWhiteSpace(DirectoryPath))
                throw new ArgumentException("Directory Path should be set");

            int i = 0;
            var pathParts = DirectoryPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            string tempPath = "";
            if (DirectoryPath.StartsWith("\\\\")) //Shared Drive
            {
                tempPath = "\\\\" + pathParts[0] + "\\";
                i = 1;
            }
            
            for (; i < pathParts.Length; i++)
            {
                tempPath += pathParts[i];
                if (!Directory.Exists(tempPath))
                {
                    DirectoryCreateCommand dirCreateCommand = new DirectoryCreateCommand(tempPath);
                    CommandHistory.Push(dirCreateCommand);
                    dirCreateCommand.Execute();
                }
                tempPath += "\\";
            }
        }


        public void Undo()
        {
            while (true)
            {
                if (CommandHistory.Count == 0) break;
                CommandHistory.Pop().Undo();
            } 
        }
    }
}
