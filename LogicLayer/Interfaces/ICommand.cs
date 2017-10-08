using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicLayer.Interfaces
{
    public interface ICommand
    {
        void Execute();
    }

    public interface IUndoableCommand
    {
        void Execute();
        void Undo();
    }
}
