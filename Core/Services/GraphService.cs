using Core.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Core.Services
{
    public class GraphService
    {
        public void BuildGraphEdges(Graph graph, Document doc, string method)
        {
            graph.Edges = new List<Edge>();
            foreach (var v1 in graph.Vertexes)
                foreach(var v2 in graph.Vertexes)
                {
                    if (v1.Id == v2.Id)
                        continue;

                    Edge edge = new Edge(v1.Id, v2.Id);
                    edge.Weight = CalculateSimilarity(v1, v2, graph, doc, method);
                    if(edge.Weight > 0)
                        graph.Edges.Add(edge);
                }
        }

        public void BuildGraphEdgesFullMethod(Graph graph, Document doc)
        {
            graph.Edges = new List<Edge>();
            foreach (var v1 in graph.Vertexes)
                foreach (var v2 in graph.Vertexes)
                {
                    if (v1.Id == v2.Id)
                        continue;

                    Edge edge = new Edge(v1.Id, v2.Id);
                    edge.WeightBySimpleMethods = new Dictionary<string, double>();
                    edge.WeightByMethods = new Dictionary<string, double>();

                    foreach (var method in SentenceSimilarityMethod.METHODS)
                    {
                        var weight = CalculateSimilarity(v1, v2, graph, doc, method);
                        edge.WeightBySimpleMethods.Add(method, weight);
                    }

                    var complexMethods = GetAllSublist(SentenceSimilarityMethod.METHODS);
                    foreach(var complexMethod in complexMethods)
                    {
                        double totalWeight = 0;
                        foreach(string method in complexMethod)
                        {
                            double weight = edge.WeightBySimpleMethods[method];
                            totalWeight += weight;
                        }
                        edge.WeightByMethods.Add(string.Join(",", complexMethod), totalWeight / complexMethod.Count);
                    }
                    graph.Edges.Add(edge);
                }
        }

        private double CalculateSimilarity(Vertex v1, Vertex v2, Graph graph, Document doc, string method)
        {
            double result = 0;
            switch (method)
            {
                case SentenceSimilarityMethod.BM25:
                    result = Bm25(v1, v2, graph, doc);
                    break;
                case SentenceSimilarityMethod.FILTERD:
                    result = Filtered(v1, v2, graph, doc);
                    break;
                case SentenceSimilarityMethod.TRCMP:
                    result = TRCmp(v1, v2, graph, doc);
                    break;
                case SentenceSimilarityMethod.LINTFIDF:
                    result = LinTFIDF(v1, v2, graph, doc);
                    break;
                case SentenceSimilarityMethod.KEYWORD:
                    result = Keyword(v1, v2, graph, doc);
                    break;
                case SentenceSimilarityMethod.COSIN:
                    result = Cosin(v1, v2, graph, doc);
                    break;
                default:
                    result = 0;
                    break;

            }
            return result;
        }
        private double Bm25(Vertex v1, Vertex v2, Graph graph, Document doc)
        {
            const double PARAM_K1 = 1.2f;
            const double PARAM_B = 0.75f;
            const double EPSILON = 0.25;

            double bm25 = 0;
            foreach(string term in v1.Terms)
            {
                Sentence sentence2 = null;
                doc.SentenceDictionary.TryGetValue(v2.Id, out sentence2);
                int f_s_v2 = 0;
                sentence2.TermFrequencies.TryGetValue(term, out f_s_v2);

                if (doc.TermDfs[term] > doc.SentenceDictionary.Count / 2)
                    bm25 += doc.TermIdfs[term] * (f_s_v2 * (PARAM_K1 + 1)) / (f_s_v2 + PARAM_K1 * (1 - PARAM_B + PARAM_B * sentence2.Length / doc.LengthAverage));
                else
                    bm25 += EPSILON * doc.IdfAverage;
            }

            return bm25;
        }
        private double Filtered(Vertex v1, Vertex v2, Graph graph, Document doc)
        {
            var commonTerms = v1.Terms.Intersect(v2.Terms);
            int v1Length = doc.SentenceDictionary[v1.Id].Length;
            int v2Length = doc.SentenceDictionary[v2.Id].Length;
            return commonTerms.Count() / (Math.Log10(v1Length) + Math.Log10(v2Length));
        }
        private double TRCmp(Vertex v1, Vertex v2, Graph graph, Document doc)
        {
            var commonTerms = v1.Terms.Intersect(v2.Terms);
            int v1Length = doc.SentenceDictionary[v1.Id].Length;
            int v2Length = doc.SentenceDictionary[v2.Id].Length;
            return commonTerms.Count() / (Math.Sqrt(v1Length + v2Length));
        }

        private double LinTFIDF(Vertex v1, Vertex v2, Graph graph, Document doc)
        {
            var s1 = doc.SentenceDictionary[v1.Id];
            var s2 = doc.SentenceDictionary[v2.Id];
            var commonTerms = v1.Terms.Intersect(v2.Terms);

            double tuso = 0;
            foreach(var commonTerm in commonTerms)
            {
                var s1tf = s1.TermFrequencies.ContainsKey(commonTerm)? s1.TermFrequencies[commonTerm] : 0;
                var s2tf = s2.TermFrequencies.ContainsKey(commonTerm) ?  s2.TermFrequencies[commonTerm] : 0;
                var idf = doc.TermIdfs[commonTerm];
                tuso += s1tf * s2tf * Math.Pow(idf, 2);
            }

            double mausos1 = 0;
            foreach(var term in s1.Terms)
            {
                mausos1 += (s1.TermFrequencies.ContainsKey(term) ? s1.TermFrequencies[term] : 0) * (Math.Pow(doc.TermIdfs[term], 2));
              
            }
            double mausos2 = 0;
            foreach (var term in s2.Terms)
            {
                mausos2 += (s2.TermFrequencies.ContainsKey(term) ? s2.TermFrequencies[term] : 0) * (Math.Pow(doc.TermIdfs[term], 2));

            }

            return tuso / (Math.Sqrt(mausos1 * mausos2));
        }

        private double Keyword(Vertex v1, Vertex v2, Graph graph, Document doc)
        {
            var s1 = doc.SentenceDictionary[v1.Id];
            var s2 = doc.SentenceDictionary[v2.Id];
            var commonTerms = v1.Terms.Intersect(v2.Terms);
            return 
            (
                commonTerms.Select(commonTerm =>
                {
                    return ((s1.TermFrequencies.ContainsKey(commonTerm) ? s1.TermFrequencies[commonTerm] : 0) + (s2.TermFrequencies.ContainsKey(commonTerm) ? s2.TermFrequencies[commonTerm] : 0)) * doc.TermIdfs[commonTerm];
                }).Sum()
            )/ (s1.Length + s1.Length);
        }

        private double Cosin(Vertex v1, Vertex v2, Graph graph, Document doc)
        {
            double tichvohuong = 0;
            double binhphuongdodai1 = 0;
            double binhphuongdodai2 = 0;

            foreach(var term in doc.Terms)
            {

                double toado1 = (doc.SentenceDictionary[v1.Id].TermFrequencies.ContainsKey(term) ? doc.SentenceDictionary[v1.Id].TermFrequencies[term] : 0) * doc.TermIdfs[term];
                double toado2 = (doc.SentenceDictionary[v2.Id].TermFrequencies.ContainsKey(term) ? doc.SentenceDictionary[v2.Id].TermFrequencies[term] : 0) * doc.TermIdfs[term];
                tichvohuong += toado1 * toado2;
                binhphuongdodai1 += Math.Pow(toado1, 2);
                binhphuongdodai2 += Math.Pow(toado2, 2);
            }

            return Math.Abs(tichvohuong) / (Math.Sqrt(binhphuongdodai1 * binhphuongdodai2));
        }
        public string SortVertexToGet(Graph graph, Document document, int count)
        {
            var vertexesSortedByScore = graph.Vertexes.OrderBy(o => 1 / o.Score).ToList().Take(count).ToList();
            var choosenVertexesOrderByIndex = vertexesSortedByScore.OrderBy(o => o.Id).ToList();
            string result = "";
            foreach(var vertex in choosenVertexesOrderByIndex)
            {
                var sentence = document.SentenceDictionary[vertex.Id].TextValue;
                result += sentence;
            }

            return result;
        }

        public string SortVertexToGetBySummaryRate(Graph graph, Document document, int percentBySentenceNumber)
        {
            int count = document.SentenceCount * percentBySentenceNumber / 100;
            var vertexesSortedByScore = graph.Vertexes.OrderBy(o => 1 / o.Score).ToList().Take(count).ToList();
            var choosenVertexesOrderByIndex = vertexesSortedByScore.OrderBy(o => o.Id).ToList();
            string result = "";
            foreach (var vertex in choosenVertexesOrderByIndex)
            {
                var sentence = document.SentenceDictionary[vertex.Id].TextValue + ". ";
                result += sentence;
            }

            return result;
        }

        public List<List<string>> GetAllSublist(List<string> input)
        {
            var result = new List<List<string>>();
            for (int i = 0; i < input.Count; i++)
            {
                for (int j = 1; j <= input.Count - i; j++)
                {
                    result.Add(input.GetRange(i, j));
                }
            }

            return result;
        }
    }
}
