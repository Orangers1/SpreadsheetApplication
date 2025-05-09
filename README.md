# Spreadsheet Solution

Copyright:  CS 3500 and Isaac Huntsman and Josh Eggett (+ various
CS3500 instructors) - This work may not be copied for use in Academic Coursework.
Date: 10/28/24

## references

consulted with David Perry on GENERAL structure, no coding took place or code sharing, pizza rule followed, etc.
I asked chatGPT "explain how I might test an instance of a delegate in mstest and c#".
I asked chatGPT how it would construct a chained stress test but did not copy any code.


### Formula project and test project.

Validates formulas, Evaluates variables

### Dependency Graph and test project.

Checks what variables depend on what other variables.

### Solution PROJECT

Represents state of a simple spreadsheet application. Can get names of all nonempty cells,
query a cell, set contents of cell, and other functionality borrowed from the two other projects.

### solution PROJECT continued

Spreadsheet can save and load, public API is updated to be simplified. Now contains an indexer overload.
Now lookups and recalculates cell values upon switching of cell contents. Differentiates between contents
and computed value.

### Spreadsheet PROJECT GUI

A GUI for the spreadsheet. Contains a modifiable size for a grid of X columns, Y rows.
It includes the following functionality:
-Show the current selected cell by name (and hopefully highlighted)
-Show the current value in that cell (not editable)
-Allow you to modify the content of that cell,
-Update all related cells upon update,
-Allow the user to save the data as a JSON file,
-Allow the user to read the data from a file and create a new spreadsheet.
-Resize the spreadsheet up to 10 million rows and/or columns
-Clear a spreadsheet

### Branching

Branches: we worked on PS7 synchronously, and did not make any branches for separate functionality.
We didn't make any extra branches because we did paired programming (synchronously) the whole assignment.
The merge process (merge to main) at the end was simple because there was no merge conflict. 
merge commit: (22d60bb)

### Partnership 

All code was completed via pair programming.

An example where our partnership was most effective was in switching between driver and navigator roles. We tried to switch every 30 minutes to 1 hr
and because of blazor issues swapping helped us to google solutions more efficiently.

As a partnership we planned to start with a plan before writing any code - we read through the assignment and finished all pre-coding requirements
and made a plan before starting to code to ensure we were on the same page.

Assigning one of us the task of understanding HTML, CSS and the other to figure out how Blazor works to display a GUI integrated with HTML and CSS worked well.

Our partnership could improve by improving our scheduling and time management, as well as starting earlier in the week. 

# Time Estimation

Time estimates were pretty good, always overestimating until the last assignment which I underestimated how hard it was. This assignment
estimation was pretty accurate but we missed the exact estimate due to not having a good enough understanding of blazor.
Our skills to learn brand new tools is nascent but getting better with time.

# timing estimates and actual worked in hours

Hours

| Estimated/Worked  |     Assignment      |  Note  
| --------------------------------------
|      7/10.5       | Formula.cs & tester | Took some time to get used to Visual Studio which lengthened estimate  
|      20/11.5      | DG & tester         | coding went smoothly. Testing took longer than I expected  
|      22/17.25     | Formula evaluator   | Override methods were easy. Wrote Eval pedantically then simplified  
|      17/10.5      | Spreadsheet proj    | Did well with this one given TDD and deep initial understanding of API  
|      12/25.5      | Spreadsheet pt 2    | Tough assignment. Not very good at est time. My time management is still good in my opinion. I never got really stuck, just was rethinking solutions to the problem.
|      12/9         | Spreadsheet GUI     | Efficiently implemented the view portion.
