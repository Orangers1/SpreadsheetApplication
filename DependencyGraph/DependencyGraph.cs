// <copyright file="DependencyGraph.cs" company="UofU-CS3500">
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
// Dependency Graph class represents cells that are dependees/dependents of other cells.
// contains methods for useful things like fetching a list of dependees/dependents and
// checking if a cell has dependees/dependents.
// </summary>

namespace CS3500.DependencyGraph;

/// <summary>
///   <para>
///     (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///     (in other words: s1 must be evaluated before t1.)
///   </para>
///   <para>
///     A DependencyGraph can be modeled as a set of ordered pairs of strings.
///     Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///     if s1 equals s2 and t1 equals t2.
///   </para>
///   <remarks>
///     Recall that sets never contain duplicates.
///     If an attempt is made to add an element to a set, and the element is already
///     in the set, the set remains unchanged.
///   </remarks>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    /// current size of DependencyGraph instance.
    /// </summary>
    private int size;

    /// <summary>
    /// dependents Dictionary: keys are dependees, values are lists of dependents.
    /// </summary>
    private Dictionary<string, HashSet<string>> dependents;

    /// <summary>
    /// dependees Dictionary: keys are dependents, values are lists of dependees.
    /// </summary>
    private Dictionary<string, HashSet<string>> dependees;

    /// <summary>
    ///   Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///   The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        size = 0;
        dependents = new ();
        dependees = new ();
    }

    /// <summary>
    /// Gets the number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get { return size; }
    }

    /// <summary>
    ///   Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        int numOfDependents = GetDependents(nodeName).Count();
        if (numOfDependents > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        int numOfDependees = GetDependees(nodeName).Count();
        if (numOfDependees > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependents of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        // dependents: dependees are keys
        if (dependents.TryGetValue(nodeName, out HashSet<string>? setOfDependents))
        {
            return setOfDependents;
        }

        // no dependents, return blank set
        return new HashSet<string>();
    }

    /// <summary>
    ///   <para>
    ///     Returns the dependees of the node with the given name.
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        // dependees: dependents are keys.
        if (dependees.TryGetValue(nodeName, out HashSet<string>? setOfDependees))
        {
            return setOfDependees;
        }

        // no dependees, return blank set
        return new HashSet<string>();
    }

    /// <summary>
    /// <para>
    ///   Adds the ordered pair (dependee, dependent), if it doesn't already exist (otherwise nothing happens).
    /// </para>
    /// <para>
    ///   This can be thought of as: dependee must be evaluated before dependent.
    /// </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first. </param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after the other node has been. </param>
    public void AddDependency(string dependee, string dependent)
    {
        // do this here to avoid problem due to short circuit evaluation.
        bool dependentAdded = AddDependent(dependee, dependent);
        bool dependeeAdded = AddDependee(dependee, dependent);

        if (dependentAdded || dependeeAdded)
        {
            size++;
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes the ordered pair (dependee, dependent), if it exists (otherwise nothing happens).
    ///   </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first. </param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until the other node has been. </param>
    public void RemoveDependency(string dependee, string dependent)
    {
        if (dependents.TryGetValue(dependee, out HashSet<string>? setOfDependents))
        {
            if (setOfDependents.Contains(dependent))
            {
                dependents[dependee].Remove(dependent);

                // * If dependents dict value contains the dependent,
                // we can safely remove the dependee from the dependees dict too.
                dependees[dependent].Remove(dependee);
                size--;
            }
        }
    }

    /// <summary>
    ///   Removes all existing ordered pairs of the form (nodeName, *).  Then, for each
    ///   t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependents are being replaced. </param>
    /// <param name="newDependents"> The new dependents for nodeName. Could be empty.</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        // remove duplicates
        HashSet<string> newDependentVals = newDependents.ToHashSet<string>();

        if (dependents.TryGetValue(nodeName, out HashSet<string>? setOfDependents))
        {
            foreach(string x in dependents[nodeName])
            {
                RemoveDependency(nodeName, x);
            }

            // add to map and mirror map
            foreach(string x in newDependentVals)
            {
                AddDependency(nodeName, x);
            }

            return;
        }

        // only add to map and mirror map if DNE
        foreach (string x in newDependentVals)
        {
            AddDependency(nodeName, x);
        }
    }

    /// <summary>
    ///   <para>
    ///     Removes all existing ordered pairs of the form (*, nodeName).  Then, for each
    ///     t in newDependees, adds the ordered pair (t, nodeName).
    ///   </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced. </param>
    /// <param name="newDependees"> The new dependees for nodeName. Could be empty.</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        HashSet<string> newDependeeVals = newDependees.ToHashSet<string>();

        if (dependees.TryGetValue(nodeName, out HashSet<string>? setOfDependees))
        {
            foreach (string x in dependees[nodeName])
            {
                RemoveDependency(x, nodeName);
            }

            // add to map and mirror map
            foreach (string x in newDependeeVals)
            {
                AddDependency(x, nodeName);
            }

            return;
        }

        // only add to map and mirror map if DNE
        foreach (string x in newDependeeVals)
        {
            AddDependency(x, nodeName);
        }
    }

    /// <summary>
    /// Adds dependent safely, by checking if the key exists, and checking if the
    /// value exists. Adds to dependents dictionary.
    /// </summary>
    /// <param name="dependee"> Check if dependents dict contains the dependee,
    /// represented as a key.</param>
    /// <param name="dependent"> Check if dependents dict contains the dependent,
    /// possibly represented in the list of values associated with the dependee key.</param>
    /// <returns>true if the dict now contains a NEW dependency, false otherwise.</returns>
    private bool AddDependent(string dependee, string dependent)
    {
        // key is already in dependents dict
        if (dependents.ContainsKey(dependee))
        {
            // value is not in list of values
            if (!dependents[dependee].Contains(dependent))
            {
                dependents[dependee].Add(dependent);
                return true;
            }

            return false;
        }

        // dict doesn't contain key, so we add the PAIR
        HashSet<string> newDependentsVal = [dependent];
        dependents.Add(dependee, newDependentsVal);

        return true;
    }

    /// <summary>
    /// Adds dependee safely, by checking if the key exists, and checking if the
    /// value exists. Adds to dependees dictionary.
    /// </summary>
    /// <param name="dependee"> Check if dependees dict contains the dependee,
    /// possibly represented in the list of values associated with the dependent key.</param>
    /// <param name="dependent"> Check if dependees dict contains the dependent,
    /// represented as a key.</param>
    /// <returns>true if the dict now contains a NEW dependency, false otherwise.</returns>
    private bool AddDependee(string dependee, string dependent)
    {
        // key is already in dependents dict
        if (dependees.ContainsKey(dependent))
        {
            // value is not in list of values
            if (!dependees[dependent].Contains(dependee))
            {
                dependees[dependent].Add(dependee);
                return true;
            }

            return false;
        }

        // dict doesn't contain key, so we add the PAIR
        HashSet<string> newDependeesVal = [dependee];
        dependees.Add(dependent, newDependeesVal);

        return true;
    }
}