using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    [Serializable]
    [XmlType("sessioninfo", AnonymousType = true)]
    [XmlRoot("sessioninfo", IsNullable = false)]
    public class Sessioninfo : Model<Sessioninfo, string>
    {

        private string _id;
        private string _start;
        private string _dump;

        [XmlAttribute("id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlAttribute("start")]
        public string Start
        {
            get { return _start; }
            set { _start = value; }
        }

        [XmlAttribute("dump")]
        public string Dump
        {
            get { return _dump; }
            set { _dump = value; }
        }

        public override Sessioninfo Merge(Sessioninfo model)
        {
            return this;
        }

        public override string Key
        {
            get { return Id; }
        }
    }
}