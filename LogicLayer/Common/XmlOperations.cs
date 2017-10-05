using LogicLayer.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LogicLayer.Common
{
    public class XmlOperations
    {
        static XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        static XmlWriterSettings settings = new XmlWriterSettings();

        static public XmlWriterSettings XmlIndentSetting
        {
            get
            {
                return settings;
            }
        }

        static XmlOperations()
        {
            ns.Add("", "");
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
        }

        static public T DeserializeFromXml<T>(string xml)
        {
            StringReader SR = new StringReader(xml);
            XmlSerializer Xs = new XmlSerializer(typeof(T));
            T pl = (T)Xs.Deserialize(SR);
            Xs = null;
            SR.Close();
            SR.Dispose();
            return pl;
        }

        static public string SerializeToXml<T>(T obj)
        {
            string retval = "";
            StringWriter sw = new StringWriter();
            XmlSerializer Xs = new XmlSerializer(typeof(T));
            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                Xs.Serialize(writer, obj, ns);
                retval= sw.ToString(); 
            }
            sw.Close();
            sw.Dispose();
            Xs = null;
            return retval;

        }
    }
}
