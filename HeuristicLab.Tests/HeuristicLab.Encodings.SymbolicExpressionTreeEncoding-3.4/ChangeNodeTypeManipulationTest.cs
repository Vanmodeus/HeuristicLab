﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Random;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding_34.Tests {
  [TestClass]
  public class ChangeNodeTypeManipulationTest {
    private const int POPULATION_SIZE = 1000;
    private const int MAX_TREE_LENGTH = 100;
    private const int MAX_TREE_DEPTH = 10;
    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext {
      get {
        return testContextInstance;
      }
      set {
        testContextInstance = value;
      }
    }

    [TestMethod()]
    public void ChangeNodeTypeManipulationDistributionsTest() {
      SymbolicExpressionTreeStringFormatter formatter = new SymbolicExpressionTreeStringFormatter();
      var trees = new List<ISymbolicExpressionTree>();
      var grammar = Grammars.CreateArithmeticAndAdfGrammar();
      var random = new MersenneTwister(31415);
      int failedEvents = 0;
      for (int i = 0; i < POPULATION_SIZE; i++) {
        var tree = ProbabilisticTreeCreator.Create(random, grammar, MAX_TREE_LENGTH, MAX_TREE_DEPTH);
        string originalTree = formatter.Format(tree);
        ChangeNodeTypeManipulation.ChangeNodeType(random, tree);
        string manipulatedTree = formatter.Format(tree);
        if (originalTree == manipulatedTree) failedEvents++;
        Util.IsValid(tree);
        trees.Add(tree);
      }
      Console.WriteLine("ChangeNodeTypeManipulation: " + Environment.NewLine +
        "Failed events: " + failedEvents * 100.0 / POPULATION_SIZE + " %" + Environment.NewLine +
        Util.GetSizeDistributionString(trees, 105, 5) + Environment.NewLine +
        Util.GetFunctionDistributionString(trees) + Environment.NewLine +
        Util.GetNumberOfSubtreesDistributionString(trees) + Environment.NewLine +
        Util.GetTerminalDistributionString(trees) + Environment.NewLine
        );
      Assert.IsTrue(failedEvents * 100.0 / POPULATION_SIZE < 5.0); // only max 5% failed mutations are allowed
    }
  }
}
