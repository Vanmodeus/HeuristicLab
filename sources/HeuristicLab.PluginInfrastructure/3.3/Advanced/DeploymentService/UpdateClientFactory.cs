﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;

namespace HeuristicLab.PluginInfrastructure.Advanced.DeploymentService {
  /// <summary>
  /// Factory class to generated update client instances for the deployment service.
  /// </summary>
  public static class UpdateClientFactory {
    private static byte[] serverCrtData;

    /// <summary>
    /// static constructor loads the embedded service certificate 
    /// </summary>
    static UpdateClientFactory() {
      var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HeuristicLab.PluginInfrastructure.Advanced.DeploymentService.services.heuristiclab.com.cer");
      serverCrtData = new byte[stream.Length];
      stream.Read(serverCrtData, 0, serverCrtData.Length);
    }

    /// <summary>
    /// Factory method to create new update clients for the deployment service.
    /// Sets the connection string and user credentials from values provided in settings.
    /// HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName
    /// HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword
    /// HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation
    /// 
    /// </summary>
    /// <returns>A new instance of an update client</returns>
    public static UpdateClient CreateClient() {
      var client = new UpdateClient();
      client.ClientCredentials.UserName.UserName = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationUserName;
      client.ClientCredentials.UserName.Password = HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocationPassword;
      client.Endpoint.Address = new EndpointAddress(HeuristicLab.PluginInfrastructure.Properties.Settings.Default.UpdateLocation);
      client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
      client.ClientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator =
          new DeploymentServerCertificateValidator(new X509Certificate2(serverCrtData));
      return client;
    }
  }
}
