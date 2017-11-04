using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Core.Models.CodeCoverage
{
    [Serializable]
    [XmlType("report", AnonymousType = true)]
    [XmlRoot("report", IsNullable = false)]
    public class Report : Model<Report, string>
    {
        private SessioninfoCollection _sessioninfo;
        private CounterCollection _counter;
        private string _name;
        private GroupCollection _groups;
        private PackageCollection _packages;

        [XmlElement("sessioninfo")]
        public SessioninfoCollection Sessioninfos
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

        public override Report Merge(Report model)
        {
            Sessioninfos.Merge(model.Sessioninfos);
            Groups.Merge(model.Groups);
            Packages.Merge(model.Packages);
            Counters.Merge(model.Counters);

            return this;
        }

        public override string Key
        {
            get { return Name; }
        }
    }
}