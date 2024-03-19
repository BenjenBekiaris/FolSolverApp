namespace FolSolverApp
{
    public static class ExampleFormulas
    {
        private static List<string> examples = new List<string>()
        {
            "P(f(x1,x3,x2)) => P(f(g(x2),j(x4),h(x3,a)))",
            "((P(x) ∧ Q(y)) => R(z)) ∧ (¬R(A) ∨ (S(B) ∧ T(C))) => ((P(x) ∧ Q(y)) => (S(B) ∧ T(C)))",
        };

        public static List<string> Examples { get { return examples; } }
    }
}
