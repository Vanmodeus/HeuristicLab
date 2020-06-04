﻿using HEAL.Attic;
#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Optimization {

  //TODO add non-generic for bwerths problem modifiers
  public interface IProblemDefinition {
    //IEncoding Encoding { get; }
  }

  [StorableType("747a3cea-b9ba-4322-a5c2-050cd7e16e2a")]
  public interface IProblemDefinition<TEncoding, TEncodedSolution> : IProblemDefinition
    where TEncoding : class, IEncoding<TEncodedSolution>
    where TEncodedSolution : class, IEncodedSolution {
    TEncoding Encoding { get; }

  }
}
