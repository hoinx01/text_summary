using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace Core.DucXmlData
{
    [XmlRoot("collection")]
    public class DucCollection
    {
        [XmlElement("document")]
        public DucDocument[] Documents { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
    
    public class DucDocument
    {
        [XmlElement("line")]
        public DucLine[] Lines { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
    public class DucLine
    {
        [XmlText]
        public string TextValue { get; set; }
        [XmlElement("annotation")]
        public DucAnnotation Annotation { get; set; }
    }
    public class DucAnnotation
    {
        [XmlElement("scu")]
        public DucScu[] Scus { get; set; }
        [XmlAttribute("scu-count")]
        public string ScuCount { get; set; }
        [XmlAttribute("sum-count")]
        public string SumCount { get; set; }
        [XmlAttribute("sums")]
        public string Sums { get; set; }
    }
    public class DucScu
    {
        [XmlAttribute("uid")]
        public string Uid { get; set; }
        [XmlAttribute("label")]
        public string Label { get; set; }
        [XmlAttribute("weight")]
        public string Weight { get; set; }

    }
}
