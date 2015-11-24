﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  public abstract class SingleObjectiveProblem<TEncoding, TSolution> : Problem<TEncoding, TSolution, SingleObjectiveEvaluator<TSolution>>, ISingleObjectiveProblem<TEncoding, TSolution>
    where TEncoding : class, IEncoding<TSolution>
    where TSolution : class, ISolution {

    protected IValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (IValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }

    public double BestKnownQuality {
      get {
        if (BestKnownQualityParameter.Value == null) return double.NaN;
        return BestKnownQualityParameter.Value.Value;
      }
      set {
        if (BestKnownQualityParameter.Value == null) BestKnownQualityParameter.Value = new DoubleValue(value);
        else BestKnownQualityParameter.Value.Value = value;
      }
    }

    [StorableConstructor]
    protected SingleObjectiveProblem(bool deserializing) : base(deserializing) { }

    protected SingleObjectiveProblem(SingleObjectiveProblem<TEncoding, TSolution> original, Cloner cloner)
      : base(original, cloner) {
      ParameterizeOperators();
    }

    protected SingleObjectiveProblem()
      : base() {
      Parameters.Add(new FixedValueParameter<BoolValue>("Maximization", "Set to false if the problem should be minimized.", (BoolValue)new BoolValue(Maximization).AsReadOnly()) { Hidden = true });
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this problem."));

      Operators.Add(Evaluator);
      Operators.Add(new SingleObjectiveAnalyzer<TSolution>());
      Operators.Add(new SingleObjectiveImprover<TSolution>());
      Operators.Add(new SingleObjectiveMoveEvaluator<TSolution>());
      Operators.Add(new SingleObjectiveMoveGenerator<TSolution>());
      Operators.Add(new SingleObjectiveMoveMaker<TSolution>());

      ParameterizeOperators();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      ParameterizeOperators();
    }

    public abstract bool Maximization { get; }
    public abstract double Evaluate(TSolution individual, IRandom random);
    public virtual void Analyze(TSolution[] individuals, double[] qualities, ResultCollection results, IRandom random) { }
    public virtual IEnumerable<TSolution> GetNeighbors(TSolution individual, IRandom random) {
      return Enumerable.Empty<TSolution>();
    }

    public virtual bool IsBetter(double quality, double bestQuality) {
      return (Maximization && quality > bestQuality || !Maximization && quality < bestQuality);
    }

    //TODO
    //protected override void OnOperatorsChanged() {
    //  base.OnOperatorsChanged();
    //  if (Encoding != null) {
    //    PruneMultiObjectiveOperators(Encoding);
    //    var multiEncoding = Encoding as MultiEncoding;
    //    if (multiEncoding != null) {
    //      foreach (var encoding in multiEncoding.Encodings.ToList()) {
    //        PruneMultiObjectiveOperators(encoding);
    //      }
    //    }
    //  }
    //}

    //private void PruneMultiObjectiveOperators(IEncoding<TSolution> encoding) {
    //  if (encoding.Operators.Any(x => x is IMultiObjectiveOperator && !(x is ISingleObjectiveOperator)))
    //    encoding.Operators = encoding.Operators.Where(x => !(x is IMultiObjectiveOperator) || x is ISingleObjectiveOperator).ToList();
    //}

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<ISingleObjectiveEvaluationOperator<TSolution>>())
        op.EvaluateFunc = Evaluate;
      foreach (var op in Operators.OfType<ISingleObjectiveAnalysisOperator<TSolution>>())
        op.AnalyzeAction = Analyze;
      foreach (var op in Operators.OfType<INeighborBasedOperator<TSolution>>())
        op.GetNeighborsFunc = GetNeighbors;
    }

    #region ISingleObjectiveHeuristicOptimizationProblem Members
    IParameter ISingleObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.BestKnownQualityParameter {
      get { return Parameters["BestKnownQuality"]; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return Evaluator; }
    }
    #endregion
  }
}