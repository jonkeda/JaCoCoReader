using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    [Serializable]
    [XmlType("report", AnonymousType = true)]
    [XmlRoot("report", IsNullable = false)]
    public class Report
    {

        private SessioninfoCollection _sessioninfo;
        private CounterCollection _counter;
        private string _name;
        private GroupCollection _groups;
        private PackageCollection _packages;


        [XmlElement("sessioninfo")]
        public SessioninfoCollection Sessioninfo
        {
            get { return _sessioninfo; }
            set { _sessioninfo = value; }
        }


        [XmlElement("group", typeof(Group))]
        public GroupCollection Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        [XmlElement("package", typeof(Package))]
        public PackageCollection Packages
        {
            get { return _packages; }
            set { _packages = value; }
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
    }
}