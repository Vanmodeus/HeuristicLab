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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;

namespace HeuristicLab.Netron {
  [ToolboxItem(true)]
  public partial class NetronVisualization : DiagramControlBase {
    private static Size INVALID_SIZE = new Size(-1, -1);
    private Size oldSize;
    public NetronVisualization()
      : base() {
      InitializeComponent();
      this.oldSize = INVALID_SIZE;

      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.ResizeRedraw, true);

      if (!DesignMode) {
        this.Controller = new Controller(this);
        this.Document = new Document();
        this.Document.Model.Selection = new Selection(this.Controller,this.Document.Model);
        this.View = new View(this);
        this.AttachToDocument(Document);
        this.Controller.View = View;
        TextEditor.Init(this);

        View.OnCursorChange += new EventHandler<CursorEventArgs>(mView_OnCursorChange);
        View.OnBackColorChange += new EventHandler<ColorEventArgs>(View_OnBackColorChange);

        this.SizeChanged += new EventHandler(NetronVisualization_SizeChanged);
        this.AllowDrop = true;
      }
    }

    private void NetronVisualization_SizeChanged(object sender, EventArgs e) {
      //if (this.oldSize == INVALID_SIZE) {
        this.View.PageSize = new Size((int)(this.Size.Width * this.Magnification.Width), (int)(this.Size.Height * this.Magnification.Height));
      //  if (!this.DesignMode)
      //    oldSize = this.Size;
      //  return;
      //}

      //SizeF magnificationChanges = new SizeF();
      //magnificationChanges.Width = ((float)this.Size.Width) / oldSize.Width;
      //magnificationChanges.Height = ((float)this.Size.Height) / oldSize.Height;

      //SizeF newMagnification = new SizeF();
      //newMagnification.Width = this.View.Magnification.Width * magnificationChanges.Width;
      //newMagnification.Height = this.View.Magnification.Height * magnificationChanges.Height;

      //this.Magnification = newMagnification;
      //this.oldSize = this.Size;
    }

    protected override void OnScroll(ScrollEventArgs se) {
      //base.OnScroll(se);
      if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll) {
        Origin = new Point(se.NewValue, Origin.Y);
        //System.Diagnostics.Trace.WriteLine(se.NewValue);
      } else {
        Origin = new Point(Origin.X, se.NewValue);
        //System.Diagnostics.Trace.WriteLine(se.NewValue);
      }
    }

    private void mView_OnCursorChange(object sender, CursorEventArgs e) {
      this.Cursor = e.Cursor;
    }
  }
}
