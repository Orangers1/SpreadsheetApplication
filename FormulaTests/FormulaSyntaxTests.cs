// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// Author:    Isaac Huntsman
// Partner:   NA TODO: insert name
// Date:      8/30/24 (V1)
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
// Formula test class for testing the functionality of
// the formula constructor, testing if a formula is valid or not.
// </summary>

namespace CS3500.Formula;

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException( typeof( FormulaFormatException ) )]
    public void FormulaConstructor_TestNoTokens_Invalid( )
    {
        _ = new Formula( string.Empty );
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens, only whitespace.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestWhiteSpace_Invalid()
    {
        _ = new Formula("   ");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with a newline character.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNewLine_Invalid()
    {
        _ = new Formula("\n");
    }

    /// <summary>
    ///   <para>
    ///     This test creates a formula with "unnecessary" braces.
    ///     Expect no exception to be thrown.
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_NumberInParens_Valid()
    {
        _ = new Formula("(6)");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure no exception is thrown
    ///     when trying to create a formula with one token.
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSingleTokenNumber_Valid()
    {
        _ = new Formula("6");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure an exception is thrown
    ///     when trying to create a formula with one token, a letter, i.e., an improper
    ///     representation of a variable.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSingleTokenVariable_Invalid()
    {
        _ = new Formula("A");
    }

    /// <summary>
    ///   <para>
    ///     This test makes sure an exception is thrown
    ///     when trying to create a formula with lots of spaces.
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSpaces_Valid()
    {
        _ = new Formula("  6    -         5");
    }

    // --- Tests for Valid Token Rule ---

    /// <summary>
    /// <para>The only tokens allowed in the expression are (, ), +, -, *, /, variables, and numbers (positive numbers).
    /// minus signs "-" are treated as subtraction, not to signify a negative number.</para>
    /// <remarks>This test attempts to create a formula that contains an invalid token. Expect it to throw an exception.
    /// </remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokens_Invalid()
    {
        _ = new Formula("A-6#");
    }

    /// <summary>
    /// <para>The only tokens allowed in the expression are (, ), +, -, *, /, variables, and numbers (positive numbers).
    /// minus signs "-" are treated as subtraction, not to signify a negative number.</para>
    /// <remarks>This test attempts to create a formula
    /// that contains an invalid token. Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokensMultiple_Invalid()
    {
        _ = new Formula("@!%$$");
    }

    /// <summary>
    /// <para>The only tokens allowed in the expression are (, ), +, -, *, /, variables, and numbers (positive numbers).
    /// minus signs "-" are treated as subtraction, not to signify a negative number.</para>
    /// <remarks>This test attempts to create a valid formula that contains
    /// all non-number valid tokens, plus a variable and a few numbers.
    /// Expect it to throw no exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    public void FormulaConstructor_TestValidTokens_Valid()
    {
        _ = new Formula("(1+2-5*8/A6)");
    }

    /// <summary>
    /// <para>The only tokens allowed in the expression are (, ), +, -, *, /, variables, and numbers (positive numbers).
    /// minus signs "-" are treated as subtraction, not to signify a negative number.</para>
    /// <remarks>This test attempts to create a valid formula that contains all numbers and letters as variables with random operators.
    /// Expect it to throw no exception.
    /// This test also tests for "very long" formula validity, implicitly.
    /// </remarks>
    /// </summary>
    ///
    [TestMethod]
    public void FormulaConstructor_TestValidTokensNumbers_Valid()
    {
        _ = new Formula("9876543210-Aa1+Bb2*Cc3/Dd4+Ee5+Ff6+Gg7+" +
            "Hh8+Ii9+Jj10+Kk11+Ll12-Mm13+Nn14+Oo15+Pp16+Qq17+Rr18+" +
            "Ss19+Tt20+Uu21+Vv22+Ww23+Xx24+Yy25+Zz26");
    }

    // --- Tests for Closing Parenthesis Rule

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with only a closing brace.
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParen_Invalid()
    {
        _ = new Formula(")6+7");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with an extra closing brace
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenDouble_Invalid()
    {
        _ = new Formula("(6+7)/4)");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create a formula full of parentheses.
    /// This test is meant especially to test the parentheses parser.
    /// Obviously expect an exception to be thrown.
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestParensOnly_Invalid()
    {
        _ = new Formula("())");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with an unclosed opening paren
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenMissing_Invalid()
    {
        _ = new Formula("(6+7/4");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with an unclosed opening paren
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenMissingInMiddle_Invalid()
    {
        _ = new Formula("6+(7/4");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with an unclosed opening paren
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenMissingAtEnd_Invalid()
    {
        _ = new Formula("6+7/4(");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with one paren not closed.
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSwappedParens_Invalid()
    {
        _ = new Formula("(6+(7/(4)");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.
    /// </para>
    /// <remarks>This test attempts to create the valid version of the formula above.
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    public void FormulaConstructor_TestSwappedParens_Valid()
    {
         _ = new Formula("(6+(7/(4)))");
    }

    // --- Tests for Balanced Parentheses Rule

    /// <summary>
    ///   <para>
    ///     If we have an opening brace, make sure there is a closing brace.
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "(1+1)" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestBalancedBraces_Valid()
    {
        _ = new Formula("(1+1)");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with an extra set of parens
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestImproperBalancedParens_Invalid()
    {
        _ = new Formula("(6+7()/4)");
    }

    /// <summary>
    /// <para>All opening parentheses must be closed.
    /// There must be an equal number of opening and closing parentheses.
    /// There cannot be any "hanging" closing parens. This means:
    /// all closing parens must be closing an opening paren that occurs BEFORE it.</para>
    /// <remarks>This test attempts to create an invalid formula, with an unopened closing paren
    /// Expect it to throw an exception.</remarks>
    /// </summary>
    ///
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUnbalancedClosingParen_Invalid()
    {
        _ = new Formula("6+7/4)");
    }

    // --- Tests for First Token Rule

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid( )
    {
        _ = new Formula( "1+1" );
    }

    /// <summary>
    ///   <para>
    ///     The first token of an expression must be a number,
    ///     a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to start the formula with an operator.
    ///   Expect to throw an exception.
    ///   We've implicitly tested for formulas starting with opening parens and variables above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstToken_Invalid()
    {
        _ = new Formula("+6-5");
    }

    // --- Tests for  Last Token Rule ---

    /// <summary>
    ///   <para>
    ///     The last token of an expression must be a number,
    ///     a variable, or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to end a formula with a variable, properly
    ///   We've implicitly tested for formulas ending with
    ///   closing parens, and numbers above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula("6-A5");
    }

    /// <summary>
    ///   <para>
    ///     The last token of an expression must be a number,
    ///     a variable, or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to end a formula with an operator
    ///   We've implicitly tested for formulas ending with
    ///   closing parens, and numbers above.
    ///   Expect an exception to be thrown.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastToken_Invalid()
    {
        _ = new Formula("6-5*");
    }

    /// <summary>
    ///   <para>
    ///     The last token of an expression must be a number,
    ///     a variable, or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to end a formula with an opening paren
    ///   We've implicitly tested for formulas ending with
    ///   closing parens, and numbers above.
    ///   Expect an exception to be thrown.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenOpeningParen_Invalid()
    {
        _ = new Formula("6-5(");
    }

    // --- Tests for Parentheses/Operator Following Rule ---

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows an opening parenthesis
    ///   or an operator must be either a number, a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to follow an opening paren with
    ///   a closing paren.
    ///   Expect an exception to be thrown.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestEmptyParensFollowing_Invalid()
    {
        _ = new Formula("()");
    }

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows an opening parenthesis
    ///   or an operator must be either a number, a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to follow an operator with an operator
    ///   Expect an exception to be thrown.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestDoubleOperator_Invalid()
    {
        _ = new Formula("6**6");
    }

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows an opening parenthesis
    ///   or an operator must be either a number, a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to follow an opening paren with
    ///   a minus sign
    ///   An exception is thrown because for this assignment we only work with positive integers.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowingParen_Invalid()
    {
        _ = new Formula("(-6+4)");
    }

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows an opening parenthesis
    ///   or an operator must be either a number, a variable, or an opening parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to follow an operator with a closing paren
    ///   An exception is thrown because for this assignment we only work with positive integers.
    ///   Expect an exception to be thrown.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpenParenFollowingOperator_Invalid()
    {
        _ = new Formula("(4-8*)");
    }

    // --- Tests for Extra Following Rule ---

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows a number, a variable,
    ///   or a closing parenthesis must be either an operator or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to follow a number with an opening paren
    ///   An exception is thrown because for this assignment we only work with positive integers.
    ///   Expect an exception to be thrown.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   This formula is correct mathematically but we don't accept it for this assignment.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpenParenExtraFollowingNumber_Invalid()
    {
        _ = new Formula("4(4-8)");
    }

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows a number, a variable,
    ///   or a closing parenthesis must be either an operator or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to place an opening paren after an operator.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   This is the correct version of the formula attempt above.
    ///   Conclusion: must explicitly use * operator.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOpenParenExtraFollowingNumber_Valid()
    {
        _ = new Formula("4*(4-8)");
    }

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows a number, a variable,
    ///   or a closing parenthesis must be either an operator or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///   this test is valid because there is no token following the last number.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestOperatorExtraFollowingNumber_Valid()
    {
        _ = new Formula("4*6");
    }

    /// <summary>
    ///   <para>
    ///   Any token that immediately follows a number, a variable,
    ///   or a closing parenthesis must be either an operator or a closing parenthesis.
    ///   </para>
    ///   <remarks>
    ///   This test conforms to this rule, but does not conform to
    ///   the parentheses/operator following rule, so it fails.
    ///   We've implicitly tested well-formed formulas following this rule above.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorExtraFollowingNumber_Invalid()
    {
        _ = new Formula("4*6*");
    }

    // --- Tests for scientific notation ---

    /// <summary>
    ///   <para>
    ///   Scientific notation is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test creates a typical number in decimal-scientific notation
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSciNotation_Valid()
    {
        _ = new Formula("4.0e5");
    }

    /// <summary>
    ///   <para>
    ///   Scientific notation is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to create a variable using scientific notation for the number.
    ///   Expect an exception to be thrown.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSciNotationVar_Invalid()
    {
        _ = new Formula("G4e5");
    }

    /// <summary>
    ///   <para>
    ///   Scientific notation is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test creates another typical number in decimal-scientific notation,
    ///   but with a minus sign.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSciNotationNeg_Valid()
    {
        _ = new Formula("900.115e-5");
    }

    /// <summary>
    ///   <para>
    ///   Scientific notation is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attemps to create a number in scientific notation.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSciNotationNonDec_Valid()
    {
        _ = new Formula("8E-5");
    }

    /// <summary>
    ///   <para>
    ///   Scientific notation is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attemps to create a number in scientific notation.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSciNotationNonDecSimple_Valid()
    {
        _ = new Formula("8e5");
    }

    /// <summary>
    ///   <para>
    ///   Scientific notation is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attemps to create a number in scientific notation.
    ///   Expect an exception to be thrown.
    ///   Cannot have an operator follow an operator.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSciNotationNonDec_Invalid()
    {
        _ = new Formula("8e--5");
    }

    /// <summary>
    ///   <para>
    ///   Scientific notation is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attemps to create a number in scientific notation
    ///   with a subtraction operator in the mix.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestSciNotationB2B_Valid()
    {
        _ = new Formula("8e-5-9.113E-12");
    }

    // --- Tests for powers ---

    /// <summary>
    ///   <para>
    ///   No "carrot" to signify powers is allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to use the constructor to raise a number to a power.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestRaiseToPower_Invalid()
    {
        _ = new Formula("4^3*5");
    }

    // --- Tests for decimal numbers ---

    /// <summary>
    ///   <para>
    ///   Decimal numbers are allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to use the constructor to create an atypical decimal number
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestDec_Valid()
    {
        _ = new Formula("0.001");
    }

    /// <summary>
    ///   <para>
    ///   Decimal numbers are allowed.
    ///   </para>
    ///   <remarks>
    ///   This test attempts to use the constructor to create another atypical decimal number
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestDecDouble0_Valid()
    {
        _ = new Formula("00.101");
    }

    // === ToString tests ===

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   It should be normalized as in: spaces removed, x and y capitalized.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestBasic_Equal()
    {
        Formula x = new Formula("x1 + y1");

        Assert.AreEqual(x.ToString(), "X1+Y1");
    }

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a one-char formula, then checks the
    ///   canonical representation with ToString.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestOneChar_Equal()
    {
        Formula x = new Formula("1");

        Assert.AreEqual(x.ToString(), "1");
    }

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a complex formula with many extra spaces
    ///   and a number in scientific notation, and a variable.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestManySpaces_Equal()
    {
        Formula x = new Formula("1       + x5  / 4.000e2");

        Assert.AreEqual(x.ToString(), "1+X5/400");
    }

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create another formula with extra spaces,
    ///   this time with both scientific notation and regular decimal numbers, and a variable.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestDec_True()
    {
        Formula x = new Formula("1       + x5  / 4.015e-2 * 4.116");

        Assert.AreEqual(x.ToString(), "1+X5/0.04015*4.116");
    }

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   It should be normalized as in: spaces removed, x and y capitalized.
    ///   Expect assert true that these are not equal, there is an extraenous space.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestBasicCapsWithSpace_NotEqual()
    {
        Formula x = new Formula("x1 + y1");

        Assert.AreNotEqual(x.ToString(), "X1+ Y1");
    }

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   It should be normalized as in: spaces removed, x and y capitalized.
    ///   Expect assert true that these are not equal, the string compared looks correct
    ///   but x and y should be normalized, as in, capitalized.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestBasicNoCaps_Equal()
    {
        Formula x = new Formula("x1 + y1");

        Assert.AreNotEqual(x.ToString(), "x1+y1");
    }

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   Numbers in scientific notation should be outputted in their canonical form, as in,
    ///   not in scientific notation.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   The formula has one token, a number in scientific notation.
    ///   It should be outputted not in scientific notation.
    ///   We should be able to compare it as a regular double.
    ///   Expect the assert equal to be true.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestSciNotationSmall_Equal()
    {
        Formula x = new Formula("4.006e-5");
        string xAsString = x.ToString();
        double xAsNum;
        double.TryParse(xAsString, out xAsNum);

        Assert.AreEqual(xAsNum, 0.00004006);
    }

    /// <summary>
    ///   <para>
    ///   ToString normalizes formula; removes spaces and capitalizes all letters in variables.
    ///   Numbers in scientific notation should be outputted in their canonical form, as in,
    ///   not in scientific notation.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   The formula has one token, a number in scientific notation.
    ///   It should be outputted not in scientific notation.
    ///   We should be able to compare it as a regular double.
    ///   Expect the assert equal to be true.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void ToString_TestSciNotationBig_Equal()
    {
        Formula x = new Formula("8E4");
        string xAsString = x.ToString();
        double xAsNum;
        double.TryParse(xAsString, out xAsNum);

        Assert.AreEqual(xAsNum, 80000);
    }

    // === test getVariables ===

    /// <summary>
    ///   <para>
    ///   GetVariables returns a SET of all variables found in a given formula.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   The formula has one token, a variable.
    ///   The set should then contain just the variable,
    ///   and be of length 1.
    ///   Expect both assertions to be true.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void GetVariables_TestOneTokenVar_True()
    {
        Formula x = new Formula("b144");
        ISet<string> varsInFmla = x.GetVariables();

        Assert.IsTrue(varsInFmla.Contains("B144"));
        Assert.IsTrue(varsInFmla.Count() == 1);
    }

    /// <summary>
    ///   <para>
    ///   GetVariables returns a SET of all variables found in a given formula.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   The formula has three tokens, each the same variable
    ///   The set should then contain just the variable,
    ///   and be of length 1.
    ///   Expect both assertions to be true.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void GetVariables_TestThreeTokensOneVar_True()
    {
        Formula x = new Formula("b144+b144*b144");
        ISet<string> varsInFmla = x.GetVariables();

        Assert.IsTrue(varsInFmla.Contains("B144"));
        Assert.IsTrue(varsInFmla.Count() == 1);
    }

    /// <summary>
    ///   <para>
    ///   GetVariables returns a SET of all variables found in a given formula.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   The formula has three tokens, but none are variables.
    ///   expect set to be empty but not null
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void GetVariables_TestOperatorNoVars_True()
    {
        Formula x = new Formula("6+1");
        ISet<string> varsInFmla = x.GetVariables();

        Assert.IsTrue(varsInFmla.Count() == 0);
        Assert.IsNotNull(varsInFmla);
    }

    /// <summary>
    ///   <para>
    ///   GetVariables returns a SET of all variables found in a given formula.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a basic valid formula.
    ///   The formula has zero variables; just one token number
    ///   The set should then be empty but not null.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void GetVariables_TestZeroVars_MultiAsserts()
    {
        Formula x = new Formula("816");
        ISet<string> varsInFmla = x.GetVariables();

        Assert.IsFalse(varsInFmla.Contains("b144"));
        Assert.IsTrue(varsInFmla.Count() == 0);
        Assert.IsNotNull(varsInFmla);
    }

    /// <summary>
    ///   <para>
    ///   GetVariables returns a SET of all variables found in a given formula.
    ///   </para>
    ///   <remarks>
    ///   This test uses formula constructor to create a complex valid formula.
    ///   The formula has many variables, some repeated.
    ///   The set should have all the unique variables from the formula.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void GetVariables_TestManyVars_MultiAsserts()
    {
        Formula x = new Formula("b12-a66*4.0e-1+b12*2-r16/q1+b12*b12/b12");
        ISet<string> varsInFmla = x.GetVariables();

        Assert.IsTrue(varsInFmla.SetEquals(["B12", "A66", "R16", "Q1"]));
    }
}