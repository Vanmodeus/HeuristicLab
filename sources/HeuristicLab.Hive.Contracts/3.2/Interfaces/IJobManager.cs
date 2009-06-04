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
using System.ServiceModel;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Contracts.Interfaces {
  /// <summary>
  /// This is the facade for the Job Manager used by the Management Console
  /// </summary>
  [ServiceContract]
  public interface IJobManager {
    [OperationContract]
    ResponseList<Job> GetAllJobs();
    [OperationContract]
    ResponseObject<Job> GetJobById(Guid jobId);
    [OperationContract]
    ResponseObject<Job> AddNewJob(Job job);
    [OperationContract]
    Response RemoveJob(Guid jobId);
    [OperationContract]
    ResponseObject<JobResult> GetLastJobResultOf(Guid jobId, bool requested);
    [OperationContract]
    ResponseList<JobResult> GetAllJobResults(Guid jobId);
    [OperationContract]
    Response RequestSnapshot(Guid jobId);
    [OperationContract]
    Response AbortJob(Guid jobId);
    [OperationContract]
    ResponseList<Project> GetAllProjects();
    [OperationContract]
    Response CreateProject(Project project);
    [OperationContract]
    Response ChangeProject(Project project);
    [OperationContract]
    Response DeleteProject(Guid projectId);
    [OperationContract]
    ResponseList<Job> GetJobsByProject(Guid projectId);
  }
}
