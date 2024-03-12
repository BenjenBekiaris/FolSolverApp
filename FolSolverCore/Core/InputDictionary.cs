namespace FolSolverCore.Core
{
    public static class InputDictionary
    {
        static List<string[]> keys = new List<string[]>
        {
            new string[] {"\\forall ", "∀"},
            new string[] {"\\exists ", "Ǝ"},
            new string[] {"\\land ", "∧ "},
            new string[] {"\\lor ", "∨ "},
            new string[] {"\\neg ", "¬"},
        };

        public static string CheckAndReplaceSpecialCharacters(string input)
        {
            string output = input;

            foreach (var key in keys)
            {
                if (output.Contains(key[0]))
                {
                    output = output.Replace(key[0], key[1]);
                }
            }
            return output;
        }
    }
}
