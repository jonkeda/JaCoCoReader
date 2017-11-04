using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Core.Models.CodeCoverage
{
    [Serializable]
    [XmlType("sourcefile", AnonymousType = true)]
    [XmlRoot("sourcefile", IsNullable = false)]
    public class Sourcefile : Model<Sourcefile, string>
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

        public override Sourcefile Merge(Sourcefile model)
        {
            Lines.Merge(model.Lines);
            Counters.Merge(model.Counters);

            return this;
        }

        public override string Key
        {
            get { return Name; }
        }

    }
}