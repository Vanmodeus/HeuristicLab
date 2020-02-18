﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.Optimization;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.JsonInterface.OptimizerIntegration {
  internal static class FileManager {
    private static OpenFileDialog openFileDialog;
    private static ExportJsonDialog exportDialog = new ExportJsonDialog();

    public static void ExportJsonTemplate() {
      IContentView activeView = MainFormManager.MainForm.ActiveView as IContentView;
      if (activeView != null) {
        ExportJsonTemplate(activeView);
      }
    }
    public static void ExportJsonTemplate(IContentView view) {
      // TODO: view to select free params, warning if no results are generated
      IStorableContent content = view.Content as IStorableContent;
      if (!view.Locked && content != null) {
        exportDialog.Content = content;
        exportDialog.ShowDialog();
      }
    }


    public static void ImportJsonTemplate() {
      if (openFileDialog == null) {
        openFileDialog = new OpenFileDialog();
        openFileDialog.Title = "Import .json-Template";
        openFileDialog.FileName = "Item";
        openFileDialog.Multiselect = false;
        openFileDialog.DefaultExt = "json";
        openFileDialog.Filter = ".json-Template|*.json|All Files|*.*";
      }

      if (openFileDialog.ShowDialog() == DialogResult.OK) {
        try {
          var content = JsonTemplateInstantiator.Instantiate(openFileDialog.FileName);
          IView view = MainFormManager.MainForm.ShowContent(content);
          if (view == null)
            ErrorHandling.ShowErrorDialog("There is no view for the loaded item. It cannot be displayed.", new InvalidOperationException("No View Available"));
        } catch (Exception ex) {
          ErrorHandling.ShowErrorDialog((Control)MainFormManager.MainForm, "Cannot open file.", ex);
        } finally {
          ((MainForm.WindowsForms.MainForm)MainFormManager.MainForm).ResetAppStartingCursor();
        }
      }
    }
  }
}