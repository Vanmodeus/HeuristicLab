﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.GeneralSymbols;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureAlteringOperators {
  /// <summary>
  /// As described in Koza, Bennett, Andre, Keane, Genetic Programming III - Darwinian Invention and Problem Solving, 1999, pp. 112
  /// </summary>
  [Item("ArgumentDeleter", "Manipulates a symbolic expression by deleting an argument from an existing function defining branch.")]
  [StorableClass]
  public sealed class ArgumentDeleter : SymbolicExpressionTreeArchitectureAlteringOperator {
    public override sealed void ModifyArchitecture(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      IntValue maxTreeSize, IntValue maxTreeHeight,
      IntValue maxFunctionDefiningBranches, IntValue maxFunctionArguments,
      out bool success) {
      success = DeleteArgument(random, symbolicExpressionTree, grammar, maxTreeSize.Value, maxTreeHeight.Value, maxFunctionDefiningBranches.Value, maxFunctionArguments.Value);
    }

    public static bool DeleteArgument(
      IRandom random,
      SymbolicExpressionTree symbolicExpressionTree,
      ISymbolicExpressionGrammar grammar,
      int maxTreeSize, int maxTreeHeight,
      int maxFunctionDefiningBranches, int maxFunctionArguments) {

      var functionDefiningBranches = symbolicExpressionTree.IterateNodesPrefix().OfType<DefunTreeNode>();
      if (functionDefiningBranches.Count() == 0)
        // no function defining branch => abort
        return false;
      var selectedDefunBranch = functionDefiningBranches.SelectRandom(random);
      if (selectedDefunBranch.NumberOfArguments <= 1)
        // argument deletion by consolidation is not possible => abort
        return false;
      var removedArgument = (from sym in selectedDefunBranch.Grammar.Symbols.OfType<Argument>()
                             select sym.ArgumentIndex).Distinct().SelectRandom(random);
      // find invocations of the manipulated funcion and remove the specified argument tree
      var invocationNodes = from node in symbolicExpressionTree.IterateNodesPrefix().OfType<InvokeFunctionTreeNode>()
                            where node.Symbol.FunctionName == selectedDefunBranch.FunctionName
                            select node;
      foreach (var invokeNode in invocationNodes) {
        invokeNode.RemoveSubTree(removedArgument);
      }

      DeleteArgumentByConsolidation(random, selectedDefunBranch, removedArgument);

      // delete the dynamic argument symbol that matches the argument to be removed
      var matchingSymbol = selectedDefunBranch.Grammar.Symbols.OfType<Argument>().Where(s => s.ArgumentIndex == removedArgument).First();
      selectedDefunBranch.Grammar.RemoveSymbol(matchingSymbol);
      // reduce arity in known functions of all root branches
      foreach (var subtree in symbolicExpressionTree.Root.SubTrees) {
        var matchingInvokeSymbol = subtree.Grammar.Symbols.OfType<InvokeFunction>().Where(s => s.FunctionName == selectedDefunBranch.FunctionName).FirstOrDefault();
        if (matchingInvokeSymbol != null) {
          subtree.Grammar.SetMinSubtreeCount(matchingInvokeSymbol, selectedDefunBranch.NumberOfArguments - 1);
          subtree.Grammar.SetMaxSubtreeCount(matchingInvokeSymbol, selectedDefunBranch.NumberOfArguments - 1);
        }
      }
      selectedDefunBranch.NumberOfArguments--;
      return true;
    }

    private static void DeleteArgumentByConsolidation(IRandom random, DefunTreeNode branch, int removedArgumentIndex) {
      // replace references to the deleted argument with random references to existing arguments
      var possibleArgumentSymbols = (from sym in branch.Grammar.Symbols.OfType<Argument>()
                                     where sym.ArgumentIndex != removedArgumentIndex
                                     select sym).ToList();
      var argNodes = from node in branch.IterateNodesPrefix().OfType<ArgumentTreeNode>()
                     where node.Symbol.ArgumentIndex == removedArgumentIndex
                     select node;
      foreach (var argNode in argNodes) {
        var replacementSymbol = possibleArgumentSymbols.SelectRandom(random);
        argNode.Symbol = replacementSymbol;
      }
    }
  }
}
