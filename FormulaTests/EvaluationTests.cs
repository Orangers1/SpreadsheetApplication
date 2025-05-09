// <copyright file="EvaluationTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// Author:    Isaac Huntsman
// Partner:   NA TODO: insert name
// Date:      9/19/24 (V1)
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
// Formula test class for testing the functionality solely
// of the Evaluate function which takes in a Lookup delegate.
// </summary>

namespace CS3500.Formula;
using System.Text;

/// <summary>
/// Tests Evaluate, operator overload, lookup delegate, and variable checking functionality.\// Some of these tests use a standard function for use with the Evaluate parameter.
/// Some of these tests use lambda functions for use with the Evaluate parameter.
/// Some of these tests have variables.
/// Some do not have variables.
/// eval errors:
/// Divide by Zero →  new Formula(“1/0”)
/// Unknown Variable → new Formula(“A1”) (A1 DNE in this example)
/// Also testing equality, ==, !=, and hashing.
/// </summary>
[TestClass]
public class EvaluationTests
{
    /// <summary>
    /// Standard function to be passed to Evaluate in a few tests below.
    /// Fulfills Lookup requirement as it takes in a string and returns a double
    /// (or throws an exception).
    /// </summary>
    /// <param name="variableName">The variable to lookup.</param>
    /// <returns>A double value depending on the variableName, or an exception if variable DNE.</returns>
    /// <exception cref="ArgumentException">throws if variableName DNE.</exception>
    public static double LookupVars(string variableName)
    {
        // Define values for variables
        if (variableName == "A1")
        {
            return 5;
        }
        else if (variableName == "B2")
        {
            return 20;
        }
        else if (variableName == "C3")
        {
            return 25;
        }
        else
        {
            throw new ArgumentException($"Variable {variableName} is not defined.");
        }
    }

    /// <summary>
    /// Attempt to divide by 0, no variables. Expect that the object output
    /// is an instance of the FormulaError type.
    /// </summary>
    [TestMethod]
    public void Constructor_DIVNoVars_FormulaError()
    {
        Formula myFormula = new Formula("1/0");

        // dummy lambda
        object result = myFormula.Evaluate((s) => 55);

        // does not throw, but is an instance of a formula error.
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }

    /// <summary>
    /// Lookup defined with lambda, formula constructed with 2 variables
    /// both defined in lambda, simple check if sum is correct.
    /// </summary>
    /// <exception cref="ArgumentException">throw exception if either variable cannot be found.</exception>
    [TestMethod]
    public void Evaluate_BigLambda_Equals()
    {
        Formula myFormula = new Formula("A1 + B2");

        // Lambda function to ACT as the Lookup delegate
        // Here's how it works: (string) => double
        // it's a "lone lambda"
        Lookup lookup = (variableName) =>
        {
            // set variable vals for test
            if (variableName == "A1")
            {
                return 5;
            }
            else if (variableName == "B2")
            {
                return 20;
            }
            else
            {
                throw new ArgumentException($"Variable {variableName} DNE.");
            }
        };

        object result = myFormula.Evaluate(lookup);
        Assert.IsTrue(Convert.ToDouble(result).Equals(25));
    }

    /// <summary>
    /// Create formula object with no variables, and so pass a dummy lambda to Evaluate.
    /// Check that sum computed correctly.
    /// </summary>
    [TestMethod]
    public void Evaluate_SmallLambda_Equals()
    {
        Formula myFormula = new Formula("1+1");

        // returns either double or formula error, so object type.
        object result = myFormula.Evaluate((s) => 0);
        Assert.IsTrue(Convert.ToDouble(result).Equals(2));
    }

    /// <summary>
    /// Create formula object with no variables, and so pass a dummy lambda to Evaluate.
    /// Use many parens and check that evaluate works.
    /// </summary>
    [TestMethod]
    public void Evaluate_SmallLambdaManyParens_Equals()
    {
        Formula myFormula = new Formula("(((((1+1)))))");

        // returns either double or formula error, so object type.
        object result = myFormula.Evaluate((s) => 0);
        Assert.IsTrue(Convert.ToDouble(result).Equals(2));
    }

    /// <summary>
    /// Calls Evaluate on a formula with 3 variables, all defined in a standard function above.
    /// Expect sum to compute correctly.
    /// </summary>
    [TestMethod]
    public void Evaluate_StandardFunction_Equals()
    {
        Formula myFormula = new Formula("A1+B2-C3");

        object result = myFormula.Evaluate(LookupVars);
        Assert.IsTrue(Convert.ToDouble(result).Equals(0));
    }

    /// <summary>
    /// Tests the use of a lambda that evaluates all strings to 4.
    /// </summary>
    [TestMethod]
    public void Evaluate_SmallSimpleLambda_Equals()
    {
        Formula myFormula = new Formula("A1+(B2-C3)");

        object result = myFormula.Evaluate((s) => 4);

        // 4 + 4 - 4 = 4
        Assert.IsTrue(Convert.ToDouble(result).Equals(4));
    }

    /// <summary>
    /// Tests the use of a lambda that evaluates all strings to 4,
    /// with two numbers in parens.
    /// </summary>
    [TestMethod]
    public void Evaluate_DivideInParens_Equals()
    {
        Formula myFormula = new Formula("(3/3)");

        object result = myFormula.Evaluate((s) => 4);

        // 3 / 3 = 1
        Assert.IsTrue(Convert.ToDouble(result).Equals(1));
    }

    /// <summary>
    /// Tests the use of a lambda that evaluates all strings to 4,
    /// with many numbers and divide and multiply ops within parens.
    /// </summary>
    [TestMethod]
    public void Evaluate_DivideInParensMultiple_FormulaError()
    {
        Formula myFormula = new Formula("0/(3/3*6/1/6)/0");

        object result = myFormula.Evaluate((s) => 4);

        Assert.IsTrue(result is FormulaError);
    }

    /// <summary>
    /// Tests subtraction without parens and no variables.
    /// </summary>
    [TestMethod]
    public void Evaluate_Subtraction_FormulaError()
    {
        Formula myFormula = new Formula("6-3");

        object result = myFormula.Evaluate((s) => 4);

        Assert.IsTrue(Convert.ToDouble(result).Equals(3));
    }

    /// <summary>
    /// Tests subtraction with parens and no variables.
    /// </summary>
    [TestMethod]
    public void Evaluate_SubtractionWithParens_FormulaError()
    {
        Formula myFormula = new Formula("(6-3)");

        object result = myFormula.Evaluate((s) => 4);

        Assert.IsTrue(Convert.ToDouble(result).Equals(3));
    }

    /// <summary>
    /// Tests the use of a lambda that evaluates all strings to 4,
    /// with two Variables in parens.
    /// </summary>
    [TestMethod]
    public void Evaluate_DivideInParensVars_Equals()
    {
        // error happens with parens. Doesnt matter if nums or vars are used.
        Formula myFormula = new Formula("(A6/A6)");

        object result = myFormula.Evaluate((s) => 4);

        // 4 / 4 = 1
        Assert.IsTrue(Convert.ToDouble(result).Equals(1));
    }

    /// <summary>
    /// Tests the use of a lambda that evals the denominator var to 0, and an intermediate var to 0.
    /// </summary>
    [TestMethod]
    public void Evaluate_DIV0SmallSimpleLambda_FormulaError()
    {
        Formula myFormula = new Formula("A1+(B2-C3/C18)/C100");

        object result = myFormula.Evaluate((s) =>
        {
            if (s.Equals("C100") || s.Equals("C18"))
            {
                return 0;
            }
            else if (s.Equals("A1") || s.Equals("B2") || s.Equals("C3"))
            {
                return 666;
            }
            else
            {
                throw new ArgumentException("Var not found");
            }
        });

        Assert.IsTrue(result is FormulaError);
    }

    /// <summary>
    /// Tests the use of a lambda that evaluates all strings to 4, but
    /// one token is a number and one is a variable.
    /// </summary>
    [TestMethod]
    public void Evaluate_SmallLambdaVarAndNum_Equals()
    {
        Formula myFormula = new Formula("A1+66");

        object result = myFormula.Evaluate((s) => 4);

        // 4 + 66
        Assert.IsTrue(Convert.ToDouble(result).Equals(70));
    }

    /// <summary>
    /// Tests the use of a lambda that evaluates all strings to a number,
    /// but only one token is used in constructor and it is not a variable,
    /// but a valid number.
    /// </summary>
    [TestMethod]
    public void Evaluate_LambdaNoVar_Equals()
    {
        Formula myFormula = new Formula("12");

        object result = myFormula.Evaluate((s) => 365);

        Assert.IsTrue(Convert.ToDouble(result).Equals(12));
    }

    /// <summary>
    /// Tests the functionality of a lambda passed to evaluate that does not define
    /// the variable used in the constructor.
    /// </summary>
    /// <exception cref="ArgumentException">Expect to throw an exception because
    /// the variable is not defined in Lookup type (lambda) passed to Evaluate.</exception>
    [TestMethod]
    public void Evaluate_LambdaVarOnly_FmlaErrTrue()
    {
        Formula myFormula = new Formula("A1");

        var result = myFormula.Evaluate((s) =>
        {
            if (s == "B4")
            {
                return 6;
            }
            else
            {
                throw new ArgumentException($"Variable {s} DNE.");
            }
        });

        Assert.IsTrue (result is FormulaError);
    }

    /// <summary>
    ///   test division by zero when it happens due to variable evals.
    /// </summary>
    [TestMethod]
    public void TestDivideByZeroVars()
    {
        Formula f = new("(5 + P1) / (G9 - 6)");
        var result = f.Evaluate(s => 6);
        Assert.IsInstanceOfType(result, typeof(FormulaError));
    }

    /// <summary>
    /// When formula constructed with numbers only, lambda shouldn't affect it in any way.
    /// </summary>
    [TestMethod]
    public void Evaluate_NumOnlyWithLamda_Equals()
    {
        Formula myFormula = new Formula("6+6");

        object result = myFormula.Evaluate((s) =>
        {
            if (s == "B4")
            {
                return 6;
            }
            else
            {
                throw new ArgumentException($"Variable {s} DNE.");
            }
        });

        Assert.IsTrue(Convert.ToDouble(result).Equals(12));
    }

    /// <summary>
    /// When formula constructor called with numbers only, standard function used as Lookup
    /// should not affect outcome in any way.
    /// </summary>
    [TestMethod]
    public void Evaluate_NumsOnlyWithStandardFunction_Equals()
    {
        Formula myFormula = new Formula("6+6");

        object result = myFormula.Evaluate(LookupVars);

        Assert.IsTrue(Convert.ToDouble(result).Equals(12));
    }

    /// <summary>
    /// DIV0 should return FormulaError object.
    /// </summary>
    [TestMethod]
    public void Evaluate_DIV0WithParens_Equals()
    {
        Formula myFormula = new Formula("((1)/0)");

        object result = myFormula.Evaluate(LookupVars);

        Assert.IsTrue(result is FormulaError);
    }

    /// <summary>
    /// Test when multiple multiples are used with nums only.
    /// </summary>
    [TestMethod]
    public void Evaluate_MultipleMultiplies_Equals()
    {
        Formula myFormula = new Formula("6*6*(1*1)");

        object result = myFormula.Evaluate(LookupVars);

        Assert.IsTrue(Convert.ToDouble(result).Equals(36));
    }

    /// <summary>
    /// Tests case when variable in denominator evals to 0.
    /// </summary>
    [TestMethod]
    public void Evaluate_DIV0Vars_FormulaError()
    {
        Formula myFormula = new Formula("F1/B666");

        object result = myFormula.Evaluate((s) =>
        {
            if (s.Equals("B666"))
            {
                return 0;
            }
            else if (s.Equals("F1"))
            {
                return 666;
            }
            else
            {
                throw new ArgumentException("Var not found");
            }
        });

        Assert.IsTrue(result is FormulaError);
    }

    // *********** op overload testing *********** //

    /// <summary>
    /// Two Formulas are considered equal if their canonical string representations
    /// (as defined by ToString) are equal.
    /// </summary>
    /// <para>Variables are converted to their canonical string representation.
    /// They look different but because of our overload they are the same.</para>
    [TestMethod]
    public void OverloadEquivalence_TwoVars_Equal()
    {
        Formula myFormula = new Formula("A6+B6");
        Formula myFormula1 = new Formula("a6   +   b6");

        Assert.IsTrue(myFormula == myFormula1);
    }

    /// <summary>
    /// Testing override of !=, using the notion of equality from the Equals method.
    /// </summary>
    /// <para>These two strings are obviously not equal (because of their difference,
    /// and because of their canonical string representation difference); we confirm that here.</para>
    [TestMethod]
    public void OverloadNonEquivalence_TwoVars_Equal()
    {
        Formula myFormula = new Formula("A6+B6");
        Formula myFormula1 = new Formula("A6-B6");

        Assert.IsTrue(myFormula != myFormula1);
    }

    /// <summary>
    /// f1.Equals(f2) only when the canonical string reps are the same.
    /// </summary>
    /// <para>Even though these formulas both eval to 2, they are not equal,
    /// because of our Equals overload. Note that they are also not equal with no overload.</para>
    [TestMethod]
    public void OverloadEquals_NumsOnly_NotEqual()
    {
        Formula f1 = new Formula("1+1");
        Formula f2 = new Formula("2");

        Assert.IsFalse(f1.Equals(f2));
    }

    /// <summary>
    /// f1.Equals(f2) only when the canonical string reps are the same.
    /// </summary>
    /// <para>Formulas are the same canonically.</para>
    [TestMethod]
    public void OverloadEquals_NumsOnly_Equals()
    {
        Formula f1 = new Formula("A6+A4");
        Formula f2 = new Formula("a6+a4");

        Assert.IsTrue(f1.Equals(f2));
    }

    // ********* getHashCode testing ********** //

    /// <summary>
    /// If f1.Equals(f2), then it must be the
    /// case that f1.GetHashCode() == f2.GetHashCode().
    /// </summary>
    /// <para>These two formulas, after converting to canonical string representation,
    /// are equivalent, so they should be guaranteed to have the same hash.</para>
    [TestMethod]
    public void GetHashCode_TwoVars_Equals()
    {
        Formula myFormula = new Formula("A6+B6");
        Formula myFormula1 = new Formula("    a6 + b6       ");

        Assert.AreEqual(myFormula.GetHashCode(), myFormula1.GetHashCode());
    }

    /// <summary>
    /// If !f1.Equals(f2), then it should almost always be the
    /// case that f1.GetHashCode() != f2.GetHashCode().
    /// </summary>
    /// <para>These two formulas, after converting to canonical string representation,
    /// are not equivalent, so they should have differing hash codes.</para>
    [TestMethod]
    public void GetHashCode_TwoVars_AreNotEqual()
    {
        Formula myFormula = new Formula("A6+B6");
        Formula myFormula1 = new Formula("B6+A6");

        Assert.AreNotEqual(myFormula.GetHashCode(), myFormula1.GetHashCode());
    }
}