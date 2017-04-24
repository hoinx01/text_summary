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

                    //if (graph.HasEdge(v1.Id, v2.Id))
                    //    continue;

                    Edge edge = new Edge(v1.Id, v2.Id);
                    edge.Weight = CalculateSimilarity(v1, v2, graph, doc, method);
                    if(edge.Weight > 0)
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
    }
}
