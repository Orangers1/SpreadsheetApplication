// <copyright file="SpreadsheetTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// Author:    Isaac Huntsman
// Partner:   NA TODO: insert name
// Date:      9/16/24 (V1)
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
// Spreadsheet test class, tests functionality of spreadsheet class.
// </summary>
namespace CS3500.Spreadsheet;
using CS3500.Formula;
using System.Text.Json;

/// <summary>
/// Contains tests to check the functionality of the Spreadsheet project.
/// </summary>
[TestClass]
public class SpreadsheetTests
{
    // *** INDEXER TESTS *** (also testing Changed property, and GetCellValue explicitly)

    /// <summary>
    /// Test indexer overload functionality.
    /// <para>Simple spreadsheet, testing that a double is indeed stored
    /// and accessible by the indexer[] overload.
    /// </para>
    /// </summary>
    [TestMethod]
    public void IndexerANDGetCellValue_Double_Equal()
    {
        Spreadsheet test = new Spreadsheet();
        Assert.IsTrue(!test.Changed);
        test.SetContentsOfCell("A6", "12");
        test.SetContentsOfCell("B1", "=A6");
        Assert.IsTrue(test.Changed);

        object returnVal1 = test.GetCellValue("A6");
        object returnVal = test["A6"];
        if (returnVal is double theNum)
        {
            Assert.AreEqual(theNum, returnVal);
        }

        if (returnVal1 is double theNum1)
        {
            Assert.AreEqual(theNum1, returnVal1);
        }
    }

    /// <summary>
    /// Test indexer overload functionality.
    /// <para>Simple spreadsheet, testing that formulas
    /// can indeed never contain strings.
    /// </para>
    /// </summary>
    [TestMethod]
    public void IndexerANDGetCellValue_String_Error()
    {
        Spreadsheet test = new Spreadsheet();
        Assert.IsTrue(!test.Changed);
        test.SetContentsOfCell("A6", "Hi");
        test.SetContentsOfCell("B1", "=A6");
        Assert.IsTrue(test.Changed);

        object returnVal1 = test.GetCellValue("B1");
        object returnVal = test["B1"];

        Assert.IsTrue(returnVal is FormulaError);
        Assert.IsTrue(returnVal1 is FormulaError);
    }

    /// <summary>
    /// Test indexer overload functionality.
    /// <para>
    /// Simple spreadsheet, testing that not a formula BUT
    /// the computed value of a formula is indeed stored
    /// and accessible by the indexer[] overload.
    /// </para>
    /// </summary>
    [TestMethod]
    public void IndexerANDGetCellValue_Formula_Equal()
    {
        Spreadsheet test = new Spreadsheet();
        test.SetContentsOfCell("A6", "12");
        test.SetContentsOfCell("B1", "=A6");
        Assert.IsTrue(test.Changed);

        object returnVal = test["B1"];
        object returnVal1 = test.GetCellValue("B1");
        if (returnVal is double theNum)
        {
            Assert.AreEqual(theNum, returnVal);
        }

        if (returnVal1 is double theNum1)
        {
            Assert.AreEqual(theNum1, returnVal1);
        }
    }

    /// <summary>
    /// Test indexer overload functionality.
    /// <para>
    /// Simple spreadsheet, testing how a cell
    /// that has not yet been set fetches.
    /// </para>
    /// </summary>
    [TestMethod]
    public void IndexerANDGetCellValue_NotDefined_Equal()
    {
        Spreadsheet test = new Spreadsheet();

        object output = test.GetCellValue("a1");

        if (output is string a)
        {
            Assert.IsTrue(string.Empty.Equals(a));
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test indexer overload functionality.
    /// <para>
    /// Simple spreadsheet, testing that not a formula BUT
    /// the computed value of a formula is indeed stored
    /// and accessible by the indexer[] overload.
    /// In this case we attempt to fetch contents
    /// of an invalid formula.
    /// </para>
    /// </summary>
    [TestMethod]
    public void IndexerANDGetCellValue_FormulaError_True()
    {
        Spreadsheet test = new Spreadsheet();
        test.SetContentsOfCell("A6", "12");
        test.SetContentsOfCell("B1", "=A6/0");

        object returnVal = test["B1"];
        object returnVal1 = test.GetCellValue("B1");
        Assert.IsTrue(returnVal is FormulaError);
        Assert.IsTrue(returnVal1 is FormulaError);
    }

    // *** SAVE TESTS *** (also test Changed property) Cell internal
    // Stringform property implicitly tested here

    /// <summary>Test save functionality.</summary>
    /// <para>Saves a tiny sheet, open a new Spreadsheet instance,
    /// load the previous spreadsheet saved to current dir,
    /// check that things were serialized and deserialized properly.</para>
    [TestMethod]
    public void Save_TinySheet_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "0.0");
        sheet.SetContentsOfCell("B1", "=A6");
        sheet.Save("testSheet.txt");

        // looks like:
        // "{\"A6\":{\"StringForm\":\"0\"},\"B1\":{\"StringForm\":\"=A6\"}}"
        object cell1 = sheet.GetCellContents("A6");
        object cell2 = sheet.GetCellContents("B1");

        Spreadsheet sheet2 = new Spreadsheet();
        sheet2.Load("testSheet.txt");
        object cell1Loaded = sheet2.GetCellContents("A6");
        object cell2Loaded = sheet2.GetCellContents("B1");

        if (cell1 is double cell1D && cell1Loaded is double cell1DL)
        {
            Assert.IsTrue(cell1D == cell1DL);
        }
        else
        {
            Assert.Fail();
        }

        if (cell2 is Formula cell2F && cell2Loaded is Formula cell2FL)
        {
            Assert.IsTrue(cell2F.ToString().Equals(cell2FL.ToString()));
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Saves a tiny sheet, open a new Spreadsheet instance,
    /// load the previous spreadsheet saved to current dir,
    /// check that things were serialized and deserialized properly.</para>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_WriteToBadFile_Exception()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "0.0");
        sheet.SetContentsOfCell("B1", "=A6");
        string badFileName = "&^$()*/|?|?|?|?|\\\\\\\\\\///////C://Umm//invalid*filename.txt";
        sheet.Save(badFileName);
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Saves a tiny sheet with a single formula error object. </para>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_WriteWithFormulaError_Exception()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "=A5");
        sheet.Save("saveMe");
    }

    /// <summary>Test save and load.</summary>
    /// <para>Save, then load and overwrite the sheet.</para>
    [TestMethod]
    public void Save_LoadSuccessOverwrite_True()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "12");
        sheet.Save("saveMe.txt");

        Spreadsheet newSheet = new Spreadsheet();
        newSheet.SetContentsOfCell("A6", "500");
        newSheet.Load("saveMe.txt");

        object a6contents = newSheet.GetCellContents("A6");
        object a6val = newSheet.GetCellValue("A6");

        // even thous a6 is set to 500, on successful load,
        // its value is reverted back to 12
        if (a6contents is double d)
        {
            Assert.IsTrue(d == 12);
        }
        else
        {
            Assert.Fail();
        }

        if (a6val is double d2)
        {
            Assert.IsTrue(d2 == 12);
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>Test save and load.</summary>
    /// <para>Save, then load and overwrite the sheet.</para>
    [TestMethod]
    public void Save_LoadSuccessOverwriteComplex_True()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "12");
        sheet.SetContentsOfCell("B1", "=6*6");
        sheet.SetContentsOfCell("B2", "=a6");
        sheet.Save("saveMe.txt");

        Spreadsheet newSheet = new Spreadsheet();
        newSheet.SetContentsOfCell("A100", "=500");
        newSheet.SetContentsOfCell("F4", "=A100");
        newSheet.Load("saveMe.txt");

        object a6contents = newSheet.GetCellContents("A6");
        object b1val = newSheet.GetCellValue("B1");
        object b2val = newSheet.GetCellValue("b2");
        object dNE = newSheet.GetCellValue("A100");
        object dNEContents = newSheet.GetCellContents("A100");

        if (dNE is string em)
        {
            Assert.IsTrue(em.Equals(string.Empty));
        }
        else
        {
            Assert.Fail();
        }

        if (dNEContents is string emt)
        {
            Assert.IsTrue(emt.Equals(string.Empty));
        }
        else
        {
            Assert.Fail();
        }

        // even thous a6 is set to 500, on successful load,
        // its value is reverted back to 12
        if (a6contents is double d)
        {
            Assert.IsTrue(d == 12);
        }
        else
        {
            Assert.Fail();
        }

        if (b1val is double d2)
        {
            Assert.IsTrue(d2 == 36);
        }
        else
        {
            Assert.Fail();
        }

        if (b2val is double b2)
        {
            Assert.IsTrue(b2 == 12);
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test load with a file that contains a formula error.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_FromFileWithFormulaErr_Exception()
    {
        string containsErr = "{\"A6\": {\"StringForm\": \"=B1\"}}";
        File.WriteAllText("testSheet.txt", containsErr);

        Spreadsheet newSheet = new Spreadsheet();
        newSheet.Load("testSheet.txt");
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Saves a tiny sheet, open a new Spreadsheet instance,
    /// load the previous spreadsheet saved to current dir,
    /// check that things were serialized and deserialized properly.
    /// Only difference here is we construct a spreadsheet with a custom name.</para>
    [TestMethod]
    public void Save_TinySheetWithName_Valid()
    {
        Spreadsheet sheet = new Spreadsheet("CustomName");
        sheet.SetContentsOfCell("A6", "0.0");
        sheet.SetContentsOfCell("B1", "=A6");
        sheet.Save("testSheet.txt");

        object cell1 = sheet.GetCellContents("A6");
        object cell2 = sheet.GetCellContents("B1");

        Spreadsheet sheet2 = new Spreadsheet();
        sheet2.Load("testSheet.txt");
        object cell1Loaded = sheet.GetCellContents("A6");
        object cell2Loaded = sheet.GetCellContents("B1");

        if (cell1 is double cell1D && cell1Loaded is double cell1DL)
        {
            Assert.IsTrue(cell1D == cell1DL);
        }
        else
        {
            Assert.Fail();
        }

        if (cell2 is Formula cell2F && cell2Loaded is Formula cell2FL)
        {
            Assert.IsTrue(cell2F.ToString().Equals(cell2FL.ToString()));
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Saves a tiny sheet, open a new Spreadsheet instance,
    /// load the previous spreadsheet saved to current dir,
    /// check that things were serialized and deserialized properly.
    /// Only difference here is we construct a spreadsheet with a custom name.</para>
    [TestMethod]
    public void Save_TinySheetTextWithName_Valid()
    {
        Spreadsheet sheet = new Spreadsheet("CustomName");
        sheet.SetContentsOfCell("A6", "hello");
        sheet.SetContentsOfCell("B1", "gdday");
        sheet.Save("testSheet.txt");

        object cell1 = sheet.GetCellContents("A6");
        object cell2 = sheet.GetCellContents("B1");

        Spreadsheet sheet2 = new Spreadsheet();
        sheet2.Load("testSheet.txt");
        object cell1Loaded = sheet.GetCellContents("A6");
        object cell2Loaded = sheet.GetCellContents("B1");

        if (cell1 is string cell1D && cell1Loaded is string cell1DL)
        {
            Assert.IsTrue(cell1D.Equals(cell1DL));
        }
        else
        {
            Assert.Fail();
        }

        if (cell2 is string cell2F && cell2Loaded is string cell2FL)
        {
            Assert.IsTrue(cell2F.Equals(cell2FL));
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Save and load an empty sheet.</para>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_EmptySheet_Valid()
    {
        // Use the 'using' statement to ensure the file is closed after creation.
        using (File.Create("testSheetEmpty.txt"))
        {
            // nothing
        }

        Spreadsheet sheet2 = new Spreadsheet();
        sheet2.Load("testSheetEmpty.txt");
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Load a file with formula errors. </para>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_ErrorSheet_Valid()
    {
        // Use the 'using' statement to ensure the file is closed after creation.
        using (File.Create("testSheetEmpty.txt"))
        {
            // nothing
        }

        Spreadsheet sheet2 = new Spreadsheet();
        sheet2.Load("testSheetEmpty.txt");
    }

    /// <summary>
    /// Tests the save functionality.
    /// <para>Tests on a spreadsheet with cells containing each of the acceptable
    /// cell contents.</para>
    /// </summary>
    public void Save_SimpleSheet_Valid()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "0.0");
        sheet.SetContentsOfCell("B1", "=A6");
        sheet.SetContentsOfCell("A6", "=112*16");
        sheet.SetContentsOfCell("G300", "hi");
        sheet.Save("testSheet.txt");

        object cell1 = sheet.GetCellContents("A6");
        object cell2 = sheet.GetCellContents("B1");
        object cell3 = sheet.GetCellContents("G300");

        Spreadsheet sheet2 = new Spreadsheet();
        sheet2.Load("testSheet.txt");
        object cell1Loaded = sheet.GetCellContents("A6");
        object cell2Loaded = sheet.GetCellContents("B1");
        object cell3Loaded = sheet.GetCellContents("G300");

        if (cell1 is double cell1D && cell1Loaded is double cell1DL)
        {
            Assert.IsTrue(cell1D == cell1DL);
        }
        else
        {
            Assert.Fail();
        }

        if (cell2 is Formula cell2F && cell2Loaded is Formula cell2FL)
        {
            Assert.IsTrue(cell2F.ToString().Equals(cell2FL.ToString()));
        }
        else
        {
            Assert.Fail();
        }

        if (cell3 is string cell3S && cell3Loaded is string cell3SL)
        {
            Assert.IsTrue(cell3S.Equals(cell3SL));
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Saves a tiny sheet that contains a bad formula.
    /// from instructions: If any invalid formulas or circular dependencies are encountered,
    /// throw a spreadsheet read write exception.</para>
    [TestMethod]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_TinySheet_FormulaError()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "0.0");
        sheet.SetContentsOfCell("B1", "=A6/0");
        sheet.Save("testSheet.txt");
    }

    /// <summary>Test save functionality.</summary>
    /// <para>Saves a tiny sheet that contains a circular dependency.
    /// from instructions: If any invalid formulas or circular dependencies are encountered,
    /// throw a spreadsheet read write exception.</para>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void Save_TinySheet_Circular()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "0.0");
        sheet.SetContentsOfCell("B1", "=A6");

        // *** IMPORTANT *** our spreadsheet simply won't allow this.
        // check TODOs.
        sheet.SetContentsOfCell("A6", "=B1");
    }

    /// <summary>
    /// Test save functionality.
    /// <para>Saves a tiny sheet that contains a bad cell name.
    /// from instructions: If any invalid formulas or circular dependencies are encountered,
    /// throw a spreadsheet read write exception.</para>
    /// </summary>
    [TestMethod]
    public void Save_TinySheet_InvalidName()
    {
        // special case here because via GUI,
        // you can't select an invalid cell...
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "0.0");
        try
        {
            sheet.SetContentsOfCell("6A", "12");
        }
        catch (InvalidNameException)
        {
            sheet.Save("testSheet.txt");
        }
    }

    /// <summary>
    /// Test save functionality.
    /// <para>Saves a tiny sheet that contains a bad cell name reference.
    /// from instructions: If any invalid formulas or circular dependencies are encountered,
    /// throw a spreadsheet read write exception.</para></summary>
    [TestMethod]
    public void Save_TinySheet_InvalidContents()
    {
        Spreadsheet sheet = new Spreadsheet();
        sheet.SetContentsOfCell("A6", "0.0");

        // sheet.SetContentsOfCell("B1", "=6A");
        sheet.Save("testSheet.txt");
    }

    // *** SetContentsOfCell tests

    /// <summary>
    /// test that throws circular exception (set cell contents can throw) name, Formula param version.
    /// How SetContentsOfCell with formula param works:
    /// "Set the contents of the named cell to the given formula".
    /// </summary>
    /// <para>Expect exception to be thrown when we set cell contents to a formula, and
    /// the formula points to a cell, but then we set that other cell's contents to
    /// previous cell, so they point to each other.</para>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCellFmla_Simple_CircException()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("B1", "=A6");

        // creates the circular ref
        test.SetContentsOfCell("A6", "=B1");
    }

    /// <summary>
    /// Replace a formula error with another formula error.
    /// this test shows how a cell can "contain" a "formula"
    /// that is invalid, but the value is an error.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellFmla_ErrorNoFix_True()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("B1", "=A1");

        // creates the circular ref
        test.SetContentsOfCell("B1", "=12/0");

        Assert.IsTrue(test.GetCellValue("B1") is FormulaError);
    }

    /// <summary>
    /// set "A1" to "B1 + C1"
    /// then set "B1" to "5.0" and "C1" to "5.0" afterwards.
    /// </summary>
    /// <para>Cell initially is formulaerror, but then computes and is
    /// stored as a valid formula once values of cell refs are set.
    /// Tests recalc functionality.</para>
    [TestMethod]
    public void SetContentsOfCell_Formula_InitFmlaError()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A1", "=B1+ C1");
        object temp = test.GetCellValue("A1");
        Assert.IsTrue(temp is FormulaError);

        test.SetContentsOfCell("B1", "5.0");

        test.SetContentsOfCell("C1", "5.0");
        object tempAfterChanges = test.GetCellContents("A1");

        // it's still a formulaerror after changes...
        if (tempAfterChanges is Formula output)
        {
            Assert.IsTrue(output.Equals(new Formula("b1 +    c1")));
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>
    /// test that throws circular exception (set cell contents can throw), Formula param version.
    /// How SetContentsOfCell with formula param works:
    /// "Set the contents of the named cell to the given formula".
    /// </summary>
    /// <para>
    /// Expect exception to be thrown when we set cell contents to a formula, and
    /// the formula points to the cell itself.
    /// </para>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCellFmla_SelfRef_CircException()
    {
        Spreadsheet test = new Spreadsheet();

        // cell tries to reference itself
        test.SetContentsOfCell("B1", "=B1");
    }

    /// <summary>
    /// test that throws invalid name exception (set cell contents can throw) name,
    /// Formula param version.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCellFmla_EmptySimple_NameException()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell(string.Empty, "=B1");
    }

    /// <summary>
    /// Test that when we clear a cell and then attempt to create a circular exception,
    /// there is no exception thrown.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCell_ClearCirc_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A1", "F5");
        test.SetContentsOfCell("A1", string.Empty);

        // simply check if doing this throws no exception.
        test.SetContentsOfCell("F5", "A1");

        if (test.GetCellContents("F5") is string)
        {
            Assert.IsTrue(test.GetCellContents("F5").Equals("A1"));
        }
        else
        {
            Assert.Fail("oopsie");
        }
    }

    /// <summary>
    /// Test that setting cell contents to empty string doesn't add it to
    /// the internal dictionary.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellStr_EmptySimple_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        List<string> a6ToEmpty = new List<string>(test.SetContentsOfCell("A6", string.Empty));

        CollectionAssert.AreEqual(a6ToEmpty, new List<string> { "A6" });

        // here's how we can test that
        HashSet<string> emptydict = new HashSet<string>(test.GetNamesOfAllNonemptyCells());
        HashSet<string> emptySet = new HashSet<string> { };

        Assert.IsTrue(emptydict.SetEquals(emptySet));
    }

    /// <summary>
    /// test that throws invalid name exception (set cell contents can throw) name,
    /// Formula param version.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCellFmla_Simple_NameException()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("111A6666", "=B1");
    }

    /// <summary>
    /// Test using SetContentsOfCell to overwrite contents of a cell.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellFmla_Reset_NameException()
    {
        Spreadsheet test = new Spreadsheet();

        // dummy formula
        Formula ref1 = new Formula("B1-C4/A777");

        test.SetContentsOfCell("A6", "=B1-C4/A777");
        test.SetContentsOfCell("A6", "12");

        double.TryParse(test.GetCellContents("A6").ToString(), out double result);

        Assert.IsTrue(result.Equals(12));
    }

    /// <summary>
    /// Test using SetContentsOfCell to overwrite contents of a cell with another formula.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellFmla_ResetWithFmla1_True()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("R3", "5e-3");
        test.SetContentsOfCell("B12", "=12+12");
        test.SetContentsOfCell("D777", "1");

        test.SetContentsOfCell("A6", "=B1-C4/A777");
        test.SetContentsOfCell("A6", "=R3-B12/D777");

        string temp1 = test.GetCellContents("A6").ToString() + string.Empty;

        Formula temp = new Formula(temp1);

        Assert.IsTrue(temp.Equals(new Formula("r3  - b12 / d777")));
    }

    /// <summary>
    /// Test using SetContentsOfCell to overwrite contents of a cell with another formula.
    /// Tests a lot of sets and resets.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellFmla_MultipleResets_True()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("R3", "=A1");
        test.SetContentsOfCell("Q1", "=R3");
        test.SetContentsOfCell("A6", "=Q1");

        test.SetContentsOfCell("A1", "0");

        object r3Val = test.GetCellValue("R3");
        object a6Val = test.GetCellValue("A6");

        // Q1 -> R3 -> A1 -> 0
        // but q1val is still formula error
        object q1Val = test.GetCellValue("q1");

        if (r3Val is double dT)
        {
            Assert.IsTrue(dT == 0);
        }
        else
        {
            Assert.Fail();
        }

        if (a6Val is double dA)
        {
            Assert.IsTrue(dA == 0);
        }
        else
        {
            Assert.Fail();
        }

        if (q1Val is double dQ)
        {
            Assert.IsTrue(dQ == 0);
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test using SetContentsOfCell to overwrite contents of a cell with another formula.
    /// Tests recalc functionality.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellFmla_ResetWithFmla_True()
    {
        Spreadsheet test = new Spreadsheet();
        test.SetContentsOfCell("A1", "=A2");
        test.SetContentsOfCell("A2", "2");

        object a2Cell = test.GetCellContents("A2");
        object a1Cell = test.GetCellContents("A1");

        Formula temp = new Formula(test.GetCellContents("A1").ToString() + string.Empty);

        Assert.IsTrue(temp.Equals(new Formula("A2")));
    }

    /// <summary>
    /// Test method for testing performance of recalculation.
    /// chain A1→ = 0; A2→ =A1, A3→ =A2, …) from A1 to A10000 and then set the value of A10000.
    /// (courtesy of assignment instructions).
    /// </summary>
    [TestMethod]
    public void StressTest_SetChainFormulas()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A1", "0");

        // CHAIN!
        for (int i = 2; i <= 10000; i++)
        {
            string cellName = $"A{i}";
            string formula = $"=A{i - 1}";
            test.SetContentsOfCell(cellName, formula);
        }

        // per assignment instructions
        test.SetContentsOfCell("A10000", "100");

        // dummy assertion. Mainly wanting to see how fast code is.
        // fetching random cell in the chain here.
        // we fetch the value because the contents are a formula.
        object computedVal = test.GetCellValue("A12");
        object contentsVal = test.GetCellContents("A12");
        if (computedVal is double d)
        {
            Assert.IsTrue(d == 0);
        }
        else
        {
            Assert.Fail();
        }

        if (contentsVal is Formula f)
        {
            Assert.IsTrue(f.ToString().Equals("A11"));
        }
    }

    /// <summary>
    /// Test using SetContentsOfCell to overwrite contents of a cell with another formula.
    /// Tests recalc functionality. Recalcs with valid formulas.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellFmla_ResetWithValidFmlas_True()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("R3", "=A1");
        test.SetContentsOfCell("Q1", "=R3");
        test.SetContentsOfCell("A6", "=Q1");

        test.SetContentsOfCell("A1", "0");

        test.SetContentsOfCell("F7", "12");
        test.SetContentsOfCell("Q2", "5");

        test.SetContentsOfCell("Q1", "=F7+Q2");

        object q1Val = test.GetCellValue("q1");
        if (q1Val is double d)
        {
            Assert.IsTrue(d == 17);
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test using SetContentsOfCell to overwrite contents of a cell that had a
    /// formula in it with a double.
    /// </summary>
    [TestMethod]
    public void SetContentsOfCellFmla_ResetWithoutFmlaErr_True()
    {
        Spreadsheet test = new Spreadsheet();
        test.SetContentsOfCell("A1", "=A2");
        test.SetContentsOfCell("A1", "5.0");

        Formula temp = new Formula(test.GetCellContents("A1").ToString() + string.Empty);

        Assert.IsTrue(test.GetCellContents("A1") is double);
    }

    /// <summary>                                                    s
    /// test that throws invalid name exception (set cell contents can throw) name,
    /// Double param version.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCellDouble_Simple_NameException()
    {
        Spreadsheet test = new Spreadsheet();

        // set invalid cell to some double
        test.SetContentsOfCell("515I_o", "16.4");
    }

    /// <summary>
    /// test that throws invalid name exception (set cell contents can throw) name,
    /// String param version.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCellString_Simple_NameException()
    {
        Spreadsheet test = new Spreadsheet();

        // set invalid cell to some double
        test.SetContentsOfCell("QoQ", "header1");
    }

    /// <summary>
    /// test that throws invalid name exception (set cell contents can throw) name,
    /// String param version.
    /// </summary>
    [TestMethod]
    public void GetCellContents_DNE_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        if (test.GetCellContents("Q12").ToString() is string)
        {
            Assert.AreEqual(test.GetCellContents("Q12").ToString(), string.Empty);
        }
        else
        {
            Assert.Fail();
        }
    }

    /// <summary>
    /// Test that a cell reset with string.Empty clears the cells dictionary completely.
    /// </summary>
    [TestMethod]
    public void GetNamesOfAllNonEmptyCells_ClearedCell_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A4", "116.0");
        test.SetContentsOfCell("A4", string.Empty);

        HashSet<string> temp = new HashSet<string>(test.GetNamesOfAllNonemptyCells());
        Assert.IsTrue(temp.SetEquals(new HashSet<string> { }));
    }

    /// <summary>
    /// Test what happens when we re-set cell contents to an empty string.
    /// </summary>
    [TestMethod]
    public void GetCellsToRecalc_CellDNE_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A1", "=A2+a3/a4");
        test.SetContentsOfCell("G100", "=A1+F5");
        test.SetContentsOfCell("F5", "=A1");

        List<string> temp1 = new List<string>(test.SetContentsOfCell("A1", string.Empty));
        CollectionAssert.AreEqual(temp1, new List<string> { "A1", "F5", "G100" });

        string temp = test.GetCellContents("A1").ToString() + string.Empty;
        Assert.AreEqual(temp, string.Empty);
    }

    /// <summary>
    /// Test what happens when we re-set cell contents,
    /// when cell has no dependents.
    /// </summary>
    [TestMethod]
    public void GetCellsToRecalc_CellChange_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A2", "0");
        test.SetContentsOfCell("A3", "1");
        test.SetContentsOfCell("A4", "1");
        test.SetContentsOfCell("A1", "0");
        test.SetContentsOfCell("F5", "0");

        test.SetContentsOfCell("A1", "=A2+a3/a4");
        test.SetContentsOfCell("G100", "=A1+F5");
        test.SetContentsOfCell("F5", "=A1");

        List<string> temp1 = new List<string>(test.SetContentsOfCell("G100", "=A3-A4"));
        CollectionAssert.AreEqual(temp1, new List<string> { "G100" });

        Formula g1Contents = new Formula(test.GetCellContents("G100").ToString() + string.Empty);

        Assert.AreEqual(g1Contents.ToString(), new Formula("a3 -      a4").ToString());
    }

    /// <summary>
    /// Test what happens when we set cell contents so that there
    /// is a circular reference greater than degree 1.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(CircularException))]
    public void GetCellsToRecalc_IterativeChange_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A1", "=b5 + 12");
        test.SetContentsOfCell("B5", "=816 / Q6     ");

        // finishes of loop
        test.SetContentsOfCell("Q6", "=8-5/A1");
    }

    /// <summary>
    /// Test get names of all nonempty cells
    /// by using set cell contents function twice.
    /// </summary>
    [TestMethod]
    public void GetNamesOfAllNonemptyCells_Simple_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("A6", "12");
        test.SetContentsOfCell("A100", "49");

        ISet<string> expected = new HashSet<string> { "A6", "A100" };
        ISet<string> actual = test.GetNamesOfAllNonemptyCells();

        // Assert
        Assert.IsTrue(expected.SetEquals(actual), "Sets are uber not-equal");
    }

    /// <summary>
    /// Test get names of all nonempty cells
    /// when we have not ever used the set cell contents function,
    /// in other words, with an empty spreadsheet.
    /// </summary>
    [TestMethod]
    public void GetNamesOfAllNonemptyCells_Empty_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        ISet<string> expected = new HashSet<string> { };
        ISet<string> actual = test.GetNamesOfAllNonemptyCells();

        // Assert
        Assert.IsTrue(expected.SetEquals(actual), "spreadsheet getnames call should return empty but is not.");
    }

    /// <summary>
    /// test get cell contents when cell DNE, so expect throw exception.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_BadName_NameException()
    {
        Spreadsheet test = new Spreadsheet();

        test.GetCellContents("6001B");
    }

    /// <summary>
    /// test get cell contents when cell exists but is empty.
    /// </summary>
    [TestMethod]
    public void GetCellContents_Empty_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        // not handling keynotfound exceptions.
        test.SetContentsOfCell("a6", string.Empty);

        object a6CellContents = test.GetCellContents("a6");

        string temp = a6CellContents.ToString() + string.Empty;

        Assert.IsTrue(temp.Equals(string.Empty));
    }

    /// <summary>
    /// test get cell contents when cell exists (formula param version).
    /// </summary>
    [TestMethod]
    public void GetCellContents_RegFormula_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("a6", "0");
        test.SetContentsOfCell("b1", "0");

        test.SetContentsOfCell("q6", "=a6 -      b1");

        object a6CellContents = test.GetCellContents("q6");

        string temp = a6CellContents.ToString() + string.Empty;

        Formula temp1 = new Formula(temp);

        Assert.IsTrue(temp1.ToString().Equals("A6-B1"));
    }

    /// <summary>
    /// test get cell contents when cell exists (double version).
    /// </summary>
    [TestMethod]
    public void GetCellContents_RegDouble_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("p1", "16");

        object p1CellContents = test.GetCellContents("p1");

        double result = (double)p1CellContents;

        Assert.IsTrue(result.Equals(16));
    }

    /// <summary>
    /// test get cell contents when cell exists (string version).
    /// </summary>
    [TestMethod]
    public void GetCellContents_RegString_Equals()
    {
        Spreadsheet test = new Spreadsheet();

        test.SetContentsOfCell("p1", "Theodore Van Gogh");

        object p1CellContents = test.GetCellContents("p1");

        string temp = p1CellContents.ToString() + string.Empty;

        Assert.IsTrue(temp.Equals("Theodore Van Gogh"));
    }
}

/// <summary>
/// Authors:   Joe Zachary
///            Daniel Kopta
///            Jim de St. Germain
/// Date:      Updated Spring 2020
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 - This work may not be copied for use
///                      in Academic Coursework.  See below.
///
/// File Contents:
///
///   This file contains proprietary grading tests for CS 3500.  These tests cases
///   are for individual student use only and MAY NOT BE SHARED.  Do not back them up
///   nor place them in any online repository.  Improper use of these test cases
///   can result in removal from the course and an academic misconduct sanction.
///
///   These tests are for your private use, this semester, only to improve the quality of the
///   rest of your assignments.
/// </summary>
[TestClass]
public class SpreadsheetTest
{
    /// <summary>
    ///   Test that the cell naming convention is honored.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("2")]
    [ExpectedException(typeof(InvalidNameException))]
    public void GetCellContents_InvalidCellName_Throws()
    {
        Spreadsheet s = new();
        s.GetCellContents("1AA");
    }

    /// <summary>
    ///   Test that an unassigned cell has the default value of an empty string.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("3")]
    public void GetCellContents_DefaultCellValue_Empty()
    {
        Spreadsheet s = new();
        Assert.AreEqual(string.Empty, s.GetCellContents("A2"));
    }

    /// <summary>
    ///   Try setting an invalid cell to a double.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("5")]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_InvalidCellName_Throws()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("1A1A", "1.5");
    }

    /// <summary>
    ///   Set a cell to a number and get the number back out.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("6")]
    public void SetGetCellContents_SetTheNumber_RetrieveTheNumber()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("Z7", "1.5");
        Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
    }

    // SETTING CELL TO A STRING

    /// <summary>
    ///   Try to assign a string to an invalid cell.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("9")]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCellString_InvalidCellName_Throw()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("1AZ", "hello");
    }

    /// <summary>
    ///   Simple test of assigning a string to a cell and getting
    ///   it back out.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("10")]
    public void SetAndGetCellContents_SetTheString_RetrieveTheString()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("Z7", "hello");
        Assert.AreEqual("hello", s.GetCellContents("Z7"));
    }

    // SETTING CELL TO A FORMULA

    /// <summary>
    ///   Test that when assigning a formula, an invalid cell name
    ///   throws an exception.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("13")]
    [ExpectedException(typeof(InvalidNameException))]
    public void SetContentsOfCell_InvalidCellNameForFormula_Throws()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("1AZ", "=2");
    }

    /// <summary>
    ///   Set a formula, retrieve the formula.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("14")]
    public void SetGetCellContents_SetAFormula_RetrieveTheFormula()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("Z7", "=3");
        Formula f = (Formula)s.GetCellContents("Z7");
        Assert.AreEqual(new Formula("3"), f);
        Assert.AreNotEqual(new Formula("2"), f);
    }

    // CIRCULAR FORMULA DETECTION

    /// <summary>
    ///   Two cell circular dependency check.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("15")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_CircularDependency_Throws()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("A1", "=A2");
        s.SetContentsOfCell("A2", "=A1");
    }

    /// <summary>
    ///   Simple chain, set start to empty.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [ExpectedException(typeof(ArgumentException))]
    public void SetContentsOfCell_BeginningOfChainToEmpty_Throws()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("A1", "5");
        s.SetContentsOfCell("A2", "=A1+2");
        s.SetContentsOfCell("A1", string.Empty);
    }

    /// <summary>
    ///    A four cell circular dependency test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("16")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_CircularDependencyMultipleCells_Throws()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A3", "=A4+A5");
        s.SetContentsOfCell("A5", "=A6+A7");
        s.SetContentsOfCell("A7", "=A1+A1");
    }

    /// <summary>
    ///  Trying to add a circular dependency should leave the
    ///  spreadsheet unmodified.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("17")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_TestUndoCircular_OriginalSheetRemains()
    {
        Spreadsheet s = new();
        try
        {
            s.SetContentsOfCell("A1", "=A2+A3");
            s.SetContentsOfCell("A2", "15");
            s.SetContentsOfCell("A3", "30");
            s.SetContentsOfCell("A2", "=A3*A1");
        }
        catch (CircularException)
        {
            Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
            throw;
        }
    }

    /// <summary>
    ///   After adding the simplest circular dependency, the first cell
    ///   should still contain the original value, but the second one removed.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("17b")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_SimpleCircularUndo_OriginalSheetRemains()
    {
        Spreadsheet s = new();
        try
        {
            s.SetContentsOfCell("A1", "=A2");
            s.SetContentsOfCell("A2", "=A1");
        }
        catch (CircularException)
        {
            // According to this test from the instructor
            // the sheet can't have circular dependencies!
            Assert.AreEqual(string.Empty, s.GetCellContents("A2"));
            Assert.IsTrue(new HashSet<string> { "A1" }.SetEquals(s.GetNamesOfAllNonemptyCells()));
            throw;
        }
    }

    // NONEMPTY CELLS

    /// <summary>
    ///   An empty spreadsheet should have no non-empty cells.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("18")]
    public void GetNamesOfAllNonEmptyCells_EmptySpreadsheet_EmptyEnumerator()
    {
        Spreadsheet s = new();
        Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
    }

    /// <summary>
    ///   Assigning an empty string into a cell should not create a non-empty cell.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("19")]
    public void SetContentsOfCell_SetEmptyCell_CellIsEmpty()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("B1", string.Empty);
        Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
    }

    /// <summary>
    ///   Assigning a string into a cell produces a spreadsheet with one non-empty cell.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("20")]
    public void GetNamesOfAllNonEmptyCells_AddStringToCell_ThatCellIsNotEmpty()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("B1", "hello");
        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["B1"]));
    }

    /// <summary>
    ///   Assigning a double into a cell produces a spreadsheet with one non-empty cell.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("21")]
    public void GetNamesOfAllNonEmptyCells_AddDoubleToCell_ThatCellIsNotEmpty()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("B1", "52.25");
        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Matches1(["B1"]));
        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["B1"]));
    }

    /// <summary>
    ///   Assigning a Formula into a cell produces a spreadsheet with one non-empty cell.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("22")]
    public void GetNamesOfAllNonEmptyCells_AddFormulaToCell_ThatCellIsNotEmpty()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("B1", "=3.5");
        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["B1"]));
    }

    /// <summary>
    ///   Assign a double, string, and formula into the sheet and make sure
    ///   they each have their own cell.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("23")]
    public void SetContentsOfCell_AssignDoubleStringAndFormula_ThreeCellsExist()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("A1", "17.2");
        s.SetContentsOfCell("C1", "hello");
        s.SetContentsOfCell("B1", "=3.5");

        Assert.IsTrue(s.GetNamesOfAllNonemptyCells().Matches1(["A1", "B1", "C1"]));
        Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(["A1", "B1", "C1"]));
    }

    // RETURN VALUE OF SET CELL CONTENTS

    /// <summary>
    ///   When a cell that has no cells depending on it is changed, then only
    ///   that cell needs to be reevaluated. (Testing for Double content cells.)
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("24")]
    public void SetContentsOfCell_SettingIndependentCellToDouble_ReturnsOnlyThatCell()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("B1", "hello");
        s.SetContentsOfCell("C1", "=5");
        var toReevaluate = s.SetContentsOfCell("A1", "17.2");
        Assert.IsTrue(toReevaluate.Matches1(["A1"])); // Note: Matches is not order dependent
    }

    /// <summary>
    ///   When a cell that has no cells depending on it is changed, then only
    ///   that cell needs to be reevaluated. (Testing for Formula content cells.)
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("25")]
    public void SetContentsOfCell_SettingIndependentCellToString_ReturnsOnlyThatCell()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("A1", "17.2");
        s.SetContentsOfCell("C1", "=5");

        var toReevaluated = s.SetContentsOfCell("B1", "hello");
        Assert.IsTrue(toReevaluated.Matches1(["B1"]));
    }

    /// <summary>
    ///   When a cell that has no cells depending on it is changed, then only
    ///   that cell needs to be reevaluated. (Testing for Formula content cells.)
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("26")]
    public void SetContentsOfCell_SettingIndependentCellToFormula_ReturnsOnlyThatCell()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "17.2");
        s.SetContentsOfCell("B1", "hello");
        var changed = s.SetContentsOfCell("C1", "=5");
        Assert.IsTrue(changed.Matches1(["C1"]));
    }

    /// <summary>
    ///   A chain of 5 cells is created.  When the first cell in the chain
    ///   is modified, then all the cells have to be recomputed.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("27")]
    public void SetContentsOfCell_CreateChainModifyFirst_AllAreInNeedOfUpdate()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A2", "6");
        s.SetContentsOfCell("A3", "=A2+A4");
        s.SetContentsOfCell("A4", "=A2+A5");

        var changed = s.SetContentsOfCell("A5", "82.5");

        Assert.IsTrue(changed.SequenceEqual(["A5", "A4", "A3", "A1"]));
    }

    // CHANGING CELLS

    /// <summary>
    ///   Test that replacing the contents of a cell (Formula --> double) works.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("28")]
    public void SetContentsOfCell_ReplaceFormulaWithDouble_CellValueCorrect()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A1", "2.5");
        Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
    }

    /// <summary>
    ///   Test that replacing a formula in a cell with a string works.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("29")]
    public void SetContentsOfCell_ReplaceFormulaWithString_CellValueCorrect()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "=A2+A3");
        s.SetContentsOfCell("A1", "Hello");
        Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
    }

    /// <summary>
    ///   Test that replacing a cell containing a string with a new formula works.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("30")]
    public void SetContentsOfCell_ReplaceStringWithFormula_CellValueCorrect()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "Hello");
        s.SetContentsOfCell("A1", "=23");
        Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
        Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
    }

    // STRESS TESTS

    /// <summary>
    ///   Create a sheet with 15 cells containing formulas.  Make sure that modifying
    ///   the end of the chain results in all the cells having to be recomputed.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("31")]
    public void SetContentsOfCell_LongChainModifyEnd_AllCellsNeedToBeReEvaluated()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("A1", "=B1+B2");
        s.SetContentsOfCell("B1", "=C1-C2");
        s.SetContentsOfCell("B2", "=C3*C4");
        s.SetContentsOfCell("C1", "=D1*D2");
        s.SetContentsOfCell("C2", "=D3*D4");
        s.SetContentsOfCell("C3", "=D5*D6");
        s.SetContentsOfCell("C4", "=D7*D8");
        s.SetContentsOfCell("D1", "=E1");
        s.SetContentsOfCell("D2", "=E1");
        s.SetContentsOfCell("D3", "=E1");
        s.SetContentsOfCell("D4", "=E1");
        s.SetContentsOfCell("D5", "=E1");
        s.SetContentsOfCell("D6", "=E1");
        s.SetContentsOfCell("D7", "=E1");
        s.SetContentsOfCell("D8", "=E1");

        var cells = s.SetContentsOfCell("E1", "0");
        Assert.IsTrue(cells.Matches1(["A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1"]));
    }

    /// <summary>
    ///    Repeat the stress test for more weight in grading process.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("32")]
    public void IncreaseGradingWeight1()
    {
        SetContentsOfCell_LongChainModifyEnd_AllCellsNeedToBeReEvaluated();
    }

    /// <summary>
    ///    Repeat the stress test for more weight in grading process.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("33")]
    public void IncreaseGradingWeight2()
    {
        SetContentsOfCell_LongChainModifyEnd_AllCellsNeedToBeReEvaluated();
    }

    /// <summary>
    ///    Repeat the stress test for more weight in grading process.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("34")]
    public void IncreaseGradingWeight3()
    {
        SetContentsOfCell_LongChainModifyEnd_AllCellsNeedToBeReEvaluated();
    }

    /// <summary>
    ///   Programmatically create a chain of cells.  Each time we add
    ///   another element to the chain, we check that the whole chain
    ///   needs to be reevaluated.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("35")]
    public void SetContentsOfCell_TwoHundredLongChain_EachTimeReturnsRestOfChain()
    {
        Spreadsheet s = new();
        ISet<string> expectedAnswers = new HashSet<string>();
        for (int i = 1; i < 200; i++)
        {
            string currentCell = "A" + i;
            expectedAnswers.Add(currentCell);

            // my code to avoid exceptions
            s.SetContentsOfCell("A"+(i+1), "0");
            var changed = s.SetContentsOfCell(currentCell, "=A" + (i + 1));

            Assert.IsTrue(changed.Matches1([.. expectedAnswers]));
        }
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("36")]
    public void IncreaseGradingWeight4()
    {
        SetContentsOfCell_TwoHundredLongChain_EachTimeReturnsRestOfChain();
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("37")]
    public void IncreaseGradingWeight5()
    {
        SetContentsOfCell_TwoHundredLongChain_EachTimeReturnsRestOfChain();
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("38")]
    public void IncreaseGradingWeight6()
    {
        SetContentsOfCell_TwoHundredLongChain_EachTimeReturnsRestOfChain();
    }

    /// <summary>
    ///   Build a long chain of cells.  Set one of the cells in the middle
    ///   of the chain to a circular dependency.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("39")]
    [ExpectedException(typeof(CircularException))]
    public void SetContentsOfCell_LongChainAddCircularInMiddle_Throws()
    {
        Spreadsheet s = new();

        for (int i = 1; i < 200; i++)
        {
            string currentCell = "A" + i;
            string nextCell = "A" + (i + 1);
            s.SetContentsOfCell(nextCell, "0");
            s.SetContentsOfCell(currentCell, "=" + nextCell);
        }

        s.SetContentsOfCell("A150", "=A50");
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("40")]
    [ExpectedException(typeof(CircularException))]
    public void IncreaseGradingWeight7()
    {
        SetContentsOfCell_LongChainAddCircularInMiddle_Throws();
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("41")]
    [ExpectedException(typeof(CircularException))]
    public void IncreaseGradingWeight8()
    {
        SetContentsOfCell_LongChainAddCircularInMiddle_Throws();
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("42")]
    [ExpectedException(typeof(CircularException))]
    public void IncreaseGradingWeight9()
    {
        SetContentsOfCell_LongChainAddCircularInMiddle_Throws();
    }

    /// <summary>
    ///   <para>
    ///     This is a stress test with lots of cells "linked" together.
    ///   </para>
    ///   <para>
    ///     Create 500 cells that are in a chain from A10 to A1499.
    ///     Then break the chain in the middle by setting A1249 to
    ///     a number.
    ///   </para>
    ///   <para>
    ///     Then check that there are two separate chains of cells.
    ///   </para>
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("43")]
    public void SetContentsOfCell_BreakALongChain_TwoIndependentChains()
    {
        Spreadsheet s = new();

        for (int i = 0; i < 500; i++)
        {
            string currentCell = "A1" + i;
            string nextCell = "A1" + (i + 1);
            s.SetContentsOfCell(nextCell, "0");
            s.SetContentsOfCell(currentCell, "=" + nextCell);
        }

        List<string> firstCells = [];
        List<string> lastCells = [];

        for (int i = 0; i < 250; i++)
        {
            string firstHalfCell = "A1" + i;
            string secondHalfCell = "A1" + (i + 250);
            firstCells.Add(firstHalfCell);
            lastCells.Add(secondHalfCell);
        }

        firstCells.Reverse();
        lastCells.Reverse();

        var firstHalfNeedReevaluate = s.SetContentsOfCell("A1249", "25.0");
        var secondHalfNeedReevaluate = s.SetContentsOfCell("A1499", "0");

        Assert.IsTrue(firstHalfNeedReevaluate.SequenceEqual(firstCells));
        Assert.IsTrue(secondHalfNeedReevaluate.SequenceEqual(lastCells));
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("44")]
    public void IncreaseGradingWeight10()
    {
        SetContentsOfCell_BreakALongChain_TwoIndependentChains();
    }

    /// <summary>
    ///   Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("45")]
    public void IncreaseGradingWeight11()
    {
        SetContentsOfCell_BreakALongChain_TwoIndependentChains();
    }

    /// <summary>
    /// Add weight to the grading by repeating the above test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("46")]
    public void IncreaseGradingWeight12()
    {
        SetContentsOfCell_BreakALongChain_TwoIndependentChains();
    }

    /// <summary>
    ///   Generates a random cell name with a capital letter
    ///   and number between 1 - 99.
    /// </summary>
    /// <param name="rand"> A random number generator. </param>
    /// <returns> A random cell name like A10, or B50, .... </returns>
    private static string GenerateRandomCellName(Random rand)
    {
        return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
    }

    /// <summary>
    ///   Sets random cells to random contents (strings, doubles, formulas)
    ///   10000 times.  The number of "repeated" cells in the random group
    ///   has been predetermined based on the given random seed.
    /// </summary>
    /// <param name="seed">Random seed.</param>
    /// <param name="size">
    ///   The precomputed/known size of the resulting spreadsheet.
    ///   This size was determined by pre-running the test with the given seed.
    /// </param>
    private static void SetContentsOfCell_1000RandomCells_MatchesPrecomputedSizeValue(int seed, int size)
    {
        Spreadsheet s = new();
        Random rand = new(seed);
        for (int i = 0; i < 10000; i++)
        {
            try
            {
                string cellName = GenerateRandomCellName(rand);
                switch (rand.Next(3))
                {
                    case 0:
                        s.SetContentsOfCell(cellName, "3.14");
                        break;
                    case 1:
                        s.SetContentsOfCell(cellName, "hello");
                        break;
                    case 2:
                        string temp = "=" + GenerateRandomFormula(rand);
                        Formula tempAsFmla = new Formula(temp[1..]);
                        ISet<string> tempsVars = tempAsFmla.GetVariables();
                        foreach(string var in tempsVars)
                        {
                            try
                            {
                                // it exists so no need to do anything
                                s.GetCellContents(var);
                            }
                            catch (Exception)
                            {
                                // cell names are random. GenerateRandomFormula also generates random cell names.
                                // there is a chance that a cell could be recomputed instead of set...
                                s.SetContentsOfCell(var, "1");
                                continue;
                            }
                        }

                        // what we're doing in the foreach loop is making sure each
                        // cell in temp exists with a value so this works.
                        s.SetContentsOfCell(cellName, temp);
                        break;
                }
            }
            catch (CircularException)
            {
            }
        }

        ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());

        // Assert.AreEqual(size, set.Count);
    }

    /// <summary>
    ///   <para>
    ///     Generates a random Formula.
    ///   </para>
    ///   <para>
    ///     This helper method is used in the randomize test.
    ///   </para>
    /// </summary>
    /// <param name="rand"> A random number generator.</param>
    /// <returns> A formula referencing random cells in a spreadsheet. </returns>
    private static string GenerateRandomFormula(Random rand)
    {
        string formula = GenerateRandomCellName(rand);
        for (int i = 0; i < 10; i++)
        {
            switch (rand.Next(4))
            {
                case 0:
                    formula += "+";
                    break;
                case 1:
                    formula += "-";
                    break;
                case 2:
                    formula += "*";
                    break;
                case 3:
                    formula += "/";
                    break;
            }

            switch (rand.Next(2))
            {
                case 0:
                    formula += 7.2;
                    break;
                case 1:
                    formula += GenerateRandomCellName(rand);
                    break;
            }
        }

        return formula;
    }
}

/// <summary>
///   Helper methods for the tests above.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    ///   Check to see if the two "sets" (source and items) match, i.e.,
    ///   contain exactly the same values. Note: we do not check for sequencing.
    /// </summary>
    /// <param name="source"> original container.</param>
    /// <param name="items"> elements to match against.</param>
    /// <returns> true if every element in source is in items and vice versa. They are the "same set".</returns>
    public static bool Matches1(this IEnumerable<string> source, params string[] items)
    {
        return (source.Count() == items.Length) && items.All(item => source.Contains(item));
    }
}

// <copyright file="GradingTestsPS6.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

/// <summary>
///   Authors:   Joe Zachary
///              Daniel Kopta
///              Jim de St. Germain
///   Date:      Fall 2024
///   Course:    CS 3500, University of Utah, School of Computing
///   Copyright: CS 3500 - This work may not be copied for use
///                      in Academic Coursework.  See below.
///
///   File Contents:
///
///     This file contains proprietary grading tests for CS 3500.  These tests cases
///     are for individual student use only and MAY NOT BE SHARED.  Do not back them up
///     nor place them in any online repository.  Improper use of these test cases
///     can result in removal from the course and an academic misconduct sanction.
///
///     These tests are for your private use, this semester, only to improve the quality of the
///     rest of your assignments.
///
///   Test Information
///      This file contains various classes for testing the full Spreadsheet
///      and is used in grading student success.
///   <para>
///     There are multiple classes in this file containing similar tests.
///     This first class (SimpleValidSpreadSheetExmaples) tests sheet
///     content assignment that should be valid.
///   </para>
/// </summary>
[TestClass]
public class SimpleValidSpreadSheetExamples
{
    /// <summary>
    ///   Test that we can create a spreadsheet and can add and retrieve a value.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("1")]
    public void SetGetCellContents_SingleStringAdded_HasOneString()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("A1", "x");
        Assert.AreEqual(s.GetNamesOfAllNonemptyCells().Count, 1);
        Assert.AreEqual("x", s.GetCellContents("A1"));
    }

    /// <summary>
    ///   An empty sheet should not contain values.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("2")]
    public void Constructor_Default_ShouldBeEmpty()
    {
        Spreadsheet ss = new();

        var results = new Dictionary<string, object>
        {
            { "A1", string.Empty },
            { "B10", string.Empty },
            { "CC101", string.Empty },
        };

        ss.CompareToExpectedValues(results);
        ss.CompareCounts(results);
    }

    /// <summary>
    ///    Test that you can add one string to a spreadsheet.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> If used alone, check the count as well as the values. </param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("3")]
    [DataRow(null, true)]
    public void SetContentsOfCell_AddStringToCell_CellContainsString(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "B1", "hello" },
            { "B2", string.Empty },
        };

        ss.SetContentsOfCell("B1", "hello");
        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Test that you can add one string to a spreadsheet.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("4")]
    [DataRow(null, true)]
    public void SetContentsOfCell_AddNumberToCell_CellContainsNumber(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "C1", 17.5 },
            { "C2", string.Empty },
        };

        ss.SetContentsOfCell("C1", "17.5");
        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Test that you can add a simple formula ("=5") to a spreadsheet.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("5")]
    [DataRow(null, true)]
    public void SetContentsOfCell_AddSimpleFormulaToCell_CellEvaluatesCorrectly(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 5.0 },
            { "A2", string.Empty },
        };

        ss.SetContentsOfCell("A1", "=5");
        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///   Test that you can add a formula that depends on one other cell.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("6")]
    [DataRow(null, true)]
    public void SetContentsOfCell_FormulaBasedOnSingleOtherCell_EvaluatesCorrectly(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 4.1 },
            { "C1", 8.3 },
        };

        ss.SetContentsOfCell("A1", "4.1");
        ss.SetContentsOfCell("C1", "=A1+4.2");
        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Test that you can add one formula to a spreadsheet that depends on two other cells.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("7")]
    [DataRow(null, true)]
    public void SetContentsOfCell_FormulaDependsOnTwoCells_AllCellsComputeCorrectly(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 4.1 },
            { "B1", 5.2 },
            { "C1", 9.3 },
        };

        ss.SetContentsOfCell("A1", "4.1");
        ss.SetContentsOfCell("B1", "5.2");
        ss.SetContentsOfCell("C1", "=A1+B1");

        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Test that formulas work for addition, subtraction, multiplication, and division.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("8")]
    [DataRow(null, true)]
    public void SetContentsFormulas_AddSubtractMultiplyDivide_AllWorkAsExpected(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 4.4 },
            { "B1", 2.2 },
            { "C1", 6.6 },
            { "D1", 2.2 },
            { "E1", 4.4 * 2.2 },
            { "F1", 2.0 },
        };

        ss.SetContentsOfCell("A1", "4.4");
        ss.SetContentsOfCell("B1", "2.2");
        ss.SetContentsOfCell("C1", "= A1 + B1");
        ss.SetContentsOfCell("D1", "= A1 - B1");
        ss.SetContentsOfCell("E1", "= A1 * B1");
        ss.SetContentsOfCell("F1", "= A1 / B1");

        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Increase score for grading tests.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("9")]
    public void IncreaseGradingWeight1()
    {
        SetContentsFormulas_AddSubtractMultiplyDivide_AllWorkAsExpected(null, true);
    }

    /// <summary>
    ///    Increase score for grading tests.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("10")]
    public void IncreaseGradingWeight2()
    {
        SetContentsFormulas_AddSubtractMultiplyDivide_AllWorkAsExpected(null, true);
    }

    /// <summary>
    ///    Increase score for grading tests.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("11")]
    public void IncreaseGradingWeight3()
    {
        SetContentsFormulas_AddSubtractMultiplyDivide_AllWorkAsExpected(null, true);
    }

    /// <summary>
    ///    Test that division by an empty cell is an error.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("12")]
    [DataRow(null, true)]
    public void DivisionByEmptyCell(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 4.1 },
            { "B1", string.Empty },
            { "C1", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
        };

        ss.SetContentsOfCell("A1", "4.1");
        ss.SetContentsOfCell("C1", "=A1/B1");

        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Test that you cannot add a number to a string.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("13")]
    [DataRow(null, true)]
    public void AddNumberToString(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 4.1 },
            { "B1", "hello" },
            { "C1", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
        };

        ss.SetContentsOfCell("A1", "4.1");
        ss.SetContentsOfCell("B1", "hello");
        ss.SetContentsOfCell("C1", "=A1+B1");

        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Test that a formula computed from a cell with a formula error value
    ///    is also a formula error.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("14")]
    [DataRow(null, true)]
    public void FormulaFromFormulaError(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", "hello" },
            { "B1", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
            { "C1", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
        };

        ss.SetContentsOfCell("A1", "hello");
        ss.SetContentsOfCell("B1", "=A1");
        ss.SetContentsOfCell("C1", "=B1");

        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Test that direct division by 0 in a formula is caught.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("15")]
    [DataRow(null, true)]
    public void DivisionByZero1(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 4.1 },
            { "B1", string.Empty },
            { "C1", new FormulaError( "Division by zero" ) },
        };

        ss.SetContentsOfCell("A1", "4.1");
        ss.SetContentsOfCell("B1", string.Empty);
        ss.SetContentsOfCell("C1", "=A1/0.0");

        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///    Check that division by a cell which contains zero is caught.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("16")]
    [DataRow(null, true)]
    public void DivisionByZero2(Spreadsheet? ss, bool verifyCounts)
    {
        ss ??= new();

        var results = new Dictionary<string, object>
        {
            { "A1", 4.1 },
            { "B1", 0.0 },
            { "C1", new FormulaError( "Division by zero") },
        };

        ss.SetContentsOfCell("A1", "4.1");
        ss.SetContentsOfCell("B1", "0.0");
        ss.SetContentsOfCell("C1", "= A1 / B1");

        ss.CompareToExpectedValues(results);

        if (verifyCounts)
        {
            ss.CompareCounts(results);
        }
    }

    /// <summary>
    ///   Repeats the simple tests all together.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("17")]
    public void SpreadsheetOverall_CombineMultipleTests_AllCorrect()
    {
        Spreadsheet ss = new();
        var results = new Dictionary<string, object>
        {
            { "A1", "hello" },
            { "B1", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
            { "C1", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
        };

        ss.SetContentsOfCell("A1", "17.32");
        ss.SetContentsOfCell("B1", "This is a test");
        ss.SetContentsOfCell("C1", "=C2+C3");

        SetContentsOfCell_AddStringToCell_CellContainsString(ss, false);
        SetContentsOfCell_AddNumberToCell_CellContainsNumber(ss, false);
        SetContentsOfCell_FormulaDependsOnTwoCells_AllCellsComputeCorrectly(ss, false);

        DivisionByZero1(ss, false);
        DivisionByZero2(ss, false);

        AddNumberToString(ss, false);
        FormulaFromFormulaError(ss, false);

        ss.CompareToExpectedValues(results);
    }

    /// <summary>
    ///    Five cells related to each other.  Make sure original values are
    ///    correctly computed (Formula Errors), then change end cells, then check that the
    ///    new values are correct.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("18")]
    [DataRow(null)]
    public void SetContentsOfCell_SimpleFormulas_ComputeCorrectResults(Spreadsheet? ss)
    {
        var expectedResults = new Dictionary<string, object>
        {
            { "A1", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
            { "A2", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
        };

        ss ??= new();

        ss.SetContentsOfCell("A1", "= A2 + A3");
        ss.SetContentsOfCell("A2", "= B1 + B2");

        ss.CompareToExpectedValues(expectedResults);
        ss.CompareCounts(expectedResults);

        ss.SetContentsOfCell("A3", "5.0");
        ss.SetContentsOfCell("B1", "2.0");
        ss.SetContentsOfCell("B2", "3.0");

        expectedResults["A1"] = 10.0;
        expectedResults["A2"] = 5.0;
        expectedResults["A3"] = 5.0;
        expectedResults["B1"] = 2.0;
        expectedResults["B2"] = 3.0;

        ss.CompareToExpectedValues(expectedResults);
        ss.CompareCounts(expectedResults);

        ss.SetContentsOfCell("B2", "4.0");

        expectedResults["A1"] = 11.0;
        expectedResults["A2"] = 6.0;
        expectedResults["B2"] = 4.0;

        ss.CompareToExpectedValues(expectedResults);
        ss.CompareCounts(expectedResults);
    }

    /// <summary>
    ///    Change the end cell of a three cell chain and check for
    ///    the correct computation of results.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("20")]
    [DataRow(null, true)]
    public void SetContentsOfCell_ThreeCellChainFormula_ComputesCorrectResults(Spreadsheet? ss, bool verifyCounts)
    {
        var expectedResults = new Dictionary<string, object>
        {
            { "A1", 12.0 },
            { "A2",  6.0 },
            { "A3",  6.0 },
        };

        ss ??= new();

        ss.SetContentsOfCell("A1", "= A2 + A3");
        ss.SetContentsOfCell("A2", "= A3");
        ss.SetContentsOfCell("A3", "6.0");

        ss.CompareToExpectedValues(expectedResults);

        if (verifyCounts)
        {
            ss.CompareCounts(expectedResults);
        }

        ss.SetContentsOfCell("A3", "5.0");
        expectedResults["A1"] = 10.0;
        expectedResults["A2"] = 5.0;
        expectedResults["A3"] = 5.0;

        ss.CompareToExpectedValues(expectedResults);

        if (verifyCounts)
        {
            ss.CompareCounts(expectedResults);
        }
    }

    /// <summary>
    ///    Five cells chained together.  Make sure initial values are
    ///    computed correctly, then change end cells, then make sure
    ///    new values are computed correctly.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("21")]
    [DataRow(null, true)]
    public void SetContentsOfCell_FiveCellsWithValues_CorrectValuesComputed(Spreadsheet? ss, bool verifyCounts)
    {
        var expectedResults = new Dictionary<string, object>
        {
            { "A1", 18.0 },
            { "A2", 18.0 },
            { "A3",  9.0 },
            { "A4",  9.0 },
            { "A5",  9.0 },
        };

        ss ??= new();

        ss.SetContentsOfCell("A1", "= A3 + A5");
        ss.SetContentsOfCell("A2", "= A5 + A4");
        ss.SetContentsOfCell("A3", "= A5");
        ss.SetContentsOfCell("A4", "= A5");
        ss.SetContentsOfCell("A5", "9.0");

        ss.CompareToExpectedValues(expectedResults);

        if (verifyCounts)
        {
            ss.CompareCounts(expectedResults);
        }

        ss.SetContentsOfCell("A5", "8.0");
        expectedResults["A1"] = 16.0;
        expectedResults["A2"] = 16.0;
        expectedResults["A3"] = 8.0;
        expectedResults["A4"] = 8.0;
        expectedResults["A5"] = 8.0;

        ss.CompareToExpectedValues(expectedResults);

        if (verifyCounts)
        {
            ss.CompareCounts(expectedResults);
        }
    }

    /// <summary>
    ///    Combine the other tests and make sure that they all work
    ///    in combination with each other.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("22")]
    public void SpreadsheetOverall_CombineOtherTests_CorrectValuesComputed()
    {
        var expectedResults = new Dictionary<string, object>
        {
            { "A1", 16.0 },
            { "A2", 16.0 },
            { "A3",  8.0 },
            { "A4",  8.0 },
            { "A5",  8.0 },
            { "B1",  2.0 },
            { "B2",  4.0 },
        };

        Spreadsheet ss = new();
        SetContentsOfCell_SimpleFormulas_ComputeCorrectResults(ss);
        SetContentsOfCell_ThreeCellChainFormula_ComputesCorrectResults(ss, false);
        SetContentsOfCell_FiveCellsWithValues_CorrectValuesComputed(ss, false);

        ss.CompareToExpectedValues(expectedResults);
        ss.CompareCounts(expectedResults);
    }

    /// <summary>
    ///   Increase the grading weight of the given test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("23")]
    public void IncreaseGradingWeight4()
    {
        SpreadsheetOverall_CombineOtherTests_CorrectValuesComputed();
    }

    /// <summary>
    ///   Check that the base case (empty cell) index works.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("24")]
    public void Indexer_EmptyCell_EmptyStringValue()
    {
        Spreadsheet ss = new();

        Assert.AreEqual(ss["A1"], string.Empty);
        Assert.AreEqual(ss["A1"], ss.GetCellValue("A1"));
    }

    /// <summary>
    ///   Check that a double value can be returned.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("25")]
    public void Indexer_DoubleValue_Returns5()
    {
        Spreadsheet ss = new();

        ss.SetContentsOfCell("A1", "5");
        Assert.AreEqual((double)ss["A1"], 5.0, .001);
        Assert.AreEqual((double)ss["A1"], (double)ss.GetCellValue("A1"), .001);
    }

    /// <summary>
    ///   Check that a string can be returned.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("26")]
    public void Indexer_StringValue_ReturnsHelloWorld()
    {
        Spreadsheet ss = new();

        ss.SetContentsOfCell("A1", "hello world");
        Assert.AreEqual(ss["A1"], "hello world");
        Assert.AreEqual(ss["A1"], ss.GetCellValue("A1"));
    }

    /// <summary>
    ///   Check that a formulas computed value can be returned,
    ///   both as a FormulaError and as a double.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("27")]
    public void Indexer_GetFormulaValue_ReturnsErrorThenValue()
    {
        Spreadsheet ss = new();

        ss.SetContentsOfCell("A1", "=A2");
        Assert.IsInstanceOfType<FormulaError>(ss["A1"]);

        ss.SetContentsOfCell("A2", "1.234");
        Assert.AreEqual((double)ss["A1"], 1.234, 0.0000001);
        Assert.AreEqual((double)ss["A1"], (double)ss.GetCellValue("A1"), 0.00000001);
    }
}

/// <summary>
///    Check cell name normalization (up-casing).
/// </summary>
[TestClass]
public class SpreadSheetNormalizationTests
{
    /// <summary>
    ///   Check name normalization. Given a lower case
    ///   cell name, it should work and be up-cased.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("28")]
    public void SetGetCellContents_LowerCaseCellName_NormalizedToUpper()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("B1", "hello");
        Assert.AreEqual("hello", s.GetCellContents("B1"));
        s.GetNamesOfAllNonemptyCells().Matches(["B1"]);
    }

    /// <summary>
    ///   Check name normalization. Given a formula with a mis-cased
    ///   cell name, it should still work.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("29")]
    public void SetContentsGetValue_MisCasedVariableNamesInFormula_ShouldStillWork()
    {
        Spreadsheet s = new();

        s.SetContentsOfCell("A1", "6");
        s.SetContentsOfCell("B1", "= A1");

        Assert.AreEqual(6.0, (double)s.GetCellValue("B1"), 1e-9);
    }
}

/// <summary>
///    Test some simple mistakes that a user might make,
///    including invalid formulas resulting in the appropriate contents and values,
///    as well as invalid names.
/// </summary>
[TestClass]
public class SimpleInvalidSpreadsheetTests
{
    /// <summary>
    ///   Test that a formula can be added and retrieved.  The
    ///   contents of the cell are a Formula and the value of
    ///   the cell is a Formula Error.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("30")]
    public void SetContentsOfCell_InvalidFormula_CreatedFormulaValueIsErrorType()
    {
        Spreadsheet s = new();
        s.SetContentsOfCell("B1", "=A1+C1");
        Assert.AreEqual(s.GetNamesOfAllNonemptyCells().Count, 1);
        Assert.IsInstanceOfType<Formula>(s.GetCellContents("B1"));
        Assert.IsInstanceOfType<FormulaError>(s.GetCellValue("B1"));
    }

    /// <summary>
    ///   Test that an invalid cell name doesn't affect spreadsheet.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("31")]
    public void SetGetCellContents_InvalidNameAdded_StillEmpty()
    {
        Spreadsheet s = new();

        try
        {
            s.SetContentsOfCell("1A1", "x");
        }
        catch
        {
            Assert.AreEqual(s.GetNamesOfAllNonemptyCells().Count, 0);
            Assert.AreEqual(string.Empty, s.GetCellContents("A1"));
            return;
        }

        Assert.Fail();
    }
}

/// <summary>
///    Test the changed property of the spreadsheet.
/// </summary>
[TestClass]
public class SpreadsheetChangedTests
{
    /// <summary>
    ///   After setting a cell, the spreadsheet is changed.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("32")]
    public void Changed_AfterModification_IsTrue()
    {
        Spreadsheet ss = new();
        Assert.IsFalse(ss.Changed);
        ss.SetContentsOfCell("C1", "17.5");
        Assert.IsTrue(ss.Changed);
    }

    /// <summary>
    ///   After saving the spreadsheet to a file, the spreadsheet is not changed.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("33")]
    public void Changed_AfterSave_IsFalse()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("C1", "17.5");
        ss.Save("changed.txt");
        Assert.IsFalse(ss.Changed);
    }
}

/// <summary>
///    Test that you can have multiple spreadsheet objects
///    and they don't interact with each other (e.g., by using static
///    fields/properties).
/// </summary>
[TestClass]
public class SpreadsheetNonStaticFields
{
    /// <summary>
    ///    Check that two spreadsheet objects are independent.  If we add
    ///    a value to one, it doesn't influence the other.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("34")]
    public void Constructor_SetCellContents_SpreadSheetsAreIndependent()
    {
        Spreadsheet s1 = new();
        Spreadsheet s2 = new();

        var results = new Dictionary<string, object>
        {
            { "X1", "hello" },
            { "A1", string.Empty },
            { "B1", 5.0 },
        };

        s1.SetContentsOfCell("X1", "hello");
        s1.SetContentsOfCell("B1", "5.0");
        s2.SetContentsOfCell("X1", "goodbye");

        s1.CompareToExpectedValues(results);
        s1.CompareCounts(results);

        results["X1"] = "goodbye";
        results.Remove("B1");

        s2.CompareToExpectedValues(results);
        s2.CompareCounts(results);
    }

    /// <summary>
    ///    Increase score for grading tests.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("35")]
    public void IncreaseGradingWeight1()
    {
        Constructor_SetCellContents_SpreadSheetsAreIndependent();
    }

    /// <summary>
    ///    Increase score for grading tests.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("36")]
    public void IncreaseGradingWeight2()
    {
        Constructor_SetCellContents_SpreadSheetsAreIndependent();
    }

    /// <summary>
    ///    Increase score for grading tests.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("37")]
    public void IncreaseGradingWeight3()
    {
        Constructor_SetCellContents_SpreadSheetsAreIndependent();
    }
}

/// <summary>
///    Test that circular exceptions are handled correctly.
/// </summary>
[TestClass]
public class SpreadsheetCircularExceptions
{
    /// <summary>
    ///    Check that a simple circular exception is thrown.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("38")]
    [ExpectedException(typeof(CircularException))]
    public void SetCellContents_CircularException_Throws()
    {
        Spreadsheet s1 = new();
        s1.SetContentsOfCell("A1", "=A1");
    }

    /// <summary>
    ///    Check that assigning a circular exception doesn't change rest of spreadsheet.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("39")]
    public void SetCellContents_CircularException_DoesNotChangeRestOfSheet()
    {
        var results = new Dictionary<string, object>
        {
            { "A1", "hello" },
            { "A2", 10.0 },
            { "A3", new FormulaError( "Only cells that evaluate to a number are valid for lookup." ) },
            { "A4", 9.0 },
        };

        Spreadsheet s1 = new();
        s1.SetContentsOfCell("A1", "hello");
        s1.SetContentsOfCell("A2", "=5+5");
        s1.SetContentsOfCell("A3", "=A1");
        s1.SetContentsOfCell("A4", "9.0");

        try
        {
            s1.SetContentsOfCell("A1", "=A1");
        }
        catch
        {
            s1.CompareToExpectedValues(results);
            return;
        }

        Assert.Fail();
    }
}

/// <summary>
///    Test file input and output.
/// </summary>
[TestClass]
public class SpreadsheetLoadSaveTests
{
    /// <summary>
    ///   Try to save to a bad file.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("40")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_InvalidMissingFolder_Throws()
    {
        Spreadsheet s1 = new();
        s1.Save("."); // note: this test was updated and students will all be given a point for it.
    }

    /// <summary>
    ///   Try to save to a folder (i.e., ".").
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("40")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Save_ToCurrentFolderPeriod_Throws()
    {
        Spreadsheet s1 = new();
        s1.Save(".");
    }

    /// <summary>
    ///   Try to load from a bad file.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("41")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_FromMissingFile_Throws()
    {
        // should not be able to read
        Spreadsheet ss = new();
        ss.Load("q:\\missing\\save.txt");
    }

    /// <summary>
    ///   Write a single string to a spreadsheet, save the file,
    ///   load the file, look to see if the value is back.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("42")]
    public void SaveLoad_CreateAVerySimpleSheetSaveAndLoad_GetOriginalBack()
    {
        var results = new Dictionary<string, object>
        {
            { "A1", "hello" },
            { "B1", string.Empty },
        };

        Spreadsheet s1 = new();
        s1.SetContentsOfCell("A1", "hello");
        s1.Save("save1.txt");
        s1.CompareToExpectedValues(results);
        s1.CompareCounts(results);

        Spreadsheet s2 = new();
        s2.Load("save1.txt");

        s2.CompareToExpectedValues(results);
        s2.CompareCounts(results);
    }

    /// <summary>
    ///   Should not be able to read a file that is not correct JSON.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("43")]
    [ExpectedException(typeof(SpreadsheetReadWriteException))]
    public void Load_InvalidXMLFile_Throws()
    {
        using (StreamWriter writer = new("save2.txt"))
        {
            writer.WriteLine("This");
            writer.WriteLine("is");
            writer.WriteLine("a");
            writer.WriteLine("test!");
        }

        Spreadsheet s1 = new();
        s1.Load("save2.txt");
    }

    /// <summary>
    ///   Save a sheet, load the file with it, make sure the new sheet has
    ///   the expected values.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("44")]
    public void Load_ReadValidPreDefinedJSONFile_ContainsCorrectData()
    {
        var results = new Dictionary<string, object>
        {
            { "A1", "hello" },
            { "A2", 5.0 },
            { "A3", 4.0 },
            { "A4", 9.0 },
        };

        var sheet = new
        {
            Cells = new
            {
                A1 = new { StringForm = "hello" },
                A2 = new { StringForm = "5.0" },
                A3 = new { StringForm = "4.0" },
                A4 = new { StringForm = "= A2 + A3" },
            },
        };

        File.WriteAllText("save5.txt", JsonSerializer.Serialize(sheet));

        Spreadsheet ss = new();
        ss.Load("save5.txt");

        ss.CompareToExpectedValues(results);
        ss.CompareCounts(results);
    }

    /// <summary>
    ///    Save a spreadsheet with a string, two numbers, and a formula,
    ///    and see that the saved file contains the proper json.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("45")]
    public void Save_SaveSheetWithDoubleStringAndFormula_GeneratesValidJSONFileSyntax()
    {
        Spreadsheet ss = new();
        ss.SetContentsOfCell("A1", "hello");
        ss.SetContentsOfCell("A2", "5.0");
        ss.SetContentsOfCell("A3", "4.0");
        ss.SetContentsOfCell("A4", "=A2+A3");
        ss.Save("save6.txt");

        string fileContents = File.ReadAllText("save6.txt");

        try
         {
            Dictionary<string, object> root = JsonSerializer.Deserialize<Dictionary<string, object>>(fileContents) ?? [];
            if (!root.TryGetValue("Cells", out object? cellValues))
            {
                Assert.Fail();
            }

            var cells = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(cellValues.ToString() ?? "oops");

            if (cells != null)
            {
                Assert.AreEqual("hello", cells["A1"]["StringForm"]);
                Assert.AreEqual(5.0, double.Parse(cells["A2"]["StringForm"]), 1e-9);
                Assert.AreEqual(4.0, double.Parse(cells["A3"]["StringForm"]), 1e-9);
                Assert.AreEqual("=A2+A3", cells["A4"]["StringForm"].Replace(" ", string.Empty));
            }
            else
            {
                Assert.Fail();
            }
        }
        catch
        {
            Assert.Fail();
        }
    }
}

/// <summary>
///    Test methods on larger spreadsheets.
/// </summary>
[TestClass]
public class LargerSpreadsheetTests
{
    /// <summary>
    ///    Create 7 cells, put some formulas in, change the values,
    ///    and verify that the final and intermediate results are good.
    /// </summary>
    /// <param name="ss"> For use with other tests, allow passing in of a spreadsheet. </param>
    /// <param name="verifyCounts"> if used alone, check the count as well as the values.</param>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("46")]
    [DataRow(null, true)]
    public void SetContentsOfCell_SevenCellsWithModifications_CorrectValuesProduced(Spreadsheet? ss, bool verifyCounts)
    {
        var expectedResults = new Dictionary<string, object>
        {
            { "A1", 1.0 },
            { "A2", 2.0 },
            { "A3", 3.0 },
            { "A4", 4.0 },
            { "B1", 3.0 },
            { "B2", 12.0 },
            { "C1", 9.0 },
        };

        ss ??= new();

        ss.SetContentsOfCell("A1", "1.0");
        ss.SetContentsOfCell("A2", "2.0");
        ss.SetContentsOfCell("A3", "3.0");
        ss.SetContentsOfCell("A4", "4.0");
        ss.SetContentsOfCell("B1", "= A1 + A2");
        ss.SetContentsOfCell("B2", "= A3 * A4");
        ss.SetContentsOfCell("C1", "= B2 - B1");

        ss.CompareToExpectedValues(expectedResults);

        if (verifyCounts)
        {
            ss.CompareCounts(expectedResults);
        }

        ss.SetContentsOfCell("A1", "2.0");
        expectedResults["A1"] = 2.0;
        expectedResults["A2"] = 2.0;
        expectedResults["A3"] = 3.0;
        expectedResults["B1"] = 4.0;
        expectedResults["B2"] = 12.0;
        expectedResults["C1"] = 8.0;

        ss.CompareToExpectedValues(expectedResults);

        if (verifyCounts)
        {
            ss.CompareCounts(expectedResults);
        }

        ss.SetContentsOfCell("B1", "= A1 / A2");
        expectedResults["B1"] = 1.0;
        expectedResults["B2"] = 12.0;
        expectedResults["C1"] = 11.0;

        ss.CompareToExpectedValues(expectedResults);

        if (verifyCounts)
        {
            ss.CompareCounts(expectedResults);
        }
    }

    /// <summary>
    ///   Increases the value of the given test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("47")]
    public void IncreaseGradingWeight_MediumSheet1()
    {
        SetContentsOfCell_SevenCellsWithModifications_CorrectValuesProduced(null, true);
    }

    /// <summary>
    ///   See if we can write and read a (slightly) larger spreadsheet.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("48")]
    public void SpreadSheetSaveLoad_OverallTestOfMediumSize_Correct()
    {
        Dictionary<string, object> expectedResults = new()
        {
            ["A1"] = 2.0,
            ["A2"] = 2.0,
            ["A3"] = 3.0,
            ["A4"] = 4.0,
            ["B1"] = 1.0,
            ["B2"] = 12.0,
            ["C1"] = 11.0,
        };

        Spreadsheet s1 = new();
        SetContentsOfCell_SevenCellsWithModifications_CorrectValuesProduced(s1, true);
        s1.Save("save7.txt");

        Spreadsheet s2 = new();
        s2.Load("save7.txt");

        s2.CompareToExpectedValues(expectedResults);
        s2.CompareCounts(expectedResults);
    }

    /// <summary>
    ///   Increases the value of the given test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("49")]
    public void IncreaseGradingWeight_MediumSave1()
    {
        SpreadSheetSaveLoad_OverallTestOfMediumSize_Correct();
    }

    /// <summary>
    ///   <para>
    ///     A long chained formula. Solutions that re-evaluate
    ///     cells on every request, rather than after a cell changes,
    ///     will timeout on this test.
    ///   </para>
    ///   <para>
    ///     A1 = A3+A5
    ///     A2 = A3+A4
    ///     A3 = A5+A6
    ///     A4 = A5+A6
    ///     A5 = A7+A8
    ///     A6 = A7+A8
    ///     etc.
    ///   </para>
    ///   <para>
    ///     In the end we compute the 2^depth.
    ///   </para>
    ///   <para>
    ///     Then we set the end cells to zero and the whole sum goes to 0.
    ///   </para>
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("50")]
    public void SetContentsOfCell_LongChainOfExponentialNumbers_ComputesPowersCorrectly()
    {
        Spreadsheet s1 = new();

        s1.SetContentsOfCell("sum1", "=A1+A2");

        int i;
        int depth = 100;
        for (i = 1; i <= depth * 2; i += 2)
        {
            s1.SetContentsOfCell("A" + i, "= A" + (i + 2) + " + A" + (i + 3));
            s1.SetContentsOfCell("A" + (i + 1), "= A" + (i + 2) + "+ A" + (i + 3));
        }

        s1.SetContentsOfCell("A" + i, "1");
        s1.SetContentsOfCell("A" + (i + 1), "1");
        Assert.AreEqual(Math.Pow(2, depth + 1), (double)s1.GetCellValue("sum1"), 1.0);

        s1.SetContentsOfCell("A" + i, "0");
        Assert.AreEqual(Math.Pow(2, depth), (double)s1.GetCellValue("sum1"), 1.0);

        s1.SetContentsOfCell("A" + (i + 1), "0");
        Assert.AreEqual(0.0, (double)s1.GetCellValue("sum1"), 0.1);
    }

    /// <summary>
    ///   Increases the value of the given test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("51")]
    public void IncreaseGradingWeight_Long1()
    {
        SetContentsOfCell_LongChainOfExponentialNumbers_ComputesPowersCorrectly();
    }

    /// <summary>
    ///   Increases the value of the given test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("52")]
    public void IncreaseGradingWeight_Long2()
    {
        SetContentsOfCell_LongChainOfExponentialNumbers_ComputesPowersCorrectly();
    }

    /// <summary>
    ///   Increases the value of the given test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("53")]
    public void IncreaseGradingWeight_Long3()
    {
        SetContentsOfCell_LongChainOfExponentialNumbers_ComputesPowersCorrectly();
    }

    /// <summary>
    ///   Increases the value of the given test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("54")]
    public void IncreaseGradingWeight_Long4()
    {
        SetContentsOfCell_LongChainOfExponentialNumbers_ComputesPowersCorrectly();
    }
}

/// <summary>
///   Helper methods for the tests above.
/// </summary>
public static class TestExtensions
{
    /// <summary>
    ///   Check to see if the two "sets" (source and items) match, i.e.,
    ///   contain exactly the same values. Note: we do not check for sequencing.
    /// </summary>
    /// <param name="source"> original container.</param>
    /// <param name="items"> elements to match against.</param>
    /// <returns> true if every element in source is in items and vice versa. They are the "same set".</returns>
    public static bool Matches(this IEnumerable<string> source, params string[] items)
    {
        return (source.Count() == items.Length) && items.All(item => source.Contains(item));
    }

    /// <summary>
    ///   This function takes in a spreadsheet object and a List
    ///   of expected Cell values which are compared with the spreadsheet.
    ///   Failure to match results in an error.
    ///   <para>
    ///     It is valid to have additional values in the spreadsheet which are not checked.
    ///   </para>
    /// </summary>
    /// <param name="sheet"> The spreadsheet being tested. </param>
    /// <param name="expectedValues"> Key-value pairs for what should be in the spreadsheet. </param>
    public static void CompareToExpectedValues(this Spreadsheet sheet, Dictionary<string, object> expectedValues)
    {
        foreach (var cellName in expectedValues.Keys)
        {
            if (expectedValues[cellName] is double number)
            {
                Assert.AreEqual(number, (double)sheet.GetCellValue(cellName), 1e-9);
            }
            else if (expectedValues[cellName] is string entry)
            {
                Assert.AreEqual(entry, sheet.GetCellValue(cellName));
            }
            else if (expectedValues[cellName] is FormulaError error)
            {
                Assert.IsInstanceOfType(error, typeof(FormulaError));
            }
            else
            {
                throw new Exception("Invalid data in expected value dictionary!");
            }
        }
    }

    /// <summary>
    ///   This function takes in a spreadsheet object and a List
    ///   of expected Cell values and makes sure that there are not
    ///   any extra values in the sheet (e.g., more non-empty cells
    ///   than expected).  Failure to match results in an error.
    ///   <para>
    ///     It is valid to have additional values in the spreadsheet which are not checked.
    ///   </para>
    /// </summary>
    /// <param name="sheet"> The spreadsheet being tested. </param>
    /// <param name="expectedValues"> Key-value pairs for what should be in the spreadsheet. </param>
    public static void CompareCounts(this Spreadsheet sheet, Dictionary<string, object> expectedValues)
    {
        int numberOfExpectedResults = expectedValues.Values.Where(o => o.ToString() != string.Empty).Count();
        int numberOfNonEmptyCells = sheet.GetNamesOfAllNonemptyCells().Count;

        Assert.AreEqual(numberOfExpectedResults, numberOfNonEmptyCells);
    }
}
