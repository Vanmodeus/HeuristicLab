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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HeuristicLab.PluginInfrastructure.Manager;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal partial class InstallationManagerForm : Form, IStatusView {
    private InstallationManager installationManager;
    private PluginManager pluginManager;
    private string pluginDir;

    public InstallationManagerForm(PluginManager pluginManager) {
      InitializeComponent();
      this.pluginManager = pluginManager;

      pluginManager.PluginLoaded += pluginManager_PluginLoaded;
      pluginManager.PluginUnloaded += pluginManager_PluginUnloaded;
      pluginManager.Initializing += pluginManager_Initializing;
      pluginManager.Initialized += pluginManager_Initialized;

      pluginDir = Application.StartupPath;

      installationManager = new InstallationManager(pluginDir);
      installationManager.PluginInstalled += new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginInstalled);
      installationManager.PluginRemoved += new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginRemoved);
      installationManager.PluginUpdated += new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginUpdated);
      installationManager.PreInstallPlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreInstallPlugin);
      installationManager.PreRemovePlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreRemovePlugin);
      installationManager.PreUpdatePlugin += new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreUpdatePlugin);

      // show or hide controls for uploading plugins based on setting
      if (!HeuristicLab.PluginInfrastructure.Properties.Settings.Default.ShowPluginUploadControls) {
        tabControl.Controls.Remove(uploadPluginsTabPage);
        tabControl.Controls.Remove(manageProductsTabPage);
      } else {
        pluginEditor.PluginManager = pluginManager;
      }

      localPluginsView.StatusView = this;
      localPluginsView.PluginManager = pluginManager;
      localPluginsView.InstallationManager = installationManager;

      basicUpdateView.StatusView = this;
      basicUpdateView.PluginManager = pluginManager;
      basicUpdateView.InstallationManager = installationManager;

      remotePluginInstaller.StatusView = this;
      remotePluginInstaller.InstallationManager = installationManager;
      remotePluginInstaller.PluginManager = pluginManager;
    }


    #region plugin manager event handlers
    void pluginManager_Initialized(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Initialized PluginInfrastructure");
    }

    void pluginManager_Initializing(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Initializing PluginInfrastructure");
    }

    void pluginManager_PluginUnloaded(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Unloaded " + e.Entity);
    }

    void pluginManager_PluginLoaded(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Loaded " + e.Entity);
    }
    #endregion

    #region installation manager event handlers
    void installationManager_PreUpdatePlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      if (e.Plugins.Count() > 0) {
        e.Cancel = ConfirmUpdateAction(e.Plugins) == false;
      }
    }

    void installationManager_PreRemovePlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      if (e.Plugins.Count() > 0) {
        e.Cancel = ConfirmRemoveAction(e.Plugins) == false;
      }
    }

    void installationManager_PreInstallPlugin(object sender, PluginInfrastructureCancelEventArgs e) {
      if (e.Plugins.Count() > 0)
        if (ConfirmInstallAction(e.Plugins) == true) {
          SetStatusStrip("Installing " + e.Plugins.Aggregate("", (a, b) => a.ToString() + "; " + b.ToString()));
          e.Cancel = false;
        } else {
          e.Cancel = true;
          SetStatusStrip("Install canceled");
        }
    }

    void installationManager_PluginUpdated(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Updated " + e.Entity);
    }

    void installationManager_PluginRemoved(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Removed " + e.Entity);
    }

    void installationManager_PluginInstalled(object sender, PluginInfrastructureEventArgs e) {
      SetStatusStrip("Installed " + e.Entity);
    }
    #endregion

    #region button events
    private void connectionSettingsToolStripMenuItem_Click(object sender, EventArgs e) {
      new ConnectionSetupView().ShowDialog();
    }

    private void tabControl_Selected(object sender, TabControlEventArgs e) {
      viewToolStripMenuItem.Enabled = e.TabPage == availablePluginsTabPage;      
      toolStripStatusLabel.Text = string.Empty;
      toolStripProgressBar.Visible = false;
    }

    private void simpleToolStripMenuItem_Click(object sender, EventArgs e) {
      remotePluginInstaller.ShowAllPlugins = false;
      advancedToolStripMenuItem.Checked = false;
    }

    private void advancedToolStripMenuItem_Click(object sender, EventArgs e) {
      remotePluginInstaller.ShowAllPlugins = true;
      simpleToolStripMenuItem.Checked = false;
    }
    #endregion

    #region confirmation dialogs
    private bool ConfirmRemoveAction(IEnumerable<IPluginDescription> plugins) {
      StringBuilder strBuilder = new StringBuilder();
      foreach (var plugin in plugins) {
        foreach (var file in plugin.Files) {
          strBuilder.AppendLine(Path.GetFileName(file.Name));
        }
      }
      return (new ConfirmationDialog("Confirm Delete", "Do you want to delete following files?", strBuilder.ToString())).ShowDialog() == DialogResult.OK;
    }

    private bool ConfirmUpdateAction(IEnumerable<IPluginDescription> plugins) {
      StringBuilder strBuilder = new StringBuilder();
      foreach (var plugin in plugins) {
        strBuilder.AppendLine(plugin.ToString());
      }
      return (new ConfirmationDialog("Confirm Update", "Do you want to update following plugins?", strBuilder.ToString())).ShowDialog() == DialogResult.OK;
    }

    private bool ConfirmInstallAction(IEnumerable<IPluginDescription> plugins) {
      foreach (var plugin in plugins) {
        if (!string.IsNullOrEmpty(plugin.LicenseText)) {
          var licenseConfirmationBox = new LicenseConfirmationBox(plugin);
          if (licenseConfirmationBox.ShowDialog() != DialogResult.OK)
            return false;
        }
      }
      return true;
    }


    #endregion

    #region helper methods
    private void SetStatusStrip(string msg) {
      if (InvokeRequired) Invoke((Action<string>)SetStatusStrip, msg);
      else {
        toolStripStatusLabel.Text = msg;
        logTextBox.Text += DateTime.Now + ": " + msg + Environment.NewLine;
      }
    }

    #endregion


    protected override void OnClosing(CancelEventArgs e) {
      installationManager.PluginInstalled -= new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginInstalled);
      installationManager.PluginRemoved -= new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginRemoved);
      installationManager.PluginUpdated -= new EventHandler<PluginInfrastructureEventArgs>(installationManager_PluginUpdated);
      installationManager.PreInstallPlugin -= new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreInstallPlugin);
      installationManager.PreRemovePlugin -= new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreRemovePlugin);
      installationManager.PreUpdatePlugin -= new EventHandler<PluginInfrastructureCancelEventArgs>(installationManager_PreUpdatePlugin);
      base.OnClosing(e);
    }

    #region IStatusView Members

    public void ShowProgressIndicator(double percentProgress) {
      if (percentProgress < 0.0 || percentProgress > 1.0) throw new ArgumentException();
      toolStripProgressBar.Visible = true;
      toolStripProgressBar.Style = ProgressBarStyle.Continuous;
      int range = toolStripProgressBar.Maximum - toolStripProgressBar.Minimum;
      toolStripProgressBar.Value = (int)(percentProgress * range + toolStripProgressBar.Minimum);
    }

    public void ShowProgressIndicator() {
      toolStripProgressBar.Visible = true;
      toolStripProgressBar.Style = ProgressBarStyle.Marquee;
    }

    public void HideProgressIndicator() {
      toolStripProgressBar.Visible = false;
    }

    public void ShowMessage(string message) {
      if (toolStripStatusLabel.Text == string.Empty)
        toolStripStatusLabel.Text = message;
      else
        toolStripStatusLabel.Text += "; " + message;
    }

    public void RemoveMessage(string message) {
      if (toolStripStatusLabel.Text.IndexOf("; " + message) > 0) {
        toolStripStatusLabel.Text = toolStripStatusLabel.Text.Replace("; " + message, "");
      }
      toolStripStatusLabel.Text = toolStripStatusLabel.Text.Replace(message, "");
      toolStripStatusLabel.Text = toolStripStatusLabel.Text.TrimStart(' ', ';');
    }
    public void LockUI() {
      Cursor = Cursors.AppStarting;
      tabControl.Enabled = false;
    }
    public void UnlockUI() {
      tabControl.Enabled = true;
      Cursor = Cursors.Default;
    }
    public void ShowError(string shortMessage, string description) {
      logTextBox.Text += DateTime.Now + ": " + shortMessage + Environment.NewLine + description + Environment.NewLine;
      MessageBox.Show(description, shortMessage);
    }
    #endregion
  }
}
