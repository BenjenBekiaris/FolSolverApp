namespace FolSolverCore.Core
{
    public static class InputDictionary
    {
        private static List<string> conjunctionValues = new List<string>
        {
            "\\land",
            "\\and",
            "\\&&",
            "\\&",
            "\\*",
            "\\.",
        };

        private static List<string> disjunctionValues = new List<string>
        {
            "\\lor",
            "\\or",
            "\\+",
            "\\|", 
            "\\||",
            "\\wedge",
        };

        private static List<string> implicationValues = new List<string>
        {
            "\\implies",
            "\\>",
            "\\rightarrow",
        };

        private static List<string> negationValues = new List<string>
        {
            "\\neg",
            "\\!",
            "\\not",
            "\\-",
            "\\~",
        };

        private static List<string> universalValues = new List<string>
        {
            "\\forall",
        };

        private static List<string> existencialValues = new List<string>
        {
            "\\exists",
        };

        private static List<string> sequentValues = new List<string>
        {
            "\\vdash",
            "\\line"
        };

        public static Dictionary<string, List<string>> specialSymbols = new Dictionary<string, List<string>>
        {
            { "∧",conjunctionValues },
            { "∨", disjunctionValues },
            { "=>", implicationValues },
            { "∀", universalValues },
            { "Ǝ", existencialValues },
            { "¬", negationValues },
            { "⊢", sequentValues }
        };

        public static string CheckAndReplaceSpecialCharacters(string input)
        {
            string output = input;

            foreach (var entry in specialSymbols)
            {
                foreach (var value in entry.Value)
                {
                    if (output.Contains(value))
                    {
                        output = output.Replace(value, entry.Key);
                    }
                }
            }

            return output;
        }
    }
}
