namespace FolSolverCore.Core
{
    public static class InputDictionary
    {
        public static List<string> conjunctionKeys = new List<string>
        {
            "\\land",
            "\\and",
            "\\&&",
            "\\&",
            "\\*",
            "\\.",
        };

        public static List<string> disjunctionKeys = new List<string>
        {
            "\\lor",
            "\\or",
            "\\+",
            "\\|", 
            "\\||",
            "\\wedge",
        };

        public static List<string> implicationKeys = new List<string>
        {
            "\\implies",
            "\\>",
            "\\rightarrow",
        };

        public static List<string> negationKeys = new List<string>
        {
            "\\neg",
            "\\!",
            "\\not",
            "\\-",
            "\\~",
        };

        public static List<string> universalKeys = new List<string>
        {
            "\\forall",
        };

        public static List<string> existencialKeys = new List<string>
        {
            "\\exists",
        };

        public static string CheckAndReplaceSpecialCharacters(string input)
        {
            string output = input;

            foreach (var key in conjunctionKeys)
            {
                if (output.Contains(key))
                {
                    output = output.Replace(key, "∧");
                }
            }
            foreach (var key in disjunctionKeys)
            {
                if (output.Contains(key))
                {
                    output = output.Replace(key, "∨");
                }
            }
            foreach (var key in implicationKeys)
            {
                if (output.Contains(key))
                {
                    output = output.Replace(key, "=>");
                }
            }
            foreach (var key in negationKeys)
            {
                if (output.Contains(key))
                {
                    output = output.Replace(key, "¬");
                }
            }
            foreach (var key in universalKeys)
            {
                if (output.Contains(key))
                {
                    output = output.Replace(key, "∀");
                }
            }
            foreach (var key in existencialKeys)
            {
                if (output.Contains(key))
                {
                    output = output.Replace(key, "Ǝ");
                }
            }
            return output;
        }
    }
}
