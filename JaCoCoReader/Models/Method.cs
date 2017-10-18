using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    [Serializable]


    [XmlType("method", AnonymousType = true)]
    [XmlRoot("method", IsNullable = false)]
    public class Method
    {

        private CounterCollection _counter;

        private string _name;

        private string _desc;

        private string _line;


        [XmlElement("counter")]
        public CounterCollection Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }


        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        [XmlAttribute("desc")]
        public string Desc
        {
            get { return _desc; }
            set { _desc = value; }
        }


        [XmlAttribute("line")]
        public string Line
        {
            get { return _line; }
            set { _line = value; }
        }
    }
}