using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    [Serializable]
    [XmlType("group", AnonymousType = true)]
    [XmlRoot("group", IsNullable = false)]
    public class Group
    {

        private CounterCollection _counter;

        private string _name;
        private GroupCollection _groups;
        private PackageCollection _packages;


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