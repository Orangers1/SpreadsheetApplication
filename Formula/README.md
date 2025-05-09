# Formula.cs

Copyright:  CS 3500 and Isaac Huntsman (+ various
CS3500 instructors) - This work may not be copied for use in Academic Coursework.
Author: Isaac Huntsman
Date: 9/24/24

## Contents

This Formula project contains a constructor, getVariables method, and toString method as part of
the spreadsheet solution. Its purpose is to take a string and decompose it into its variables,
and output a normalized string representation. It also contains a lookup delegate and evaluate
function for evaluating formulas and looking up variables.

## references

Asked chatGPT to explain more about lambdas, delegates, and how I should test a lookup delegate in mstest.
However, all tests and regular code were written by Isaac Huntsman.

## consulted peers

Spoke with David Perry about how the assignment was going for him, and also spoke with Timothy Felt
about how he overrode the hashcode method and equals, ==, != methods. He said he spoke with a TA,
who showed him how Visual Studio can override hashcode and .Equals "in a really easy way". He did not 
actually show me how to do it.

## time estimates and actuals

### part 1: Initial creation

My original estimate for how long pt. 1 of this project will take is no more than 5 hours.

9/3: 4 hours
9/4: 3 hours
9/5: 1.5 hours

expected: 5 hours
total: ~8.5 hours.

### Part 2: Evaluation, lookup

My est is 16 hours

9/16: 30 minutes, project setup with templates
9/18: .5 hrs implemented overridden operators and .Equals, adjusted formatting
9/18: 6 hrs implementing Evaluate and debugging. At this point all my tests are passing.
9/19: 3 hrs adjusting formatting and simplifying code.

expected: 16 hrs
total: 10 hrs