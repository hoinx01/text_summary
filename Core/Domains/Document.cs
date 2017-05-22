using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Core.Domains
{
    public class Document
    {
        [XmlIgnore]
        public static string JoinedStopWords =
            "all six eleven just less being indeed over both anyway detail four front already through yourselves fify" +
            "mill still its before move whose one system also somewhere herself thick show had enough should to only " +
            "seeming under herein ours two has might thereafter do them his around thereby get very de none cannot " +
            "every whether they not during thus now him nor name regarding several hereafter did always cry whither " +
            "beforehand this someone she each further become thereupon where side towards few twelve because often ten " +
            "anyhow doing km eg some back used go namely besides yet are cant our beyond ourselves sincere out even " +
            "what throughout computer give for bottom mine since please while per find everything behind does various " +
            "above between kg neither seemed ever across t somehow be we who were sixty however here otherwise whereupon " +
            "nowhere although found hers re along quite fifteen by on about didn last would anything via of could thence " +
            "put against keep etc s became ltd hence therein onto or whereafter con among own co afterwards formerly " +
            "within seems into others whatever yourself down alone everyone done least another whoever moreover couldnt " +
            "must your three from her their together top there due been next anyone whom much call too interest thru " +
            "themselves hundred was until empty more himself elsewhere mostly that fire becomes becoming hereby but " +
            "else part everywhere former don with than those he me forty myself made full twenty these bill using up us " +
            "will nevertheless below anywhere nine can theirs toward my something and sometimes whenever sometime then " +
            "almost wherever is describe am it doesn an really as itself at have in seem whence ie any if again hasnt " +
            "inc un thin no perhaps latter meanwhile when amount same wherein beside how other take which latterly you " +
            "fill either nobody unless whereas see though may after upon therefore most hereupon eight amongst never " +
            "serious nothing such why a off whereby third i whole noone many well except amoungst yours rather without " +
            "so five the first having once";
        [XmlIgnore]
        public static List<string> StopWords = new List<string>(JoinedStopWords.Split(' '));
        public string TextValue { get; set; }
        [XmlIgnore]
        public Dictionary<int, Sentence> SentenceDictionary { get; set; }
        [XmlIgnore]
        public Dictionary<string, int> TermDfs { get; set; }
        [XmlIgnore]
        public Dictionary<string, double> TermIdfs { get; set; }
        [XmlIgnore]
        public List<string> Terms { get; set; }
        [XmlIgnore]
        public int Length { get; set; }
        [XmlIgnore]
        public int SentenceCount { get; set; }
        [XmlIgnore]
        public double LengthAverage { get; set; }
        [XmlIgnore]
        public double IdfAverage { get; set; }

        public Document()
        {

        }
        public Document(string input)
        {
            this.TextValue = input;
            var preSentences = input.Split('.');

            this.SentenceDictionary = new Dictionary<int, Sentence>();
            this.Terms = new List<string>();
            this.TermDfs = new Dictionary<string, int>();
            this.TermIdfs = new Dictionary<string, double>();
            
            this.SentenceCount = 0;
            this.Length = 0;

            for(int i = 0; i< preSentences.Length; i++)
            {
                if (String.IsNullOrWhiteSpace(preSentences[i]))
                    continue;
                var sentence = new Sentence(i, preSentences[i]);
                this.SentenceDictionary.Add(i, sentence);
                this.SentenceCount++;
                this.Length += sentence.Length;
                
                if (sentence.Terms.Count == 0)
                    continue;
                foreach(var term in sentence.Terms)
                {
                    if (String.IsNullOrWhiteSpace(term))
                        continue;

                    if (!this.Terms.Contains(term))
                        this.Terms.Add(term);

                    if (this.TermDfs.ContainsKey(term))
                    {
                        int frequency = this.TermDfs[term];
                        this.TermDfs[term] = frequency++;
                    }
                    else
                        this.TermDfs[term] = 1;

                    
                }
            }

            double totalTermIdf = 0;
            foreach(var term in this.Terms)
            {
                double termIdf = Math.Log(this.SentenceCount - TermDfs[term] + 0.5) - Math.Log(TermDfs[term] + 0.5);
                totalTermIdf += termIdf;
                this.TermIdfs.Add(term, termIdf);
            }
            this.LengthAverage = this.Length / this.SentenceCount;
            this.IdfAverage = totalTermIdf / this.SentenceCount;
        }
    }
    public class Sentence
    {
        public int Id { get; set; }
        public string TextValue { get; set; }
        public List<string> Terms { get; set; }
        public Dictionary<string, int> TermFrequencies { get; set; }
        public int Length { get; set; }

        public Sentence()
        {
        }
        public Sentence(int id, string input)
        {
            this.Id = id;
            this.TextValue = input;
            var analysisedText = input.Trim().ToLower();
            //foreach(var stopword in Document.StopWords)
            //{
            //    analysisedText = analysisedText.Replace(stopword, "").Trim().ToLower();
            //}
            this.Length = analysisedText.Length;

            var preTerms = new List<string>(analysisedText.Split(' '));

            this.Terms = new List<string>();
            this.TermFrequencies = new Dictionary<string, int>();
            foreach(var term in preTerms)
            {
                if (Document.StopWords.Contains(term))
                    continue;
                if (!this.Terms.Contains(term))
                    this.Terms.Add(term);
                if (this.TermFrequencies.ContainsKey(term))
                {
                    int frequency = this.TermFrequencies[term];
                    this.TermFrequencies[term] = frequency + 1;
                }
                else
                    this.TermFrequencies[term] = 1;
            }
        }
    }
}
