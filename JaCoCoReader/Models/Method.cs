using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    [Serializable]
    [XmlType("method", AnonymousType = true)]
    [XmlRoot("method", IsNullable = false)]
    public class Method : Model<Method, string>
    {
        private CounterCollection _counter;
        private string _name;
        private string _desc;
        private int _line;

        [XmlElement("counter")]
        public CounterCollection Counters
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
        public string Description
        {
            get { return _desc; }
            set { _desc = value; }
        }

        [XmlAttribute("line")]
        public int Line
        {
            get { return _line; }
            set { _line = value; }
        }

        public override Method Merge(Method model)
        {
            Counters.Merge(model.Counters);

            return this;
        }

        public override string Key
        {
            get { return Name; }
        }
    }
}