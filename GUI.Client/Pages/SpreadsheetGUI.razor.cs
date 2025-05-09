// <copyright file="SpreadsheetGUI.razor.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// Author:    Isaac Huntsman
// Partner:   Josh Eggett
// Date:      10/26/24
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
//    Contains the logic for the GUI, the view component of Spreadsheet model.
// </summary>

namespace SpreadsheetNS;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using Microsoft.VisualBasic;
using System.Runtime.Intrinsics.X86;
using CS3500.Spreadsheet;
using CS3500.Formula;
using System.Collections;

/// <summary>
///  <remarks>
///    <para>
///      This is a partial class because class SpreadsheetGUI is also automatically
///      generated from the SpreadsheetGUI.razor file.  Any code in that file, and variable in
///      that file can be referenced here, and vice versa.
///    </para>
///    <para>
///      It is usually better to put the code in a separate CS isolation file so that Visual Studio
///      can use intellisense better.
///    </para>
///    <para>
///      Note: only GUI related information should go in the sheet. All (Model) spreadsheet
///      operations should happen through the Spreadsheet class API.
///    </para>
///    <para>
///      The "backing stores" are strings that are used to affect the content of the GUI
///      display.  When you update the Spreadsheet, you will then have to copy that information
///      into the backing store variable(s).
///    </para>
///  </remarks>
/// </summary>
public partial class SpreadsheetGUI
{
    /// <summary>
    /// This is how the view connects to the model. A spreadsheet object instance.
    /// </summary>
    private Spreadsheet spreadsheet = new();

    /// <summary>
    ///    Gets the alphabet for ease of creating columns.
    /// </summary>
    private static char[] Alphabet { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    ///   Gets or sets the javascript object for this web page that allows
    ///   you to interact with any javascript in the associated file.
    /// </summary>
    private IJSObjectReference? JSModule { get; set; }

    /// <summary>
    ///   Gets or sets the file name.
    /// </summary>
    private string FileSaveName { get; set; } = "Spreadsheet.sprd";

    /// <summary>
    ///   <para> Gets or sets the data for the Tool Bar Cell Contents text area, e.g., =57+A2. </para>
    ///   <para> TODO: Fix toolbar to: a non-editable widget showing the name of the selected cell.
    ///    a non-editable widget showing the value of the selected cell.
    ///    an editable widget showing the contents of the selected cell.
    ///   </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string ToolBarCellContents { get; set; } = string.Empty;

    /// <summary>
    ///   <para> Gets or sets the data for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML</remarks>
    /// </summary>
    private string[,] CellsBackingStore { get; set; } = new string[10, 10];

    /// <summary>
    ///   <para> Gets or sets the html class string for all of the cells in the spreadsheet GUI. </para>
    ///   <remarks>Backing Store for HTML CLASS strings</remarks>
    /// </summary>
    private string[,] CellsClassBackingStore { get; set; } = new string[10, 10];

    /// <summary>
    ///   Gets or sets a value indicating whether we are showing the save "popup" or not.
    /// </summary>
    private bool SaveGUIView { get; set; }

    /// <summary>
    ///   Query the spreadsheet to see if it has been changed.
    ///   <remarks>
    ///     Any method called from JavaScript must be public
    ///     and JSInvokable!
    ///   </remarks>
    /// </summary>
    /// <returns>
    ///   true if the spreadsheet is changed.
    /// </returns>
    [JSInvokable]
    public bool HasSpreadSheetChanged()
    {
        Debug.WriteLine($"{"HasSpreadSheetChanged",-30}: {Navigator.Uri}. Remove Me.");
        return false;
    }

    /// <summary>
    ///   Example of how JavaScript can talk "back" to the C# side.
    /// </summary>
    /// <param name="message"> string from javascript side. </param>
    [JSInvokable]
    public void TestBlazorInterop(string message)
    {
        Debug.WriteLine($"JavaScript has send me a message: {message}");
    }

    /// <summary>
    ///   Set up initial state and event handlers.
    ///   <remarks>
    ///     This is somewhat like a constructor for a Blazor Web Page (object).
    ///     You probably don't need to do anything here.
    ///   </remarks>
    /// </summary>
    protected override void OnInitialized()
    {
        CellsClassBackingStore[0, 0] = "selected"; // select A1 by default

        Debug.WriteLine($"{"OnInitialized",-30}: {Navigator.Uri}. Remove Me.");
    }

    /// <summary>
    ///   Called anytime in the lifetime of the web page were the page is re-rendered.
    ///   <remarks>
    ///     You probably don't need to do anything in here beyond what is provided.
    ///   </remarks>
    /// </summary>
    /// <param name="firstRender"> true the very first time the page is rendered.</param>
    protected async override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        Debug.WriteLine($"{"OnAfterRenderStart",-30}: {Navigator.Uri} - first time({firstRender}). Remove Me.");

        if (firstRender)
        {
            /////////////////////////////////////////////////
            //
            // The following three lines setup and test the
            // ability for Blazor to talk to javascript and vice versa.
            JSModule = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/SpreadsheetGUI.razor.js"); // create/read the javascript
            await JSModule.InvokeVoidAsync("SetDotNetInterfaceObject", DotNetObjectReference.Create(this)); // tell the javascript about us (dot net)
            await JSModule.InvokeVoidAsync("TestJavaScriptInterop", "Hello JavaScript!"); // test that it is working.  You could remove this.
            await FormulaContentEditableInput.FocusAsync(); // when we start up, put the focus on the input. you will want to do this anytime a cell is clicked.
        }

        Debug.WriteLine($"{"OnAfterRender Done",-30}: {Navigator.Uri} - Remove Me.");
    }

    private void Resize(int r, int c)
    {
        Math.Clamp(c, 0, 10000000);
        Math.Clamp(r, 0, 10000000);

        // before this, resize spreadsheet.
        CellsBackingStore = new string[r, c];
        CellsClassBackingStore = new string[r, c];

        var (tempR, tempC) = ConvertFromCellName(selectedCell);
        HighlightCell(tempR, tempC);

        foreach (string s in spreadsheet.GetNamesOfAllNonemptyCells())
        {
            var (row, col) = ConvertFromCellName(s);

            // update backing store
            object value = spreadsheet.GetCellValue(s);
            Console.WriteLine("test");
            CellsBackingStore[row, col] = GetCellValueAsString(value);
        }

        StateHasChanged();
    }

    /// <summary>
    /// Can handle more columns than just the 26 in the alphabet (i.e. aa = 27 type thing works).
    /// Converts a Row Column to the associated Cell name.
    /// </summary>
    /// <param name="row">the row to build cell name from.</param>
    /// <param name="col">the column to build cell name from.</param>
    /// <returns>a string, the cell representation of a row and column number.</returns>
    private string CellNameFromRowCol(int row, int col)
    {
        return GenerateColNames(col) + (row + 1);
    }

    /// <summary>
    /// generates "infinite" col names, such as "AA26", given an integer.
    /// </summary>
    /// <param name="col">the col number to convert to string.</param>
    /// <returns>the string representation of the column number.</returns>
    private string GenerateColNames(int col)
    {
        string colStr = string.Empty;
        col++;

        while (col > 0)
        {
            col--;
            colStr = (char)('A' + (col % 26)) + colStr;
            col /= 26;
        }

        // Append the row number after the column string
        return colStr;
    }

    /// <summary>
    /// Converts A Cell name to the associated Row, Column.
    /// Essentially the same as in the other file ConvertCellNameToRowCol but doesn't use references.
    /// </summary>
    /// <param name="cellName">The name of cell to convert from.</param>
    /// <returns>a tuple, the row and column extracted from cell name, both as ints.</returns>
    private (int row, int col) ConvertFromCellName(string cellName)
    {
        int col = 0;
        int row = 0;

        if (cellName.Equals(string.Empty))
        {
            return (row, col);
        }

        // Parse the column letters
        int i = 0;
        while (i < cellName.Length && char.IsLetter(cellName[i]))
        {
            col = (col * 26) + (cellName[i] - 'A' + 1);
            i++;
        }

        // Convert to zero-based column
        col--;

        // Parse the row number
        row = int.Parse(cellName.Substring(i)) - 1;

        return (row, col);
    }

    /// <summary>
    ///   Called when the input widget (representing the data in a particular cell) is modified.
    /// </summary>
    /// <param name="newInput"> The new value to put at row/col. </param>
    /// <param name="row"> The matrix row identifier. </param>
    /// <param name="col"> The matrix column identifier. </param>
    private async void HandleUpdateCellInSpreadsheet(string newInput, int row, int col)
    {
        try
        {
            InputWidgetBackingStore = $"{row},{col}";

            string cellName = CellNameFromRowCol(row, col);

            List<string> list = new List<string>(this.spreadsheet.SetContentsOfCell(cellName, newInput));
            object value = spreadsheet.GetCellValue(cellName);
            string showValue = GetCellValueAsString(value);

            CellsBackingStore[row, col] = showValue;

            foreach (string s in list)
            {
                var (r, c) = ConvertFromCellName(s);
                string val = GetCellValueAsString(spreadsheet.GetCellValue(s));

                CellsBackingStore[r, c] = val;
            }

            UpdateToolbar();
        }
        catch (CircularException)
        {
            await JS.InvokeVoidAsync("alert", "A circular exception occurred.");
        }
        catch (InvalidNameException)
        {
            await JS.InvokeVoidAsync("alert", "An invalid name was provided for a formula.");
        }
        catch (FormulaFormatException)
        {
            await JS.InvokeVoidAsync("alert", "An invalid formula was provided.");
        }
        catch
        {
            // a way to communicate to the user that something went wrong.
            await JS.InvokeVoidAsync("alert", "Something went wrong.");
        }
    }

    /// <summary>
    /// Helper method that gets the string associated with a cellValue object.
    /// </summary>
    /// <param name="value">The cell value object.</param>
    /// <returns>The string for that object.</returns>
    private string GetCellValueAsString(object value)
    {
        string showValue = string.Empty;
        if (value is FormulaError)
        {
            showValue = ((FormulaError)value).Reason;
        }
        else if (value is double d)
        {
            showValue = d.ToString();
        }
        else if (value is string s)
        {
            showValue = s;
        }

        return showValue;
    }

    /// <summary>
    ///   <para>
    ///     Using a Web Input ask the user for a file and then process the
    ///     data in the file.
    ///   </para>
    ///   <remarks>
    ///     Unfortunately, this happens after the file is chosen, but we will live with that.
    ///   </remarks>
    /// </summary>
    /// <param name="args"> Information about the file that has been selected. </param>
    private async void HandleLoadFile(EventArgs args)
    {
        try
        {
            bool success = false;
            if (spreadsheet.Changed)
            {
                success = await JS.InvokeAsync<bool>("confirm", "Unsaved changes will be deleted on load, Continue?");
            }

            if (!success && spreadsheet.Changed)
            {
                // user canceled the action.
                return;
            }

            string fileContent = string.Empty;

            InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("that didn't work");
            if (eventArgs.FileCount == 1)
            {
                var file = eventArgs.File;
                if (file is null)
                {
                    return;
                }

                using var stream = file.OpenReadStream();
                using var reader = new System.IO.StreamReader(stream);
                fileContent = await reader.ReadToEndAsync();

                await JS.InvokeVoidAsync("alert", fileContent);

                ClearGUI();
                spreadsheet.InstantiateFromJSON(fileContent);

                foreach (string s in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    var (r, c) = ConvertFromCellName(s);

                    // update backing store
                    object value = spreadsheet.GetCellValue(s);
                    Console.WriteLine("test");
                    CellsBackingStore[r, c] = GetCellValueAsString(value);
                }

                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("something went wrong with loading the file..." + e);
        }
    }

    /// <summary>
    /// Helper method that sets all modified cells to string.Empty.
    /// </summary>
    private void ClearGUI()
    {
        foreach (string s in spreadsheet.GetNamesOfAllNonemptyCells())
        {
            var (r, c) = ConvertFromCellName(s);

            // update backing store
            CellsBackingStore[r, c] = string.Empty;
        }

        HighlightCell(0, 0);
    }

    /// <summary>
    ///   Switch between the file save view or main view.
    /// </summary>
    /// <param name="show"> if true, show the file save view. </param>
    private void ShowHideSaveGUI(bool show)
    {
        SaveGUIView = show;
        StateHasChanged();
    }

    /// <summary>
    ///   Call the JavaScript necessary to download the data via the Browser's Download
    ///   Folder.
    /// </summary>
    /// <param name="e"> Ignored. </param>
    private async void HandleSaveFile(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        // <remarks> this null check is done because Visual Studio doesn't understand
        // the Blazor life cycle and cannot assure of non-null. </remarks>
        if (JSModule is not null)
        {
            var success = await JSModule.InvokeAsync<bool>("saveToFile", FileSaveName, spreadsheet.GetJSON());
            if (success)
            {
                ShowHideSaveGUI(false);
                StateHasChanged();
            }
        }
    }

    /// <summary>
    ///   Clear the spreadsheet if not modified.
    /// </summary>
    /// <param name="e"> Ignored. </param>
    private async void HandleClear(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        bool success = false;
        if (JSModule is not null)
        {
            success = await JS.InvokeAsync<bool>("confirm", "Clear the sheet?");
        }

        if (success && spreadsheet.Changed)
        {
            _ = await JS.InvokeAsync<bool>("confirm", "Cannot clear with unsaved changes.");
        }

        if (success && !spreadsheet.Changed)
        {
            // fetch all nonempty cells
            ClearGUI();
            spreadsheet = new Spreadsheet();
            UpdateToolbar();
        }
    }
}
