using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    public class LineHitDictionary : Dictionary<int, bool>
    { }

    [Serializable]
    [XmlType("sourcefile", AnonymousType = true)]
    [XmlRoot("sourcefile", IsNullable = false)]
    public class Sourcefile
    {

        private LineCollection _lines;
        private CounterCollection _counters;
        private string _name;

        [XmlElement("line")]
        public LineCollection Lines
        {
            get { return _lines; }
            set { _lines = value; }
        }


        [XmlElement("counter")]
        public CounterCollection Counters
        {
            get { return _counters; }
            set { _counters = value; }
        }


        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


    }
}