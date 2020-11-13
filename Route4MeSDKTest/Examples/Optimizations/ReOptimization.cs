﻿using Route4MeSDK.DataTypes;
using Route4MeSDK.QueryTypes;
using System;

namespace Route4MeSDK.Examples
{
  public sealed partial class Route4MeExamples
  {
    public void ReOptimization(string optimizationProblemID)
    {
      // Create the manager with the api key
      var route4Me = new Route4MeManager(ActualApiKey);
    
      var optimizationParameters = new OptimizationParameters()
      {
        OptimizationProblemID = optimizationProblemID,
        ReOptimize = true
      };

      // Run the query
      DataObject dataObject = route4Me.UpdateOptimization(
          optimizationParameters, 
          out string errorString);

      Console.WriteLine("");

      if (dataObject != null)
      {
        Console.WriteLine("ReOptimization executed successfully");

        Console.WriteLine("Optimization Problem ID: {0}", dataObject.OptimizationProblemId);
        Console.WriteLine("State: {0}", dataObject.State);
      }
      else
      {
        Console.WriteLine("ReOptimization error: {0}", errorString);
      }
    }
  }
}
