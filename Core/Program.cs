using Core.Domains;
using Core.DucXmlData;
using Core.Services;
using Core.Sumaries;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlService = new XmlService();

            string directory = @"\DUC2007\2007 SCU-marked corpus";
            string[] fileEntries = Directory.GetFiles(directory);

            foreach(var fileName in fileEntries)
            {
                var summariedDocuments = xmlService.ReadFile(fileName);

                foreach(var summariedDocument in summariedDocuments)
                {
                    var document = new Document(summariedDocument.OriginalText);
                    var graph = new Graph();
                    foreach (var sentence in document.SentenceDictionary)
                    {
                        var vertex = new Vertex(sentence.Value);
                        graph.AddVertex(vertex);
                    }
                    var graphService = new GraphService();

                    graphService.BuildGraphEdges(graph, document, SentenceSimilarityMethod.BM25);
                    graph.CalculateScore(0.001);

                    var automaticSummaryText = graphService.SortVertexToGet(graph, document, 3);
                    var automaticSummaryDocument = new AutomaticSummaryDocument();
                    automaticSummaryDocument.SentenceSimilarlyMethod = "bm25";
                    automaticSummaryDocument.Summary = automaticSummaryText;

                    summariedDocument.AutomaticSummaryDocuments = new List<AutomaticSummaryDocument>();
                    summariedDocument.AutomaticSummaryDocuments.Add(automaticSummaryDocument);


                    xmlService.Write(summariedDocument, @"\SummaryDocuments\" + summariedDocument.Name + ".xml");
                }
            }
            Console.ReadKey();
        }


    }
}