using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Core.Domains
{
    public class Graph
    {
        public List<Vertex> Vertexes { get; set; }
        public List<Edge> Edges { get; set; }
        const double damping = 0.85;
        public void AddVertex(Vertex vertex)
        {
            if (this.Vertexes == null)
                this.Vertexes = new List<Vertex>();
            this.Vertexes.Add(vertex);
        }
        public bool HasEdge(int firstVertexId, int secondVertextId)
        {
            if (this.Edges == null)
                return false;
            foreach(var edge in this.Edges)
            {
                if (
                    (edge.FirstVertexId == firstVertexId && edge.SecondVertexId == secondVertextId)
                    || (edge.FirstVertexId == secondVertextId && edge.SecondVertexId == firstVertexId)
                )
                    return true;
            }
            return false;
        }

        public void CalculateScore(double threshold)
        {
            foreach(var vertex in this.Vertexes)
            {
                vertex.Score = 0.15;
            }

            double maxScoreChanged = 0;
            int iteratorCount = 0;
            do
            {
                iteratorCount++;
                double currentLoopMaxScoreChanged = 0;
                foreach (var v2 in this.Vertexes)
                {
                    Console.WriteLine("calculating for vertex: " + v2.Id + " round: "+ iteratorCount);
                    double sumScoreFromOtherToV2 = 0;
                    List<Edge> inV2Edges = this.Edges.Where(w => w.SecondVertexId == v2.Id).ToList();
                    foreach(var inV2Edge in inV2Edges)
                    {
                        int v1Id = inV2Edge.FirstVertexId;
                        Vertex v1 = this.Vertexes.Where(w => w.Id == v1Id).Single();
                        List<Edge> outV1Edges = this.Edges.Where(w => w.FirstVertexId == v1Id).ToList();
                        double sumOutV1EdgeWeights = outV1Edges.Select(s => s.Weight).Sum();
                        double v1ToV2Weight = outV1Edges.Where(w => w.SecondVertexId == v2.Id).Single().Weight;
                        double scoreV1ForV2 = (v1ToV2Weight / (sumOutV1EdgeWeights - v1ToV2Weight)) * v1.Score;
                        sumScoreFromOtherToV2 += scoreV1ForV2;
                    }
                    double newScoreV2 = (1 - damping) + damping * sumScoreFromOtherToV2;
                    double scoreChanged = Math.Abs(newScoreV2 - v2.Score);
                    if (scoreChanged > currentLoopMaxScoreChanged)
                    {
                        maxScoreChanged = scoreChanged;
                        Console.WriteLine("MaxScoreChanged on Vertex " + v2.Id);
                    }
                        
                    v2.Score = newScoreV2;
                    maxScoreChanged = currentLoopMaxScoreChanged;
                }
            }
            while (maxScoreChanged > threshold);
        }

        public void CalculateScoreWithMethod(double threshold, string method)
        {
            foreach (var vertex in this.Vertexes)
            {
                vertex.Score = 0.15;
            }

            double maxScoreChanged = 0;
            int iteratorCount = 0;
            do
            {
                iteratorCount++;
                double currentLoopMaxScoreChanged = 0;
                foreach (var v2 in this.Vertexes)
                {
                    double sumScoreFromOtherToV2 = 0;
                    List<Edge> inV2Edges = this.Edges.Where(w => w.SecondVertexId == v2.Id).ToList();
                    foreach (var inV2Edge in inV2Edges)
                    {
                        int v1Id = inV2Edge.FirstVertexId;
                        Vertex v1 = this.Vertexes.Where(w => w.Id == v1Id).Single();
                        List<Edge> outV1Edges = this.Edges.Where(w => w.FirstVertexId == v1Id).ToList();
                        double sumOutV1EdgeWeights = outV1Edges.Select(s => s.WeightByMethods[method]).Sum();
                        double v1ToV2Weight = outV1Edges.Where(w => w.SecondVertexId == v2.Id).Single().WeightByMethods[method];
                        double scoreV1ForV2 = (v1ToV2Weight / (sumOutV1EdgeWeights)) * v1.Score;
                        sumScoreFromOtherToV2 += scoreV1ForV2;
                    }
                    double newScoreV2 = (1 - damping) + damping * sumScoreFromOtherToV2;
                    double scoreChanged = Math.Abs(newScoreV2 - v2.Score);
                    if (scoreChanged > currentLoopMaxScoreChanged)
                    {
                        currentLoopMaxScoreChanged = scoreChanged;
                    }
                    v2.Score = newScoreV2;                   
                }
                maxScoreChanged = currentLoopMaxScoreChanged;
            }
            while (maxScoreChanged > threshold);
        }

    }
    public class Vertex
    {
        //Bằng số thứ tự của câu trong văn bản
        public int Id { get; set; }
        //Nguyên văn câu
        public string TextValue { get; set; }
        public double Score { get; set; }
        //Các cụm từ trong câu (có thể có được qua một số cách khác nhau)
        public List<string> Terms { get; set; }

        public Vertex()
        {

        }
        public Vertex(Sentence sentence)
        {
            this.Id = sentence.Id;
            this.TextValue = sentence.TextValue;
            this.Terms = sentence.Terms;
        }
    }
    public class Edge
    {
        public int FirstVertexId { get; set; }
        public int SecondVertexId { get; set; }
        public double Weight { get; set; }
        public Dictionary<string, double> WeightBySimpleMethods { get; set; }
        public Dictionary<string, double> WeightByMethods { get; set; }

        public Edge()
        {

        }
        public Edge(int firstVertexId, int secondVertexId)
        {
            this.FirstVertexId = firstVertexId;
            this.SecondVertexId = secondVertexId;
        }
    }
}
