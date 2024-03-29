﻿namespace FolSolverApp
{
    public static class ExampleFormulas
    {
        private static List<string> examples = new List<string>()
        {
            "P(f(x1,x3,x2)) => P(f(g(x2),j(x4),h(x3,a)))",
            "((P(x) ∧ Q(y)) => R(z)) ∧ (¬R(A) ∨ (S(B) ∧ T(C))) => ((P(x) ∧ Q(y)) => (S(B) ∧ T(C)))",
            "P(Punto)\n(∀x)(P(x) => Z(x))\n(∀x)(Z(x) => ¬R(x))\n⊢\n¬R(Punto)",
            "(∀x)(V(x) => O(x)), (∀x)(O(x) => Z(x)), V(Horislav) ⊢ Z(Horislav)",
        };

        public static List<string> Examples { get { return examples; } }
    }
}
