using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services
{
    public class SentenceSimilarityMethod
    {
        public const string BM25 = "bm25";
        public const string FILTERD = "filtered";
        public const string TRCMP = "trcmp";
        public const string LINTFIDF = "lintfidf";
        public const string KEYWORD = "keyword";
        public const string COSIN = "cosin";
        public static List<string> METHODS = new List<string>() { BM25, FILTERD, TRCMP, LINTFIDF, COSIN};
    }
}
