using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Core.Models
{
    [Serializable]
    [XmlType("counter", AnonymousType = true)]
    [XmlRoot("counter", IsNullable = false)]
    public class Counter : Model<Counter, CounterType>
    {
        private CounterType _type;
        private int _missed;
        private int _covered;

        [XmlAttribute("type")]
        public CounterType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute("missed")]
        public int Missed
        {
            get { return _missed; }
            set { _missed = value; }
        }

        [XmlAttribute("covered")]
        public int Covered
        {
            get { return _covered; }
            set { _covered = value; }
        }

        public override Counter Merge(Counter model)
        {
            Missed += model.Missed;
            Covered += model.Covered;

            return this;
        }

        public override CounterType Key
        {
            get { return Type; }
        }
    }
}