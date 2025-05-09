// <copyright file="Formula.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// Author:    Isaac Huntsman
// Partner:   NA TODO: insert name
// Date:      9/5/24 (V1)
// Course:    CS 3500, University of Utah, School of Computing
// Copyright: CS 3500 and Isaac Huntsman - This work may not
//            be copied for use in Academic Coursework.
//
// I, Isaac Huntsman, certify that I wrote this code from scratch and
// did not copy it in part or whole from another source.  All
// references used in the completion of the assignments are cited
// in my README file.
//
// File Contents
//
//    Formula class for validating formulas; defines a lookup delegate and evaluate
//    function for computation.
// </summary>
namespace CS3500.Formula;

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers. This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    /// Variable is initialized in constructor as long as there is at least one token in
    /// provided formula variable in the constructor.
    /// The formula constructor then parses the formula and inputs all VALID tokens into
    /// this list. It is then used by getVariables method.
    /// </summary>
    private List<string> tokensInFormula;

    /// <summary>
    /// This stores the canonical representation of any given VALID formula.
    /// Normalized as in, whitespace trimmed and all letters in valid variables capitalized,
    /// and scientific notation shown as regular numbers. Used by ToString method. Output by constructor.
    /// </summary>
    private string fmlaAsNormalizedString;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        // rule 3, 4: closing and balanced paren rules.
        if (!this.ValidParens(formula))
        {
            throw new FormulaFormatException("parentheses don't match up.");
        }

        // split formula into tokens
        List<string> tokens = GetTokens(formula);

        // rule 1: one token rule
        if (tokens.Count == 0)
        {
            throw new FormulaFormatException("need at least one token besides whitespace");
        }

        // initialize tokens in formula only after verifying there is at least one in formula
        this.tokensInFormula = new ();

        // rule 5, 6: first & last token rule
        this.CheckFirstOrLastToken(tokens[0], "(");
        this.CheckFirstOrLastToken(tokens[tokens.Count - 1], ")");

        StringBuilder buildFormula = new StringBuilder();

        int loopFlag = 0;
        foreach (string token in tokens)
        {
            string opPattern = @"^[\+\-*/]$";

            // need this conditional as we check previous token for rule 7, 8
            if (loopFlag > 0)
            {
                string prevToken = tokens[loopFlag - 1];
                bool itsAnOperator = Regex.IsMatch(prevToken, opPattern);

                // rule 7: parenthesis/operator following rule
                if (prevToken.Equals("(") || itsAnOperator)
                {
                    this.CheckFollowingToken(token);
                }

                // rule 8: extra following rule
                bool prevTokenIsNum = double.TryParse(prevToken, out _);
                if (prevTokenIsNum || IsVar(prevToken) || prevToken.Equals(")"))
                {
                    this.CheckExtraFollowingToken(token);
                }
            }

            loopFlag++;

            // after checking rule 7, 8, proceed to check if:
            // var, operator, or paren exists, covering rule 2.
            if (IsVar(token)
                || Regex.IsMatch(token, opPattern)
                || token.Equals("(")
                || token.Equals(")"))
            {
                buildFormula.Append(token);
                this.tokensInFormula.Add(token);
                continue;
            }

            // special case for numbers, we need to normalize them
            // this includes normalization of numbers in scientific notation.
            double tokenAsNum;
            if (double.TryParse(token, out tokenAsNum))
            {
                buildFormula.Append(tokenAsNum);
                this.tokensInFormula.Add(token);
                continue;
            }
        }

        // update the normalized string. Happens only when entire formula has been parsed.
        this.fmlaAsNormalizedString = buildFormula.ToString().ToUpper();
    } // end of constructor

    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator ==(Formula f1, Formula f2)
    {
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !(f1 == f2);
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> setOfTokens = new ();

        foreach (string token in this.tokensInFormula)
        {
            if (IsVar(token))
            {
                setOfTokens.Add(token.ToUpper());
            }
        }

        return setOfTokens;
    }

    /// <summary>
    ///   <para>
    ///     Evaluates this Formula, using the lookup delegate to determine the values of
    ///     variables.
    ///   </para>
    ///   <remarks>
    ///     When the lookup method is called, it will always be passed a Normalized (capitalized)
    ///     variable name.  The lookup method will throw an ArgumentException if there is
    ///     not a definition for that variable token.
    ///   </remarks>
    ///   <para>
    ///     If no undefined variables or divisions by zero are encountered when evaluating
    ///     this Formula, the numeric value of the formula is returned.  Otherwise, a
    ///     FormulaError is returned (with a meaningful explanation as the Reason property).
    ///   </para>
    ///   <para>
    ///     This method does not throw any exceptions.
    ///   </para>
    /// </summary>
    /// <param name="lookup">
    ///   <para>
    ///     Given a variable symbol as its parameter, lookup returns the variable's (double) value
    ///     (if it has one) or throws an ArgumentException (otherwise).  This method should expect
    ///     variable names to be capitalized.
    ///   </para>
    /// </param>
    /// <returns> Either a double or a formula error, based on evaluating the formula.</returns>
    public object Evaluate(Lookup lookup)
    {
        Stack<double> val = new();
        Stack<string> op = new();

        List<string> tokens = GetTokens(fmlaAsNormalizedString);

        foreach (string token  in tokens)
        {
            // token is an number
            if (double.TryParse(token, out double tokenAsNum))
            {
                if(!ProcessOperator(tokenAsNum, op, val))
                {
                    return new FormulaError("DIV by 0 err");
                }
            }

            // token is a variable
            else if (IsVar(token))
            {
                // use lookup delegate to find value of variable
                try
                {
                    double varAsNum = lookup(token);

                    if (!ProcessOperator(varAsNum, op, val))
                    {
                        return new FormulaError("DIV by 0 err");
                    }
                }
                catch (ArgumentException)
                {
                    return new FormulaError("DIV by 0 err");
                }
            }

            // token is one of these 3 operators, we just push them onto the op stack
            else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
            {
                op.Push(token);
            }

            // token is one of these operators
            else if (token.Equals("+") || token.Equals("-"))
            {
                if (op.TryPeek(out string? topOper) && (topOper == "+" || topOper == "-"))
                {
                    Compute(val, op, topOper);
                }

                op.Push(token);
            }

            // token is a closing paren AND the top of the op stack has + or - on it
            else if (token.Equals(")"))
            {
                if (op.TryPeek(out string? topOp) && (topOp == "+" || topOp == "-"))
                {
                    // WILL be a "("
                    Compute(val, op, topOp);
                }

                op.Pop();

                if (op.TryPeek(out string? currOp) && (currOp == "/" || currOp == "*"))
                {
                    if (!Compute(val, op, currOp))
                    {
                        return new FormulaError("DIV by 0 err");
                    }
                }
            }
        } // end of foreach

        // last token. Either the operator stack has something on it,
        // so we do the final computation, or we've done it, so we just
        // return the final number.
        if (op.Count > 0)
        {
            double val2 = val.Pop();
            double val1 = val.Pop();
            string topOp = op.Pop();

            switch (topOp)
            {
                case "+":
                    return val1 + val2;

                case "-":
                    return val1 - val2;
            }
        }

        // the value after no errors encountered, and all variables have been looked up,
        // and all computations have been made.
        return val.Pop();
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        return obj is Formula formula &&
               fmlaAsNormalizedString == formula.fmlaAsNormalizedString;
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(fmlaAsNormalizedString);
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   </para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return this.fmlaAsNormalizedString;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results =
        [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }

    /// <summary>
    /// Processes a token with the operator on top of the stack, and the value on top of the stack.
    /// Mutates the value stack and operator stack.
    /// </summary>
    /// <param name="token">The token to use as part of the computation within the operator processor.</param>
    /// <param name="op">stack of operators. Pop operator to use in computation.</param>
    /// <param name="val">stack of values. We pop a value from here to use as part of computation.</param>
    /// <returns>true if value computed, false if a formula error object should be
    /// created due to a divide by 0 error only.</returns>
    private bool ProcessOperator(double token, Stack<string> op, Stack<double> val)
    {
        if (op.TryPeek(out string? topOp) && (topOp == "*" || topOp == "/"))
        {
            double topVal = val.Pop();
            op.Pop();

            switch (topOp)
            {
                case "/":
                    if (token == 0)
                    {
                        return false;
                    }

                    val.Push(topVal / token);
                    return true;

                case "*":
                    val.Push(topVal * token);
                    return true;
            }
        }

        val.Push(token);
        return true;
    }

    /// <summary>
    /// Needed whenever token is operator or closing paren, and value should be computed.
    /// Mutates stack by popping two vals and one op, computing the outcome, and pushing back to val stack.
    /// </summary>
    /// <param name="val">Stack of values. Pop twice off this and use both in computation.
    /// Mutates the stack of values also by pushing the computed value onto it.</param>
    /// <param name="op">Stack of operators.</param>
    /// <param name="oper">The operator to use in the computation.</param>
    private bool Compute(Stack<double> val, Stack<string> op, string oper)
    {
        double val2 = val.Pop();
        double val1 = val.Pop();
        op.Pop();

        if (oper == "*")
        {
            val.Push(val1 * val2);
        }
        else if (oper == "+")
        {
            val.Push(val1 + val2);
        }
        else if (oper == "-")
        {
            val.Push(val1 - val2);
        }
        else if (oper == "/" && val2 != 0)
        {
            val.Push(val1 / val2);
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// This helper method checks whether ( and ) are in the proper ordering and
    /// if the count of ( and ) matches.
    /// It uses a stack to accomplish the task.
    /// </summary>
    /// <param name="fmla">
    /// a string that may or may not contain one or more parenthesis.</param>
    /// <returns>true if the parentheses match and are in the proper order,
    /// false in all other cases.</returns>
    private bool ValidParens(string fmla)
    {
        Stack<char> st = new ();

        for (int i = 0; i < fmla.Length; i++)
        {
            char curr = fmla[i];

            if (curr == ')')
            {
                // try block needed when stack is empty.
                try
                {
                    if (st.Pop() == '(')
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else if (curr == '(')
            {
                st.Push(fmla[i]);
            }
        } // end of for loop

        if (st.Count != 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks end or beginning token of formula for rule 5, 6 adherence:
    /// first token must be number, var, or opening paren
    /// last token must be a number, var, or closing paren
    /// The way in which this helper is used avoids redundant code,
    /// as in, the code which calls this method always calls with the
    /// proper startOrEnd param (an opening or closing paren according to the context).
    /// </summary>
    /// <param name="token"> first or last token of formula.</param>
    /// <param name="openOrCloseBrace"> is only called with either ( or ), properly
    /// according to context.</param>
    /// <exception cref="FormulaFormatException">Throws exception if the first/last tokens
    /// don't adhere to the rules set out in the assignment.</exception>
    private void CheckFirstOrLastToken(string token, string openOrCloseBrace)
    {
        bool isNum = double.TryParse(token, out _);
        if (token.Equals(openOrCloseBrace) || IsVar(token) || isNum)
        {
            return;
        }

        throw new FormulaFormatException("last token must be num, var, or closing paren");
    }

    /// <summary>
    /// Rule 7: following an opening paren or operator:
    /// token can only be a number, var, or another opening paren.
    /// </summary>
    /// <param name="token">single token coming after opening paren.</param>
    /// <exception cref="FormulaFormatException">throw exception if not comply with rule 7.</exception>
    private void CheckFollowingToken(string token)
    {
        bool isNum = double.TryParse(token, out _);
        if (IsVar(token) || isNum || token.Equals("("))
        {
            return;
        }

        throw new FormulaFormatException("token following open paren or" +
            " operator must be a var, num, or another open paren");
    }

    /// <summary>
    /// Rule 8: extra following rule:
    /// any token that immediately follows a number, a variable, or a closing parenthesis
    /// must be either an operator or a closing parenthesis.
    /// </summary>
    /// <param name="token">the token following a number, var, or closing paren.</param>
    /// <exception cref="FormulaFormatException">throw exception if not comply with rule 8.</exception>
    private void CheckExtraFollowingToken(string token)
    {
        string opPattern = @"[\+\-*/]";

        if (Regex.IsMatch(token, opPattern) || token.Equals(")"))
        {
            return;
        }

        throw new FormulaFormatException("Token following num, var, or closing paren" +
            " must be an operator or a closing paren");
    }
}

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.  In general the expected behavior is that
///   the Lookup method will "know" about all variables in a formula
///   and return their appropriate value.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);
