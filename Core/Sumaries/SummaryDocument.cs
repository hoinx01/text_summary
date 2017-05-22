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
        public List<EvaluationResult> EvaluationResults { get; set; }
    }
    public class EvaluationResult
    {
        public int MaxLength { get; set; }
        public double Result { get; set; }
    }
    public class SummarizationPrecision
    {
        public int N { get; set; }
        public string Method { get; set; }
        public double Precision { get; set; }

        public SummarizationPrecision()
        {

        }
        public SummarizationPrecision(int n, string method, double precision)
        {
            this.N = n;
            this.Method = method;
            this.Precision = precision;
        }
    }
    public class SummarizationStatistic
    {
        public string SimilarlyMethod { get; set; }
        public int NgramLength { get; set; }
        public double Max { get; set; }
        public double Average { get; set; }
        public SummarizationStatistic()
        {

        }
        public SummarizationStatistic(string similarlyMethod, int ngramLength, double max, double average)
        {
            this.SimilarlyMethod = similarlyMethod;
            this.NgramLength = ngramLength;
            this.Max = max;
            this.Average = average;
        }
    }
    public class SummarizationStatistics
    {
        public List<SummarizationStatistic> Result { get; set; }
    }
}
