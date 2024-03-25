namespace FolSolverCore.Core
{
    public static class Factorization
    {
        public static void CNFFactorization(List<Predicate[]> cnf)
        {
            for (int i = 0; i < cnf.Count; i++)
            {
                cnf[i] = ClauseFactorization(cnf[i]);
            }
        }

        private static Predicate[] ClauseFactorization(Predicate[] clause)
        {
            var list = new List<Predicate>(clause);
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 1 + i; j < list.Count; j++)
                {
                    if (list[i].PrintPredicate() == list[j].PrintPredicate())
                    {
                        list.RemoveAt(j);
                        j--;
                        continue;
                    }
                    if (!Utils.PredicatesContainSameVariable(list[i], list[j]) && CheckUnifiability(list[i], list[j]))
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
            return list.ToArray();
        }

        private static bool CheckUnifiability(Predicate predicate1,  Predicate predicate2)
        {
            var predicate1Backup = (string[])predicate1.StringArguments.Clone();
            var predicate2Backup = (string[])predicate2.StringArguments.Clone();

            var mGUnificator = Unification.Unify(predicate1, predicate2);

            if (mGUnificator != null) { return true; }
            else 
            { 
                predicate1.StringArguments = predicate1Backup; 
                predicate2.StringArguments = predicate2Backup;
            }

            return false;
        }
    }
}
