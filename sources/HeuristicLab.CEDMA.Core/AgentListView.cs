#region License Information
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.CEDMA.Core {
  public partial class AgentListView : ViewBase {
    public IAgentList AgentList {
      get { return (IAgentList)Item; }
      set { base.Item = value; }
    }

    public AgentListView() {
      InitializeComponent();
      Caption = "Agent View";
    }
    public AgentListView(IAgentList agentList)
      : this() {
      AgentList = agentList;
      agentList.Changed += new EventHandler(agentList_Changed);
    }

    void agentList_Changed(object sender, EventArgs e) {
      UpdateControls();
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      if(AgentList == null) {
        Caption = "Agents View";
        agentTreeView.Enabled = false;
      } else {
        agentTreeView.Enabled = true;
        agentTreeView.Nodes.Clear();
        foreach(IAgent agent in AgentList) {
          TreeNode node = new TreeNode();
          node.Text = agent.Name;
          node.Tag = agent;
          node.Nodes.Add("dummy");
          agentTreeView.Nodes.Add(node);
        }
      }
    }

    #region Button Events
    private void addButton_Click(object sender, EventArgs e) {
      AgentList.CreateAgent();
      UpdateControls();
    }
    #endregion

    private void agentTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
      e.Node.Nodes.Clear();
      IAgent agent = (IAgent)e.Node.Tag;
      foreach(IAgent subAgent in agent.SubAgents) {
        TreeNode node = new TreeNode();
        node.Text = subAgent.Name;
        node.Tag = subAgent;
        node.Nodes.Add("dummy");
        e.Node.Nodes.Add(node);
      }
      foreach(IResult result in agent.Results) {
        TreeNode node = new TreeNode();
        node.Text = result.Summary;
        node.Tag = result;
        node.Nodes.Add("dummy");
        e.Node.Nodes.Add(node);
      }
    }

    private void agentTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if(detailsGroupBox.Controls.Count > 0)
        detailsGroupBox.Controls[0].Dispose();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      if(agentTreeView.SelectedNode != null) {
        IViewable viewable = (IViewable)agentTreeView.SelectedNode.Tag;
        Control control = (Control)viewable.CreateView();
        detailsGroupBox.Controls.Add(control);
        control.Dock = DockStyle.Fill;
        detailsGroupBox.Enabled = true;
      }
    }
  }
}

