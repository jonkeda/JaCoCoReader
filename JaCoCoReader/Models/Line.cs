using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Models
{
    [Serializable]
    [XmlType("line", AnonymousType = true)]
    [XmlRoot("line", IsNullable = false)]
    public class Line : Model<Line, int>
    {
        private int _nr;
        private int _mi;
        private int _ci;
        private int _mb;
        private int _cb;

        [XmlAttribute("nr")]
        public int Nr
        {
            get { return _nr; }
            set { _nr = value; }
        }

        [XmlAttribute("mi")]
        public int Mi
        {
            get { return _mi; }
            set { _mi = value; }
        }

        [XmlAttribute("ci")]
        public int Ci
        {
            get { return _ci; }
            set { _ci = value; }
        }

        [XmlAttribute("mb")]
        public int Mb
        {
            get { return _mb; }
            set { _mb = value; }
        }

        [XmlAttribute("cb")]
        public int Cb
        {
            get { return _cb; }
            set { _cb = value; }
        }

        public override Line Merge(Line model)
        {
            Mi += model.Mi;
            Ci += model.Ci;
            Mb += model.Mb;
            Cb += model.Cb;

            return this;
        }

        public override int Key
        {
            get { return Nr; }
        }
    }
}