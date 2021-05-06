﻿using Route4MeSDK.DataTypes;
using System.Collections.Generic;
using Route4MeSDK.QueryTypes;
using System;
using System.Linq;

namespace Route4MeSDK.Examples
{
  public sealed partial class Route4MeExamples
  {
        /// <summary>
        /// Add destinations to a route.
        /// </summary>
        /// <param name="routeId">Route ID</param>
        /// <returns>An array of the added address IDs</returns>
        public int[] AddRouteDestinations(string routeId = null)
    {
      // Create the manager with the api key
            var route4Me = new Route4MeManager(ActualApiKey);

            bool isInnerExample = routeId == null ? true : false;

            if (isInnerExample)
            {
                RunOptimizationSingleDriverRoute10Stops();

                OptimizationsToRemove = new List<string>() { SD10Stops_optimization_problem_id };
                routeId = SD10Stops_route_id;
            }

      // Prepare the addresses
      Address[] addresses = new Address[]
      {
        #region Addresses

        new Address() { AddressString =  "146 Bill Johnson Rd NE Milledgeville GA 31061",
                        Latitude = 	33.143526,
                        Longitude = -83.240354,
                        Time = 0 },

        new Address() { AddressString =  "222 Blake Cir Milledgeville GA 31061",
                        Latitude = 	33.177852,
                        Longitude = -83.263535,
                        Time = 0 }

        #endregion
      };

      // Run the query
      bool optimalPosition = true;
            string errorString;
            int[] destinationIds = route4Me.AddRouteDestinations(routeId, addresses, optimalPosition, out errorString)
                .Select(address => address.RouteDestinationId.GetValueOrDefault())
                .ToArray();

            PrintExampleRouteResult(SD10Stops_route, errorString);

            if (isInnerExample)
      {
                RemoveTestOptimizations();
                return null;
      }
      else
      {
                return destinationIds;
      }
    }
  }
}
