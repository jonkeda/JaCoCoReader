using System;
using System.Xml.Serialization;

namespace JaCoCoReader.Core.Models.CodeCoverage
{
    [Serializable]
    [XmlType("counterType", AnonymousType = true)]
    public enum CounterType
    {

        [XmlEnum("INSTRUCTION")]
        Instruction,

        [XmlEnum("BRANCH")]
        Branch,

        [XmlEnum("LINE")]
        Line,

        [XmlEnum("COMPLEXITY")]
        Complexity,

        [XmlEnum("METHOD")]
        Method,

        [XmlEnum("CLASS")]
        Class
    }
}