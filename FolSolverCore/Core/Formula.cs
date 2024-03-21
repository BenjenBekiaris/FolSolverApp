namespace FolSolverCore.Core
{
    public class Formula
    {
        private bool _negated = false;
        private LogicSymbol _logicSymbol = LogicSymbol.None;
        private List<Quantifier> _quantifiers = new List<Quantifier>();
        private List<string> _quantifiedVariables = new List<string>();

        private Formula? _leftSideF = null;
        private Predicate? _leftSideP = null;
        private Formula? _rightSideF = null;
        private Predicate? _rightSideP = null;

        public bool Negated { get { return _negated; } }
        public LogicSymbol LogicSymbol { get { return _logicSymbol; } }
        public List<Quantifier> Quantifiers { get { return _quantifiers; } }
        public List<string> QuantifiedVariables { get { return _quantifiedVariables; } }

        public Formula? LeftSideF { get { return _leftSideF; } set { _leftSideF = value; } }
        public Predicate? LeftSideP { get { return _leftSideP; } set { _leftSideP = value; } }
        public Formula? RightSideF { get { return _rightSideF; } set { _rightSideF = value; } }
        public Predicate? RightSideP { get { return _rightSideP; } set { _rightSideP= value; } }

        public Formula()
        {
            
        }

        public void Negate()
        {
            _negated = !_negated;
            for (int i = 0;  i < _quantifiers.Count; i++)
            {
                if (_quantifiers[i] == Quantifier.Existencial) _quantifiers[i] = Quantifier.Universal;
                else _quantifiers[i] = Quantifier.Existencial;
            }
        }

        public void RemoveImplications()
        {
            if (_logicSymbol == LogicSymbol.Implies)
            {
                _logicSymbol = LogicSymbol.Or;
                if (_leftSideF != null) { _leftSideF.Negate(); }
                else { _leftSideP.Negate(); }
            }
            
            if (_leftSideF != null) { _leftSideF.RemoveImplications(); }
            if (_rightSideF != null) { _rightSideF.RemoveImplications(); }
        }

        private void PushNegationInside()
        {
            if (!_negated) return;

            _negated = false;
            
            switch (_logicSymbol)
            {
                case LogicSymbol.And:
                    _logicSymbol = LogicSymbol.Or;
                    break;
                case LogicSymbol.Or:
                    _logicSymbol = LogicSymbol.And;
                    break;
                case LogicSymbol.Implies:
                    _logicSymbol = LogicSymbol.And;
                    if (_rightSideP != null) _rightSideP.Negate();
                    else _rightSideF?.Negate();
                    return;
                default:
                    break;
            }

            if (_leftSideP != null) _leftSideP.Negate();
            else _leftSideF?.Negate();
            if (_rightSideP != null) _rightSideP.Negate();
            else _rightSideF?.Negate();

            return;
        }
        public void PushNegationInsideCompletely()
        {
            PushNegationInside();
            if (_leftSideF != null) _leftSideF.PushNegationInsideCompletely();         
            if (_rightSideF != null) _rightSideF.PushNegationInsideCompletely();

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

            if (_leftSideF != null) { _leftSideF.MakeQuantifiedVariablesUnique(ref quantifiedVariables); }
            else { _leftSideP?.MakeQuantifiedVariablesUnique(ref quantifiedVariables); }
            if (_rightSideF != null) { _rightSideF.MakeQuantifiedVariablesUnique(ref quantifiedVariables); }
            else { _rightSideP?.MakeQuantifiedVariablesUnique(ref quantifiedVariables); }

        }

        public void MakeQuantifiedVariablesUnique()
        {
            List<string> quantifiedVariables = new List<string>();
            MakeQuantifiedVariablesUnique(ref quantifiedVariables);
            
            for (int i = 0; i < quantifiedVariables.Count; i++)
            {
                var newVariable = Utils.ReplaceApostrophiesWithNumber(quantifiedVariables[i]);
                if (newVariable != quantifiedVariables[i])
                {
                    RewriteVariable(quantifiedVariables[i], newVariable);
                }
            }

        }

        public void RewriteVariable(string oldName, string newName)
        {
            if (_leftSideF != null) { _leftSideF.RewriteVariable(oldName, newName); }
            else { _leftSideP?.RewriteVariable(oldName, newName); }
            if (_rightSideF != null) { _rightSideF.RewriteVariable(oldName, newName); }
            else { _rightSideP?.RewriteVariable(oldName, newName); }
        }

        public void CompareAndSwap()
        {
            List<Quantifier> leftSide = new List<Quantifier>();
            List<Quantifier> rightSide = new List<Quantifier>();
            List<Quantifier> possibleOutcome = new List<Quantifier>();

            int swap = 0;
            int noSwap = 0;
            int universalCount = 0;

            if (_leftSideF != null) leftSide = new List<Quantifier>(_leftSideF.Quantifiers);
            else if (_leftSideP != null) leftSide = new List<Quantifier>(_leftSideP.Quantifiers);

            if (_rightSideF != null) rightSide = new List<Quantifier>(_rightSideF.Quantifiers);
            else if (_rightSideP != null) rightSide = new List<Quantifier>(_rightSideP.Quantifiers);

            possibleOutcome.AddRange(leftSide);      
            possibleOutcome.AddRange(rightSide);
            foreach (var quantifier in possibleOutcome)
            {
                if (quantifier == Quantifier.Universal) universalCount++;
                if (quantifier == Quantifier.Existencial) noSwap += universalCount;
            }

            universalCount = 0;
            possibleOutcome.Clear();

            possibleOutcome.AddRange(rightSide);
            possibleOutcome.AddRange(leftSide);

            foreach (var quantifier in possibleOutcome)
            {
                if (quantifier == Quantifier.Universal) universalCount++;
                if (quantifier == Quantifier.Existencial) swap += universalCount;
            }

            if (swap < noSwap)
            {
                Formula? tempLeftF = _leftSideF;
                Formula? tempRightF = _rightSideF;
                Predicate? tempLeftP = _leftSideP;
                Predicate? tempRightP = _rightSideP;

                _rightSideF = tempLeftF;
                _leftSideF = tempRightF;
                _rightSideP = tempLeftP;
                _leftSideP = tempRightP;
            }
        }

        public void ToPrenexForm()
        {
            //first to prenex both sides, then compare and flip, then suck out quantifiers

            if (_leftSideF != null) _leftSideF.ToPrenexForm();
            if (_rightSideF != null) _rightSideF.ToPrenexForm();

            CompareAndSwap();

            if (_leftSideP != null) 
            {              
                _quantifiers.AddRange(_leftSideP.Quantifiers);
                _quantifiedVariables.AddRange(_leftSideP.QuantifiedVariables);
                _leftSideP.ClearQuantifiers();
            }

            if (_leftSideF != null)
            {
                _quantifiers.AddRange(_leftSideF.Quantifiers);
                _quantifiedVariables.AddRange(_leftSideF.QuantifiedVariables);
                _leftSideF.ClearQuantifiers();
            }

            if (_rightSideP != null)
            {
                _quantifiers.AddRange(_rightSideP.Quantifiers);
                _quantifiedVariables.AddRange(_rightSideP.QuantifiedVariables);
                _rightSideP.ClearQuantifiers();
            }

            if (_rightSideF != null)
            {
                _quantifiers.AddRange(_rightSideF.Quantifiers);
                _quantifiedVariables.AddRange(_rightSideF.QuantifiedVariables);
                _rightSideF.ClearQuantifiers();
            }

        }

        public void AddQuantifier(Quantifier quantifier, string variable)
        {
            _quantifiers.Add(quantifier);
            _quantifiedVariables.Add(variable);
        }

        public void ClearQuantifiers()
        {
            _quantifiers.Clear();
            _quantifiedVariables.Clear();
        }

        public void ToSkolemForm()
        {
            int range = _quantifiers.Count;
            int functionCount = 1;
            for (int i = 0; i < range; i++)
            {
                if (_quantifiers[i] == Quantifier.Existencial)
                {
                    string replacementFunction;
                    if (i == 0) { replacementFunction = "A" + Utils.ChangeToLowerIndex(functionCount.ToString()); } 
                    else
                    {
                        replacementFunction = "f" + Utils.ChangeToLowerIndex(functionCount.ToString()) + "(";
                        for (int j = 0; j < i; j++)
                        {
                            replacementFunction += _quantifiedVariables[j];
                            if (j + 1 != i) { replacementFunction += ","; }
                        }
                        replacementFunction += ")";
                    }
                    
                    RewriteVariable(_quantifiedVariables[i], replacementFunction);
                    functionCount++;
                    _quantifiers.RemoveAt(i);
                    _quantifiedVariables.RemoveAt(i);
                    i--;
                    range--;
                }            
            }
        }

        public List<Predicate[]> ToCNF()
        {
            List<Predicate[]> output = new List<Predicate[]>();
            List<Predicate[]> leftSide = new List<Predicate[]>();
            List<Predicate[]> rightSide = new List<Predicate[]>();

            if (_leftSideF != null) { leftSide = _leftSideF.ToCNF(); }
            else
            {
                Predicate[] clause = { _leftSideP };
                leftSide.Add(clause);
            }
            if (_rightSideF != null) { rightSide = _rightSideF.ToCNF(); }
            else
            {
                Predicate[] clause = { _rightSideP };
                rightSide.Add(clause);
            }

            if (_logicSymbol == LogicSymbol.And)
            {
                foreach (var clause in leftSide)
                {
                    output.Add(clause);
                }
                foreach (var clause in rightSide)
                {
                    output.Add(clause);
                }
            }
            else if (_logicSymbol == LogicSymbol.Or)
            {
                foreach (var clauseL in leftSide)
                {
                    foreach (var clauseR in rightSide)
                    {
                        output.Add(clauseL.Concat(clauseR).ToArray());
                    }
                }
            }
            return output;
        }

        public void ParseFormula(string formula)
        {
            int index = 0;
            int bracketDepth = 0;
            int[] splitPoint = { 0, 0, 2048}; //index of split, logic priority, bracket depth at the point of split
            char c;

            //find split point (logic symbol with highest priority in lowest bracket depth)
            while (index < formula.Length) 
            {
                c = formula[index];
                switch (c)
                {
                    case '(':
                        bracketDepth++;
                        break;                   
                    case ')':
                        bracketDepth--;
                        break;
                   
                    case '∧':
                        if ((bracketDepth <= splitPoint[2] && 4 > splitPoint[1]) || (bracketDepth < splitPoint[2]))
                        {
                            splitPoint[0] = index;
                            splitPoint[1] = 4;
                            splitPoint[2] = bracketDepth;
                        }
                        break;

                    case '∨':
                        if ((bracketDepth <= splitPoint[2] && 5 > splitPoint[1]) || (bracketDepth < splitPoint[2]))
                        {
                            splitPoint[0] = index;
                            splitPoint[1] = 5;
                            splitPoint[2] = bracketDepth;
                        }
                        break;

                    case '=':
                        if (formula[index + 1] == '>')
                        {
                            if ((bracketDepth <= splitPoint[2] && 6 > splitPoint[1]) || (bracketDepth < splitPoint[2]))
                            {
                                splitPoint[0] = index;
                                splitPoint[1] = 6;
                                splitPoint[2] = bracketDepth;
                            }
                            index++;
                        }
                        break;
                }
                index++;   
            }
       
            index = 0;
            bracketDepth = 0;
            string quantifiedVariable = "";

            //find quantifiers and negation with range on whole formula (quantifiers which bracket depth is the same as bracket depth of splitpoint)
            //quantifiers that are in range of negation are automaticaly stored negated and pushed out of negation range
            while (index < formula.Length) 
            {
                c = formula[index++];
                switch (c)
                {
                    case '(':
                        bracketDepth++;
                        break;
                    case ')':
                        bracketDepth--;
                        break;
                    case '∀':
                        if (bracketDepth == splitPoint[2])
                        {
                            if (_negated) { _quantifiers.Add(Quantifier.Existencial); }
                            else { _quantifiers.Add(Quantifier.Universal); }

                            c = formula[index++];
                            while (Char.IsAsciiLetterLower(c))
                            {
                                quantifiedVariable += c;
                                c = formula[index++];    
                            }
                            _quantifiedVariables.Add(quantifiedVariable);
                            
                            index -= quantifiedVariable.Length + 2; //move index pointer to the quantifier character
                            formula = formula.Remove(index, quantifiedVariable.Length + 1); //remove from formula string quantifier and variables it quantifies
                            splitPoint[0] -= 1 + quantifiedVariable.Length; //move splitpoint index back by the number of characters removed

                            quantifiedVariable = "";
                            
                        }
                        break;
                    case 'Ǝ':
                        if (bracketDepth == splitPoint[2])
                        {
                            if (_negated) { _quantifiers.Add(Quantifier.Universal); }
                            else { _quantifiers.Add(Quantifier.Existencial); }

                            c = formula[index++];
                            while (Char.IsAsciiLetterLower(c))
                            {
                                quantifiedVariable += c;
                                c = formula[index++];
                            }
                            _quantifiedVariables.Add(quantifiedVariable);
                            
                            index -= quantifiedVariable.Length + 2 ; //move index pointer to the quantifier character
                            formula = formula.Remove(index, quantifiedVariable.Length + 1); //remove from formula string quantifier and variables it quantifies
                            splitPoint[0] -= 1 + quantifiedVariable.Length; //move splitpoint index back by the number of characters removed
                            
                            quantifiedVariable = "";
                        }
                        break;
                    case '¬':
                        if (bracketDepth < splitPoint[2])
                        {
                            _negated = true;
                            index--;
                            formula = formula.Remove(index,1);
                            splitPoint[0]--;
                        }
                        break;
                }
            }
  
            string leftHalf = formula.Substring(startIndex: 0, length: splitPoint[0]);

            string rightHalf;
            if (splitPoint[1] != 6) { rightHalf = formula.Substring(splitPoint[0] + 1); }
            else { rightHalf = formula.Substring(splitPoint[0] + 2); }

            if (Utils.IsPredicate(leftHalf))
            {
                _leftSideP = new Predicate();
                _leftSideP.ParsePredicate(leftHalf.Trim());
            }
            else
            {
                _leftSideF = new Formula();
                _leftSideF.ParseFormula(leftHalf.Trim());
            }

            if (Utils.IsPredicate(rightHalf))
            {
                _rightSideP = new Predicate();
                _rightSideP.ParsePredicate(rightHalf.Trim());
            }
            else
            {
                _rightSideF = new Formula();
                _rightSideF.ParseFormula(rightHalf.Trim());
            }

            _logicSymbol = (LogicSymbol)splitPoint[1];
        }

        public string PrintFormula()
        {
            string output = "";
            if (_logicSymbol == LogicSymbol.None) { return  output; }

            //print out quantifiers
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

            output += "(";

            //print out left side of formula
            if (_leftSideP != null) 
            {
                output += _leftSideP.PrintPredicate() + " ";           
            }
            else
            {
                output += _leftSideF.PrintFormula() + " ";
            }

            //print out logical connective
            switch (_logicSymbol)
            {
                case LogicSymbol.And:
                    output += '∧';
                    break;

                case LogicSymbol.Or:
                    output += '∨';
                    break;

                case LogicSymbol.Implies:
                    output += "=>";
                    break;
            }

            //print out right side of formula
            if (_rightSideP != null)
            {
                output += " " + _rightSideP.PrintPredicate();
            }
            else
            {
                output += " " + _rightSideF.PrintFormula();
            }

            output += ")";

            return output;
        }

    }
}
