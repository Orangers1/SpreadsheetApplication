# Spreadsheet.cs

Copyright:  CS 3500 and Isaac Huntsman (+ various
CS3500 instructors) - This work may not be copied for use in Academic Coursework.
Author: Isaac Huntsman
Date: 9/22/24 (started)
10/20/24 latest version

## Contents

Spreadsheet project represents current state of spreadsheet application,
working hand in hand with the dependency graph and the formula class.
Contains various methods for viewing and changing state of spreadsheet application.
Contains a second class for initializing a cell.
Contains save and load features, cell computation functionality, a catch-all cell contents setter,
read-write exception checking, an indexer, and an "any new changes" tracker.

## references

I talked with classmates about importing IsVar from formula class vs just using the formula constructor.
TA advised on this one and explained why that might be unnecessary.
See solution-level readme for other references.

## consulted peers

NA

## time estimates and actuals

I expect the prep & coding to take 6 hours,
debugging to take 4 hours,
making 10 hours total of programming.

9/24/24: .5 hours, update readme, thinking more about cell class.
9/24/24: (evening): 3 hours, major progress
9/25/24: 3 hours, passing all tests.
6.5 total, 10 expected.

** PART 2 **
I expect the prep and coding to take 5 hours,
debugging to take 3 hours,
makes 8 hrs

10/12/24: 1 hr stubbing out the methods
10/17/24: 2 hrs, partner work
10/18/24 2 hrs, working through spreadsheet class
10/18/24 1.5 hrs, trying to understand vague assignment instructions
10/19/24 5 hrs, trying to wrestle with state and recalc'ing cells that were
previously formulaerrors but were rectified.
10/20/24 4 hrs, working through recalc solution
10/20/24 5 hrs, debugging