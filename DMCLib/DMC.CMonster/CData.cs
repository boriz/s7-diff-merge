using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace DMC.CMonster
{
    public class CData : List<CStateData>
    {
        public void SerializeToXML()
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(CData));
            using (FileStream stream = new FileStream(@"C:\temp\test.xml", FileMode.OpenOrCreate, FileAccess.Write))
            {
                x.Serialize(stream, this);
            }
        }
    }
}
