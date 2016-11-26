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
using System.Linq;
using HeuristicLab.MainForm;


namespace HeuristicLab.Problems.DataAnalysis.Views {
  [View("Line Chart (prognosis)")]
  [Content(typeof(ITimeSeriesPrognosisSolution))]
  public partial class TimeSeriesPrognosisLineChartView : RegressionSolutionLineChartView {


    public new ITimeSeriesPrognosisSolution Content {
      get { return (ITimeSeriesPrognosisSolution)base.Content; }
      set { base.Content = value; }
    }

    public TimeSeriesPrognosisLineChartView()
      : base() {
      InitializeComponent();

    }

    protected override void GetTestSeries(out int[] x, out double[] y) {
      // treat the whole test partition as prognosis horizon
      x = Content.ProblemData.TestIndices.ToArray();
      y = Content.PrognosedTestValues.ToArray();
    }

    protected override void GetAllValuesSeries(out int[] x, out double[] y) {
      // not supported
      x = new int[0];
      y = new double[0];
    }
  }
}
