﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2017 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;

namespace HeuristicLab.Encodings.IntegerVectorEncoding {
  public class IntegerVectorEqualityComparer : EqualityComparer<IntegerVector> {
    public override bool Equals(IntegerVector x, IntegerVector y) {
      if (x == null && y == null) return true;
      if (x == null || y == null) return false;
      if (x.Length != y.Length) return false;
      for (var i = 0; i < x.Length; i++)
        if (x[i] != y[i]) return false;
      return true;
    }

    public override int GetHashCode(IntegerVector obj) {
      unchecked {
        int hash = 17;
        foreach (var o in obj)
          hash = hash * 31 + o.GetHashCode();
        return hash;
      }
    }
  }
}
