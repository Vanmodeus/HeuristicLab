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
using System.Text;
using System.Xml;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  /// <summary>
  /// An abstract base class for all kinds of arrays.
  /// </summary>
  public abstract class ArrayDataBase : ObjectData {
    /// <summary>
    /// Gets or sets the elements of the array.
    /// </summary>
    /// <remarks>Uses property <see cref="ObjectData.Data"/> of base class <see cref="ObjectData"/>. 
    /// No own data storage present.</remarks>
    [Storable]
    public new virtual Array Data {
      get { return (Array)base.Data; }
      set { base.Data = value; }
    }

    /// <summary>
    /// The string representation of the array.
    /// </summary>
    /// <returns>The elements of the array as a string seperated by a semicolon.</returns>
    public override string ToString() {
      if (Data.Length <= 0) return "Empty Array";
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < Data.Length; i++) {
        builder.Append(";");
        builder.Append(Data.GetValue(i).ToString());
      }
      if (builder.Length > 0) builder.Remove(0, 1);
      return builder.ToString();
    }
  }
}
