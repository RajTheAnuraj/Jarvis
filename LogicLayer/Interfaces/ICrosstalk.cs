using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer.Interfaces
{
   

    public interface ICrosstalkService
    {
        Dictionary<string,ICrosstalk> CallbackObjectIndex { get; set; }
        void RegisterCallback(string CallbackObjectIdentifier, ICrosstalk CallbackObject);
        object Crosstalk(string CallbackObjectIdentifier, string ActionName, object[] Parameters);
    }

    public interface ICrosstalk{
        object IncomingCrosstalk(string ActionName, object[] Parameters);
    }
}
