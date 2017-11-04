using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Core.Models.CodeCoverage
{
    [Serializable]
    [XmlType("class", AnonymousType = true)]
    [XmlRoot("class", IsNullable = false)]
    public class Class : Model<Class, string>
    {
        private MethodCollection _method;
        private CounterCollection _counter;
        private string _name;

        [XmlElement("method")]
        public MethodCollection Methods
        {
            get { return _method; }
            set { _method = value; }
        }

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

        public override Class Merge(Class model)
        {
            Methods.Merge(model.Methods);
            Counters.Merge(model.Counters);

            return this;
        }

        public override string Key
        {
            get { return Name; }
        }
    }
}