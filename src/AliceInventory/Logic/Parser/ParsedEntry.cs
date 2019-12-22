using EP.Ner;
using EP.Ner.Core;
using EP.Morph;

namespace AliceInventory.Logic
{
    public class ParsedEntry
    {
        private string name;
        public string Name
        {
            get
            {
                return name; 
            }
            set
            {
                ProcessorService.Initialize();
                using (Processor proc = ProcessorService.CreateProcessor())
                {
                    if (value != null)
                    {
                        string[] valueparts = value.Trim().Split();
                        string result = "";
                        foreach (string valuepart in valueparts)
                        {
                            AnalysisResult ar = proc.Process(new SourceOfAnalysis(valuepart));
                            for (Token t = ar.FirstToken; t != null; t = t.Next)
                            {
                                NounPhraseToken npt = NounPhraseHelper.TryParse(t, NounPhraseParseAttr.AdjectiveCanBeLast);
                                result += npt?.GetMorphVariant(MorphCase.Nominative, false).ToLower() ?? t.GetNormalCaseText().ToString().ToLower();
                            }
                            result += " ";
                        }
                        name = result.Trim();
                    }
                    else
                    {
                        name = "";
                    }
                }
                 
            }
        }
        public double? Quantity { get; set; }
        public UnitOfMeasure? Unit { get; set; }
    }
}
