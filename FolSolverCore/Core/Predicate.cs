namespace FolSolverCore.Core
{
    public class Predicate
    {
        private string? _name;
        private int? _arity;
        private string[] _stringArguments = Array.Empty<string>();
        private bool _negated = false;
        private List<Quantifier> _quantifiers = new List<Quantifier>();
        private List<string> _quantifiedVariables = new List<string>();

        public string? Name { get { return _name; } set { _name = value; } }
        public int? Arity { get { return _arity; } set { _arity = value; } }
        public string[] StringArguments { get { return _stringArguments; } set { _stringArguments = value; } }
        public bool Negated { get { return _negated; } }
        public List<Quantifier> Quantifiers { get { return _quantifiers; } }
        public List<string> QuantifiedVariables { get { return _quantifiedVariables; } }

        public Predicate()
        {
            
        }

        public void Negate()
        {
            _negated = !_negated;
            for (int i = 0; i < _quantifiers.Count; i++)
            {
                if (_quantifiers[i] == Quantifier.Existencial) _quantifiers[i] = Quantifier.Universal;
                else _quantifiers[i] = Quantifier.Existencial;
            }
        }

        public void MakeQuantifiedVariablesUnique(ref List<string> quantifiedVariables)
        {
            foreach (var variable in quantifiedVariables)
            { 
                for (int i = 0; i < _quantifiedVariables.Count; i++)
                { 
                    if (_quantifiedVariables[i] == variable) 
                    {
                        string newName = variable + '©';
                        _quantifiedVariables[i] = newName;
                        RewriteVariable(variable, newName);
                    }
                }  
            }

            foreach (var variable in _quantifiedVariables)
            {
                quantifiedVariables.Add(variable);
            }
        }

        public void RewriteVariable(string oldName, string newName)
        {
            for (int i = 0; i < _stringArguments.Length; i++)
            {
                if (_stringArguments[i] == oldName) { _stringArguments[i] = newName; }
                if (Utils.IsFunction(_stringArguments[i])) { _stringArguments[i] = Utils.RewriteVariableInFunction(oldName, newName, _stringArguments[i]); }

            }
            for (int i = 0; i < _quantifiedVariables.Count; i++)
            {
                if (_quantifiedVariables[i] == oldName) { _quantifiedVariables[i] = newName; }
            }
        }

        public void ClearQuantifiers()
        {
            _quantifiers.Clear();
            _quantifiedVariables.Clear();
        }

        public void ParsePredicate(string input)
        {
            
            int index = 0; 
            string predicate = input.Trim();
            char c;
            string name = "";
            string quantifiedVariable = "";
            string argumentsBuffer = "";

            c = predicate[index++];
            while (!Char.IsAsciiLetterUpper(c))
            {
                if (c == '∀')
                {
                    if (_negated) { _quantifiers.Add(Quantifier.Existencial); }
                    else { _quantifiers.Add(Quantifier.Universal); }
          
                    c = predicate[index++];
                    while (Char.IsAsciiLetterLower(c))
                    {
                        quantifiedVariable += c;
                        c = predicate[index++];
                    }
                    _quantifiedVariables.Add(quantifiedVariable);
                    quantifiedVariable = "";
                }
                else if (c == 'Ǝ')
                {
                    if (_negated) { _quantifiers.Add(Quantifier.Universal); }
                    else { _quantifiers.Add(Quantifier.Existencial); }
                    
                    c = predicate[index++];
                    while (Char.IsAsciiLetterLower(c))
                    {
                        quantifiedVariable += c;
                        c = predicate[index++];
                    }
                    _quantifiedVariables.Add(quantifiedVariable);
                    quantifiedVariable = "";
                }
                else if (c == '¬')
                {
                    _negated = true;
                    c = predicate[index++];
                }
                else
                {
                    c = predicate[index++];
                }
  
            }

            while (Char.IsLetterOrDigit(c))
            {
                name += c;
                if (index >= predicate.Length) break;
                c = predicate[index++];
            }

            if (index < predicate.Length)
            {
                c = predicate[index++];
                int bracketDepth = 1;

                while (bracketDepth > 0)
                {
                    if (c == '(') bracketDepth++;
                    else if (c == ')') bracketDepth--;

                    if (bracketDepth > 0) { argumentsBuffer += c; c = predicate[index++]; }
                }
            }

            _name = name;
            _stringArguments = Utils.SplitArguments(argumentsBuffer);
            _arity = _stringArguments.Length;
        }

        public string PrintPredicate()
        {
            string output = "";
            if (_name == null || _arity == null) { return output; }

            int index = 0;
            foreach (Quantifier quantifier in _quantifiers)
            {
                output += '(';

                if (quantifier == Quantifier.Existencial)
                {
                    output += 'Ǝ';
                }
                else
                {
                    output += '∀';
                }

                output += _quantifiedVariables[index++] + ')';
            }

            if (_negated) { output += '¬'; }

            output += _name;
            if (_stringArguments.Length > 0)
            {
                output += '(' + string.Join(',', _stringArguments) + ')';
            }
            return output;
        }

        public Predicate DeepCopy()
        {
            Predicate copy = new Predicate();
            copy._name = _name;
            copy._arity = _arity;
            
            copy._stringArguments = new string[_stringArguments.Length];
            Array.Copy(_stringArguments, copy._stringArguments, _stringArguments.Length);

            copy._negated = _negated;

            foreach (var quantifier in _quantifiers)
            {
                copy._quantifiers.Add(quantifier);
            }
            foreach (var variable in _quantifiedVariables)
            {
                copy._quantifiedVariables.Add(variable);
            }

            return copy;
        }
    }
}
