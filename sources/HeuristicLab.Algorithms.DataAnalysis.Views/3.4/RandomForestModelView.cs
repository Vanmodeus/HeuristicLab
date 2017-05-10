﻿#region License Information
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

using System.Drawing;
using HeuristicLab.MainForm;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Classification;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.DataAnalysis.Views;

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  [View("Random forest model")]
  [Content(typeof(IRandomForestRegressionSolution), false)]
  [Content(typeof(IRandomForestClassificationSolution), false)]
  public partial class RandomForestModelView : DataAnalysisSolutionEvaluationView {
    public override Image ViewImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Function; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      listBox.Enabled = Content != null;
      viewHost.Enabled = Content != null;
    }

    public RandomForestModelView()
      : base() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        viewHost.Content = null;
        listBox.Items.Clear();
      } else {
        viewHost.Content = null;
        listBox.Items.Clear();
        var classSol = Content as IRandomForestClassificationSolution;
        var regSol = Content as IRandomForestRegressionSolution;
        var numTrees = classSol != null ? classSol.NumberOfTrees : regSol != null ? regSol.NumberOfTrees : 0;
        for (int i = 0; i < numTrees; i++) {
          listBox.Items.Add(i + 1);
        }
      }
    }

    private void listBox_SelectedIndexChanged(object sender, System.EventArgs e) {
      if (listBox.SelectedItem == null) viewHost.Content = null;
      else {
        var idx = (int)listBox.SelectedItem;
        idx -= 1;
        var rfModel = Content.Model as RandomForestModel;
        var regProblemData = Content.ProblemData as IRegressionProblemData;
        var classProblemData = Content.ProblemData as IClassificationProblemData;
        if (rfModel != null) {
          if (idx < 0 || idx >= rfModel.NumberOfTrees) return;
          if (regProblemData != null) {
            var syModel = new SymbolicRegressionModel(regProblemData.TargetVariable, rfModel.ExtractTree(idx),
              new SymbolicDataAnalysisExpressionTreeLinearInterpreter());
            viewHost.Content = syModel.CreateRegressionSolution(regProblemData);
          } else if (classProblemData != null) {
            var syModel = new SymbolicDiscriminantFunctionClassificationModel(classProblemData.TargetVariable, rfModel.ExtractTree(idx),
              new SymbolicDataAnalysisExpressionTreeLinearInterpreter(), new NormalDistributionCutPointsThresholdCalculator());
            syModel.RecalculateModelParameters(classProblemData, classProblemData.TrainingIndices);
            viewHost.Content = syModel.CreateClassificationSolution(classProblemData);
          }
        }
      }
    }
  }
}