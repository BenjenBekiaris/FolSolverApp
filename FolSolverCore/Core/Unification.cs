namespace FolSolverCore.Core
{
    public static class Unification
    {
        public static List<string[]>? Unify(Predicate predicate1, Predicate predicate2)
        {
            //Console.WriteLine("\n Unifying {0} and {1}", predicate1.PrintPredicate(), predicate2.PrintPredicate());

            var mGUnificator = new List<string[]>();
            if (predicate1.Name != predicate2.Name)
            {
                //Console.WriteLine("Predicate symbols are not equal ({0} =/= {1})", predicate1.Name, predicate2.Name);
                return null;
            }
            //Console.WriteLine("Predicate symbols are equal ({0})", predicate1.Name);

            if (predicate1.StringArguments.Length != predicate2.StringArguments.Length)
            {
                //Console.WriteLine("Number of arguments is not same ({0} =/= {1})", predicate1.StringArguments.Length, predicate2.StringArguments.Length);
                return null;
            }
            //Console.WriteLine("Number of arguments is equal ({0})", predicate1.StringArguments.Length);

            for (int i = 0; i < predicate1.StringArguments.Length; i++)
            {
                var unificator = Unify(predicate1.StringArguments[i], predicate2.StringArguments[i]);
                if (unificator == null) { return null; }
                if (unificator != new List<string[]>())
                {
                    ApplyUnificator(unificator, predicate1);
                    ApplyUnificator(unificator, predicate2);
                    ApplyUnificator(unificator, ref mGUnificator);
                    foreach (var substitution in unificator)
                    {
                        mGUnificator.Add(substitution);
                    }
                }
            }
            return mGUnificator;
        }

        private static List<string[]>? Unify(string argument1, string argument2)
        {
            //Console.WriteLine("\n Unifying {0} and {1}", argument1, argument2);

            var mGUnificator = new List<string[]>();

            if (argument1 == argument2)
            {
                //Console.WriteLine("{0} and {1} are equal, returning empty substitution", argument1, argument2);
                return mGUnificator;
            }

            if (Utils.IsVariable(argument1))
            {
                //Console.WriteLine("{0} is variable", argument1);

                if (Utils.FunctionContainsVariable(argument2, argument1))
                {
                    //Console.WriteLine("{0} is in {1}, returning NULL", argument1, argument2);
                    return null;
                }

                string[] substitution = { argument2.Trim(), argument1.Trim() };
                //Console.WriteLine("Adding new substitution: [{0}/{1}] to the unificator", argument2, argument1);

                mGUnificator.Add(substitution);
                return mGUnificator;
            }
            else if (Utils.IsVariable(argument2))
            {
                //Console.WriteLine("{0} is variable", argument2);

                if (Utils.FunctionContainsVariable(argument1, argument2))
                {
                    //Console.WriteLine("{0} is in {1}, returning NULL", argument2, argument1);
                    return null;
                }
                string[] substitution = { argument1.Trim(), argument2.Trim() };
                //Console.WriteLine("Adding new substitution: [{0}/{1}] to the unificator", argument1, argument2);

                mGUnificator.Add(substitution);
                return mGUnificator;
            }
            else if (Utils.IsFunction(argument1) && Utils.IsFunction(argument2))
            {
                //Console.WriteLine("{0} and {1} are both functions", argument1, argument2);

                var splitFunction1 = Utils.SplitFunction(argument1);
                var splitFunction2 = Utils.SplitFunction(argument2);

                if (splitFunction1[0] != splitFunction2[0])
                {
                    //Console.WriteLine("Function symbols are not equal ({0} =/= {1})", splitFunction1[0], splitFunction2[0]);
                    return null;
                }
                //Console.WriteLine("Function symbols are equal ({0})", splitFunction1[0]);

                var function1Arguments = Utils.SplitArguments(splitFunction1[1]);
                var function2Arguments = Utils.SplitArguments(splitFunction2[1]);

                if (function1Arguments.Length != function2Arguments.Length)
                {
                    //Console.WriteLine("Number of arguments is not equal ({0} =/= {1})", function1Arguments.Length, function2Arguments.Length);
                    return null;
                }
                //Console.WriteLine("Number of arguments is equal ({0})", function1Arguments.Length);

                for (int i = 0; i < function1Arguments.Length; i++)
                {
                    var unificator = Unify(function1Arguments[i], function2Arguments[i]);
                    if (unificator == null) { return null; }
                    if (unificator != new List<string[]>())
                    {
                        function1Arguments = ApplyUnificator(unificator, function1Arguments);
                        function2Arguments = ApplyUnificator(unificator, function2Arguments);
                        ApplyUnificator(unificator, ref mGUnificator);
                        foreach (var substitution in unificator)
                        {
                            mGUnificator.Add(substitution);
                        }
                        //Console.WriteLine("Current MGU: " + Utils.PrintUnificator(mGUnificator));
                    }
                }
            }
            return mGUnificator;
        }

        private static string[] ApplyUnificator(List<string[]> unificator, string[] functionArguments)
        {
            //Console.WriteLine("\n Applying {0} to {1}", Utils.PrintUnificator(unificator), string.Join(',', functionArguments));
            List<string> output = new List<string>();
            foreach (var substitution in unificator)
            {
                for (int i = 0; i < functionArguments.Length; i++)
                {
                    if (substitution[1] == functionArguments[i]) { output.Add(substitution[0]); }
                    else if (Utils.FunctionContainsVariable(functionArguments[i], substitution[1]))
                    {
                        output.Add(Utils.RewriteVariableInFunction(substitution[1], substitution[0], functionArguments[i]));
                    }
                    else { output.Add(functionArguments[i]); }
                }
            }
            //Console.WriteLine("Arguments after aplication: {0}", string.Join(',', output.ToArray()));
            return output.ToArray();
        }
        public static void ApplyUnificator(List<string[]> unificator, Predicate predicate)
        {
            //Console.WriteLine("\n Applying {0} to {1}", Utils.PrintUnificator(unificator), predicate.PrintPredicate());

            foreach (var substitution in unificator)
            {
                predicate.RewriteVariable(substitution[1], substitution[0]);
            }
        }
        private static void ApplyUnificator(List<string[]> unificator, ref List<string[]> mGUnificator)
        {
            //Console.WriteLine("\n Applying {0} to {1}", Utils.PrintUnificator(unificator), Utils.PrintUnificator(mGUnificator));

            foreach (var substitution in unificator)
            {
                for (int i = 0; i < mGUnificator.Count; i++)
                {
                    if (substitution[1] == mGUnificator[i][0]) { mGUnificator[i][0] = substitution[0]; }
                    else if (Utils.FunctionContainsVariable(mGUnificator[i][0], substitution[1]))
                    {
                        mGUnificator[i][0] = Utils.RewriteVariableInFunction(substitution[1], substitution[0], mGUnificator[i][0]);
                    }
                }
            }
        }
    }
}
