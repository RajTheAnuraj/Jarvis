using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicLayer.Interfaces
{
    public interface IXmlLoadable
    {
        void LoadFromXml(string xml);
        string ConvertToString();
    }
}
