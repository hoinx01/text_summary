using Core.Domains;
using Core.Sumaries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Core.DucXmlData
{
    public class XmlService
    {
        public List<Document> Read()
        {
            string path = @".\D0701.xml";
            XmlSerializer ser = new XmlSerializer(typeof(DucCollection));
            FileStream myFileStream = new FileStream(path, FileMode.Open);
            var collection = (DucCollection)ser.Deserialize(myFileStream);

            List<Document> result = new List<Document>();
            foreach(var document in collection.Documents)
            {
                string documentTextValue = "";
                for (int i=0; i< document.Lines.Length; i++)
                {
                    documentTextValue += ". " + document.Lines[i].TextValue;
                }
                result.Add(new Document(documentTextValue));
            }
            return result;
        }
        public List<SummariedDocument> ReadFile(string resourceDirectory)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DucCollection));
            FileStream myFileStream = new FileStream(resourceDirectory, FileMode.Open);
            var collection = (DucCollection)ser.Deserialize(myFileStream);

            List<SummariedDocument> result = new List<SummariedDocument>();
            foreach (var document in collection.Documents)
            {
                string documentTextValue = "";
                for (int i = 0; i < document.Lines.Length; i++)
                {
                    documentTextValue += ". " + document.Lines[i].TextValue;
                }
                SummariedDocument summariedDocument = new SummariedDocument();
                summariedDocument.OriginalText = documentTextValue;
                summariedDocument.Name = document.Name;

                List<ManualSummaryDocument> manualSummaryDocuments = new List<ManualSummaryDocument>();
                for(int j =0; j < document.Lines.Length; j++)
                {
                    var line = document.Lines[j];
                    if (line.Annotation == null)
                        continue;
                    var annotation = line.Annotation;
                    var participantJoinedIds = annotation.Sums;
                    var participantIds = new List<string>(participantJoinedIds.Split(','));
                    foreach(var participantId in participantIds)
                    {
                        var participantIdStandard = participantId.Trim();
                        var manualSummaryDocument = manualSummaryDocuments.Find(f => f.ParticipantId.Equals(participantIdStandard));
                        if(manualSummaryDocument == null)
                        {
                            manualSummaryDocument = new ManualSummaryDocument();
                            manualSummaryDocument.ParticipantId = participantIdStandard;
                            manualSummaryDocument.Summary = line.TextValue;
                            manualSummaryDocuments.Add(manualSummaryDocument);
                        }
                        else
                        {
                            manualSummaryDocument.Summary += line.TextValue;
                        }
                    }
                }
                summariedDocument.ManualSummaryDocuments = manualSummaryDocuments;
                result.Add(summariedDocument);
            }
            return result;
        }
        public void Write(Object obj, string path)
        {
            XmlSerializer ser = new XmlSerializer(obj.GetType());
            if (File.Exists(path))
                File.Delete(path);

            FileStream outputStream = new FileStream(path, FileMode.CreateNew);
            ser.Serialize(outputStream, obj);

            outputStream.Flush();
        }
        
    }
}
