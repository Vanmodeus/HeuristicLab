﻿namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionEqualityConstraintView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.txtConstraintData = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // txtConstraintData
      // 
      this.txtConstraintData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtConstraintData.Location = new System.Drawing.Point(127, 56);
      this.txtConstraintData.Name = "txtConstraintData";
      this.txtConstraintData.Size = new System.Drawing.Size(246, 20);
      this.txtConstraintData.TabIndex = 9;
      this.txtConstraintData.TextChanged += new System.EventHandler(this.txtConstraintData_TextChanged);
      // 
      // RunCollectionEqualityConstraintView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.txtConstraintData);
      this.Name = "RunCollectionEqualityConstraintView";
      this.Controls.SetChildIndex(this.txtConstraintData, 0);
      this.Controls.SetChildIndex(this.cmbConstraintOperation, 0);
      this.Controls.SetChildIndex(this.cmbConstraintColumn, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtConstraintData;
  }
}
