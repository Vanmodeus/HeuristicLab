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

using System.ServiceModel;

namespace HeuristicLab.Clients.Hive.SlaveCore.ServiceContracts {

  [ServiceContract(CallbackContract = typeof(ISlaveCommunicationCallbacks))]
  public interface ISlaveCommunication {

    [OperationContract]
    StatusCommons Subscribe();

    [OperationContract]
    bool Unsubscribe();


    [OperationContract]
    void Restart();

    [OperationContract]
    void Sleep();

    [OperationContract]
    void PauseAll();

    [OperationContract]
    void StopAll();

    [OperationContract]
    void AbortAll();

    //callbacks
    [OperationContract]
    void LogMessage(string message);

    [OperationContract]
    void StatusChanged(StatusCommons status);

    [OperationContract]
    void Shutdown();
  }
}
