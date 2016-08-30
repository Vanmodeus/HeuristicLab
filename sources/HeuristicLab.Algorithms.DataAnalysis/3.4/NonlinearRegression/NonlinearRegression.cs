#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Nonlinear regression data analysis algorithm.
  /// </summary>
  [Item("Nonlinear Regression (NLR)", "Nonlinear regression (curve fitting) data analysis algorithm (wrapper for ALGLIB).")]
  [Creatable(CreatableAttribute.Categories.DataAnalysisRegression, Priority = 120)]
  [StorableClass]
  public sealed class NonlinearRegression : FixedDataAnalysisAlgorithm<IRegressionProblem> {
    private const string RegressionSolutionResultName = "Regression solution";
    private const string ModelStructureParameterName = "Model structure";
    private const string IterationsParameterName = "Iterations";
    private const string RestartsParameterName = "Restarts";
    private const string SetSeedRandomlyParameterName = "SetSeedRandomly";
    private const string SeedParameterName = "Seed";

    public IFixedValueParameter<StringValue> ModelStructureParameter {
      get { return (IFixedValueParameter<StringValue>)Parameters[ModelStructureParameterName]; }
    }
    public IFixedValueParameter<IntValue> IterationsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[IterationsParameterName]; }
    }

    public IFixedValueParameter<BoolValue> SetSeedRandomlyParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[SetSeedRandomlyParameterName]; }
    }

    public IFixedValueParameter<IntValue> SeedParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[SeedParameterName]; }
    }

    public IFixedValueParameter<IntValue> RestartsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[RestartsParameterName]; }
    }

    public string ModelStructure {
      get { return ModelStructureParameter.Value.Value; }
      set { ModelStructureParameter.Value.Value = value; }
    }

    public int Iterations {
      get { return IterationsParameter.Value.Value; }
      set { IterationsParameter.Value.Value = value; }
    }

    public int Restarts {
      get { return RestartsParameter.Value.Value; }
      set { RestartsParameter.Value.Value = value; }
    }

    public int Seed {
      get { return SeedParameter.Value.Value; }
      set { SeedParameter.Value.Value = value; }
    }

    public bool SetSeedRandomly {
      get { return SetSeedRandomlyParameter.Value.Value; }
      set { SetSeedRandomlyParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private NonlinearRegression(bool deserializing) : base(deserializing) { }
    private NonlinearRegression(NonlinearRegression original, Cloner cloner)
      : base(original, cloner) {
    }
    public NonlinearRegression()
      : base() {
      Problem = new RegressionProblem();
      Parameters.Add(new FixedValueParameter<StringValue>(ModelStructureParameterName, "The function for which the parameters must be fit (only numeric constants are tuned).", new StringValue("1.0 * x*x + 0.0")));
      Parameters.Add(new FixedValueParameter<IntValue>(IterationsParameterName, "The maximum number of iterations for constants optimization.", new IntValue(200)));
      Parameters.Add(new FixedValueParameter<IntValue>(RestartsParameterName, "The number of independent random restarts", new IntValue(10)));
      Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The PRNG seed value.", new IntValue()));
      Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "Switch to determine if the random number seed should be initialized randomly.", new BoolValue(true)));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (!Parameters.ContainsKey(RestartsParameterName))
        Parameters.Add(new FixedValueParameter<IntValue>(RestartsParameterName, "The number of independent random restarts", new IntValue(1)));
      if (!Parameters.ContainsKey(SeedParameterName))
        Parameters.Add(new FixedValueParameter<IntValue>(SeedParameterName, "The PRNG seed value.", new IntValue()));
      if (!Parameters.ContainsKey(SetSeedRandomlyParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(SetSeedRandomlyParameterName, "Switch to determine if the random number seed should be initialized randomly.", new BoolValue(true)));
      #endregion
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new NonlinearRegression(this, cloner);
    }

    #region nonlinear regression
    protected override void Run() {
      if (SetSeedRandomly) Seed = (new System.Random()).Next();
      var rand = new MersenneTwister((uint)Seed);
      IRegressionSolution bestSolution = null;
      for (int r = 0; r < Restarts; r++) {
        var solution = CreateRegressionSolution(Problem.ProblemData, ModelStructure, Iterations, rand);
        if (bestSolution == null || solution.TrainingRootMeanSquaredError < bestSolution.TrainingRootMeanSquaredError) {
          bestSolution = solution;
        }
      }

      Results.Add(new Result(RegressionSolutionResultName, "The nonlinear regression solution.", bestSolution));
      Results.Add(new Result("Root mean square error (train)", "The root of the mean of squared errors of the regression solution on the training set.", new DoubleValue(bestSolution.TrainingRootMeanSquaredError)));
      Results.Add(new Result("Root mean square error (test)", "The root of the mean of squared errors of the regression solution on the test set.", new DoubleValue(bestSolution.TestRootMeanSquaredError)));

    }

    /// <summary>
    /// Fits a model to the data by optimizing the numeric constants.
    /// Model is specified as infix expression containing variable names and numbers. 
    /// The starting point for the numeric constants is initialized randomly if a random number generator is specified (~N(0,1)). Otherwise the user specified constants are
    /// used as a starting point. 
    /// </summary>
    /// <param name="problemData">Training and test data</param>
    /// <param name="modelStructure">The function as infix expression</param>
    /// <param name="maxIterations">Number of constant optimization iterations (using Levenberg-Marquardt algorithm)</param>
    /// <param name="random">Optional random number generator for random initialization of numeric constants.</param>
    /// <returns></returns>
    public static ISymbolicRegressionSolution CreateRegressionSolution(IRegressionProblemData problemData, string modelStructure, int maxIterations, IRandom random = null) {
      var parser = new InfixExpressionParser();
      var tree = parser.Parse(modelStructure);

      if (!SymbolicRegressionConstantOptimizationEvaluator.CanOptimizeConstants(tree)) throw new ArgumentException("The optimizer does not support the specified model structure.");

      // initialize constants randomly
      if (random != null) {
        foreach (var node in tree.IterateNodesPrefix().OfType<ConstantTreeNode>()) {
          node.Value = NormalDistributedRandom.NextDouble(random, 0, 1);
        }
      }
      var interpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();

      SymbolicRegressionConstantOptimizationEvaluator.OptimizeConstants(interpreter, tree, problemData, problemData.TrainingIndices,
        applyLinearScaling: false, maxIterations: maxIterations,
        updateVariableWeights: false, updateConstantsInTree: true);

      var scaledModel = new SymbolicRegressionModel(problemData.TargetVariable, tree, (ISymbolicDataAnalysisExpressionTreeInterpreter)interpreter.Clone());
      scaledModel.Scale(problemData);
      SymbolicRegressionSolution solution = new SymbolicRegressionSolution(scaledModel, (IRegressionProblemData)problemData.Clone());
      solution.Model.Name = "Regression Model";
      solution.Name = "Regression Solution";
      return solution;
    }
    #endregion
  }
}