﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface.App {
  internal static class Runner {
    internal static void Run(string template, string config, string outputFile) {
      InstantiatorResult instantiatorResult = JsonTemplateInstantiator.Instantiate(template, config);
      IOptimizer optimizer = instantiatorResult.Optimizer;
      IEnumerable<IResultJsonItem> configuredResultItem = instantiatorResult.ConfiguredResultItems;

      optimizer.Runs.Clear();
      if (optimizer is EngineAlgorithm e)
        e.Engine = new ParallelEngine.ParallelEngine();

      Task task = optimizer.StartAsync();
      while (!task.IsCompleted) {
        WriteResultsToFile(outputFile, optimizer, configuredResultItem, instantiatorResult.PostProcessors);
        Thread.Sleep(100);
      }

      WriteResultsToFile(outputFile, optimizer, configuredResultItem, instantiatorResult.PostProcessors);
    }

    private static void WriteResultsToFile(string file, IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItem, IEnumerable<IResultCollectionPostProcessor> postProcessors) {
      if (optimizer.Runs.Count > 0) 
        File.WriteAllText(file, FetchResults(optimizer, configuredResultItem, postProcessors));
    }
      

    private static IEnumerable<IResultFormatter> ResultFormatter { get; } =
      PluginInfrastructure.ApplicationManager.Manager.GetInstances<IResultFormatter>();

    private static IResultFormatter GetResultFormatter(string fullName) =>
      ResultFormatter?.Where(x => x.GetType().FullName == fullName).Last();

    private static string FetchResults(IOptimizer optimizer, IEnumerable<IResultJsonItem> configuredResultItems, IEnumerable<IResultCollectionPostProcessor> postProcessors) {
      JArray arr = new JArray();
      IEnumerable<string> configuredResults = configuredResultItems.Select(x => x.Name);

      foreach (var run in optimizer.Runs) {
        JObject obj = new JObject();
        arr.Add(obj);
        obj.Add("Run", JToken.FromObject(run.ToString()));

        // zip and filter the results with the ResultJsonItems
        var filteredResults = new List<Tuple<IResultJsonItem, IItem>>();
        foreach(var resultItem in configuredResultItems) {
          foreach(var result in run.Results) {
            if(resultItem.Name == result.Key) {
              filteredResults.Add(Tuple.Create(resultItem, result.Value));
            }
          }
        }

        // add results to the JObject
        foreach(var result in filteredResults) {
          var formatter = GetResultFormatter(result.Item1.ResultFormatterType);
          if(!obj.ContainsKey(result.Item1.Name)) // to prevent duplicates
            obj.Add(result.Item1.Name, formatter.Format(result.Item2));
        }

        IDictionary<string, string> resultDict = new Dictionary<string, string>();
        foreach (var processor in postProcessors) {
          processor.Apply(run.Results, resultDict);
        }
        foreach(var kvp in resultDict) {
          obj.Add(kvp.Key, kvp.Value);
        }
      }
      return SingleLineArrayJsonWriter.Serialize(arr);
    }
  }
}