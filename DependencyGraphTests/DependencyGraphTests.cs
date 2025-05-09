// <copyright file="DependencyGraphTests.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <summary>
// Author:    Isaac Huntsman
// Partner:   NA TODO: insert name
// Date:      9/13/24 (V1)
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
// Dependency graph test class for testing the functionality of the dependency graph.
// </summary>

namespace CS3500.DependencyGraph;
using CS3500.DependencyGraph;

/// <summary>
/// Tests to ensure the functionality of the DependencyGraph project.
/// Includes everything but stress tests.
/// Includes 11 opaque tests, and a set of transparent tests.
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    /// Tests the constructor alone, with no dependencies added.
    /// </summary>
    [TestMethod]
    public void Constructor_TestBlank_AssertTrue()
    {
        DependencyGraph emptyDG = new();

        Assert.IsTrue(emptyDG.Size == 0);
    }

    /// <summary>
    /// Tests a very simple dependency graph, with just one dependency.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void GetDependents_TestAddGetDependent_AssertTrue()
    {
        DependencyGraph smallDG = new ();

        smallDG.AddDependency("A1", "B9");

        Assert.IsTrue(smallDG.HasDependents("A1"));

        IEnumerable<string> list = smallDG.GetDependents("A1");
        IEnumerable<string> testOutput = new List<string> { "B9" };
        Assert.IsTrue(list.SequenceEqual(testOutput));
    }

    /// <summary>
    /// Tests a very simple dependency graph, with just one dependency
    /// and the remove functionality. Tests that graph is empty after effectively
    /// cancelling modification.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void RemoveDependency_TestAddRemoveGetDependent_AssertFalse()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A12", "B4");
        smallDG.RemoveDependency("A12", "B4");

        Assert.IsFalse(smallDG.HasDependents("A12"));
        int temp = smallDG.Size;
        Assert.IsTrue(smallDG.Size == 0);
        HashSet<string> test = new();
        IEnumerable<string> dependents = smallDG.GetDependents("A12");
        IEnumerable<string> dependees = smallDG.GetDependees("B4");
        Assert.IsTrue(test.SetEquals(smallDG.GetDependents("A12")));
        Assert.IsTrue(test.SetEquals(smallDG.GetDependees("B4")));
    }

    /// <summary>
    /// Tests a very simple dependency graph, with just one dependency
    /// and the remove functionality. Tests that graph is empty after effectively
    /// cancelling modification.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void HasDependees_TestAddRemoveGetDependees_AssertFalse()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A11", "B1");
        smallDG.RemoveDependency("A11", "B1");

        Assert.IsFalse(smallDG.HasDependees("B1"));
        Assert.IsTrue(smallDG.Size == 0);
    }

    /// <summary>
    /// Tests a very simple dependency graph, with just one dependency
    /// and the has and get dependees functionality.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void GetDependees_TestAddGetDependee_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A2", "B4");
        Assert.IsTrue(smallDG.HasDependees("B4"));

        IEnumerable<string> list = smallDG.GetDependees("B4");
        IEnumerable<string> testOutput = new List<string> { "A2" };
        Assert.IsTrue(list.SequenceEqual(testOutput));
    }

    /// <summary>
    /// Tests replacing a DG with a few dependencies with an empty DG.
    /// Uses ReplaceDependents like a "clear dependents" method.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void ReplaceDependents_TestClear_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A2", "B1");
        smallDG.AddDependency("A2", "B15");
        smallDG.AddDependency("A2", "F144");
        smallDG.AddDependency("A2", "Q101");

        smallDG.ReplaceDependents("A2", new List<string>());

        // since we're replacing, A2 can have a dependents list that is empty.
        Assert.IsTrue(smallDG.GetDependents("A2").SequenceEqual(new List<string>()));
    }

    /// <summary>
    /// Tests replacing a DG with a few dependencies with an empty DG.
    /// Uses ReplaceDependents like a "clear dependents" method.
    /// Tests the correctness of Size getter.
    /// This is an opaque test, written before any functionality was implemented.
    /// We expect this to pass before any functionality is implemented.
    /// </summary>
    [TestMethod]
    public void Size_TestClearDependents_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A2", "B1");
        smallDG.AddDependency("A2", "B15");
        smallDG.AddDependency("A2", "F144");
        smallDG.AddDependency("A2", "Q101");

        smallDG.ReplaceDependents("A2", new List<string>());

        // since we're replacing, A2 can have a dependents list that is empty.
        Assert.IsTrue(smallDG.Size == 0);
    }

    /// <summary>
    /// Tests replacing a DG with a few dependencies with an empty DG.
    /// Uses ReplaceDependees like a "clear dependees" method.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void ReplaceDependees_TestClear_AssertTrue()
    {
        DependencyGraph smallDG = new();

        // uses "mirror" dictionary
        smallDG.AddDependency("A16", "B1");
        smallDG.AddDependency("F5", "B1");
        smallDG.AddDependency("U77", "B1");
        smallDG.AddDependency("U261", "B1");

        smallDG.ReplaceDependees("B1", new List<string>());

        // since we're replacing, B1 can have a dependees list that is empty.
        Assert.IsTrue(smallDG.GetDependees("B1").SequenceEqual(new List<string>()));
    }

    /// <summary>
    /// Tests replacing a DG with a few dependencies with an empty DG.
    /// Uses ReplaceDependees like a "clear dependees" method.
    /// Tests the correctness of Size getter.
    /// This is an opaque test, written before any functionality was implemented.
    /// We expect this to pass before any functionality is implemented.
    /// </summary>
    [TestMethod]
    public void Size_TestClearDependees_AssertTrue()
    {
        DependencyGraph smallDG = new();

        // uses "mirror" dictionary
        smallDG.AddDependency("A16", "B1");
        smallDG.AddDependency("F5", "B1");
        smallDG.AddDependency("U77", "B1");
        smallDG.AddDependency("U261", "B1");

        smallDG.ReplaceDependees("B1", new List<string>());

        // since we're replacing, B1 can have a dependees list that is empty.
        Assert.IsTrue(smallDG.Size == 0);
    }

    /// <summary>
    /// Tests replacing a DG with no dependencies with a cell that has a short list of dependents
    /// Uses ReplaceDependents like an "add all dependents" method.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void ReplaceDependents_TestAddAll_AssertTrue()
    {
        DependencyGraph smallDG = new();

        List<string> newDependents = new List<string>();

        newDependents.Add("V3");
        newDependents.Add("G3");
        newDependents.Add("F3");

        smallDG.ReplaceDependents("A2", newDependents);

        Assert.IsTrue(smallDG.GetDependents("A2").SequenceEqual(newDependents));
    }

    /// <summary>
    /// Tests replacing a DG with no dependencies with a cell that has a short list of dependees
    /// Uses ReplaceDependees like an "add all dependees" method.
    /// This is an opaque test, written before any functionality was implemented.
    /// </summary>
    [TestMethod]
    public void ReplaceDependees_TestAddAll_AssertTrue()
    {
        DependencyGraph smallDG = new();

        List<string> newDependees = ["V3", "G3", "F5"];

        smallDG.ReplaceDependees("A2", newDependees);

        Assert.IsTrue(smallDG.GetDependees("A2").SequenceEqual(newDependees));
    }

    // === transparent tests ===

    /// <summary>
    /// Test attempts to add a key with the same value multiple times.
    /// We expect that this variables list of dependents contains only one variable.
    /// </summary>
    [TestMethod]
    public void ReplaceDependents_TestAddDuplicate_AssertTrue()
    {
        DependencyGraph smallDG = new();

        List<string> newDependees = ["V3", "V3", "V3"];

        smallDG.ReplaceDependees("A2", newDependees);

        Assert.IsTrue(smallDG.GetDependees("A2").SequenceEqual(newDependees.ToHashSet()));
    }

    /// <summary>
    /// Test attempts to add a key with the same value multiple times in a different way.
    /// We expect that this variables list of dependents contains only one variable.
    /// </summary>
    [TestMethod]
    public void AddDependency_TestAddDuplicate_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A2", "G12");
        smallDG.AddDependency("A2", "G12");
        smallDG.AddDependency("A2", "G12");

        Assert.IsTrue(smallDG.GetDependents("A2").SequenceEqual(new HashSet<string> { "G12" }));
    }

    /// <summary>
    /// Test adds a variable that is a dependent and dependee of another variables,
    ///  and vice versa.
    /// </summary>
    [TestMethod]
    public void AddDependency_TestCircular_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A2", "G12");
        smallDG.AddDependency("G12", "A2");

        Assert.IsTrue(smallDG.GetDependents("A2").SequenceEqual(new HashSet<string> { "G12" }));
        Assert.IsTrue(smallDG.GetDependents("G12").SequenceEqual(new HashSet<string> { "A2" }));
        Assert.IsTrue(smallDG.Size == 2);
    }

    /// <summary>
    /// Tests adding a circular reference, removing one, and focusing on
    /// the correctness of the size variable.
    /// </summary>
    [TestMethod]
    public void AddDependency_TestCircularRemove_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A11", "B1");
        smallDG.AddDependency("B1", "A11");

        smallDG.RemoveDependency("B1", "A11");
        Assert.IsTrue(smallDG.Size == 1);
        Assert.IsFalse(smallDG.HasDependents("B1"));
        Assert.IsTrue(smallDG.HasDependees("B1"));
    }

    /// <summary>
    /// Tests adding a circular reference, removing one, and focusing on
    /// the correctness of the size variable.
    /// </summary>
    [TestMethod]
    public void RemoveDependency_TestAddRemoveComplex_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("A11", "B1");
        smallDG.AddDependency("B1", "A11");
        smallDG.AddDependency("A11", "Q1");
        smallDG.RemoveDependency("B1", "A11");
        smallDG.RemoveDependency("A11", "Q1");
        smallDG.RemoveDependency("B1", "A11");
        smallDG.AddDependency("B1", "A11");
        smallDG.AddDependency("B1", "A11");
        smallDG.AddDependency("B1", "A11");
        smallDG.AddDependency("B1", "A11");
        smallDG.AddDependency("B1", "A11");
        smallDG.AddDependency("B1", "A11");

        Assert.IsTrue(smallDG.Size == 2);
        Assert.IsTrue(smallDG.HasDependents("B1"));
        Assert.IsTrue(smallDG.HasDependees("A11"));
    }

    /// <summary>
    /// Tests adding a circular reference, removing one, and focusing on
    /// the correctness of the size variable.
    /// </summary>
    [TestMethod]
    public void RemoveDependency_TestRemoveReAdd_AssertTrue()
    {
        DependencyGraph smallDG = new();

        smallDG.AddDependency("B1", "A11");
        smallDG.RemoveDependency("B1", "A11");

        Assert.IsTrue(smallDG.Size == 0);
        Assert.IsFalse(smallDG.HasDependents("B1"));

        smallDG.AddDependency("B1", "A11");

        Assert.IsTrue(smallDG.Size == 1);
        Assert.IsTrue(smallDG.HasDependents("B1"));
        Assert.IsTrue(smallDG.HasDependees("A11"));
    }
}

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Stress Tests.
/// </summary>
/// <author>CS3500 instructors and Isaac Huntsman.</author>
[TestClass]
public class DependencyGraphExampleStressTests
{
    /// <summary>
    /// generates a set of 200 strings with char concatenation and a loop,
    /// initializes 2 arrays of length 200 filled with hashsets,
    /// adds, removes, adds, then removes a bunch of dependencies (not completely clearing or re-adding),
    /// then checks if the proper elements are in the modified sets.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest()
    {
        DependencyGraph dg = new();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // Initialize 2 arrays length 200 of hashsets
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = new HashSet<string>();
            dependees[i] = new HashSet<string>();
        }

        // Add a bunch of dependencies
        // line up the dg and the array of dependents/dependees
        // for the final assertion. (this happens on each of the
        // nested loop sets below.
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    /// This stress test does something similar to the first, but on a larger dg, and also
    /// with an extra set of add/removes, meant to check the set functionality and duplicate
    /// handling of the dg class.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    public void StressTest2()
    {
        DependencyGraph dg = new();

        // A bunch of strings to use
        const int SIZE = 300;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // Initialize 2 arrays length 300 of hashsets
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = new HashSet<string>();
            dependees[i] = new HashSet<string>();
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // add 300 of the same dependencies,
        // effectively adding one new dependency to dg
        // and to dependents and dependees
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency("a1", "F555");

                // adding to a set, so this will get added just once
                dependents[0].Add("a1");
                dependees[0].Add("F555");
            }
        }

        // attempt to remove 300 of the same dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency("a1", "F555");
            }
        }

        dependents[0].Remove("a1");
        dependees[0].Remove("F555");

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }

        // there should be 300 plus 1 addition minus 1 removal
        Assert.IsTrue(dependents.Count() == 300);
        Assert.IsTrue(dependees.Count() == 300);
    }
}

/// <summary>
/// Authors:   Joe Zachary
///            Daniel Kopta
///            Jim de St. Germain
///            Travis Martin
/// Date:      Updated Spring 2024
/// Course:    CS 3500, University of Utah, Kahlert School of Computing
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
///   These tests are for your private use only to improve the quality of the
///   rest of your assignments.
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests.
/// </summary>
[TestClass]
public class DependencyGraphGradingTest
{
    // ************************** TESTS ON EMPTY DGs ************************* //

    /// <summary>
    ///   Empty graph should contain nothing.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("1")]
    public void Size_EmptyGraph_ReturnsZero()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(0, dg.Size);
    }

    /// <summary>
    ///   Empty graph should not have a node with dependees.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("2")]
    public void HasDependees_EmptyGraph_NoDependees()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependees("x"));
    }

    /// <summary>
    ///   Empty graph should not have a node with dependees.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("3")]
    public void HasDependents_EmptyGraph_NoDependents()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependents("x"));
    }

    /// <summary>
    ///   Removing from an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("4")]
    public void RemoveDependency_EmptyGraph_NoEffect()
    {
        DependencyGraph dg = new();
        Assert.IsFalse(dg.HasDependents("x"));
        dg.RemoveDependency("x", "y");
        Assert.IsFalse(dg.HasDependents("x"));
    }

    /// <summary>
    ///   Replace on an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("5")]
    public void ReplaceDependents_EmptyGraph_NoEffect()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependents("x", []);
    }

    /// <summary>
    ///   Replace on an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("6")]
    public void ReplaceDependees_EmptyGraph_NoEffect()
    {
        DependencyGraph dg = new();
        dg.ReplaceDependees("y", []);
    }

    // ************************ MORE TESTS ON EMPTY DGs *********************** //

    /// <summary>
    ///   Add one element, check the size, remove the element, check the size.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("7")]
    public void AddRemove_FromEmpty_OneThenZero()
    {
        DependencyGraph dg = new();
        dg.AddDependency("x", "y");
        Assert.AreEqual(1, dg.Size);
        dg.RemoveDependency("x", "y");
        Assert.AreEqual(0, dg.Size);
    }

    /// <summary>
    ///   Add multiple (two) elements, check the size, remove them, check the size.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("8")]
    public void AddRemove_FromEmpty_TwoThenZero()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "y");
        Assert.IsTrue(t.HasDependees("y"));
        Assert.IsTrue(t.HasDependents("x"));
        t.RemoveDependency("x", "y");
        Assert.IsFalse(t.HasDependees("y"));
        Assert.IsFalse(t.HasDependents("x"));
    }

    /// <summary>
    ///    <para>
    ///      Check if after adding the x->y we have the correct dependees and
    ///      the correct dependents.
    ///    </para>
    ///    <para>
    ///      After removing, there should be no contents left.
    ///    </para>
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("9")]
    public void AddAndRemoveDependency_FromEmpty_Valid()
    {
        DependencyGraph dg = new();
        dg.AddDependency("x", "y");

        Assert.IsTrue(dg.GetDependees("y").Matches(["x"]));
        Assert.IsTrue(dg.GetDependents("x").Matches(["y"]));

        dg.RemoveDependency("x", "y");

        Assert.IsTrue(dg.GetDependees("y").Matches([]));
        Assert.IsTrue(dg.GetDependents("x").Matches([]));
    }

    /// <summary>
    ///   Add a dependency and the number of dependees should be 1.
    ///   Remove it, and the number should be zero.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("10")]
    public void GetDependees_AddThenRemove_SizeOneThenZero()
    {
        DependencyGraph dg = new();
        dg.AddDependency("x", "y");
        Assert.AreEqual(1, dg.GetDependees("y").Count());
        dg.RemoveDependency("x", "y");
        Assert.AreEqual(0, dg.GetDependees("x").Count());
    }

    /// <summary>
    ///   Removing from an empty DG shouldn't fail.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("11")]
    public void RemoveDependency_DoItTwice_ShouldNotError()
    {
        DependencyGraph dg = new();
        dg.AddDependency("x", "y");
        Assert.AreEqual(dg.Size, 1);
        dg.RemoveDependency("x", "y");
        Assert.AreEqual(dg.Size, 0);
        dg.RemoveDependency("x", "y");
        Assert.AreEqual(dg.Size, 0);
    }

    /// <summary>
    ///   Replace dependencies with an empty list should not fail.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("12")]
    public void RemoveAndReplace_CanReplaceWithEmpty()
    {
        DependencyGraph dg = new();
        dg.AddDependency("x", "y");
        Assert.AreEqual(dg.Size, 1);

        dg.RemoveDependency("x", "y");
        dg.ReplaceDependents("x", []);
        dg.ReplaceDependees("y", []);
        Assert.AreEqual(dg.Size, 0);
    }

    // ********************** Making Sure that Static Variables Weren't Used ****************** //

    /// <summary>
    ///   <para>
    ///     It should be possible to have more than one DG at a time.
    ///   </para>
    ///   <remark>
    ///     This test is repeated to increase its weighting in the grading.
    ///   </remark>
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("13")]
    public void Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1()
    {
        DependencyGraph dg_1 = new();
        DependencyGraph dg_2 = new();
        dg_1.AddDependency("x", "y");
        Assert.AreEqual(1, dg_1.Size);
        Assert.AreEqual(0, dg_2.Size);
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("14")]
    public void Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_2()
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("15")]
    public void TestStatic1_3()
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("16")]
    public void TestStatic1_4()
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /// <summary>
    ///   Increase weight of Previous Test. Not necessary in Non-Grading test suite.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("17")]
    public void TestStatic1_5()
    {
        Constructor_CreateMultipleInstances_EachIsIndependentOfTheOther_1();
    }

    /**************************** SIMPLE NON-EMPTY TESTS ****************************/

    /// <summary>
    ///   Add four dependencies and the size should be four.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("18")]
    public void Size_AddFour_ResultIsFour()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("c", "b");
        dg.AddDependency("b", "d");
        Assert.AreEqual(4, dg.Size);
    }

    /// <summary>
    ///    After adding two items that depend on "b", check the dependees on the "b" node.
    ///    There should be two.
    ///  </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("19")]
    public void AddDependencyGetDependees_AddTwoDependenciesToSameNode_GetDependeesValueOfTwo()
    {
        DependencyGraph dg = new();

        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("c", "b");
        dg.AddDependency("b", "d");

        var dependees = dg.GetDependees("b");

        Assert.AreEqual(2, dependees.Count());
        Assert.IsTrue(dependees.Matches(["a", "c"]));
    }

    /// <summary>
    ///   Given a -> b, a -> c, c -> b, and b -> d.
    ///   a has dependents. a does not have dependees.  b had both dependents and dependees.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("20")]
    public void HasDependentsHasDependees_SmallGraph_ReturnsAppropriateTrueFalse()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("c", "b");
        dg.AddDependency("b", "d");
        Assert.IsTrue(dg.HasDependents("a"));
        Assert.IsFalse(dg.HasDependees("a"));
        Assert.IsTrue(dg.HasDependents("b"));
        Assert.IsTrue(dg.HasDependees("b"));
    }

    /// <summary>
    ///    Check that the simple graph contains the right dependents and dependees.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("21")]
    public void GetDependeesGetDependents_SimpleGraph_ContainsCorrectValues()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("c", "b");
        dg.AddDependency("b", "d");

        IEnumerable<string> dependentsOfA = dg.GetDependents("a");
        IEnumerable<string> dependeesOfB = dg.GetDependees("b");

        Assert.IsTrue(dependentsOfA.Matches(["b", "c"]));
        Assert.IsTrue(dependeesOfB.Matches(["a", "c"]));
    }

    /// <summary>
    ///   Test that a simple graph does not have values for items
    ///   that were not added to the graph....
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("22")]
    public void GetDependentsGetDependees_GetValuesFromValidAndInvalidNodes_ReturnsZeroOrOne()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        dg.AddDependency("c", "b");
        dg.AddDependency("b", "d");

        IEnumerable<string> dependeesOfE = dg.GetDependees("e");
        IEnumerable<string> dependentsOfE = dg.GetDependents("e");
        IEnumerable<string> dependeesOfF = dg.GetDependees("f");
        IEnumerable<string> dependentsOfF = dg.GetDependents("f");
        IEnumerable<string> dependeesOfD = dg.GetDependees("d");

        Assert.AreEqual(0, dependeesOfE.Count());
        Assert.AreEqual(0, dependeesOfF.Count());
        Assert.AreEqual(1, dependeesOfD.Count());
        Assert.AreEqual(0, dependentsOfE.Count());
        Assert.AreEqual(0, dependentsOfF.Count());
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("23")]
    public void TestDuplicatesSize()
    {
        DependencyGraph t = new();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "b");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        t.AddDependency("c", "b");
        Assert.AreEqual(4, t.Size);
    }

    /// <summary>
    ///   Test that "set" functionality is correct. Cannot add the same item multiple times.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("24")]
    public void AddDependency_AddTheSameItemMultipleTimes_ShouldOnlyAddOnce()
    {
        DependencyGraph t = new();
        t.AddDependency("a", "b");
        t.AddDependency("a", "b");
        t.AddDependency("c", "b");
        t.AddDependency("c", "b");
        Assert.AreEqual(2, t.GetDependees("b").Count());
        Assert.AreEqual(1, t.GetDependents("a").Count());
        Assert.AreEqual(1, t.GetDependents("c").Count());
    }

    /// <summary>
    ///   Attempting to add the same dependency multiple time also
    ///   does not affect the HasDependents functionality.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("25")]
    public void HasDependentsDependees_AddDuplicatesDependencies_DoesNotChangeGraph()
    {
        DependencyGraph t = new();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "b");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        t.AddDependency("c", "b");
        Assert.IsTrue(t.HasDependents("a"));
        Assert.IsFalse(t.HasDependees("a"));
        Assert.IsTrue(t.HasDependents("b"));
        Assert.IsTrue(t.HasDependees("b"));
    }

    /// <summary>
    ///   Here we test that adding an item multiple times does not create
    ///   multiple entries in the graph.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("26")]
    public void GetDependees_Duplicates_NotStored()
    {
        DependencyGraph t = new();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "b");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        t.AddDependency("c", "b");

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());

        string s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        string s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }

    /// <summary>
    ///   Similar to last test, check that multiple inserts do not
    ///   add multiple dependents into the graph.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("27")]
    public void GetDependents_Duplicates_NotStored()
    {
        DependencyGraph t = new();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "b");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        t.AddDependency("c", "b");

        IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
        Assert.IsTrue(e.MoveNext());

        string s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        string s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

        e = t.GetDependents("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());

        Assert.AreEqual("d", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependents("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependents("d").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("28")]
    public void TestComplexAddRemove()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "y");
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "d");
        t.AddDependency("c", "b");
        t.RemoveDependency("a", "d");
        t.AddDependency("e", "b");
        t.AddDependency("b", "d");
        t.RemoveDependency("e", "b");
        t.RemoveDependency("x", "y");
        Assert.AreEqual(4, t.Size);
    }

    /// <summary>
    ///    Add and remove a number of items from the graph.
    ///    Check that b has 2 dependees at the end.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("29")]
    public void AddRemove_MultipleItemsAddedThemRemoved_EndsWithTwo()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "y");
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "d");
        t.AddDependency("c", "b");
        t.RemoveDependency("a", "d");
        t.AddDependency("e", "b");
        t.AddDependency("b", "d");
        t.RemoveDependency("e", "b");
        t.RemoveDependency("x", "y");
        Assert.AreEqual(2, t.GetDependees("b").Count());
    }

    /// <summary>
    ///    Add and remove multiple items from the graph, and then
    ///    check that a and b are valid at the end.  A has dependents.
    ///    B has both dependents and dependees.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("30")]
    public void HasDependentsDependees_AddRemoveMultipleItems_AHasDependentsBHasDependeesAndDependents()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "y");
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "d");
        t.AddDependency("c", "b");
        t.RemoveDependency("a", "d");
        t.AddDependency("e", "b");
        t.AddDependency("b", "d");
        t.RemoveDependency("e", "b");
        t.RemoveDependency("x", "y");
        Assert.IsTrue(t.HasDependents("a"));
        Assert.IsFalse(t.HasDependees("a"));
        Assert.IsTrue(t.HasDependents("b"));
        Assert.IsTrue(t.HasDependees("b"));
    }

    /// <summary>
    ///    Test that the returned type from the GetDependees can be enumerated properly over.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("31")]
    public void GetDependees_MultipleAddsRemoves_CanBeEnumeratedOver()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "y");
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "d");
        t.AddDependency("c", "b");
        t.RemoveDependency("a", "d");
        t.AddDependency("e", "b");
        t.AddDependency("b", "d");
        t.RemoveDependency("e", "b");
        t.RemoveDependency("x", "y");

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());

        string s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        string s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }

    /// <summary>
    ///    Repeat previous test but for GetDependents.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("32")]
    public void GetDependents_MultipleAddsRemoves_CanBeEnumeratedOver()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "y");
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("a", "d");
        t.AddDependency("c", "b");
        t.RemoveDependency("a", "d");
        t.AddDependency("e", "b");
        t.AddDependency("b", "d");
        t.RemoveDependency("e", "b");
        t.RemoveDependency("x", "y");

        IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
        Assert.IsTrue(e.MoveNext());

        string s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        string s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

        e = t.GetDependents("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("d", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependents("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependents("d").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
    }

    /// <summary>
    ///   FIXME.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("33")]
    public void TestEmptyReplaceDependees()
    {
        DependencyGraph dg = new();

        dg.ReplaceDependees("b", ["a"]); //a is new dependee

        Assert.IsTrue(new HashSet<string> { "b" }.SetEquals(dg.GetDependents("a")));
        Assert.AreEqual(1, dg.Size);
    }

    /// <summary>
    ///    Test that the replace method works properly including
    ///    replacing with an empty list, and "replacing" something
    ///    that previously had no values.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("34")]
    public void ReplaceDependentsDependees_MultipleReplaces_FinalSizeFour()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", []);
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", ["c"]);
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", ["a", "c"]);
        t.ReplaceDependees("d", ["b"]);
        Assert.AreEqual(4, t.Size);
    }

    /// <summary>
    ///    Same as previous test, but check another element for the proper value.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("35")]
    public void ReplaceDependentsDependees_MultipleReplacements_BHadTwo()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", []);
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", ["c"]);
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", ["a", "c"]);
        t.ReplaceDependees("d", ["b"]);
        Assert.AreEqual(2, t.GetDependees("b").Count());
    }

    /// <summary>
    ///    Check that replace dependents and dependees also works with the
    ///    HasDependents and HasDependees.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("36")]
    public void ReplaceAndHas_MultipleReplacements_AHasDependentsBHasBoth()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", []);
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", ["c"]);
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", ["a", "c"]); // b is now a dependee of a,c
        t.ReplaceDependees("d", ["b"]); // d is now a dependee of b
        Assert.IsTrue(t.HasDependents("a"));
        Assert.IsFalse(t.HasDependees("a"));
        Assert.IsTrue(t.HasDependents("b"));
        Assert.IsTrue(t.HasDependees("b"));
    }

    /// <summary>
    ///    Test again using a helper method to setup the intial graph.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("37")]
    public void GetDependees_UseHelperMethodForSetup_Passes()
    {
        DependencyGraph t = SetupComplexDependencies();

        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());

        string s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        string s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }

    /// <summary>
    ///    Use a helper method to setup graph. More checks on getdependents.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("38")]
    public void GetDependents_UseHelperMethodForSetup_Passes()
    {
        DependencyGraph t = SetupComplexDependencies();

        IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
        Assert.IsTrue(e.MoveNext());

        string s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        string s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "b") && (s2 == "c")) || ((s1 == "c") && (s2 == "b")));

        e = t.GetDependents("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("d", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependents("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());

        e = t.GetDependents("d").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
    }

    // ************************** STRESS TESTS REPEATED MULTIPLE TIMES ******************************** //

    /// <summary>
    ///    Using lots of data.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("39")]
    public void AddRemove_StressTestLotsOfData_Passes()
    {
        // Dependency graph
        DependencyGraph t = new();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = "a" + i;
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = [];
            dees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    ///   Increase weight of Stress Test 1.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("40")]
    public void IncreaseGradeWeight2()
    {
        AddRemove_StressTestLotsOfData_Passes();
    }

    /// <summary>
    ///   Increase weight of Stress Test 1.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("41")]
    public void IncreaseGradeWeight3()
    {
        AddRemove_StressTestLotsOfData_Passes();
    }

    /// <summary>
    ///    Another stress test that also uses replace dependencies.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("42")]
    public void AddRemoveReplace_StressTestLotsOfData_Passes()
    {
        // Dependency graph
        DependencyGraph t = new();

        // A bunch of strings to use
        const int SIZE = 400;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = "a" + i;
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = [];
            dees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 2; j < SIZE; j += 3)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        // Replace a bunch of dependents
        for (int i = 0; i < SIZE; i += 2)
        {
            HashSet<string> newDents = [];
            for (int j = 0; j < SIZE; j += 5)
            {
                newDents.Add(letters[j]);
            }

            t.ReplaceDependents(letters[i], newDents);

            foreach (string s in dents[i])
            {
                dees[int.Parse(s[1..])].Remove(letters[i]);
            }

            foreach (string s in newDents)
            {
                dees[int.Parse(s[1..])].Add(letters[i]);
            }

            dents[i] = newDents;
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    ///   Increase weight of prior stress test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("43")]
    public void StressTest8_2()
    {
        AddRemoveReplace_StressTestLotsOfData_Passes();
    }

    /// <summary>
    ///   Increase weight of prior stress test.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("44")]
    public void StressTest10()
    {
        AddRemoveReplace_StressTestLotsOfData_Passes();
    }

    // ********************************** A THIRD STESS TEST ******************** //

    /// <summary>
    ///    Using lots of data with replacement.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("45")]
    public void AddRemoveReplace_StressTestThree_Passes()
    {
        // Dependency graph
        DependencyGraph t = new();

        // A bunch of strings to use
        const int SIZE = 1000;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = "a" + i;
        }

        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = [];
            dees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]);
                dees[j].Add(letters[i]);
            }
        }

        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 2; j < SIZE; j += 3)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]);
                dees[j].Remove(letters[i]);
            }
        }

        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }

        // Replace a bunch of dependees
        for (int i = 0; i < SIZE; i += 2)
        {
            HashSet<string> newDees = [];
            for (int j = 0; j < SIZE; j += 9)
            {
                newDees.Add(letters[j]);
            }

            t.ReplaceDependees(letters[i], newDees);

            foreach (string s in dees[i])
            {
                dents[int.Parse(s[1..])].Remove(letters[i]);
            }

            foreach (string s in newDees)
            {
                dents[int.Parse(s[1..])].Add(letters[i]);
            }

            dees[i] = newDees;
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
            Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    ///   Increase grade value.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("46")]
    public void IncreaseGradingWeight1()
    {
        AddRemoveReplace_StressTestThree_Passes();
    }

    /// <summary>
    ///   Increase grade value.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("47")]
    public void IncreaseGradingWeight2()
    {
        AddRemoveReplace_StressTestThree_Passes();
    }

    /// <summary>
    ///   Helper code to build a simple dependency graph.
    /// </summary>
    /// <returns> The new graph. </returns>
    private static DependencyGraph SetupComplexDependencies()
    {
        DependencyGraph t = new();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", []);
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", ["c"]);
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", ["a", "c"]);
        t.ReplaceDependees("d", ["b"]);
        return t;
    }
}

/// <summary>
///   Helper methods for the tests above.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    ///   Check to see if the two "sets" (source and items) match, i.e.,
    ///   contain exactly the same values.
    /// </summary>
    /// <param name="source"> original container.</param>
    /// <param name="items"> elements to match against.</param>
    /// <returns> true if every element in source is in items and vice versa. They are the "same set".</returns>
    public static bool Matches(this IEnumerable<string> source, params string[] items)
    {
        return (source.Count() == items.Length) && items.All(item => source.Contains(item));
    }
}