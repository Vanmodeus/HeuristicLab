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
using HeuristicLab.Core;

namespace HeuristicLab.BitVector {
  /// <summary>
  /// Single point crossover for bit vectors.
  /// </summary>
  public class SinglePointCrossover : BitVectorCrossoverBase {
    /// <inheritdoc select="summary"/>
    public override string Description {
      get { return "Single point crossover for bit vectors."; }
    }

    /// <summary>
    /// Performs a single point crossover at a randomly chosen position of the two 
    /// given parent bit vectors.
    /// </summary>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created bit vector, resulting from the single point crossover.</returns>
    public static bool[] Apply(IRandom random, bool[] parent1, bool[] parent2) {
      int length = parent1.Length;
      bool[] result = new bool[length];
      int breakPoint = random.Next(1, length);

      for (int i = 0; i < breakPoint; i++)
        result[i] = parent1[i];
      for (int i = breakPoint; i < length; i++)
        result[i] = parent2[i];

      return result;
    }

    /// <summary>
    /// Performs a single point crossover at a randomly chosen position of the two 
    /// given parent bit vectors.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="random">A random number generator.</param>
    /// <param name="parent1">The first parent for crossover.</param>
    /// <param name="parent2">The second parent for crossover.</param>
    /// <returns>The newly created bit vector, resulting from the single point crossover.</returns>
    protected override bool[] Cross(IScope scope, IRandom random, bool[] parent1, bool[] parent2) {
      return Apply(random, parent1, parent2);
    }
  }
}
