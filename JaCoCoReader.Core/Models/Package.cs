﻿using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Core.Models
{
    [Serializable]
    [XmlType("package", AnonymousType = true)]
    [XmlRoot("package", IsNullable = false)]
    public class Package : Model<Package, string>
    {
        private string _name;
        private CounterCollection _counter;
        private ClassCollection _classes;
        private SourcefileCollection _sourcefiles;

        [XmlElement("class", typeof(Class))]
        public ClassCollection Classes
        {
            get { return _classes; }
            set { _classes = value; }
        }

        [XmlElement("sourcefile", typeof(Sourcefile))]
        public SourcefileCollection Sourcefiles
        {
            get { return _sourcefiles; }
            set { _sourcefiles = value; }
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

        public override Package Merge(Package model)
        {
            Classes.Merge(model.Classes);
            Sourcefiles.Merge(model.Sourcefiles);
            Counters.Merge(model.Counters);

            return this;
        }

        public override string Key
        {
            get { return Name; }
        }
    }
}