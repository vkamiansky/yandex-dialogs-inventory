using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP.Ner;
using EP.Ner.Core;
using EP.Morph;
using EP.;

namespace AliceInventory.Logic.Parser
{
    public static class WordNormalizer
    {
        public static string Normalize(string word)
        {
            if (word == null) return null;
            string newWord;
            ProcessorService.Initialize();
            using (var proc = ProcessorService.CreateProcessor())
            {
                
                var valueParts = word.Trim().Split();
                var result = new StringBuilder();
                proc;
                foreach (var valuePart in valueParts)
                {
                    var ar = proc.Process(new SourceOfAnalysis(valuePart));
                    for (var t = ar.FirstToken; t != null; t = t.Next)
                    {
                        var npt = NounPhraseHelper.TryParse(t, NounPhraseParseAttr.AdjectiveCanBeLast);
                        result.Append(npt?.GetMorphVariant(MorphCase.Nominative, true).ToLower() ??
                                  t.GetNormalCaseText().ToLower());
                    }

                    result.Append(" ");
                }

                newWord = result.ToString().Trim();
            }
            

            return newWord;

        }
    }
}
