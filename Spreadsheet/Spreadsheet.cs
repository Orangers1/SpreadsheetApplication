// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// Author:    Isaac Huntsman
// Partner:   Josh Eggett
// Date:      9/18/24 (V1) Latest update 10/20/24
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
// Spreadsheet class representing an instance of a spreadsheet; i.e., a dictionary of
// cell names and values as Cell class, internal cell class contains contents and computed
// contents (if possible to compute). Also contains a dependency graph, a changed flag bool,
// and a name for the spreadsheet, by default, any spreadsheet instance is name "default".
// </summary>
namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Runtime.CompilerServices;

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpreadsheetReadWriteException"/> class.
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
    {
    }
}

/// <summary>
///   <para> TODO: add following api method: ;. The method is save but returns string instead of making a file.</para>
///   <para> TODO: add api method: ; // can throw ArgumentException. - load but with string not filename.
///          if json provided is invalid throw argument exception, gui displays a message.
///          The load should warn the user if an unsaved spreadsheet currently exists.
///          The load should remove all the current data before loading new data.
/// </para>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.)
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>If a cell's contents is a Formula, its value is a formula object.</item>
/// </list>
/// </summary>
public class Spreadsheet
{
    /// <summary>
    /// The dependency graph backing this spreadsheet instance, keeps track of cell references.
    /// </summary>
    private DependencyGraph masterGraph;

    /// <summary>
    /// Dictionary with string keys representing VALIDATED cell names,
    /// and Cell object values.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("Cells")]
    private Dictionary<string, Cell> cells;

    /// <summary>
    /// The name of this instance of Spreadsheet.
    /// </summary>
    private string name = "default";

    /// <summary>
    /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
    /// Initializes an empty dictionary and dependencygraph.
    /// </summary>
    public Spreadsheet()
    {
        masterGraph = new();
        cells = new();

        Changed = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Spreadsheet"/> class
    /// with a file name.
    /// Initializes an empty dictionary and dependencygraph.
    /// </summary>
    /// <param name="name">The name to set for this Spreadsheet instance. </param>
    public Spreadsheet(string name)
    {
        masterGraph = new();
        cells = new();
        this.name = name;

        Changed = false;
    }

    /// <summary>
    /// Gets a value indicating whether the spreadsheet has been modified since the last save or initial load.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }

    /// <summary>
    ///   <para>
    ///     Shortcut syntax to for getting the value of the cell
    ///     using the [] operator.
    ///     This will only ever return a double or a string, not a formula.
    ///     can also return a formulaError object. (not exception).
    ///   </para>
    ///   <para>
    ///     See: <see cref="GetCellValue(string)"/>.
    ///   </para>
    ///   <para>
    ///     Example Usage:
    ///   </para>
    ///   <code>
    ///      sheet.SetContentsOfCell( "A1", "=5+5" );
    ///
    ///      sheet["A1"] == 10;
    ///      // vs.
    ///      sheet.GetCellValue("A1") == 10;
    ///   </code>
    /// </summary>
    /// <param name="cellName"> Any valid cell name. </param>
    /// <returns>
    ///   Returns the value of a cell. Already Computed.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If the name parameter is invalid, throw an InvalidNameException.
    /// </exception>
    public object this[string cellName]
    {
        get { return GetCellValue(cellName); }
    }

    /// <summary>
    /// Get the file contents as a string, in JSON.
    /// </summary>
    /// <returns>a string representing the state of the cells.</returns>
    public string GetJSON()
    {
        string cellsStr = JsonSerializer.Serialize(cells);
        Changed = false;
        return "{\"Cells\":" + cellsStr + "}";
    }

    /// <summary>
    /// Instantiates a spreadsheet object from JSON.
    /// </summary>
    /// <param name="json">the json that contains spreadsheet state.</param>
    public void InstantiateFromJSON(string json)
    {
        Dictionary<string, Cell> cellsOld = this.cells;
        bool changedOld = Changed;
        DependencyGraph oldGraph = this.masterGraph;

        try
        {
            int startIndex = json.IndexOf("{\"Cells\":") + "{\"Cells\":".Length;
            int endIndex = json.LastIndexOf("}");
            string trimmedJson = json.Substring(startIndex, endIndex - startIndex);

            // checks if any formulaerrors in current sheet.
            this.Save("tempStorage.txt");
            this.cells = new();

            // Json exception if this is null
            Dictionary<string, Cell> eligible =
                JsonSerializer.Deserialize<Dictionary<string, Cell>>(trimmedJson)!;

            AttemptReplace(eligible);
        }
        catch
        {
            // revert changes
            this.cells = cellsOld;
            Changed = changedOld;
            this.masterGraph = oldGraph;
            throw new SpreadsheetReadWriteException("Spreadsheet contains invalid cell names," +
                "invalid formulas, or circular dependencies. Cannot load.");
        }
    }

    /// <summary>
    ///   Provides a copy of the names of all of the cells in the spreadsheet
    ///   that contain information (i.e., not empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        return cells.Keys.ToHashSet();
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        name = name.ToUpper();
        ValidateCell(name);

        if (cells.ContainsKey(name))
        {
            return cells[name].Contents;
        }

        return string.Empty;
    }

    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1"
    ///     with contents being the double 5.0, and a cell "B3" with contents
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} }.
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    ///   <para>
    ///     After saving the file, the spreadsheet is no longer "changed".
    ///   </para>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file,
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        // this call throws exceptions if any formulaerrors exist
        string cellsStr = JsonSerializer.Serialize(cells);
        string cellsStrFormatted = "{\"Cells\":" + cellsStr + "}";

        try
        {
            File.WriteAllText(filename, cellsStrFormatted);
            Changed = false;
        }
        catch
        {
            throw new SpreadsheetReadWriteException("File name/path error");
        }
    }

    /// <summary>
    ///   <para>
    ///     Read the data (JSON) from the file and instantiate the current
    ///     spreadsheet.  See <see cref="Save(string)"/> for expected format.
    ///   </para>
    ///   <para>
    ///     Note: First deletes any current data in the spreadsheet.
    ///   </para>
    ///   <para>
    ///     Loading a spreadsheet should set changed to false.  External
    ///     programs should alert the user before loading over a changed sheet.
    ///   </para>
    /// </summary>
    /// <param name="filename"> The saved file name including the path. </param>
    /// <exception cref="SpreadsheetReadWriteException"> When the file cannot be opened or the json is bad.</exception>
    public void Load(string filename)
    {
        Dictionary<string, Cell> cellsOld = this.cells;
        bool changedOld = Changed;
        DependencyGraph oldGraph = this.masterGraph;

        // cannot be null but can be string.Empty
        try
        {
            // catches IO exception if this is a bad file.
            string loadFile = File.ReadAllText(filename);

            // trim
            int startIndex = loadFile.IndexOf("{\"Cells\":") + "{\"Cells\":".Length;
            int endIndex = loadFile.LastIndexOf("}");
            string trimmedJson = loadFile.Substring(startIndex, endIndex - startIndex);

            // checks if any formulaerrors in current sheet.
            this.Save("tempStorage.txt");
            this.cells = new();

            // Json exception if this is null
            Dictionary<string, Cell> eligible =
                JsonSerializer.Deserialize<Dictionary<string, Cell>>(trimmedJson)!;

            // if any cells in eligible are formulas,
            // the deserializer sets their computed vals to empty strings.
            // this is just custome behavior. AttemptReplace will
            // set Computed any time Contents is detected as a formula.
            AttemptReplace(eligible);
        }
        catch
        {
            // revert changes
            this.cells = cellsOld;
            Changed = changedOld;
            this.masterGraph = oldGraph;
            throw new SpreadsheetReadWriteException("Spreadsheet contains invalid cell names," +
                "invalid formulas, or circular dependencies. Cannot load.");
        }
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="cellName"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value's type should be either a string, a double, or a CS3500.Formula.FormulaError.
    ///   If the cell contents are a formula, the value should have already been computed
    ///   at this point.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string cellName)
    {
        cellName = cellName.ToUpper();
        ValidateCell(cellName);

        try
        {
            object test = cells[cellName].Computed;
        }
        catch (KeyNotFoundException)
        {
            return string.Empty;
        }

        // can hold string, double, formula error, or computed formula only (a double)
        return cells[cellName].Computed;
    }

    /// <summary>
    ///   <para>
    ///       Sets the contents of the named cell to the appropriate object
    ///       based on the string in <paramref name="content"/>.
    ///   </para>
    ///   <para>
    ///       First, if the <paramref name="content"/> parses as a double, the contents of the named
    ///       cell becomes that double.
    ///   </para>
    ///   <para>
    ///       Otherwise, if the <paramref name="content"/> begins with the character '=', an attempt is made
    ///       to parse the remainder of content into a Formula.
    ///   </para>
    ///   <para>
    ///       There are then three possible outcomes when a formula is detected:
    ///   </para>
    ///
    ///   <list type="number">
    ///     <item>
    ///       If the remainder of content cannot be parsed into a Formula, a
    ///       FormulaFormatException is thrown.
    ///     </item>
    ///     <item>
    ///       If changing the contents of the named cell to be f
    ///       would cause a circular dependency, a CircularException is thrown,
    ///       and no change is made to the spreadsheet.
    ///     </item>
    ///     <item>
    ///       Otherwise, the contents of the named cell becomes f.
    ///     </item>
    ///   </list>
    ///   <para>
    ///     Finally, if the content is a string that is not a double and does not
    ///     begin with an "=" (equal sign), save the content as a string.
    ///   </para>
    ///   <para>
    ///     On successfully changing the contents of a cell, the spreadsheet will be <see cref="Changed"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell name that is being changed.</param>
    /// <param name="content"> The new content of the cell.</param>
    /// <returns>
    ///   <para>
    ///     This method returns a list consisting of the passed in cell name,
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell. The order of the list MUST BE any
    ///     order such that if cells are re-evaluated in that order, their dependencies
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the name parameter is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   If changing the contents of the named cell to be the formula would
    ///   cause a circular dependency, throw a CircularException.
    ///   (NOTE: No change is made to the spreadsheet.)
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        name = name.ToUpper();
        ValidateCell(name);
        IList<string> recalcingCells;

        if (content.Length > 0 && content[0] == '=')
        {
            // don't need to worry about formulaformatexceptions in assignment 6
            recalcingCells = SetCellContents(name, new Formula(content[1..]));
        }
        else if (double.TryParse(content, out double result))
        {
            Changed = true;
            recalcingCells = SetCellContents(name, result);
        }
        else
        {
            Changed = true;
            recalcingCells = SetCellContents(name, content);
        }

        Recalculate(recalcingCells);
        return recalcingCells;
    }

    /// <summary>
    /// Attempts to replace the current spreadsheet with a loaded one.
    /// </summary>
    /// <param name="eligible">The dictionary full of potential cells and their names.</param>
    /// <exception cref="SpreadsheetReadWriteException"> When the file cannot be opened or the json is bad.</exception>
    private void AttemptReplace(Dictionary<string, Cell> eligible)
    {
        foreach (var r in eligible)
        {
            if (r.Value.Contents is Formula f)
            {
                this.SetContentsOfCell(r.Key, "=" + f.ToString());

                // quick check if it produced a formula error
                if (cells[r.Key].Computed is FormulaError)
                {
                    throw new Exception();
                }
            }
            else if (r.Value.Contents is double d)
            {
                this.SetContentsOfCell(r.Key, d.ToString());
            }
            else if (r.Value.Contents is string s)
            {
                this.SetContentsOfCell(r.Key, s);
            }
        }

        // didn't throw anything, we've successfully loaded new sheet in.
        Changed = false;
    }

    /// <summary>
    /// Recalculates after setcellcontents call (ANY call to that method).
    /// Uses the IList returned from the instructor-provided
    /// recursive dependency search code.
    /// </summary>
    /// <param name="recalcingCells">The list of cells to recalculate,
    /// starting with this cell.</param>
    private void Recalculate(IList<string> recalcingCells)
    {
        foreach (string s in recalcingCells)
        {
            if (!cells.ContainsKey(s))
            {
                continue;
            }

            // check if smthng has been computed as formulaerr and is a formula
            if (cells[s].Contents is Formula f)
            {
                try
                {
                    object computed = f.Evaluate(q => (double)cells[q].Computed);
                    if (computed is double d)
                    {
                        cells[s].Computed = d;
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new content of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cell in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        masterGraph.ReplaceDependees(name, new List<string>());

        AttemptAdd(name, number);

        return GetCellsToRecalculate(name).ToList();
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    ///   If text contains a valid variable (cell reference) it will
    ///   still be only acknowledged as text.
    /// </summary>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new content of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        if (this.masterGraph.GetDependents(name).Count() != 0)
        {
           throw new ArgumentException("Cell has dependents when trying to remove.");
        }

        var cellsToRecalc = new List<string>();
        if (text.Equals(string.Empty))
        {
            cellsToRecalc = GetCellsToRecalculate(name).ToList();
            cells.Remove(name);
            masterGraph.ReplaceDependees(name, new List<string>());
        }
        else
        {
            cellsToRecalc = GetCellsToRecalculate(name).ToList();
            masterGraph.ReplaceDependees(name, new List<string>());
            AttemptAdd(name, text);
        }

        return cellsToRecalc;
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException.
    ///   </para>
    ///   <para>
    ///     No change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new content of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        // get vars from formula
        ISet<string> setDependees = new HashSet<string>(formula.GetVariables());

        IEnumerable<string> currDees = masterGraph.GetDependees(name);

        masterGraph.ReplaceDependees(name, setDependees); // add to masterGraph

        try
        {
            IList<string> recalcCells = GetCellsToRecalculate(name).ToList();
            AttemptAdd(name, formula);
            Changed = true;
            return recalcCells;
        }
        catch (CircularException)
        {
            masterGraph.ReplaceDependees(name, currDees);
            throw;
        }
    }

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        return masterGraph.GetDependents(name);
    }

    /// <summary>
    /// Adds or replaces a K/V pair in the cells dictionary. This is part of the
    /// invariant where if we add to masterGraph we must add to cells.
    /// </summary>
    /// <param name="name">The validated name of a cell.</param>
    /// <param name="element">the object to add as value, can be formula object, double, or string.</param>
    private void AttemptAdd(string name, object element)
    {
        if (cells.ContainsKey(name))
        {
            InDictReplacer(name, element);
        }
        else
        {
            NotInDictAdder(name, element);
        }
    }

    /// <summary>
    /// replaces anything currently in cells with an element with
    /// given (validated) cell name.
    /// If formula: adds as formula in Contents property,
    /// and either a computed formula (double) or formula error
    /// in Computed property.
    /// </summary>
    /// <param name="name">the validated name of the cell.</param>
    /// <param name="element">the object to set as the cells computed and contents properties.</param>
    private void InDictReplacer(string name, object element)
    {
        if (element is Formula f)
        {
            try
            {
                object computed = f.Evaluate(s => (double)cells[s].Contents);
                if (computed is FormulaError fE)
                {
                    // never want to store a formula err in contents
                    // only in computed.
                    cells[name].Contents = element;
                    cells[name].Computed = fE;
                    return;
                }

                cells[name].Contents = element;
                cells[name].Computed = computed;
            }

            // catches keynotfound exceptions and invalidcast exceptions,
            // i.e., formula errors.
            catch
            {
                // store the bad formula
                cells[name].Contents = f;

                // store it in computed form, i.e., a formula error object.
                cells[name].Computed = new FormulaError("Invalid formula");
            }
        }

        // str or double so safe to store in both as object itself
        else
        {
            cells[name].Contents = element;
            cells[name].Computed = element;
        }
    }

    /// <summary>
    /// Adds an element with given (validated) cell name.
    /// If formula: adds as formula in Contents property,
    /// and either a computed formula (double) or formula error
    /// in Computed property.
    /// </summary>
    /// <param name="name">the validated name of the cell.</param>
    /// <param name="element">the object to set as the cells computed and contents properties.</param>
    private void NotInDictAdder(string name, object element)
    {
        if (element is Formula f)
        {
            try
            {
                object computed = f.Evaluate(s => (double)cells[s].Contents);
                if (computed is FormulaError fE)
                {
                    // constructor is: (Contents, Computed)
                    cells.Add(name, new Cell(element, fE));
                    return;
                }

                cells.Add(name, new Cell(element, computed));
            }

            // catches keynotfound exceptions and invalidcast exceptions,
            // i.e., formula errors.
            catch
            {
                // store the bad formula, and the formula error using 2 param constructor
                cells.Add(name, new Cell(element, new FormulaError("Invalid Formula")));
            }
        }

        // str or double so safe to store in both as object itself
        else
        {
            cells.Add(name, new Cell(element, element));
        }
    }

    /// <summary>
    ///   Reports whether "name" is a valid cell name.
    /// </summary>
    /// <param name="name"> A cell name that may be valid. </param>
    /// <exception cref="InvalidNameException"> If cell name is invalid.</exception>
    private void ValidateCell(string name)
    {
        string standaloneVarPattern = @"^[a-zA-Z]+\d+$";
        if (!Regex.IsMatch(name, standaloneVarPattern))
        {
            throw new InvalidNameException();
        }
    }

    /// <summary>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    ///   A helper for the GetCellsToRecalculate method.
    ///   Visits every cell in a chain of dependency,
    ///   starting with original the named cell from the GetCellsToReCalculate parameter.
    /// </summary>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        // add current cell to set of visited cells
        visited.Add(name);

        // cycle through EACH DIRECT dependents current cell
        foreach (string dependent in GetDirectDependents(name))
        {
            // if any dependent is found that "points" back to the original node
            if (dependent.Equals(start))
            {
                throw new CircularException();
            }

            // else visit every single one recursively
            else if (!visited.Contains(dependent))
            {
                Visit(start, dependent, visited, changed);
            }
        }

        // after looping through each dependent cell of current cell,
        // add the current cell to the HEAD of the linked list
        // even though this is void return, we mutate the linked list
        // so we can return the dependency chain as req'd by API.
        changed.AddFirst(name);
    }
}

/// <summary>
/// Class representing an instance of a single cell, used within a spreadsheet context.
/// A cell has no name, as the name is specified in the Spreadsheet class. A Cell simply
/// stores an object, one of a Formula object, double, or string.
/// </summary>
internal class Cell
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Cell"/> class.
    /// </summary>
    /// <param name="elmnt">the object to be stored as contents.</param>
    /// <param name="computedElmnt">the object to be stored as the computed component.</param>
    public Cell(object elmnt, object computedElmnt)
    {
        this.Contents = elmnt;
        this.Computed = computedElmnt;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Cell"/> class.
    /// </summary>
    /// <param name="stringForm">The string stored in the JSON.</param>
    [JsonConstructor]
    public Cell(string stringForm)
    {
        Contents = string.Empty;
        Computed = string.Empty;
        DeserializeStringForm(stringForm);
    }

    /// <summary>
    /// Gets the stringform (used for serialization).
    /// </summary>
    public string StringForm
    {
        get
        {
            if (Contents is double d)
            {
                return d.ToString();
            }

            if (Contents is string s)
            {
                return s;
            }

            if (Contents is Formula f && Computed is not FormulaError)
            {
                return "=" + f.ToString();
            }

            throw new SpreadsheetReadWriteException("Spreadsheet contains at least one invalid formula");
        }
    }

    /// <summary>
    /// Gets or sets the contents prop.
    /// </summary>
    [JsonIgnore]
    public object Contents { get; set; }

    /// <summary>
    /// Gets or sets the computed val of the cell.
    /// </summary>
    [JsonIgnore]
    public object Computed { get; set; }

    /// <summary>
    /// Method to handle deserialization of StringForm.
    /// </summary>
    /// <param name="stringForm">The stringform coming from the JSON.</param>
    private void DeserializeStringForm(string stringForm)
    {
        // Check if the stringForm represents a valid number
        if (double.TryParse(stringForm, out double result))
        {
            Contents = result;
            Computed = result;
        }
        else if (stringForm.StartsWith("="))
        {
            string formulaStr = stringForm.Substring(1);
            Contents = new Formula(formulaStr);
        }
        else
        {
            // reg string case
            Contents = stringForm;
            Computed = stringForm;
        }
    }
}
