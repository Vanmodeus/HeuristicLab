﻿namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  partial class JsonItemValueControl {
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
      this.components = new System.ComponentModel.Container();
      this.textBoxValue = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.numericRangeControl1 = new HeuristicLab.JsonInterface.OptimizerIntegration.NumericRangeControl();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // textBoxValue
      // 
      this.textBoxValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxValue.Location = new System.Drawing.Point(89, 1);
      this.textBoxValue.Margin = new System.Windows.Forms.Padding(0);
      this.textBoxValue.Name = "textBoxValue";
      this.textBoxValue.Size = new System.Drawing.Size(408, 20);
      this.textBoxValue.TabIndex = 14;
      this.textBoxValue.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxValue_Validating);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 1);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(34, 13);
      this.label2.TabIndex = 15;
      this.label2.Text = "Value";
      // 
      // numericRangeControl1
      // 
      this.numericRangeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericRangeControl1.Location = new System.Drawing.Point(0, 24);
      this.numericRangeControl1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.numericRangeControl1.Name = "numericRangeControl1";
      this.numericRangeControl1.Size = new System.Drawing.Size(497, 72);
      this.numericRangeControl1.TabIndex = 16;
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // JsonItemValueControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.numericRangeControl1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.textBoxValue);
      this.errorProvider.SetIconAlignment(this, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.Name = "JsonItemValueControl";
      this.Size = new System.Drawing.Size(500, 97);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textBoxValue;
    private System.Windows.Forms.Label label2;
    private NumericRangeControl numericRangeControl1;
    private System.Windows.Forms.ErrorProvider errorProvider;
  }
}
