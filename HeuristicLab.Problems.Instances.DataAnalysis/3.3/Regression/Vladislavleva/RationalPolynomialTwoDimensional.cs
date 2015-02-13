﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class RationalPolynomialTwoDimensional : ArtificialRegressionDataDescriptor {

    public override string Name { get { return "Vladislavleva-8 F8(X1, X2) = ((X1 - 3)^4 + (X2 - 3)³ - (X2 -3)) / ((X2 - 2)^4 + 10)"; } }
    public override string Description {
      get {
        return "Paper: Order of Nonlinearity as a Complexity Measure for Models Generated by Symbolic Regression via Pareto Genetic Programming " + Environment.NewLine
        + "Authors: Ekaterina J. Vladislavleva, Member, IEEE, Guido F. Smits, Member, IEEE, and Dick den Hertog" + Environment.NewLine
        + "Function: F8(X1, X2) = ((X1 - 3)^4 + (X2 - 3)³ - (X2 -3)) / ((X2 - 2)^4 + 10)" + Environment.NewLine
        + "Training Data: 50 points X1, X2 = Rand(0.05, 6.05)" + Environment.NewLine
        + "Test Data: 34*34 points X1, X2 = (-0.25:0.2:6.35)" + Environment.NewLine
        + "Function Set: +, -, *, /, square, x^eps, x + eps, x * eps";
      }
    }
    protected override string TargetVariable { get { return "Y"; } }
    protected override string[] VariableNames { get { return new string[] { "X1", "X2", "Y" }; } }
    protected override string[] AllowedInputVariables { get { return new string[] { "X1", "X2" }; } }
    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return 50; } }
    protected override int TestPartitionStart { get { return 50; } }
    protected override int TestPartitionEnd { get { return 50 + (34 * 34); } }

    protected override List<List<double>> GenerateValues() {
      List<List<double>> data = new List<List<double>>();

      List<double> oneVariableTestData = ValueGenerator.GenerateSteps(-0.25m, 6.35m, 0.2m).Select(v => (double)v).ToList();

      List<List<double>> testData = new List<List<double>>() { oneVariableTestData, oneVariableTestData };
      var combinations = ValueGenerator.GenerateAllCombinationsOfValuesInLists(testData).ToList<IEnumerable<double>>();

      for (int i = 0; i < AllowedInputVariables.Count(); i++) {
        data.Add(ValueGenerator.GenerateUniformDistributedValues(50, 0.05, 6.05).ToList());
        data[i].AddRange(combinations[i]);
      }

      double x1, x2;
      List<double> results = new List<double>();
      for (int i = 0; i < data[0].Count; i++) {
        x1 = data[0][i];
        x2 = data[1][i];
        results.Add((Math.Pow(x1 - 3, 4) + Math.Pow(x2 - 3, 3) - x2 + 3) / (Math.Pow(x2 - 2, 4) + 10));
      }
      data.Add(results);

      return data;
    }
  }
}
