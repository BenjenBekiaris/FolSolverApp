namespace FolSolverCore.Core
{
    public static class Unification
    {
        public static List<string[]>? Unify(Predicate predicate1, Predicate predicate2)
        {
            var mGUnificator = new List<string[]>();
            if (predicate1.Name != predicate2.Name)
            {
                return null;
            }

            if (predicate1.StringArguments.Length != predicate2.StringArguments.Length)
            {
                return null;
            }

            for (int i = 0; i < predicate1.StringArguments.Length; i++)
            {
                var unificator = Unify(predicate1.StringArguments[i], predicate2.StringArguments[i]);
                if (unificator == null) { return null; }
                if (unificator != new List<string[]>())
                {
                    ApplyUnificator(unificator, predicate1, i);
                    ApplyUnificator(unificator, predicate2, i);
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

            var mGUnificator = new List<string[]>();

            if (argument1 == argument2)
            {
                return mGUnificator;
            }

            if (Utils.IsVariable(argument1))
            {
                if (Utils.FunctionContainsVariable(argument2, argument1))
                {
                    return null;
                }

                string[] substitution = { argument2.Trim(), argument1.Trim() };

                mGUnificator.Add(substitution);
                return mGUnificator;
            }
            else if (Utils.IsVariable(argument2))
            {
                if (Utils.FunctionContainsVariable(argument1, argument2))
                {
                    return null;
                }
                string[] substitution = { argument1.Trim(), argument2.Trim() };

                mGUnificator.Add(substitution);
                return mGUnificator;
            }
            else if (Utils.IsFunction(argument1) && Utils.IsFunction(argument2))
            {
                var splitFunction1 = Utils.SplitFunction(argument1);
                var splitFunction2 = Utils.SplitFunction(argument2);

                if (splitFunction1[0] != splitFunction2[0])
                {
                    return null;
                }

                var function1Arguments = Utils.SplitArguments(splitFunction1[1]);
                var function2Arguments = Utils.SplitArguments(splitFunction2[1]);

                if (function1Arguments.Length != function2Arguments.Length)
                {
                    return null;
                }

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
                    }
                }
            }
            return mGUnificator;
        }

        private static string[] ApplyUnificator(List<string[]> unificator, string[] functionArguments)
        {
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
            return output.ToArray();
        }
        public static void ApplyUnificator(List<string[]> unificator, Predicate predicate, int counter)
        {
            foreach (var substitution in unificator)
            {
                predicate.RewriteVariable(substitution[1], substitution[0], counter);
            }
        }
        private static void ApplyUnificator(List<string[]> unificator, ref List<string[]> mGUnificator)
        {
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
