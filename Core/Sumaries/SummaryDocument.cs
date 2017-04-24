using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Sumaries
{
    public class SummariedDocument
    {
        public string Name { get; set; }
        public string OriginalText { get; set; }
        public List<ManualSummaryDocument> ManualSummaryDocuments { get; set; }
        public List<AutomaticSummaryDocument> AutomaticSummaryDocuments { get; set; }

    }
    public class ManualSummaryDocument
    {
        public string ParticipantId { get; set; }
        public string Summary { get; set; }
    }
    public class AutomaticSummaryDocument
    {
        public string SentenceSimilarlyMethod { get; set; }
        public string Summary { get; set; }
    }
}
