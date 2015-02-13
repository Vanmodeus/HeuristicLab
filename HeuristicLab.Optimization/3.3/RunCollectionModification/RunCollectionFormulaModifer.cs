﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {

  [Item("RunCollection Formula Modifier", "Modifies a RunCollection by adding results using the given formula.")]
  [StorableClass]
  public class RunCollectionFormulaModifer : ParameterizedNamedItem, IRunCollectionModifier {

    public override bool CanChangeName { get { return false; } }
    public override bool CanChangeDescription { get { return false; } }

    public ValueParameter<StringValue> ResultNameParameter {
      get { return (ValueParameter<StringValue>)Parameters["ResultName"]; }
    }

    public ValueParameter<StringValue> FormulaParameter {
      get { return (ValueParameter<StringValue>)Parameters["Formula"]; }
    }

    private string ResultName { get { return ResultNameParameter.Value.Value; } }
    private string Formula { get { return FormulaParameter.Value.Value; } }

    #region Construction & Cloning
    [StorableConstructor]
    protected RunCollectionFormulaModifer(bool deserializing) : base(deserializing) { }
    protected RunCollectionFormulaModifer(RunCollectionFormulaModifer original, Cloner cloner)
      : base(original, cloner) {
      RegisterEvents();
    }
    public RunCollectionFormulaModifer() {
      Parameters.Add(new ValueParameter<StringValue>("ResultName", "The name of the result to be generated by this formula.", new StringValue("Calc.Value")));
      Parameters.Add(new ValueParameter<StringValue>("Formula",
@"RPN formula for new value in postfix notation.

This can contain the following elements:

literals:
  numbers, true, false, null and strings in single quotes
variables (run parameters or results):
  unquoted or in double quotes if they contain special characters or whitespace
mathematical functions:
  +, -, /, ^ (power), log
predicates:
  ==, <, >, isnull, not
stack manipulation:
  drop swap dup
string matching:
  <string> <pattern> ismatch
string replacing:
  <string> <pattern> <replacement> rename
conditionals:
  <then> <else> <condition> if

If the final value is null, the result variable is removed if it exists.",
        new StringValue("1 1 +")));
      UpdateName();
      RegisterEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RunCollectionFormulaModifer(this, cloner);
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEvents();
    }
    #endregion

    private void RegisterEvents() {
      ResultNameParameter.ToStringChanged += Parameter_NameChanged;
      FormulaParameter.ToStringChanged += Parameter_NameChanged;
    }

    void Parameter_NameChanged(object sender, EventArgs e) {
      UpdateName();
    }

    private void UpdateName() {
      name = string.Format("{0} := {1}", ResultName, Formula);
      OnNameChanged();
    }

    public void Modify(List<IRun> runs) {
      var calc = new Calculator { Formula = Formula };
      foreach (var run in runs) {
        var variables = new Dictionary<string, IItem>();
        foreach (var param in run.Parameters)
          variables[param.Key] = param.Value;
        foreach (var result in run.Results)
          variables[result.Key] = result.Value;
        try {
          var value = calc.GetValue(variables);
          if (value != null)
            run.Results[ResultName] = value;
          else
            run.Results.Remove(ResultName);
        } catch (Exception x) {
          throw new Exception(string.Format("Calculation failed at Run {0}", run.Name), x);
        }
      }
    }

  }
}
