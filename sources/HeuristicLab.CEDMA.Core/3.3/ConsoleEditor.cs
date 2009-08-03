﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using System.Windows.Forms;
using System.ServiceModel;
using HeuristicLab.PluginInfrastructure;
using System.Drawing;

namespace HeuristicLab.CEDMA.Core {
  class ConsoleEditor : EditorBase {
    private ComboBox viewComboBox;
    private Button resultsButton;
    private Console console;

    public ConsoleEditor(Console console) {
      InitializeComponent();
      this.console = console;
      PopulateViewComboBox();
    }

    #region autogenerated code
    private void InitializeComponent() {
      this.viewComboBox = new System.Windows.Forms.ComboBox();
      this.resultsButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // viewComboBox
      // 
      this.viewComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.viewComboBox.FormattingEnabled = true;
      this.viewComboBox.Location = new System.Drawing.Point(3, 3);
      this.viewComboBox.Name = "viewComboBox";
      this.viewComboBox.Size = new System.Drawing.Size(121, 21);
      this.viewComboBox.TabIndex = 7;
      // 
      // resultsButton
      // 
      this.resultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resultsButton.Location = new System.Drawing.Point(130, 1);
      this.resultsButton.Name = "resultsButton";
      this.resultsButton.Size = new System.Drawing.Size(86, 23);
      this.resultsButton.TabIndex = 6;
      this.resultsButton.Text = "Show results";
      this.resultsButton.UseVisualStyleBackColor = true;
      this.resultsButton.Click += new System.EventHandler(this.resultsButton_Click);
      // 
      // ConsoleEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.viewComboBox);
      this.Controls.Add(this.resultsButton);
      this.Name = "ConsoleEditor";
      this.Size = new System.Drawing.Size(445, 189);
      this.ResumeLayout(false);

    }

    #endregion

    private void PopulateViewComboBox() {
      DiscoveryService service = new DiscoveryService();
      IResultsViewFactory[] factories = service.GetInstances<IResultsViewFactory>();
      viewComboBox.DataSource = factories;
      viewComboBox.ValueMember = "Name";
    }


    private void resultsButton_Click(object sender, EventArgs e) {
      IResultsViewFactory factory = (IResultsViewFactory)viewComboBox.SelectedItem;
      IControl control = factory.CreateView(console.Results);
      PluginManager.ControlManager.ShowControl(control);
    }
  }
}
