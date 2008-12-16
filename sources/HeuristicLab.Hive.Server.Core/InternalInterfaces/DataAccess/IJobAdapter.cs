﻿#region License Information
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
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Server.Core.InternalInterfaces.DataAccess {
  public interface IJobAdapter: IDataAdapter<Job> {
    /// <summary>
    /// Gets all subjobs of the specified job
    /// </summary>
    /// <returns></returns>
    ICollection<Job> GetAllSubjobs(Job job);

    /// <summary>
    /// Gets the result of a job
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    JobResult GetResult(Job job);

    /// <summary>
    /// Gets all jobs of the client
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    ICollection<Job> GetJobsOf(ClientInfo client);

    /// <summary>
    /// Gets all jobs of the user
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    ICollection<Job> GetJobsOf(User user);
  }
}
