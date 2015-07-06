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

using HeuristicLab.MainForm;

namespace HeuristicLab.Optimization.Views {
  [View("Basic Algorithm View")]
  [Content(typeof(BasicAlgorithm), true)]
  public partial class BasicAlgorithmView : AlgorithmView {
    public BasicAlgorithmView() {
      InitializeComponent();
    }
    public new BasicAlgorithm Content {
      get { return (BasicAlgorithm)base.Content; }
      set { base.Content = value; }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      pauseButton.Enabled = false;
    }

    protected override void SetEnabledStateOfExecutableButtons() {
      base.SetEnabledStateOfExecutableButtons();
      pauseButton.Enabled = false;
    }
  }
}