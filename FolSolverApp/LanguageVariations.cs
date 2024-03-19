namespace FolSolverApp
{
    public static class LanguageVariations
    {
        public static List<string> inputMessage = new List<string>
        {
            "Enter formula to resolve...",
            "Zadajte formulu pre rezolúciu..."
        };

        public static List<string> processButton = new List<string>
        {
            "Process formula",
            "Spracuj formulu"
        };

        static string[] processDescriptionsEN =
        {
            "Show procces of modifying formula",
            "Read formula",
            "Negated formula",
            "Implication rewritten as disjunction and negation",
            "Negation pushed to predicates",
            "Uniquely renamed quantified variables",
            "Prenex form of formula",
            "Skolem form of formula",
            "Ommited universal quantifiers",
            "Conjuctive normal form of formula",
            "CNF as set of clauses",
            "CNF after factorization"
        };
        static string[] processDescriptionsSK =
        {
            "Zobraziť postup úpravy formuly",
            "Prečítaná formula",
            "Negovaná formula",
            "Prepísanie implikácie pomocou negácie a disjunkcie",
            "Vnorenie negácie k predikátom",
            "Premenovanie kvantifikovaných premenných aby nedošlo k zámene nezávislých premenných s rovnakým menom",
            "Prepis formuly na prenexný tvar",
            "Prepis formuly na Skolemovský tvar",
            "Vynechanie všeobecných kvantifikátorov",
            "Prepis formuly do konjuktívnej normálovej formy",
            "Formula v podobe množiny klauzúl",
            "Množina klauzúl po faktorizácii"
        };
        public static List<string[]> processDescriptions = new List<string[]>
        {
            processDescriptionsEN, 
            processDescriptionsSK
        };

        static string[] tableHeadersEN =
        {
            "Step",
            "Formula",
            "Derivation",
            "Unificator"
        };
        static string[] tableHeadersSK =
        {
            "Krok",
            "Formula",
            "Derivácia",
            "Unifikátor"
        };
        public static List<string[]> tableHeaders = new List<string[]>
        {
            tableHeadersEN, 
            tableHeadersSK
        };

        public static List<string> copyButton = new List<string>
        {
            "Copy table to clipboard",
            "Skopírovať tabuľku do schránky"
        };

        public static List<string> chooseClause = new List<string>
        {
            "Choose a clause",
            "Vyber klauzulu"
        };

        public static List<string> unificationError = new List<string>
        {
            "Can't unify",
            "Nemožno unifikovať"
        };

        public static string[] availibleLanguages = {"EN", "SK"};

    }
}
