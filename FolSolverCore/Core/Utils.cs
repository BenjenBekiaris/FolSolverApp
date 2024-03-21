namespace FolSolverCore.Core
{
    public static class Utils
    {
        public static bool IsPredicate(string predicate)
        {
            if (predicate.Contains('∧') || predicate.Contains('∨') || predicate.Contains("=>"))
            {
                return false;
            }
            return true;
        }

        public static bool AreComplementary(Predicate predicate1, Predicate predicate2)
        {
            if (predicate1.Name != predicate2.Name) return false;
           
            if (predicate1.Arity != predicate2.Arity) return false;
           
            for (int i = 0; i < predicate1.StringArguments.Length; i++)
            {
                if (predicate1.StringArguments[i] != predicate2.StringArguments[i]) return false;
            }
           
            if (predicate1.Negated == predicate2.Negated) return false;
           
            return true;
        }

        public static bool IsFunction(string function)
        {
            if (function.Contains('(') && function.Contains(')'))
            {
                return true;
            }
            return false;
        }

        public static string[] SplitFunction(string function)
        {
            var splitFunction = function.Split('(', 2);
            splitFunction[0] = splitFunction[0].Trim();
            splitFunction[1] = splitFunction[1].Remove(splitFunction[1].Length - 1);

            return splitFunction;
        }

        public static string[] SplitArguments(string functionArguments)
        {
            var output = new List<string>();
            string argument = "";
            int bracketDepth = 0;

            foreach (var character in functionArguments)
            {
                switch (character)
                {
                    case '(':
                        bracketDepth++;
                        argument += character;
                        break;
                    case ')':
                        bracketDepth--;
                        argument += character;
                        break;
                    case ',':
                        if (bracketDepth == 0)
                        {
                            output.Add(argument.Trim());
                            argument = "";
                        }
                        else { argument += character; }
                        break;
                    default:
                        argument += character;
                        break;
                }
            }
            if (argument != "") { output.Add(argument); }
            return output.ToArray();
        }

        public static string RewriteVariableInFunction(string oldName, string newName, string function)
        {
            string output = "";
            var splitFunction = SplitFunction(function);

            var functionArguments = SplitArguments(splitFunction[1]);
            for (int i = 0; i < functionArguments.Length; i++)
            {
                if (functionArguments[i] == oldName) { functionArguments[i] = newName; }
                if (IsFunction(functionArguments[i]))
                {
                    functionArguments[i] = RewriteVariableInFunction(oldName, newName, functionArguments[i]);
                }
            }

            output += splitFunction[0] + "(";
            output += string.Join(',', functionArguments) + ")";

            return output;
        }

        public static bool FunctionContainsVariable(string function, string variable)
        {
            if (!IsFunction(function)) { return false; }
            var splitFunction = SplitFunction(function);

            var functionArguments = SplitArguments(splitFunction[1]);
            for (int i = 0; i < functionArguments.Length; i++)
            {
                if (functionArguments[i] == variable) { return true; }
                if (IsFunction(functionArguments[i]))
                {
                    if (FunctionContainsVariable(functionArguments[i], variable)) { return true; }
                }
            }
            return false;
        }

        public static bool IsVariable(string variable)
        {
            if (!IsFunction(variable) && char.IsLower(variable[0])) { return true; }
            return false;
        }

        public static string PrintCNFasFormula(List<Predicate[]> cnf)
        {
            string output = "";
            for (int i = 0; i < cnf.Count; i++)
            {
                output += "(";
                for (int j = 0; j < cnf[i].Length; j++)
                {
                    output += cnf[i][j].PrintPredicate();
                    if (j + 1 != cnf[i].Length)
                    {
                        output += " ∨ ";
                    }
                }
                output += ")";
                if (i +1 != cnf.Count)
                {
                    output += " ∧ ";
                }
            }
            return output;
        }

        public static string PrintCNFasSet(List<Predicate[]> cnf)
        {
            string output = "{ ";
            for (int i = 0; i < cnf.Count; i++)
            {
                output += PrintClause(cnf[i]);
                if (i + 1 != cnf.Count)
                {
                    output += ", ";
                }
            }
            output += " }";
            return output;
        }

        public static string PrintClause(Predicate[] clause)
        {
            if (clause.Length == 0) return "□";
            string output = "{";
            for (int j = 0; j < clause.Length; j++)
            {
                output += clause[j].PrintPredicate();
                if (j + 1 != clause.Length)
                {
                    output += ",";
                }
            }
            output += "}";
            return output;
        }

        public static string PrintUnificator(List<string[]>? mGUnificator) 
        {
            if (mGUnificator == null) { return "Unification not possible"; }

            string output = "[ ";
            for (int i = 0; i < mGUnificator.Count; i++)
            {
                output += mGUnificator[i][0];
                output += "/";
                output += mGUnificator[i][1];

                if (i + 1 != mGUnificator.Count)
                {
                    output += ", ";
                }
            }
            output += " ]";
            return output;
        }

        public static bool CompareClauses(Predicate[] clause1, Predicate[] clause2)
        {
            if (clause1.Length != clause2.Length) return false;
            for (int i = 0; i < clause1.Length; i++)
            {
                if (clause1[i].PrintPredicate() != clause2[i].PrintPredicate()) return false;
            }
            return true;
        }

        public static string ChangeToLowerIndex(string input)
        {
            string output = "";
            foreach (var character in input)
            {
                output += (char)(character - '0' + '₀');
            }
            return output;
        }

        public static string ReplaceApostrophiesWithNumber(string input)
        {
            int count = 0;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (input[i] == '©') count++;
                else break;
            }

            var countSuffix = ChangeToLowerIndex(count.ToString());

            if (countSuffix == "₀") return input;

            return input.Substring(0, input.Length - count) + countSuffix;
        }

        public static string SequentToFormula(string input)
        {
            string output = "";

            var tempArray = input.Split('⊢');

            if (tempArray.Length == 1) { return input; }

            var premises = tempArray[0].Trim().Split(new char[] { ',', '\n' }).ToList();
            var conclusions = tempArray[1].Trim().Split(new char[] { ',', '\n' }).ToList();

            for (int i = 0; i < premises.Count; i++)
            {
                premises[i] = premises[i].Trim();
                if (premises[i].Length == 0)
                {
                    premises.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < conclusions.Count; i++)
            {
                conclusions[i] = conclusions[i].Trim();
                if (conclusions[i].Length == 0)
                {
                    conclusions.RemoveAt(i);
                    i--;
                }
            }

            output += "(";
            for (int i = 0; i < premises.Count; i++)
            {
                output += "(" + premises[i] + ")";
                if(i != premises.Count - 1) output += " ∧ ";
            }
            output += ") => (";
            for (int i = 0; i < conclusions.Count; i++)
            {
                output += "(" + conclusions[i] + ")";
                if (i != conclusions.Count - 1) output += " ∧ ";
            }
            output += ")";

            return output;
        }
        
    }

}
