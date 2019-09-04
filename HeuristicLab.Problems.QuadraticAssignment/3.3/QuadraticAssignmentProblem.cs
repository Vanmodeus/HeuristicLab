#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("Quadratic Assignment Problem (QAP)", "The Quadratic Assignment Problem (QAP) can be described as the problem of assigning N facilities to N fixed locations such that there is exactly one facility in each location and that the sum of the distances multiplied by the connection strength between the facilities becomes minimal.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 140)]
  [StorableType("A86B1F49-D8E6-45E4-8EFB-8F5CCA2F9DC7")]
  public sealed class QuadraticAssignmentProblem : PermutationProblem,
    IProblemInstanceConsumer<QAPData>,
    IProblemInstanceConsumer<TSPData> {

    public static new Image StaticItemImage {
      get { return Common.Resources.VSImageLibrary.Type; }
    }

    public override bool Maximization { get { return false; } }

    #region Parameter Properties
    [Storable]
    private IValueParameter<ItemSet<Permutation>> bestKnownSolutionsParameter;
    public IValueParameter<ItemSet<Permutation>> BestKnownSolutionsParameter {
      get { return bestKnownSolutionsParameter; }
    }
    [Storable]
    private IValueParameter<Permutation> bestKnownSolutionParameter;
    public IValueParameter<Permutation> BestKnownSolutionParameter {
      get { return bestKnownSolutionParameter; }
    }
    [Storable]
    private IValueParameter<DoubleMatrix> weightsParameter;
    public IValueParameter<DoubleMatrix> WeightsParameter {
      get { return weightsParameter; }
    }
    [Storable]
    private IValueParameter<DoubleMatrix> distancesParameter;
    public IValueParameter<DoubleMatrix> DistancesParameter {
      get { return distancesParameter; }
    }
    [Storable]
    private IValueParameter<DoubleValue> lowerBoundParameter;
    public IValueParameter<DoubleValue> LowerBoundParameter {
      get { return lowerBoundParameter; }
    }
    [Storable]
    private IValueParameter<DoubleValue> averageQualityParameter;
    public IValueParameter<DoubleValue> AverageQualityParameter {
      get { return averageQualityParameter; }
    }
    #endregion

    #region Properties
    public ItemSet<Permutation> BestKnownSolutions {
      get { return bestKnownSolutionsParameter.Value; }
      set { bestKnownSolutionsParameter.Value = value; }
    }
    public Permutation BestKnownSolution {
      get { return bestKnownSolutionParameter.Value; }
      set { bestKnownSolutionParameter.Value = value; }
    }
    public DoubleMatrix Weights {
      get { return weightsParameter.Value; }
      set { weightsParameter.Value = value; }
    }
    public DoubleMatrix Distances {
      get { return distancesParameter.Value; }
      set { distancesParameter.Value = value; }
    }
    public DoubleValue LowerBound {
      get { return lowerBoundParameter.Value; }
      set { lowerBoundParameter.Value = value; }
    }
    public DoubleValue AverageQuality {
      get { return averageQualityParameter.Value; }
      set { averageQualityParameter.Value = value; }
    }

    private BestQAPSolutionAnalyzer BestQAPSolutionAnalyzer {
      get { return Operators.OfType<BestQAPSolutionAnalyzer>().FirstOrDefault(); }
    }

    private QAPAlleleFrequencyAnalyzer QAPAlleleFrequencyAnalyzer {
      get { return Operators.OfType<QAPAlleleFrequencyAnalyzer>().FirstOrDefault(); }
    }

    private QAPPopulationDiversityAnalyzer QAPPopulationDiversityAnalyzer {
      get { return Operators.OfType<QAPPopulationDiversityAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private QuadraticAssignmentProblem(StorableConstructorFlag _) : base(_) { }
    private QuadraticAssignmentProblem(QuadraticAssignmentProblem original, Cloner cloner)
      : base(original, cloner) {
      bestKnownSolutionsParameter = cloner.Clone(original.bestKnownSolutionsParameter);
      bestKnownSolutionParameter = cloner.Clone(original.bestKnownSolutionParameter);
      weightsParameter = cloner.Clone(original.weightsParameter);
      distancesParameter = cloner.Clone(original.distancesParameter);
      lowerBoundParameter = cloner.Clone(original.lowerBoundParameter);
      averageQualityParameter = cloner.Clone(original.averageQualityParameter);
      RegisterEventHandlers();
    }
    public QuadraticAssignmentProblem()
      : base(new PermutationEncoding("Assignment") { Length = 5 }) {
      Parameters.Add(bestKnownSolutionsParameter = new OptionalValueParameter<ItemSet<Permutation>>("BestKnownSolutions", "The list of best known solutions which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(bestKnownSolutionParameter = new OptionalValueParameter<Permutation>("BestKnownSolution", "The best known solution which is updated whenever a new better solution is found or may be the optimal solution if it is known beforehand.", null));
      Parameters.Add(weightsParameter = new ValueParameter<DoubleMatrix>("Weights", "The strength of the connection between the facilities.", new DoubleMatrix(5, 5)));
      Parameters.Add(distancesParameter = new ValueParameter<DoubleMatrix>("Distances", "The distance matrix which can either be specified directly without the coordinates, or can be calculated automatically from the coordinates.", new DoubleMatrix(5, 5)));
      Parameters.Add(lowerBoundParameter = new OptionalValueParameter<DoubleValue>("LowerBound", "The Gilmore-Lawler lower bound to the solution quality."));
      Parameters.Add(averageQualityParameter = new OptionalValueParameter<DoubleValue>("AverageQuality", "The expected quality of a random solution."));

      WeightsParameter.GetsCollected = false;
      Weights = new DoubleMatrix(new double[,] {
        { 0, 1, 0, 0, 1 },
        { 1, 0, 1, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 1, 0, 1 },
        { 1, 0, 0, 1, 0 }
      });

      DistancesParameter.GetsCollected = false;
      Distances = new DoubleMatrix(new double[,] {
        {   0, 360, 582, 582, 360 },
        { 360,   0, 360, 582, 582 },
        { 582, 360,   0, 360, 582 },
        { 582, 582, 360,   0, 360 },
        { 360, 582, 582, 360,   0 }
      });

      InitializeOperators();
      RegisterEventHandlers();
    }

    public override double Evaluate(Permutation assignment, IRandom random) {
      return Evaluate(assignment);
    }

    public double Evaluate(Permutation assignment) {
      double quality = 0;
      for (int i = 0; i < assignment.Length; i++) {
        for (int j = 0; j < assignment.Length; j++) {
          quality += Weights[i, j] * Distances[assignment[i], assignment[j]];
        }
      }
      return quality;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QuadraticAssignmentProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      if (bestKnownSolutionsParameter == null)
        bestKnownSolutionsParameter = (IValueParameter<ItemSet<Permutation>>)Parameters["BestKnownSolutions"];
      if (bestKnownSolutionParameter == null)
        bestKnownSolutionParameter = (IValueParameter<Permutation>)Parameters["BestKnownSolution"];
      if (weightsParameter == null)
        weightsParameter = (IValueParameter<DoubleMatrix>)Parameters["Weights"];
      if (distancesParameter == null)
        distancesParameter = (IValueParameter<DoubleMatrix>)Parameters["Distances"];
      if (lowerBoundParameter == null)
        lowerBoundParameter = (IValueParameter<DoubleValue>)Parameters["LowerBound"];
      if (averageQualityParameter == null)
        averageQualityParameter = (IValueParameter<DoubleValue>)Parameters["AverageQuality"];
      #endregion
      RegisterEventHandlers();
    }

    #region Events
    //TODO check with abhem if this is necessary
    //protected override void OnSolutionCreatorChanged() {
    //  Parameterize();
    //  base.OnSolutionCreatorChanged();
    //}
    protected override void OnEvaluatorChanged() {
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      Parameterize();
      base.OnEvaluatorChanged();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      Parameterize();
    }
    private void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      Weights.RowsChanged += Weights_RowsChanged;
      Weights.ColumnsChanged += Weights_ColumnsChanged;
      Weights.ToStringChanged += Weights_ToStringChanged;
      AdjustDistanceMatrix();
    }
    private void Weights_RowsChanged(object sender, EventArgs e) {
      if (Weights.Rows != Weights.Columns)
        ((IStringConvertibleMatrix)Weights).Columns = Weights.Rows;
      else {
        AdjustDistanceMatrix();
      }
    }
    private void Weights_ColumnsChanged(object sender, EventArgs e) {
      if (Weights.Rows != Weights.Columns)
        ((IStringConvertibleMatrix)Weights).Rows = Weights.Columns;
      else {
        AdjustDistanceMatrix();
      }
    }
    private void Weights_ToStringChanged(object sender, EventArgs e) {
      UpdateParameterValues();
    }
    private void DistancesParameter_ValueChanged(object sender, EventArgs e) {
      Distances.RowsChanged += Distances_RowsChanged;
      Distances.ColumnsChanged += Distances_ColumnsChanged;
      Distances.ToStringChanged += Distances_ToStringChanged;
      AdjustWeightsMatrix();
    }
    private void Distances_RowsChanged(object sender, EventArgs e) {
      if (Distances.Rows != Distances.Columns)
        ((IStringConvertibleMatrix)Distances).Columns = Distances.Rows;
      else {
        AdjustWeightsMatrix();
      }
    }
    private void Distances_ColumnsChanged(object sender, EventArgs e) {
      if (Distances.Rows != Distances.Columns)
        ((IStringConvertibleMatrix)Distances).Rows = Distances.Columns;
      else {
        AdjustWeightsMatrix();
      }
    }
    private void Distances_ToStringChanged(object sender, EventArgs e) {
      UpdateParameterValues();
    }
    #endregion

    private void RegisterEventHandlers() {
      WeightsParameter.ValueChanged += WeightsParameter_ValueChanged;
      Weights.RowsChanged += Weights_RowsChanged;
      Weights.ColumnsChanged += Weights_ColumnsChanged;
      Weights.ToStringChanged += Weights_ToStringChanged;
      DistancesParameter.ValueChanged += DistancesParameter_ValueChanged;
      Distances.RowsChanged += Distances_RowsChanged;
      Distances.ColumnsChanged += Distances_ColumnsChanged;
      Distances.ToStringChanged += Distances_ToStringChanged;
    }

    #region Helpers
    private void InitializeOperators() {
      var defaultOperators = new HashSet<IPermutationOperator>(new IPermutationOperator[] {
        new PartiallyMatchedCrossover(),
        new Swap2Manipulator(),
        new ExhaustiveSwap2MoveGenerator()
      });
      Operators.AddRange(defaultOperators);
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IQAPMoveEvaluator>());
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IQAPLocalImprovementOperator>());
      Operators.Add(new BestQAPSolutionAnalyzer());
      Operators.Add(new QAPAlleleFrequencyAnalyzer());

      Operators.Add(new HammingSimilarityCalculator());
      Operators.Add(new QAPSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));
      Parameterize();
    }
    private void Parameterize() {
      var operators = new List<IItem>();
      if (BestQAPSolutionAnalyzer != null) {
        operators.Add(BestQAPSolutionAnalyzer);
        BestQAPSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        BestQAPSolutionAnalyzer.DistancesParameter.ActualName = DistancesParameter.Name;
        BestQAPSolutionAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
        BestQAPSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestQAPSolutionAnalyzer.BestKnownSolutionsParameter.ActualName = BestKnownSolutionsParameter.Name;
        BestQAPSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      }
      if (QAPAlleleFrequencyAnalyzer != null) {
        operators.Add(QAPAlleleFrequencyAnalyzer);
        QAPAlleleFrequencyAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        QAPAlleleFrequencyAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        QAPAlleleFrequencyAnalyzer.DistancesParameter.ActualName = DistancesParameter.Name;
        QAPAlleleFrequencyAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        QAPAlleleFrequencyAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
      }
      if (QAPPopulationDiversityAnalyzer != null) {
        operators.Add(QAPPopulationDiversityAnalyzer);
        QAPPopulationDiversityAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        QAPPopulationDiversityAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      }
      foreach (var localOpt in Operators.OfType<IQAPLocalImprovementOperator>()) {
        operators.Add(localOpt);
        localOpt.DistancesParameter.ActualName = DistancesParameter.Name;
        localOpt.MaximizationParameter.ActualName = MaximizationParameter.Name;
        localOpt.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        localOpt.WeightsParameter.ActualName = WeightsParameter.Name;
      }

      foreach (var moveOp in Operators.OfType<IQAPMoveEvaluator>()) {
        operators.Add(moveOp);
        moveOp.DistancesParameter.ActualName = DistancesParameter.Name;
        moveOp.WeightsParameter.ActualName = WeightsParameter.Name;
        moveOp.QualityParameter.ActualName = Evaluator.QualityParameter.Name;

        var swaMoveOp = moveOp as QAPSwap2MoveEvaluator;
        if (swaMoveOp != null) {
          var moveQualityName = swaMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationSwap2MoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
        var invMoveOp = moveOp as QAPInversionMoveEvaluator;
        if (invMoveOp != null) {
          var moveQualityName = invMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationInversionMoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
        var traMoveOp = moveOp as QAPTranslocationMoveEvaluator;
        if (traMoveOp != null) {
          var moveQualityName = traMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationTranslocationMoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
        var scrMoveOp = moveOp as QAPScrambleMoveEvaluator;
        if (scrMoveOp != null) {
          var moveQualityName = scrMoveOp.MoveQualityParameter.ActualName;
          foreach (var o in Encoding.Operators.OfType<IPermutationScrambleMoveQualityOperator>())
            o.MoveQualityParameter.ActualName = moveQualityName;
        }
      }
      foreach (var similarityCalculator in Operators.OfType<ISolutionSimilarityCalculator>()) {
        similarityCalculator.SolutionVariableName = Encoding.Name;
        similarityCalculator.QualityVariableName = Evaluator.QualityParameter.ActualName;
        var qapsimcalc = similarityCalculator as QAPSimilarityCalculator;
        if (qapsimcalc != null) {
          qapsimcalc.Weights = Weights;
          qapsimcalc.Distances = Distances;
        }
      }

      if (operators.Count > 0) Encoding.ConfigureOperators(operators);
    }

    private void AdjustDistanceMatrix() {
      if (Distances.Rows != Weights.Rows || Distances.Columns != Weights.Columns) {
        ((IStringConvertibleMatrix)Distances).Rows = Weights.Rows;
        Encoding.Length = Weights.Rows;
      }
    }

    private void AdjustWeightsMatrix() {
      if (Weights.Rows != Distances.Rows || Weights.Columns != Distances.Columns) {
        ((IStringConvertibleMatrix)Weights).Rows = Distances.Rows;
        Encoding.Length = Distances.Rows;
      }
    }

    private void UpdateParameterValues() {
      Permutation lbSolution;
      // calculate the optimum of a LAP relaxation and use it as lower bound of our QAP
      LowerBound = new DoubleValue(GilmoreLawlerBoundCalculator.CalculateLowerBound(Weights, Distances, out lbSolution));
      // evalute the LAP optimal solution as if it was a QAP solution
      var lbSolutionQuality = Evaluate(lbSolution);
      // in case both qualities are the same it means that the LAP optimum is also a QAP optimum
      if (LowerBound.Value.IsAlmost(lbSolutionQuality)) {
        BestKnownSolution = lbSolution;
        BestKnownQuality = LowerBound.Value;
      }
      AverageQuality = new DoubleValue(ComputeAverageQuality());
    }

    private double ComputeAverageQuality() {
      double rt = 0, rd = 0, wt = 0, wd = 0;
      int n = Weights.Rows;
      for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++) {
          if (i == j) {
            rd += Distances[i, i];
            wd += Weights[i, i];
          } else {
            rt += Distances[i, j];
            wt += Weights[i, j];
          }
        }

      return rt * wt / (n * (n - 1)) + rd * wd / n;
    }
    #endregion

    public void Load(QAPData data) {
      var weights = new DoubleMatrix(data.Weights);
      var distances = new DoubleMatrix(data.Distances);
      Name = data.Name;
      Description = data.Description;
      Load(weights, distances);
      if (data.BestKnownQuality.HasValue) BestKnownQuality = data.BestKnownQuality.Value;
      EvaluateAndLoadAssignment(data.BestKnownAssignment);
      OnReset();
    }

    public void Load(TSPData data) {
      if (data.Dimension > 1000)
        throw new System.IO.InvalidDataException("Instances with more than 1000 customers are not supported by the QAP.");
      var weights = new DoubleMatrix(data.Dimension, data.Dimension);
      for (int i = 0; i < data.Dimension; i++)
        weights[i, (i + 1) % data.Dimension] = 1;
      var distances = new DoubleMatrix(data.GetDistanceMatrix());
      Name = data.Name;
      Description = data.Description;
      Load(weights, distances);
      if (data.BestKnownQuality.HasValue) BestKnownQuality = data.BestKnownQuality.Value;
      EvaluateAndLoadAssignment(data.BestKnownTour);
      OnReset();
    }

    public void Load(DoubleMatrix weights, DoubleMatrix distances) {
      if (weights == null || weights.Rows == 0)
        throw new System.IO.InvalidDataException("The given instance does not contain weights!");
      if (weights.Rows != weights.Columns)
        throw new System.IO.InvalidDataException("The weights matrix is not a square matrix!");
      if (distances == null || distances.Rows == 0)
        throw new System.IO.InvalidDataException("The given instance does not contain distances!");
      if (distances.Rows != distances.Columns)
        throw new System.IO.InvalidDataException("The distances matrix is not a square matrix!");
      if (weights.Rows != distances.Columns)
        throw new System.IO.InvalidDataException("The weights matrix and the distance matrix are not of equal size!");

      Weights = weights;
      Distances = distances;
      Encoding.Length = weights.Rows;

      BestKnownQualityParameter.Value = null;
      BestKnownSolution = null;
      BestKnownSolutions = null;
      UpdateParameterValues();
    }

    public void EvaluateAndLoadAssignment(int[] assignment) {
      if (assignment == null || assignment.Length == 0) return;
      var vector = new Permutation(PermutationTypes.Absolute, assignment);
      var result = Evaluate(vector);
      BestKnownQuality = result;
      BestKnownSolution = vector;
      BestKnownSolutions = new ItemSet<Permutation> { (Permutation)vector.Clone() };
    }
  }
}
