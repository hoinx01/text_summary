using Core.Domains;
using Core.DucXmlData;
using Core.Services;
using Core.Sumaries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var xmlService = new XmlService();
            var graphService = new GraphService();

            string directory = @".\DUC2007\2007 SCU-marked corpus";
            string outputDirectory = @"\SummaryDocuments\";
            string[] fileEntries = Directory.GetFiles(directory);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var complexMethods = graphService.GetAllSublist(SentenceSimilarityMethod.METHODS);

            List<SummarizationPrecision> sumarizationPrecisions = new List<SummarizationPrecision>();

            foreach (var fileName in fileEntries)
            {
                var summariedDocuments = xmlService.ReadFile(fileName);

                foreach(var summariedDocument in summariedDocuments)
                {
                    Console.WriteLine("Processing ----" + fileName + " ---- " + summariedDocument.Name);
                    var document = new Document(summariedDocument.OriginalText);
                    var graph = new Graph();
                    foreach (var sentence in document.SentenceDictionary)
                    {
                        var vertex = new Vertex(sentence.Value);
                        graph.AddVertex(vertex);
                    }
                    
                    Console.WriteLine("Graph is being builded");
                    graphService.BuildGraphEdgesFullMethod(graph, document);
                    Console.WriteLine("Build graph completely");

                    
                    summariedDocument.AutomaticSummaryDocuments = new List<AutomaticSummaryDocument>();
                    foreach (var complexMethod in complexMethods)
                    {
                        var key = string.Join(",", complexMethod);
                        Console.WriteLine("Calculating score, method: " + key);
                        graph.CalculateScoreWithMethod(0.05, key);
                        Console.WriteLine("Calculated");
                        var automaticSummaryText = graphService.SortVertexToGetBySummaryRate(graph, document, 20);

                        var automaticSummaryDocument = new AutomaticSummaryDocument();
                        automaticSummaryDocument.SentenceSimilarlyMethod = key;
                        automaticSummaryDocument.Summary = automaticSummaryText;
                        

                        List<EvaluationResult> evaluationResults = new List<EvaluationResult>();
                        for(int n = 1; n< 3; n++)
                        {
                            double result = Evaluator.RougeN(automaticSummaryDocument.Summary, summariedDocument.ManualSummaryDocuments.Select(s => s.Summary).ToList(), n);
                            EvaluationResult evaluationResult = new EvaluationResult();
                            evaluationResult.MaxLength = n;
                            evaluationResult.Result = result;
                            evaluationResults.Add(evaluationResult);

                            sumarizationPrecisions.Add(new SummarizationPrecision(n, key, result));
                        }
                        automaticSummaryDocument.EvaluationResults = evaluationResults;

                        summariedDocument.AutomaticSummaryDocuments.Add(automaticSummaryDocument);
                    }
                                     
                    xmlService.Write(summariedDocument, outputDirectory + summariedDocument.Name + ".xml");

                    Console.WriteLine(fileName + " ---- " + summariedDocument.Name + ". Done!");
                }
            }

            Console.WriteLine("Sumarization completed!");

            Console.WriteLine("Getting statistics");

            var summarizationStatistics = new SummarizationStatistics();
            summarizationStatistics.Result = new List<SummarizationStatistic>();
            foreach (var complexMethod in complexMethods)
            {
                string key = string.Join(",", complexMethod);
                for(int n=1; n<3; n++)
                {
                    var precisions = sumarizationPrecisions.Where(s => s.Method.Equals(key) && s.N == n).Select(s=>s.Precision).ToList();
                    double max = precisions.Max();
                    double average = precisions.Average();
                    var summarizationStatistic = new SummarizationStatistic(key, n, max, average);
                    summarizationStatistics.Result.Add(summarizationStatistic);
                }
            }
            xmlService.Write(summarizationStatistics, outputDirectory + "Thong_Ke.xml");

            Console.WriteLine("Completed!");

            Console.WriteLine("All done! Press any key to exit...");
            Console.ReadKey();
        }


    }
}