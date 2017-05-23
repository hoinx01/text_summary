using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Core.Services
{
    public class Evaluator
    {
        public static List<string> GetTokens(string doc, int tokenLength)
        {
            List<string> result = new List<string>();
            List<string> sentences = new List<string>(doc.Split('.')).Select(s => s.Trim()).ToList();
            foreach(string sentence in sentences)
            {
                var termElements = new List<string>(sentence.Split(' ')).Select(s => s.Trim()).ToList();
                for(int i = 0; i< termElements.Count - tokenLength; i++)
                {
                    var term = string.Join(" ", termElements.GetRange(i, tokenLength));
                    if (!result.Contains(term))
                        result.Add(term);
                }
            }
            return result;
        }
        public static double RougeN(string doc, List<string> targets, int n)
        {
            List<string> ngrams = GetTokens(doc, n);
            int sumCommonNgrams = 0;
            int sumTargetNgrams = 0;

            if (targets == null || targets.Count == 0)
                return double.NaN;
            foreach(var target in targets)
            {
                List<string> targetNgrams = GetTokens(target, n);
                sumTargetNgrams += targetNgrams.Count;
                List<string> commonNgrams = ngrams.Intersect(targetNgrams).ToList();
                sumCommonNgrams += commonNgrams.Count;
            }
            return (double) sumCommonNgrams / sumTargetNgrams;
        }
    }
}
