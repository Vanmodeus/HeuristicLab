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
using HeuristicLab.Data;

namespace HeuristicLab.Logging {
  /// <summary>
  /// Visual representation of the <see cref="Log"/> class.
  /// </summary>
  public partial class LogView : ViewBase {
    /// <summary>
    /// Gets or sets the Log item to represent visually.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public Log Log {
      get { return (Log)base.Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LogView"/>.
    /// </summary>
    public LogView() {
      InitializeComponent();
      Caption = "Log View";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="LogView"/> with the given <paramref name="log"/>.
    /// </summary>
    /// <param name="log">The log object to represent visually.</param>
    public LogView(Log log)
      : this() {
      Log = log;
    }

    /// <summary>
    /// Removes the event handlers from the underlying <see cref="Log"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      if (Log != null) {
        Log.Items.ItemAdded -= new EventHandler<ItemIndexEventArgs>(Items_ItemAdded);
        Log.Items.ItemRemoved -= new EventHandler<ItemIndexEventArgs>(Items_ItemRemoved);
      }
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds event handlers to the underlying <see cref="Log"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      if (Log != null) {
        Log.Items.ItemAdded += new EventHandler<ItemIndexEventArgs>(Items_ItemAdded);
        Log.Items.ItemRemoved += new EventHandler<ItemIndexEventArgs>(Items_ItemRemoved);
      }
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class 
    /// <see cref="UpdateControls"/>.</remarks>
    protected override void UpdateControls() {
      base.UpdateControls();
      qualityLogTextBox.Clear();
      if (Log == null) {
        qualityLogTextBox.Enabled = false;
      } else {
        string[] lines = new string[Log.Items.Count];
        for (int i = 0; i < Log.Items.Count; i++) {
          lines[i] = Log.Items[i].ToString().Replace(';','\t');
        }
        qualityLogTextBox.Lines = lines;
        qualityLogTextBox.Enabled = true;
      }
    }

    #region ItemList Events
    private delegate void IndexDelegate(int index);
    private void Items_ItemRemoved(object sender, ItemIndexEventArgs e) {
      RemoveItem(e.Index);
    }
    private void RemoveItem(int index) {
      if (InvokeRequired)
        Invoke(new IndexDelegate(RemoveItem), index);
      else {
        string[] lines = new string[qualityLogTextBox.Lines.Length - 1];
        Array.Copy(qualityLogTextBox.Lines, 0, lines, 0, index);
        Array.Copy(qualityLogTextBox.Lines, index + 1, lines, index, lines.Length - index);
        qualityLogTextBox.Lines = lines;
      }
    }
    private void Items_ItemAdded(object sender, ItemIndexEventArgs e) {
      AddItem(e.Index);
    }
    private void AddItem(int index) {
      if (InvokeRequired)
        Invoke(new IndexDelegate(AddItem), index);
      else {
        string[] lines = new string[qualityLogTextBox.Lines.Length + 1];
        Array.Copy(qualityLogTextBox.Lines, 0, lines, 0, index);
        Array.Copy(qualityLogTextBox.Lines, index, lines, index + 1, qualityLogTextBox.Lines.Length - index);
        lines[index] = Log.Items[index].ToString().Replace(';', '\t');
        qualityLogTextBox.Lines = lines;
      }
    }
    #endregion
  }
}
