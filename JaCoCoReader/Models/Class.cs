using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    [Serializable]
    [XmlType("class", AnonymousType = true)]
    [XmlRoot("class", IsNullable = false)]
    public class Class
    {
        private MethodCollection _method;
        private CounterCollection _counter;
        private string _name;

        [XmlElement("method")]
        public MethodCollection Method
        {
            get { return _method; }
            set { _method = value; }
        }

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
    }
}