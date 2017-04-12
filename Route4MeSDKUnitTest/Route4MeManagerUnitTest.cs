﻿using Moq;
using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NUnit.Framework;
using Route4MeSDK;
using Route4MeSDK.DataTypes;
using Route4MeSDK.QueryTypes;
using System.Collections.Generic;
using System.Reflection;
using System.CodeDom.Compiler;

namespace Route4MeSDKUnitTest
{
    [TestClass]
    public class SingleDriverRoute10StopsGrgoup
    {
        static DataObject dataObject1, dataObject, dataObject2;
        static DataObjectRoute routeSingleDriverRoute10Stops;
        static string routeId_SingleDriverRoute10Stops;
        static int removeRouteDestinationID;
        
        static string routeId_DuplicateRoute;

        static string c_ApiKey = "11111111111111111111111111111111";

        [TestMethod]
        public void RunOptimizationSingleDriverRoute10StopsTest()
        {
            Route4MeManager r4mm = new Route4MeManager(c_ApiKey);

            // Prepare the addresses
            Address[] addresses = new Address[]
            {
            #region Addresses

            new Address() { AddressString = "151 Arbor Way Milledgeville GA 31061",
                            //indicate that this is a departure stop
                            //single depot routes can only have one departure depot 
                            IsDepot = true, 
                        
                            //required coordinates for every departure and stop on the route
                            Latitude = 33.132675170898,
                            Longitude = -83.244743347168,
                        
                            //the expected time on site, in seconds. this value is incorporated into the optimization engine
                            //it also adjusts the estimated and dynamic eta's for a route
                            Time = 0, 
                        
                        
                            //input as many custom fields as needed, custom data is passed through to mobile devices and to the manifest
                            CustomFields = new Dictionary<string, string>() {{"color", "red"}, {"size", "huge"}}
            },

            new Address() { AddressString = "230 Arbor Way Milledgeville GA 31061",
                            Latitude = 33.129695892334,
                            Longitude = -83.24577331543,
                            Time = 0 },

            new Address() { AddressString = "148 Bass Rd NE Milledgeville GA 31061",
                            Latitude = 33.143497,
                            Longitude = -83.224487,
                            Time = 0 },

            new Address() { AddressString = "117 Bill Johnson Rd NE Milledgeville GA 31061",
                            Latitude = 33.141784667969,
                            Longitude = -83.237518310547,
                            Time = 0 },

            new Address() { AddressString = "119 Bill Johnson Rd NE Milledgeville GA 31061",
                            Latitude = 33.141086578369,
                            Longitude = -83.238258361816,
                            Time = 0 },

            new Address() { AddressString =  "131 Bill Johnson Rd NE Milledgeville GA 31061",
                            Latitude = 33.142036437988,
                            Longitude = -83.238845825195,
                            Time = 0 },

            new Address() { AddressString =  "138 Bill Johnson Rd NE Milledgeville GA 31061",
                            Latitude = 33.14307,
                            Longitude = -83.239334,
                            Time = 0 },

            new Address() { AddressString =  "139 Bill Johnson Rd NE Milledgeville GA 31061",
                            Latitude = 33.142734527588,
                            Longitude = -83.237442016602,
                            Time = 0 },

            new Address() { AddressString =  "145 Bill Johnson Rd NE Milledgeville GA 31061",
                            Latitude = 33.143871307373,
                            Longitude = -83.237342834473,
                            Time = 0 },

            new Address() { AddressString =  "221 Blake Cir Milledgeville GA 31061",
                            Latitude = 33.081462860107,
                            Longitude = -83.208511352539,
                            Time = 0 }

            #endregion
            };

            // Set parameters
            RouteParameters parameters = new RouteParameters()
            {
                AlgorithmType = AlgorithmType.TSP,
                StoreRoute = false,
                RouteName = "Single Driver Route 10 Stops Test",

                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                RouteTime = 60 * 60 * 7,
                Optimize = Optimize.Distance.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description()
            };

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                Addresses = addresses,
                Parameters = parameters
            };

            // Run the query
            string errorString;
            dataObject1 = r4mm.RunOptimization(optimizationParameters, out errorString);

            Assert.IsNotNull(dataObject1, "Run optimization test with Single Driver Route 10 Stops failed... " + errorString);

            routeSingleDriverRoute10Stops = (dataObject1 != null && dataObject1.Routes != null && dataObject1.Routes.Length > 0) ? dataObject1.Routes[0] : null;

            Assert.IsNotNull(routeSingleDriverRoute10Stops, "Run optimization test with Single Driver Route 10 Stops failed...");
            
            routeId_SingleDriverRoute10Stops = (routeSingleDriverRoute10Stops != null) ? routeSingleDriverRoute10Stops.RouteID : null;

            Assert.IsInstanceOfType(routeId_SingleDriverRoute10Stops,typeof(System.String));

            Assert.IsNotNull(routeId_SingleDriverRoute10Stops, "The optimization problem ID is wrong in Single Driver Route 10 Stops test...");

            ResequenceRouteDestinationsTest();

            ResequenceReoptimizeRouteTest();

            AddRouteDestinationsTest();

            RemoveRouteDestinationTest();

            UpdateRouteTest();

            ReoptimizeRouteTest();

            GetRouteDirectionsTest();

            GetRoutePathPointsTest();

            LogCustomActivityTest();

            GetActivitiesTest();

            DuplicateRouteTest();

            DeleteRoutesTest();
        }

        public void ResequenceRouteDestinationsTest( )
        {
            DataObjectRoute route = routeSingleDriverRoute10Stops;
            Assert.IsNotNull(route, "Route for the test Route Destinations Resequence is null...");

            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AddressesOrderInfo addressesOrderInfo = new AddressesOrderInfo();
            addressesOrderInfo.RouteId = route.RouteID;
            addressesOrderInfo.Addresses = new AddressInfo[0];
            for (int i = 0; i < route.Addresses.Length; i++)
            {
                Address address = route.Addresses[i];
                AddressInfo addressInfo = new AddressInfo();
                addressInfo.DestinationId = address.RouteDestinationId.Value;
                addressInfo.SequenceNo = i;
                if (i == 1)
                    addressInfo.SequenceNo = 2;
                else if (i == 2)
                    addressInfo.SequenceNo = 1;
                addressInfo.IsDepot = (addressInfo.SequenceNo == 0);
                List<AddressInfo> addressesList = new List<AddressInfo>(addressesOrderInfo.Addresses);
                addressesList.Add(addressInfo);
                addressesOrderInfo.Addresses = addressesList.ToArray();
            }

            string errorString1 = "";
            DataObjectRoute route1 = route4Me.GetJsonObjectFromAPI<DataObjectRoute>(addressesOrderInfo,
                                                                                                R4MEInfrastructureSettings.RouteHost,
                                                                                                HttpMethodType.Put,
                                                                                                out errorString1);
            Assert.IsNotNull(route1, "ResequenceRouteDestinationsTest failed...");
        }

        public void ResequenceReoptimizeRouteTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string route_id = routeId_SingleDriverRoute10Stops;

            Assert.IsNotNull(route_id, "rote_id is null...");

            Dictionary<string, string> roParameters = new Dictionary<string, string>()
            {
                {"route_id",route_id},
                {"disable_optimization","0"},
                {"optimize","Distance"},
            };

            // Run the query
            string errorString;
            bool result = route4Me.ResequenceReoptimizeRoute(roParameters, out errorString);

            Assert.IsTrue(result, "ResequenceReoptimizeRouteTest failed...");
        }

        public void AddRouteDestinationsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string route_id = routeId_SingleDriverRoute10Stops;

            Assert.IsNotNull(route_id, "rote_id is null...");

            // Prepare the addresses
            #region Addresses
            Address[] addresses = new Address[]
            {
                new Address() { AddressString =  "146 Bill Johnson Rd NE Milledgeville GA 31061",
                                Latitude = 	33.143526,
                                Longitude = -83.240354,
                                Time = 0 },

                new Address() { AddressString =  "222 Blake Cir Milledgeville GA 31061",
                                Latitude = 	33.177852,
                                Longitude = -83.263535,
                                Time = 0 }
            };
            #endregion

            // Run the query
            bool optimalPosition = true;
            string errorString;
            int[] destinationIds = route4Me.AddRouteDestinations(route_id, addresses, optimalPosition, out errorString);

            Assert.IsInstanceOfType(destinationIds, typeof(System.Int32[]), "AddRouteDestinationsTest failed...");

            removeRouteDestinationID = destinationIds[0];
        }

        public void RemoveRouteDestinationTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string route_id = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(route_id, "rote_id is null...");

            int destination_id = removeRouteDestinationID;
            Assert.IsNotNull(destination_id, "destination_id is null...");

            // Run the query
            string errorString;
            bool deleted = route4Me.RemoveRouteDestination(route_id, destination_id, out errorString);

            Assert.IsTrue(deleted, "RemoveRouteDestinationTest");
        }

        public void UpdateRouteTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(routeId, "routeId_SingleDriverRoute10Stops is null...");

            RouteParameters parametersNew = new RouteParameters()
            {
                RouteName = "New name of the route"
            };

            RouteParametersQuery routeParameters = new RouteParametersQuery()
            {
                RouteId = routeId,
                Parameters = parametersNew
            };

            string errorString;
            dataObject = route4Me.UpdateRoute(routeParameters, out errorString);

            Assert.IsNotNull(dataObject, "UpdateRouteTest failed... " + errorString);
        }

        public void ReoptimizeRouteTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(routeId, "routeId_SingleDriverRoute10Stops is null...");

            RouteParametersQuery routeParameters = new RouteParametersQuery()
            {
                RouteId = routeId,
                ReOptimize = true
            };

            // Run the query
            string errorString;
            dataObject = route4Me.UpdateRoute(routeParameters, out errorString);

            Assert.IsNotNull(dataObject, "ReoptimizeRouteTest failed... " + errorString);
        }

        public void GetRouteDirectionsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(routeId, "routeId_SingleDriverRoute10Stops is null...");

            RouteParametersQuery routeParameters = new RouteParametersQuery()
            {
                RouteId = routeId
            };

            routeParameters.Directions = true;

            // Run the query
            string errorString;
            dataObject = route4Me.GetRoute(routeParameters, out errorString);

            Assert.IsNotNull(dataObject, "GetRouteDirectionsTest failed... " + errorString);
        }

        public void GetRoutePathPointsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(routeId, "routeId_SingleDriverRoute10Stops is null...");

            RouteParametersQuery routeParameters = new RouteParametersQuery()
            {
                RouteId = routeId
            };

            routeParameters.RoutePathOutput = RoutePathOutput.Points.ToString();

            // Run the query
            string errorString;
            dataObject = route4Me.GetRoute(routeParameters, out errorString);

            Assert.IsNotNull(dataObject, "GetRoutePathPointsTest failed... " + errorString);
        }

        public void LogCustomActivityTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(routeId, "routeId_SingleDriverRoute10Stops is null...");

            string message = "Test User Activity " + DateTime.Now.ToString();

            Activity activity = new Activity()
            {
                ActivityType = "user_message",
                ActivityMessage = message,
                RouteId = routeId
            };

            // Run the query
            string errorString;
            bool added = route4Me.LogCustomActivity(activity, out errorString);

            Assert.IsTrue(added, "LogCustomActivityTest failed... " + errorString);
        }

        public void GetActivitiesTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(routeId, "routeId_SingleDriverRoute10Stops is null...");

            ActivityParameters activityParameters = new ActivityParameters()
            {
                RouteId = routeId,
                Limit = 10,
                Offset = 0
            };

            // Run the query
            string errorString;
            Activity[] activities = route4Me.GetActivityFeed(activityParameters, out errorString);

            Assert.IsInstanceOfType(activities, typeof(Activity[]), "GetActivitiesTest failed... " + errorString);
        }

        public void DuplicateRouteTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoute10Stops;
            Assert.IsNotNull(routeId, "routeId is null...");

            RouteParametersQuery routeParameters = new RouteParametersQuery()
            {
                RouteId = routeId
            };

            // Run the query
            string errorString;
            routeId_DuplicateRoute = route4Me.DuplicateRoute(routeParameters, out errorString);

            Assert.IsNotNull(routeId_DuplicateRoute, "DuplicateRouteTest failed... " + errorString);
        }

        public void DeleteRoutesTest()
        {
            List<string> routeIdsToDelete = new List<string>();

            if (routeId_SingleDriverRoute10Stops != null)
                routeIdsToDelete.Add(routeId_SingleDriverRoute10Stops);
            if (routeId_DuplicateRoute != null)
                routeIdsToDelete.Add(routeId_DuplicateRoute);

            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Run the query
            string errorString;
            string[] deletedRouteIds = route4Me.DeleteRoutes(routeIdsToDelete.ToArray(), out errorString);

            Assert.IsInstanceOfType(deletedRouteIds, typeof(string[]), "DeleteRoutesTest failed... " + errorString);
        }

    }

    [TestClass]
    public class SingleDriverRoundTripGeneric
    {
        static string optimizationProblemID;

        static string c_ApiKey = "11111111111111111111111111111111";

        static DataObject dataObject;

        [TestMethod]
        public void SingleDriverRoundTripGenericTest()
        {
            const string uri = R4MEInfrastructureSettings.MainHost + "/api.v4/optimization_problem.php";
            const string myApiKey = "11111111111111111111111111111111";

            // Create the manager with the api key
            Route4MeManager route4Me = new Route4MeManager(myApiKey);

            // Prepare the addresses
            // Using the defined class, can use user-defined class instead
            Address[] addresses = new Address[]
              {
                #region Addresses

                new Address() { AddressString = "754 5th Ave New York, NY 10019",
                                Alias         = "Bergdorf Goodman",
                                IsDepot       = true,
                                Latitude      = 40.7636197,
                                Longitude     = -73.9744388,
                                Time          = 0 },

                new Address() { AddressString = "717 5th Ave New York, NY 10022",
                                Alias         = "Giorgio Armani",
                                Latitude      = 40.7669692,
                                Longitude     = -73.9693864,
                                Time          = 0 },

                new Address() { AddressString = "888 Madison Ave New York, NY 10014",
                                Alias         = "Ralph Lauren Women's and Home",
                                Latitude      = 40.7715154,
                                Longitude     = -73.9669241,
                                Time          = 0 },

                new Address() { AddressString = "1011 Madison Ave New York, NY 10075",
                                Alias         = "Yigal Azrou'l",
                                Latitude      = 40.7772129,
                                Longitude     = -73.9669,
                                Time          = 0 },

                 new Address() { AddressString = "440 Columbus Ave New York, NY 10024",
                                Alias         = "Frank Stella Clothier",
                                Latitude      = 40.7808364,
                                Longitude     = -73.9732729,
                                Time          = 0 },

                new Address() { AddressString = "324 Columbus Ave #1 New York, NY 10023",
                                Alias         = "Liana",
                                Latitude      = 40.7803123,
                                Longitude     = -73.9793079,
                                Time          = 0 },

                new Address() { AddressString = "110 W End Ave New York, NY 10023",
                                Alias         = "Toga Bike Shop",
                                Latitude      = 40.7753077,
                                Longitude     = -73.9861529,
                                Time          = 0 }, 

                new Address() { AddressString = "555 W 57th St New York, NY 10019",
                                Alias         = "BMW of Manhattan",
                                Latitude      = 40.7718005,
                                Longitude     = -73.9897716,
                                Time          = 0 },

                new Address() { AddressString = "57 W 57th St New York, NY 10019",
                                Alias         = "Verizon Wireless",
                                Latitude      = 40.7558695,
                                Longitude     = -73.9862019,
                                Time          = 0 },

                #endregion
              };

            // Set parameters
            // Using the defined class, can use user-defined class instead
            RouteParameters parameters = new RouteParameters()
            {
                AlgorithmType = AlgorithmType.TSP,
                StoreRoute = false,
                RouteName = "Single Driver Round Trip",

                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                RouteTime = 60 * 60 * 7,
                RouteMaxDuration = 86400,
                VehicleCapacity = "1",
                VehicleMaxDistanceMI = "10000",

                Optimize = Optimize.Distance.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description(),
                TravelMode = TravelMode.Driving.Description(),
            };

            MyAddressAndParametersHolder myParameters = new MyAddressAndParametersHolder()
            {
                addresses = addresses,
                parameters = parameters
            };

            // Run the query
            string errorString;
            MyDataObjectGeneric dataObject0 = route4Me.GetJsonObjectFromAPI<MyDataObjectGeneric>
               (myParameters, uri, HttpMethodType.Post, out errorString);

            optimizationProblemID = dataObject0.OptimizationProblemId;

            Assert.IsNotNull(dataObject0, "SingleDriverRoundTripGenericTest failed...");

            GetOptimizationTest();

            AddDestinationToOptimizationTest();

            ReOptimizationTest();

            RemoveDestinationFromOptimizationTest();

            RemoveOptimizationTest();
        }

        public void GetOptimizationTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                OptimizationProblemID = optimizationProblemID
            };

            // Run the query
            string errorString;
            dataObject = route4Me.GetOptimization(optimizationParameters, out errorString);

            Assert.IsNotNull(dataObject, "GetOptimizationTest failed... " + errorString);
        }

        public void AddDestinationToOptimizationTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Prepare the address that we are going to add to an existing route optimization
            Address[] addresses = new Address[]
            {
                new Address() { AddressString = "717 5th Ave New York, NY 10021",
                            Alias         = "Giorgio Armani",
                            Latitude      = 40.7669692,
                            Longitude     = -73.9693864,
                            Time          = 0
                }
            };

            //Optionally change any route parameters, such as maximum route duration, maximum cubic constraints, etc.
            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                OptimizationProblemID = optimizationProblemID,
                Addresses = addresses,
                ReOptimize = true
            };

            // Execute the optimization to re-optimize and rebalance all the routes in this optimization
            string errorString;
            dataObject = route4Me.UpdateOptimization(optimizationParameters, out errorString);

            Assert.IsNotNull(dataObject, "AddDestinationToOptimization and reoptimized Test  failed... " + errorString);

            optimizationParameters.ReOptimize = false;
            dataObject = route4Me.UpdateOptimization(optimizationParameters, out errorString);

            Assert.IsNotNull(dataObject, "AddDestinationToOptimization and not reoptimized Test  failed... " + errorString);
        }

        public void ReOptimizationTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                OptimizationProblemID = optimizationProblemID,
                ReOptimize = true
            };

            // Run the query
            string errorString;
            dataObject = route4Me.UpdateOptimization(optimizationParameters, out errorString);

            Assert.IsNotNull(dataObject, "ReOptimizationTest failed... " + errorString);
        }

        public void RemoveDestinationFromOptimizationTest()
        {
            Address destinationToRemove = (dataObject != null && dataObject.Addresses.Length > 0) ? dataObject.Addresses[dataObject.Addresses.Length - 1] : null;

            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string OptimizationProblemId = dataObject.OptimizationProblemId;
            Assert.IsNotNull(OptimizationProblemId, "OptimizationProblemId is null...");

            int destinationId = destinationToRemove.RouteDestinationId != null ? Convert.ToInt32(destinationToRemove.RouteDestinationId) : -1;
            Assert.AreNotEqual(-1, "destinationId is null...");
            // Run the query
            string errorString;
            bool removed = route4Me.RemoveDestinationFromOptimization(dataObject.OptimizationProblemId, destinationId, out errorString);

            Assert.IsTrue(removed, "RemoveDestinationFromOptimizationTest failed...");
        }

        public void AddOrdersToOptimizationTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            OptimizationParameters rQueryParams = new OptimizationParameters()
            {
                OptimizationProblemID = optimizationProblemID,
                Redirect = false
            };

            #region Addresses
            Address[] addresses = new Address[] {
		    new Address {
			    AddressString = "273 Canal St, New York, NY 10013, USA",
			    Latitude = 40.7191558,
			    Longitude = -74.0011966,
			    Alias = "",
			    CurbsideLatitude = 40.7191558,
			    CurbsideLongitude = -74.0011966,
			    IsDepot = true
		    },
		    new Address {
			    AddressString = "106 Liberty St, New York, NY 10006, USA",
			    Alias = "BK Restaurant #: 2446",
			    Latitude = 40.709637,
			    Longitude = -74.011912,
			    CurbsideLatitude = 40.709637,
			    CurbsideLongitude = -74.011912,
			    Email = "",
			    Phone = "(917) 338-1887",
			    FirstName = "",
			    LastName = "",
			    CustomFields = new Dictionary<string, string> { {"icon", null} },
			    Time = 0,
			    TimeWindowStart = 1472544000,
			    TimeWindowEnd = 1472544300,
			    OrderId = 7205705
		    },
		    new Address {
			    AddressString = "325 Broadway, New York, NY 10007, USA",
			    Alias = "BK Restaurant #: 20333",
			    Latitude = 40.71615,
			    Longitude = -74.00505,
			    CurbsideLatitude = 40.71615,
			    CurbsideLongitude = -74.00505,
			    Email = "",
			    Phone = "(212) 227-7535",
			    FirstName = "",
			    LastName = "",
			    CustomFields = new Dictionary<string, string> { {"icon", null} },
			    Time = 0,
			    TimeWindowStart = 1472545000,
			    TimeWindowEnd = 1472545300,
			    OrderId = 7205704
		    },
		    new Address {
			    AddressString = "106 Fulton St, Farmingdale, NY 11735, USA",
			    Alias = "BK Restaurant #: 17871",
			    Latitude = 40.73073,
			    Longitude = -73.459283,
			    CurbsideLatitude = 40.73073,
			    CurbsideLongitude = -73.459283,
			    Email = "",
			    Phone = "(212) 566-5132",
			    FirstName = "",
			    LastName = "",
			    CustomFields = new Dictionary<string, string> { {"icon", null} },
			    Time = 0,
			    TimeWindowStart = 1472546000,
			    TimeWindowEnd = 1472546300,
			    OrderId = 7205703
		    }
	    };
            #endregion

            RouteParameters rParams = new RouteParameters()
            {
                RouteName = "Wednesday 15th of June 2016 07:01 PM (+03:00)",
                RouteDate = 1465948800,
                RouteTime = 14400,
                Optimize = "Time",
                RouteType = "single",
                AlgorithmType = AlgorithmType.TSP,
                RT = false,
                LockLast = false,
                MemberId = "1",
                VehicleId = "",
                DisableOptimization = false
            };

            string errorString = "";
            DataObject dataObject = route4Me.AddOrdersToOptimization(rQueryParams, addresses, rParams, out errorString);

            Assert.IsNotNull(dataObject, "AddOrdersToOptimizationTest failed... " + errorString);

        }

        public void RemoveOptimizationTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string opt_id = optimizationProblemID;
            Assert.IsNotNull(opt_id, "optimizationProblemID is null...");

            List<string> lsOptIDs = new List<string>();
            lsOptIDs.Add(optimizationProblemID);

            // Run the query
            string errorString;
            bool removed = route4Me.RemoveOptimization(lsOptIDs.ToArray(), out errorString);

            Assert.IsTrue(removed, "RemoveOptimizationTest failed... " + errorString);
        }

    }

    [TestClass]
    public class SingleDriverRoundTrip
    {
        static string c_ApiKey = "11111111111111111111111111111111";
        static DataObject dataObject2;
        static string routeId_SingleDriverRoundTrip;

        [TestMethod]
        public void SingleDriverRoundTripTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Prepare the addresses
            Address[] addresses = new Address[]
              {
                #region Addresses

                new Address() { AddressString = "754 5th Ave New York, NY 10019",
                                Alias         = "Bergdorf Goodman",
                                IsDepot       = true,
                                Latitude      = 40.7636197,
                                Longitude     = -73.9744388,
                                Time          = 0 },

                new Address() { AddressString = "717 5th Ave New York, NY 10022",
                                Alias         = "Giorgio Armani",
                                Latitude      = 40.7669692,
                                Longitude     = -73.9693864,
                                Time          = 0 },

                new Address() { AddressString = "888 Madison Ave New York, NY 10014",
                                Alias         = "Ralph Lauren Women's and Home",
                                Latitude      = 40.7715154,
                                Longitude     = -73.9669241,
                                Time          = 0 },

                new Address() { AddressString = "1011 Madison Ave New York, NY 10075",
                                Alias         = "Yigal Azrou'l",
                                Latitude      = 40.7772129,
                                Longitude     = -73.9669,
                                Time          = 0 },

                 new Address() { AddressString = "440 Columbus Ave New York, NY 10024",
                                Alias         = "Frank Stella Clothier",
                                Latitude      = 40.7808364,
                                Longitude     = -73.9732729,
                                Time          = 0 },

                new Address() { AddressString = "324 Columbus Ave #1 New York, NY 10023",
                                Alias         = "Liana",
                                Latitude      = 40.7803123,
                                Longitude     = -73.9793079,
                                Time          = 0 },

                new Address() { AddressString = "110 W End Ave New York, NY 10023",
                                Alias         = "Toga Bike Shop",
                                Latitude      = 40.7753077,
                                Longitude     = -73.9861529,
                                Time          = 0 }, 

                new Address() { AddressString = "555 W 57th St New York, NY 10019",
                                Alias         = "BMW of Manhattan",
                                Latitude      = 40.7718005,
                                Longitude     = -73.9897716,
                                Time          = 0 },

                new Address() { AddressString = "57 W 57th St New York, NY 10019",
                                Alias         = "Verizon Wireless",
                                Latitude      = 40.7558695,
                                Longitude     = -73.9862019,
                                Time          = 0 },

                #endregion
              };

            // Set parameters
            RouteParameters parameters = new RouteParameters()
            {
                AlgorithmType = AlgorithmType.TSP,
                StoreRoute = false,
                RouteName = "Single Driver Round Trip",

                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                RouteTime = 60 * 60 * 7,
                RouteMaxDuration = 86400,
                VehicleCapacity = "1",
                VehicleMaxDistanceMI = "10000",

                Optimize = Optimize.Distance.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description(),
                TravelMode = TravelMode.Driving.Description(),
            };

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                Addresses = addresses,
                Parameters = parameters
            };

            // Run the query
            string errorString;
            dataObject2 = route4Me.RunOptimization(optimizationParameters, out errorString);

            routeId_SingleDriverRoundTrip = (dataObject2 != null && dataObject2.Routes != null && dataObject2.Routes.Length > 0) ? dataObject2.Routes[0].RouteID : null;

            Assert.IsNotNull(dataObject2, "SingleDriverRoundTripTest failed... " + errorString);

            GetAddressTest();

            AddAddressNoteTest();

            AddAddressNoteWithFileTest();

            GetAddressNotesTest();

            DeleteRoutesTest();
        }

        public void GetAddressTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeIdToMoveTo = routeId_SingleDriverRoundTrip;
            Assert.IsNotNull(routeIdToMoveTo, "routeId_SingleDriverRoundTrip is null...");

            int addressId = (dataObject2 != null && dataObject2.Routes != null && dataObject2.Routes.Length > 0 && dataObject2.Routes[0].Addresses.Length > 1 && dataObject2.Routes[0].Addresses[1].RouteDestinationId != null) ? dataObject2.Routes[0].Addresses[1].RouteDestinationId.Value : 0;

            AddressParameters addressParameters = new AddressParameters()
            {
                RouteId = routeIdToMoveTo,
                RouteDestinationId = addressId,
                Notes = true
            };

            // Run the query
            string errorString;
            Address dataObject = route4Me.GetAddress(addressParameters, out errorString);

            Assert.IsNotNull(dataObject, "GetAddressTest failed... " + errorString);
        }

        public void AddAddressNoteTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeIdToMoveTo = routeId_SingleDriverRoundTrip;
            Assert.IsNotNull(routeIdToMoveTo, "routeId_SingleDriverRoundTrip is null...");

            int addressId = (dataObject2 != null && dataObject2.Routes != null && dataObject2.Routes.Length > 0 && dataObject2.Routes[0].Addresses.Length > 1 && dataObject2.Routes[0].Addresses[1].RouteDestinationId != null) ? dataObject2.Routes[0].Addresses[1].RouteDestinationId.Value : 0;

            NoteParameters noteParameters = new NoteParameters()
            {
                RouteId = routeIdToMoveTo,
                AddressId = addressId,
                Latitude = 33.132675170898,
                Longitude = -83.244743347168,
                DeviceType = DeviceType.Web.Description(),
                ActivityType = StatusUpdateType.DropOff.Description()
            };

            // Run the query
            string errorString;
            string contents = "Test Note Contents " + DateTime.Now.ToString();
            AddressNote note = route4Me.AddAddressNote(noteParameters, contents, out errorString);

            Assert.IsNotNull(note, "AddAddressNoteTest failed... " + errorString);
        }

        public void AddAddressNoteWithFileTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoundTrip;

            Assert.IsNotNull(routeId, "routeId_SingleDriverRoundTrip is null...");

            int addressId = (dataObject2 != null && dataObject2.Routes != null && dataObject2.Routes.Length > 0 && dataObject2.Routes[0].Addresses.Length > 1 && dataObject2.Routes[0].Addresses[1].RouteDestinationId != null) ? dataObject2.Routes[0].Addresses[1].RouteDestinationId.Value : 0;
            addressId = 162916895;

            NoteParameters noteParameters = new NoteParameters()
            {
                RouteId = routeId,
                AddressId = addressId,
                Latitude = 33.132675170898,
                Longitude = -83.244743347168,
                DeviceType = DeviceType.Web.Description(),
                ActivityType = StatusUpdateType.DropOff.Description()
            };

            string tempFilePath = null;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Route4MeSDKUnitTest.Resources.test.png"))
            {
                var tempFiles = new TempFileCollection();
                {
                    tempFilePath = tempFiles.AddExtension("png");
                    System.Console.WriteLine(tempFilePath);
                    using (Stream fileStream = File.OpenWrite(tempFilePath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }

            // Run the query
            string errorString;
            string contents = "Test Note Contents with Attachment " + DateTime.Now.ToString();
            AddressNote note = route4Me.AddAddressNote(noteParameters, contents, tempFilePath, out errorString);

            Assert.IsNotNull(note, "AddAddressNoteWithFileTest failed... " + errorString);

        }

        public void GetAddressNotesTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string routeId = routeId_SingleDriverRoundTrip;
            Assert.IsNotNull(routeId, "routeId_SingleDriverRoundTrip is null...");

            int routeDestinationId = (dataObject2 != null && dataObject2.Routes != null && dataObject2.Routes.Length > 0 && dataObject2.Routes[0].Addresses.Length > 1 && dataObject2.Routes[0].Addresses[1].RouteDestinationId != null) ? dataObject2.Routes[0].Addresses[1].RouteDestinationId.Value : 0;

            NoteParameters noteParameters = new NoteParameters()
            {
                RouteId = routeId,
                AddressId = routeDestinationId
            };

            // Run the query
            string errorString;
            AddressNote[] notes = route4Me.GetAddressNotes(noteParameters, out errorString);

            Assert.IsInstanceOfType(notes, typeof(AddressNote[]), "GetAddressNotesTest failed... " + errorString);
        }

        public void AddOrdersToRouteTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            RouteParametersQuery rQueryParams = new RouteParametersQuery()
            {
                RouteId = routeId_SingleDriverRoundTrip,
                Redirect = false
            };

            #region Addresses
            Address[] addresses = new Address[] {
		        new Address {
			        AddressString = "273 Canal St, New York, NY 10013, USA",
			        Latitude = 40.7191558,
			        Longitude = -74.0011966,
			        Alias = "",
			        CurbsideLatitude = 40.7191558,
			        CurbsideLongitude = -74.0011966
		        },
		        new Address {
			        AddressString = "106 Liberty St, New York, NY 10006, USA",
			        Alias = "BK Restaurant #: 2446",
			        Latitude = 40.709637,
			        Longitude = -74.011912,
			        CurbsideLatitude = 40.709637,
			        CurbsideLongitude = -74.011912,
			        Email = "",
			        Phone = "(917) 338-1887",
			        FirstName = "",
			        LastName = "",
			        CustomFields = new Dictionary<string, string> { {
				        "icon",
				        null
			        } },
			        Time = 0,
			        OrderId = 7205705
		        },
		        new Address {
			        AddressString = "106 Fulton St, Farmingdale, NY 11735, USA",
			        Alias = "BK Restaurant #: 17871",
			        Latitude = 40.73073,
			        Longitude = -73.459283,
			        CurbsideLatitude = 40.73073,
			        CurbsideLongitude = -73.459283,
			        Email = "",
			        Phone = "(212) 566-5132",
			        FirstName = "",
			        LastName = "",
			        CustomFields = new Dictionary<string, string> { {
				        "icon",
				        null
			        } },
			        Time = 0,
			        OrderId = 7205703
		        }
	        };
            #endregion

            RouteParameters rParams = new RouteParameters()
            {
                RouteName = "Wednesday 15th of June 2016 07:01 PM (+03:00)",
                RouteDate = 1465948800,
                RouteTime = 14400,
                Optimize = "Time",
                RouteType = "single",
                AlgorithmType = AlgorithmType.TSP,
                RT = false,
                LockLast = false,
                MemberId = "1",
                VehicleId = "",
                DisableOptimization = false
            };

            string errorString;
            RouteResponse result = route4Me.AddOrdersToRoute(rQueryParams, addresses, rParams, out errorString);

            Assert.IsNotNull(result, "AddOrdersToRouteTest failed... " + errorString);
        }

        public void DeleteRoutesTest()
        {
            List<string> routeIdsToDelete = new List<string>();

            if (routeId_SingleDriverRoundTrip != null)
                routeIdsToDelete.Add(routeId_SingleDriverRoundTrip);

            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Run the query
            string errorString;
            string[] deletedRouteIds = route4Me.DeleteRoutes(routeIdsToDelete.ToArray(), out errorString);

            Assert.IsInstanceOfType(deletedRouteIds, typeof(string[]), "DeleteRoutesTest failed... " + errorString);
        }
    }

    [TestClass]
    public class OtherRouteTypesGroup
    {
        static string c_ApiKey = "11111111111111111111111111111111";

        static string routeId_MultipleDepotMultipleDriver;
        static string routeId_MultipleDepotMultipleDriverTimeWindow;
        static string routeId_SingleDepotMultipleDriverNoTimeWindow;
        static string routeId_MultipleDepotMultipleDriverWith24StopsTimeWindow;
        static string routeId_SingleDriverMultipleTimeWindows;

        static DataObject dataObject, dataObjectMDMD24;

        [TestMethod]
        public void MultipleDepotMultipleDriverTest()
        {
            // Create the manager with the api key
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Prepare the addresses
            Address[] addresses = new Address[]
              {
                #region Addresses

                new Address() { AddressString   = "3634 W Market St, Fairlawn, OH 44333",
                                //all possible originating locations are depots, should be marked as true
                                //stylistically we recommend all depots should be at the top of the destinations list
                                IsDepot         = true, 
                                Latitude        = 41.135762259364,
                                Longitude       = -81.629313826561,
                        
                                //the number of seconds at destination
                                Time            = 300,
                        
                                //together these two specify the time window of a destination
                                //seconds offset relative to the route start time for the open availability of a destination
                                TimeWindowStart = 28800,  
                        
                                //seconds offset relative to the route end time for the open availability of a destination
                                TimeWindowEnd   = 29465},

                new Address() { AddressString   = "1218 Ruth Ave, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.135762259364,
                                Longitude       = -81.629313826561,
                                Time            = 300,
                                TimeWindowStart = 29465,
                                TimeWindowEnd   = 30529},

                new Address() { AddressString   = "512 Florida Pl, Barberton, OH 44203",
                                Latitude        = 41.003671512008,
                                Longitude       = -81.598461046815,
                                Time            = 300,
                                TimeWindowStart = 30529,
                                TimeWindowEnd   = 33779},

                new Address() { AddressString   = "512 Florida Pl, Barberton, OH 44203",
                                Latitude        = 41.003671512008,
                                Longitude       = -81.598461046815,
                                Time            = 100,
                                TimeWindowStart = 33779,
                                TimeWindowEnd   = 33944},

                new Address() { AddressString   = "3495 Purdue St, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.162971496582,
                                Longitude       = -81.479049682617,
                                Time            = 300,
                                TimeWindowStart = 33944,
                                TimeWindowEnd   = 34801},

                new Address() { AddressString   = "1659 Hibbard Dr, Stow, OH 44224",
                                Latitude        = 41.194505989552,
                                Longitude       = -81.443351581693,
                                Time            = 300,
                                TimeWindowStart = 34801,
                                TimeWindowEnd   = 36366},

                new Address() { AddressString   = "2705 N River Rd, Stow, OH 44224",
                                Latitude        = 41.145240783691,
                                Longitude       = -81.410247802734,
                                Time            = 300,
                                TimeWindowStart = 36366,
                                TimeWindowEnd   = 39173},

                new Address() { AddressString   = "10159 Bissell Dr, Twinsburg, OH 44087",
                                Latitude        = 41.340042114258,
                                Longitude       = -81.421226501465,
                                Time            = 300,
                                TimeWindowStart = 39173,
                                TimeWindowEnd   = 41617},

                new Address() { AddressString   = "367 Cathy Dr, Munroe Falls, OH 44262",
                                Latitude        = 41.148578643799,
                                Longitude       = -81.429229736328,
                                Time            = 300,
                                TimeWindowStart = 41617,
                                TimeWindowEnd   = 43660},

                new Address() { AddressString   = "367 Cathy Dr, Munroe Falls, OH 44262",
                                Latitude        = 41.148578643799,
                                Longitude       = -81.429229736328,
                                Time            = 300,
                                TimeWindowStart = 43660,
                                TimeWindowEnd   = 46392},

                new Address() { AddressString   = "512 Florida Pl, Barberton, OH 44203",
                                Latitude        = 41.003671512008,
                                Longitude       = -81.598461046815,
                                Time            = 300,
                                TimeWindowStart = 46392,
                                TimeWindowEnd   = 48389},

                new Address() { AddressString   = "559 W Aurora Rd, Northfield, OH 44067",
                                Latitude        = 41.315116882324,
                                Longitude       = -81.558746337891,
                                Time            = 50,
                                TimeWindowStart = 48389,
                                TimeWindowEnd   = 48449},

                new Address() { AddressString   = "3933 Klein Ave, Stow, OH 44224",
                                Latitude        = 41.169467926025,
                                Longitude       = -81.429420471191,
                                Time            = 300,
                                TimeWindowStart = 48449,
                                TimeWindowEnd   = 50152},

                new Address() { AddressString   = "2148 8th St, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.136692047119,
                                Longitude       = -81.493492126465,
                                Time            = 300,
                                TimeWindowStart = 50152,
                                TimeWindowEnd   = 51982},

                new Address() { AddressString   = "3731 Osage St, Stow, OH 44224",
                                Latitude        = 41.161357879639,
                                Longitude       = -81.42293548584,
                                Time            = 100,
                                TimeWindowStart = 51982,
                                TimeWindowEnd   = 52180},

                new Address() { AddressString   = "3731 Osage St, Stow, OH 44224",
                                Latitude        = 41.161357879639,
                                Longitude       = -81.42293548584,
                                Time            = 300,
                                TimeWindowStart = 52180,
                                TimeWindowEnd   = 54379}

                #endregion
              };

            // Set parameters
            RouteParameters parameters = new RouteParameters()
            {
                //specify capacitated vehicle routing with time windows and multiple depots, with multiple drivers
                AlgorithmType = AlgorithmType.CVRP_TW_MD,

                //set an arbitrary route name
                //this value shows up in the website, and all the connected mobile device
                RouteName = "Multiple Depot, Multiple Driver",

                //the route start date in UTC, unix timestamp seconds (Tomorrow)
                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                //the time in UTC when a route is starting (7AM)
                RouteTime = 60 * 60 * 7,

                //the maximum duration of a route
                RouteMaxDuration = 86400,
                VehicleCapacity = "1",
                VehicleMaxDistanceMI = "10000",

                Optimize = Optimize.Distance.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description(),
                TravelMode = TravelMode.Driving.Description(),
                Metric = Metric.Geodesic
            };

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                Addresses = addresses,
                Parameters = parameters
            };

            // Run the query
            string errorString;
            dataObject = route4Me.RunOptimization(optimizationParameters, out errorString);

            routeId_MultipleDepotMultipleDriver = (dataObject != null && dataObject.Routes != null && dataObject.Routes.Length > 0) ? dataObject.Routes[0].RouteID : null;

            Assert.IsNotNull(dataObject, "MultipleDepotMultipleDriverTest failed... " + errorString);

            DeleteRoutesTest(routeId_MultipleDepotMultipleDriver);
        }

        [TestMethod]
        public void MultipleDepotMultipleDriverTimeWindowTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Prepare the addresses
            Address[] addresses = new Address[]
              {
                #region Addresses

                new Address() { AddressString   = "455 S 4th St, Louisville, KY 40202",
                                IsDepot         = true,
                                Latitude        = 38.251698,
                                Longitude       = -85.757308,
                                Time            = 300,
                                TimeWindowStart = 28800,
                                TimeWindowEnd   = 30477 },

                new Address() { AddressString   = "1604 PARKRIDGE PKWY, Louisville, KY, 40214",
                                Latitude        = 38.141598,
                                Longitude       = -85.793846,
                                Time            = 300,
                                TimeWindowStart = 30477,
                                TimeWindowEnd   = 33406 },

                new Address() { AddressString   = "1407 א53MCCOY, Louisville, KY, 40215",
                                Latitude        = 38.202496,
                                Longitude       = -85.786514,
                                Time            = 300,
                                TimeWindowStart = 33406,
                                TimeWindowEnd   = 36228 },
                new Address() { 
                                AddressString   = "4805 BELLEVUE AVE, Louisville, KY, 40215",
                                Latitude        = 38.178844,
                                Longitude       = -85.774864,
                                Time            = 300,
                                TimeWindowStart = 36228,
                                TimeWindowEnd   = 37518 },

                new Address() { AddressString   = "730 CECIL AVENUE, Louisville, KY, 40211",
                                Latitude        = 38.248684,
                                Longitude       = -85.821121,
                                Time            = 300,
                                TimeWindowStart = 37518,
                                TimeWindowEnd   = 39550 },

                new Address() { AddressString   = "650 SOUTH 29TH ST UNIT 315, Louisville, KY, 40211",
                                Latitude        = 38.251923,
                                Longitude       = -85.800034,
                                Time            = 300,
                                TimeWindowStart = 39550,
                                TimeWindowEnd   = 41348 },

                new Address() { AddressString   = "4629 HILLSIDE DRIVE, Louisville, KY, 40216",
                                Latitude        = 38.176067,
                                Longitude       = -85.824638,
                                Time            = 300,
                                TimeWindowStart = 41348,
                                TimeWindowEnd   = 42261 },

                new Address() { AddressString   = "4738 BELLEVUE AVE, Louisville, KY, 40215",
                                Latitude        = 38.179806,
                                Longitude       = -85.775558,
                                Time            = 300,
                                TimeWindowStart = 42261,
                                TimeWindowEnd   = 45195 },

                new Address() { AddressString   = "318 SO. 39TH STREET, Louisville, KY, 40212",
                                Latitude        = 38.259335,
                                Longitude       = -85.815094,
                                Time            = 300,
                                TimeWindowStart = 45195,
                                TimeWindowEnd   = 46549 },

                new Address() { AddressString   = "1324 BLUEGRASS AVE, Louisville, KY, 40215",
                                Latitude        = 38.179253,
                                Longitude       = -85.785118,
                                Time            = 300,
                                TimeWindowStart = 46549,
                                TimeWindowEnd   = 47353 },

                new Address() { AddressString   = "7305 ROYAL WOODS DR, Louisville, KY, 40214",
                                Latitude        = 38.162472,
                                Longitude       = -85.792854,
                                Time            = 300,
                                TimeWindowStart = 47353,
                                TimeWindowEnd   = 50924 },

                new Address() { AddressString   = "1661 W HILL ST, Louisville, KY, 40210",
                                Latitude        = 38.229584,
                                Longitude       = -85.783966,
                                Time            = 300,
                                TimeWindowStart = 50924,
                                TimeWindowEnd   = 51392 },

                new Address() { AddressString   = "3222 KINGSWOOD WAY, Louisville, KY, 40216",
                                Latitude        = 38.210606,
                                Longitude       = -85.822594,
                                Time            = 300,
                                TimeWindowStart = 51392,
                                TimeWindowEnd   = 52451 },

                new Address() { AddressString   = "1922 PALATKA RD, Louisville, KY, 40214",
                                Latitude        = 38.153767,
                                Longitude       = -85.796783,
                                Time            = 300,
                                TimeWindowStart = 52451,
                                TimeWindowEnd   = 55631 },

                new Address() { AddressString   = "1314 SOUTH 26TH STREET, Louisville, KY, 40210",
                                Latitude        = 38.235847,
                                Longitude       = -85.796852,
                                Time            = 300,
                                TimeWindowStart = 55631,
                                TimeWindowEnd   = 58516 },

                new Address() { AddressString   = "2135 MCCLOSKEY AVENUE, Louisville, KY, 40210",
                                Latitude        = 38.218662,
                                Longitude       = -85.789032,
                                Time            = 300,
                                TimeWindowStart = 58516,
                                TimeWindowEnd   = 61080 },

                new Address() { AddressString   = "1409 PHYLLIS AVE, Louisville, KY, 40215",
                                Latitude        = 38.206154,
                                Longitude       = -85.781387,
                                Time            = 100,
                                TimeWindowStart = 61080,
                                TimeWindowEnd   = 61504 },

                new Address() { AddressString   = "4504 SUNFLOWER AVE, Louisville, KY, 40216",
                                Latitude        = 38.187511,
                                Longitude       = -85.839149,
                                Time            = 300,
                                TimeWindowStart = 61504,
                                TimeWindowEnd   = 62061 },

                new Address() { AddressString   = "2512 GREENWOOD AVE, Louisville, KY, 40210",
                                Latitude        = 38.241405,
                                Longitude       = -85.795059,
                                Time            = 300,
                                TimeWindowStart = 62061,
                                TimeWindowEnd   = 65012 },

                new Address() { AddressString   = "5500 WILKE FARM AVE, Louisville, KY, 40216",
                                Latitude        = 38.166065,
                                Longitude       = -85.863319,
                                Time            = 300,
                                TimeWindowStart = 65012,
                                TimeWindowEnd   = 67541 },

                new Address() { AddressString   = "3640 LENTZ AVE, Louisville, KY, 40215",
                                Latitude        = 38.193283,
                                Longitude       = -85.786201,
                                Time            = 300,
                                TimeWindowStart = 67541,
                                TimeWindowEnd   = 69120 },

                new Address() { AddressString   = "1020 BLUEGRASS AVE, Louisville, KY, 40215",
                                Latitude        = 38.17952,
                                Longitude       = -85.780037,
                                Time            = 300,
                                TimeWindowStart = 69120,
                                TimeWindowEnd   = 70572 },

                new Address() { AddressString   = "123 NORTH 40TH ST, Louisville, KY, 40212",
                                Latitude        = 38.26498,
                                Longitude       = -85.814156,
                                Time            = 300,
                                TimeWindowStart = 70572,
                                TimeWindowEnd   = 73177 },

                new Address() { AddressString   = "7315 ST ANDREWS WOODS CIRCLE UNIT 104, Louisville, KY, 40214",
                                Latitude        = 38.151072,
                                Longitude       = -85.802867,
                                Time            = 300,
                                TimeWindowStart = 73177,
                                TimeWindowEnd   = 75231 },

                new Address() { AddressString   = "3210 POPLAR VIEW DR, Louisville, KY, 40216",
                                Latitude        = 38.182594,
                                Longitude       = -85.849937,
                                Time            = 300,
                                TimeWindowStart = 75231,
                                TimeWindowEnd   = 77663 },

                new Address() { AddressString   = "4519 LOUANE WAY, Louisville, KY, 40216",
                                Latitude        = 38.1754,
                                Longitude       = -85.811447,
                                Time            = 300,
                                TimeWindowStart = 77663,
                                TimeWindowEnd   = 79796 },

                new Address() { AddressString   = "6812 MANSLICK RD, Louisville, KY, 40214",
                                Latitude        = 38.161839,
                                Longitude       = -85.798279,
                                Time            = 300,
                                TimeWindowStart = 79796,
                                TimeWindowEnd   = 80813 },

                new Address() { AddressString   = "1524 HUNTOON AVENUE, Louisville, KY, 40215",
                                Latitude        = 38.172031,
                                Longitude       = -85.788353,
                                Time            = 300,
                                TimeWindowStart = 80813,
                                TimeWindowEnd   = 83956 },

                new Address() { AddressString   = "1307 LARCHMONT AVE, Louisville, KY, 40215",
                                Latitude        = 38.209663,
                                Longitude       = -85.779816,
                                Time            = 300,
                                TimeWindowStart = 83956,
                                TimeWindowEnd   = 84365 },

                new Address() { AddressString   = "434 N 26TH STREET #2, Louisville, KY, 40212",
                                Latitude        = 38.26844,
                                Longitude       = -85.791962,
                                Time            = 300,
                                TimeWindowStart = 84365,
                                TimeWindowEnd   = 85367 },

                new Address() { AddressString   = "678 WESTLAWN ST, Louisville, KY, 40211",
                                Latitude        = 38.250397,
                                Longitude       = -85.80629,
                                Time            = 300,
                                TimeWindowStart = 85367,
                                TimeWindowEnd   = 86400 },

                new Address() { AddressString   = "2308 W BROADWAY, Louisville, KY, 40211",
                                Latitude        = 38.248882,
                                Longitude       = -85.790421,
                                Time            = 300,
                                TimeWindowStart = 86400,
                                TimeWindowEnd   = 88703},

                new Address() { AddressString   = "2332 WOODLAND AVE, Louisville, KY, 40210",
                                Latitude        = 38.233579,
                                Longitude       = -85.794257,
                                Time            = 300,
                                TimeWindowStart = 88703,
                                TimeWindowEnd   = 89320 },

                new Address() { AddressString   = "1706 WEST ST. CATHERINE, Louisville, KY, 40210",
                                Latitude        = 38.239697,
                                Longitude       = -85.783928,
                                Time            = 300,
                                TimeWindowStart = 89320,
                                TimeWindowEnd   = 90054 },

                new Address() { AddressString   = "1699 WATHEN LN, Louisville, KY, 40216",
                                Latitude        = 38.216465,
                                Longitude       = -85.792397,
                                Time            = 300,
                                TimeWindowStart = 90054,
                                TimeWindowEnd   = 91150 },

                new Address() { AddressString   = "2416 SUNSHINE WAY, Louisville, KY, 40216",
                                Latitude        = 38.186245,
                                Longitude       = -85.831787,
                                Time            = 300,
                                TimeWindowStart = 91150,
                                TimeWindowEnd   = 91915 },

                new Address() { AddressString   = "6925 MANSLICK RD, Louisville, KY, 40214",
                                Latitude        = 38.158466,
                                Longitude       = -85.798355,
                                Time            = 300,
                                TimeWindowStart = 91915,
                                TimeWindowEnd   = 93407 },

                new Address() { AddressString   = "2707 7TH ST, Louisville, KY, 40215",
                                Latitude        = 38.212438,
                                Longitude       = -85.785082,
                                Time            = 300,
                                TimeWindowStart = 93407,
                                TimeWindowEnd   = 95992 },

                new Address() { AddressString   = "2014 KENDALL LN, Louisville, KY, 40216",
                                Latitude        = 38.179394,
                                Longitude       = -85.826668,
                                Time            = 300,
                                TimeWindowStart = 95992,
                                TimeWindowEnd   = 99307 },

                new Address() { AddressString   = "612 N 39TH ST, Louisville, KY, 40212",
                                Latitude        = 38.273354,
                                Longitude       = -85.812012,
                                Time            = 300,
                                TimeWindowStart = 99307,
                                TimeWindowEnd   = 102906 },

                new Address() { AddressString   = "2215 ROWAN ST, Louisville, KY, 40212",
                                Latitude        = 38.261703,
                                Longitude       = -85.786781,
                                Time            = 300,
                                TimeWindowStart = 102906,
                                TimeWindowEnd   = 106021 },

                new Address() { AddressString   = "1826 W. KENTUCKY ST, Louisville, KY, 40210",
                                Latitude        = 38.241611,
                                Longitude       = -85.78653,
                                Time            = 300,
                                TimeWindowStart = 106021,
                                TimeWindowEnd   = 107276 },

                new Address() { AddressString   = "1810 GREGG AVE, Louisville, KY, 40210",
                                Latitude        = 38.224716,
                                Longitude       = -85.796211,
                                Time            = 300,
                                TimeWindowStart = 107276,
                                TimeWindowEnd   = 107948 },

                new Address() { AddressString   = "4103 BURRRELL DRIVE, Louisville, KY, 40216",
                                Latitude        = 38.191753,
                                Longitude       = -85.825836,
                                Time            = 300,
                                TimeWindowStart = 107948,
                                TimeWindowEnd   = 108414 },

                new Address() { AddressString   = "359 SOUTHWESTERN PKWY, Louisville, KY, 40212",
                                Latitude        = 38.259903,
                                Longitude       = -85.823463,
                                Time            = 200,
                                TimeWindowStart = 108414,
                                TimeWindowEnd   = 108685 },

                new Address() { AddressString   = "2407 W CHESTNUT ST, Louisville, KY, 40211",
                                Latitude        = 38.252781,
                                Longitude       = -85.792109,
                                Time            = 300,
                                TimeWindowStart = 108685,
                                TimeWindowEnd   = 110109 },

                new Address() { AddressString   = "225 S 22ND ST, Louisville, KY, 40212",
                                Latitude        = 38.257616,
                                Longitude       = -85.786658,
                                Time            = 300,
                                TimeWindowStart = 110109,
                                TimeWindowEnd   = 111375 },

                new Address() { AddressString   = "1404 MCCOY AVE, Louisville, KY, 40215",
                                Latitude        = 38.202122,
                                Longitude       = -85.786072,
                                Time            = 300,
                                TimeWindowStart = 111375,
                                TimeWindowEnd   = 112120 },

                new Address() { AddressString   = "117 FOUNT LANDING CT, Louisville, KY, 40212",
                                Latitude        = 38.270061,
                                Longitude       = -85.799438,
                                Time            = 300,
                                TimeWindowStart = 112120,
                                TimeWindowEnd   = 114095 },

                new Address() { AddressString   = "5504 SHOREWOOD DRIVE, Louisville, KY, 40214",
                                Latitude        = 38.145851,
                                Longitude       = -85.7798,
                                Time            = 300,
                                TimeWindowStart = 114095,
                                TimeWindowEnd   = 115743 },

                new Address() { AddressString   = "1406 CENTRAL AVE, Louisville, KY, 40208",
                                Latitude        = 38.211025,
                                Longitude       = -85.780251,
                                Time            = 300,
                                TimeWindowStart = 115743,
                                TimeWindowEnd   = 117716 },

                new Address() { AddressString   = "901 W WHITNEY AVE, Louisville, KY, 40215",
                                Latitude        = 38.194115,
                                Longitude       = -85.77494,
                                Time            = 300,
                                TimeWindowStart = 117716,
                                TimeWindowEnd   = 119078 },

                new Address() { AddressString   = "2109 SCHAFFNER AVE, Louisville, KY, 40210",
                                Latitude        = 38.219699,
                                Longitude       = -85.779363,
                                Time            = 300,
                                TimeWindowStart = 119078,
                                TimeWindowEnd   = 121147 },

                new Address() { AddressString   = "2906 DIXIE HWY, Louisville, KY, 40216",
                                Latitude        = 38.209278,
                                Longitude       = -85.798653,
                                Time            = 300,
                                TimeWindowStart = 121147,
                                TimeWindowEnd   = 124281 },

                new Address() { AddressString   = "814 WWHITNEY AVE, Louisville, KY, 40215",
                                Latitude        = 38.193596,
                                Longitude       = -85.773521,
                                Time            = 300,
                                TimeWindowStart = 124281,
                                TimeWindowEnd   = 124675 },

                new Address() { AddressString   = "1610 ALGONQUIN PWKY, Louisville, KY, 40210",
                                Latitude        = 38.222153,
                                Longitude       = -85.784187,
                                Time            = 300,
                                TimeWindowStart = 124675,
                                TimeWindowEnd   = 127148 }, 

                new Address() { AddressString   = "3524 WHEELER AVE, Louisville, KY, 40215",
                                Latitude        = 38.195293,
                                Longitude       = -85.788643,
                                Time            = 300,
                                TimeWindowStart = 127148,
                                TimeWindowEnd   = 130667 },

                new Address() { AddressString   = "5009 NEW CUT RD, Louisville, KY, 40214",
                                Latitude        = 38.165905,
                                Longitude       = -85.779701,
                                Time            = 300,
                                TimeWindowStart = 130667,
                                TimeWindowEnd   = 131980 },

                new Address() { AddressString   = "3122 ELLIOTT AVE, Louisville, KY, 40211",
                                Latitude        = 38.251213,
                                Longitude       = -85.804199,
                                Time            = 300,
                                TimeWindowStart = 131980,
                                TimeWindowEnd   = 134402 },

                new Address() { AddressString   = "911 GAGEL AVE, Louisville, KY, 40216",
                                Latitude        = 38.173512,
                                Longitude       = -85.807854,
                                Time            = 300,
                                TimeWindowStart = 134402,
                                TimeWindowEnd   = 136787 },

                new Address() { AddressString   = "4020 GARLAND AVE #lOOA, Louisville, KY, 40211",
                                Latitude        = 38.246181,
                                Longitude       = -85.818901,
                                Time            = 300,
                                TimeWindowStart = 136787,
                                TimeWindowEnd   = 138073 },

                new Address() { AddressString   = "5231 MT HOLYOKE DR, Louisville, KY, 40216",
                                Latitude        = 38.169369,
                                Longitude       = -85.85704,
                                Time            = 300,
                                TimeWindowStart = 138073,
                                TimeWindowEnd   = 141407 },

                new Address() { AddressString   = "1339 28TH S #2, Louisville, KY, 40211",
                                Latitude        = 38.235275,
                                Longitude       = -85.800156,
                                Time            = 300,
                                TimeWindowStart = 141407,
                                TimeWindowEnd   = 143561 },

                new Address() { AddressString   = "836 S 36TH ST, Louisville, KY, 40211",
                                Latitude        = 38.24651,
                                Longitude       = -85.811234,
                                Time            = 300,
                                TimeWindowStart = 143561,
                                TimeWindowEnd   = 145941 },

                new Address() { AddressString   = "2132 DUNCAN STREET, Louisville, KY, 40212",
                                Latitude        = 38.262135,
                                Longitude       = -85.785172,
                                Time            = 300,
                                TimeWindowStart = 145941,
                                TimeWindowEnd   = 148296 },

                new Address() { AddressString   = "3529 WHEELER AVE, Louisville, KY, 40215",
                                Latitude        = 38.195057,
                                Longitude       = -85.787949,
                                Time            = 300,
                                TimeWindowStart = 148296,
                                TimeWindowEnd   = 150177 },

                new Address() { AddressString   = "2829 DE MEL #11, Louisville, KY, 40214",
                                Latitude        = 38.171662,
                                Longitude       = -85.807271,
                                Time            = 300,
                                TimeWindowStart = 150177,
                                TimeWindowEnd   = 150981 },

                new Address() { AddressString   = "1325 EARL AVENUE, Louisville, KY, 40215",
                                Latitude        = 38.204556,
                                Longitude       = -85.781555,
                                Time            = 300,
                                TimeWindowStart = 150981,
                                TimeWindowEnd   = 151854 },

                new Address() { AddressString   = "3632 MANSLICK RD #10, Louisville, KY, 40215",
                                Latitude        = 38.193542,
                                Longitude       = -85.801147,
                                Time            = 300,
                                TimeWindowStart = 151854,
                                TimeWindowEnd   = 152613 },

                new Address() { AddressString   = "637 S 41ST ST, Louisville, KY, 40211",
                                Latitude        = 38.253632,
                                Longitude       = -85.81897,
                                Time            = 300,
                                TimeWindowStart = 152613,
                                TimeWindowEnd   = 156131 },

                new Address() { AddressString   = "3420 VIRGINIA AVENUE, Louisville, KY, 40211",
                                Latitude        = 38.238693,
                                Longitude       = -85.811386,
                                Time            = 300,
                                TimeWindowStart = 156131,
                                TimeWindowEnd   = 157212 },

                new Address() { AddressString   = "3501 MALIBU CT APT 6, Louisville, KY, 40216",
                                Latitude        = 38.166481,
                                Longitude       = -85.825928,
                                Time            = 300,
                                TimeWindowStart = 157212,
                                TimeWindowEnd   = 158655 },

                new Address() { AddressString   = "4912 DIXIE HWY, Louisville, KY, 40216",
                                Latitude        = 38.170728,
                                Longitude       = -85.826817,
                                Time            = 300,
                                TimeWindowStart = 158655,
                                TimeWindowEnd   = 159145 },

                new Address() { AddressString   = "7720 DINGLEDELL RD, Louisville, KY, 40214",
                                Latitude        = 38.162472,
                                Longitude       = -85.792854,
                                Time            = 300,
                                TimeWindowStart = 159145,
                                TimeWindowEnd   = 161831 },

                new Address() { AddressString   = "2123 RATCLIFFE AVE, Louisville, KY, 40210",
                                Latitude        = 38.21978,
                                Longitude       = -85.797615,
                                Time            = 300,
                                TimeWindowStart = 161831,
                                TimeWindowEnd   = 163705 },

                new Address() { 
                                AddressString   = "1321 OAKWOOD AVE, Louisville, KY, 40215",
                                Latitude        = 38.17704,
                                Longitude       = -85.783829,
                                Time            = 300,
                                TimeWindowStart = 163705,
                                TimeWindowEnd   = 164953 },

                new Address() { AddressString   = "2223 WEST KENTUCKY STREET, Louisville, KY, 40210",
                                Latitude        = 38.242516,
                                Longitude       = -85.790695,
                                Time            = 300,
                                TimeWindowStart = 164953,
                                TimeWindowEnd   = 166189 },

                new Address() { AddressString   = "8025 GLIMMER WAY #3308, Louisville, KY, 40214",
                                Latitude        = 38.131981,
                                Longitude       = -85.77935,
                                Time            = 300,
                                TimeWindowStart = 166189,
                                TimeWindowEnd   = 166640 },

                new Address() { AddressString   = "1155 S 28TH ST, Louisville, KY, 40211",
                                Latitude        = 38.238621,
                                Longitude       = -85.799911,
                                Time            = 300,
                                TimeWindowStart = 166640,
                                TimeWindowEnd   = 168147 },

                new Address() { AddressString   = "840 IROQUOIS AVE, Louisville, KY, 40214",
                                Latitude        = 38.166355,
                                Longitude       = -85.779396,
                                Time            = 300,
                                TimeWindowStart = 168147,
                                TimeWindowEnd   = 170385
                },

                new Address() { AddressString   = "5573 BRUCE AVE, Louisville, KY, 40214",
                                Latitude        = 38.145222,
                                Longitude       = -85.779205,
                                Time            = 300,
                                TimeWindowStart = 170385,
                                TimeWindowEnd   = 171096 },

                new Address() { AddressString   = "1727 GALLAGHER, Louisville, KY, 40210",
                                Latitude        = 38.239334,
                                Longitude       = -85.784882,
                                Time            = 300,
                                TimeWindowStart = 171096,
                                TimeWindowEnd   = 171951 },

                new Address() { AddressString   = "1309 CATALPA ST APT 204, Louisville, KY, 40211",
                                Latitude        = 38.236524,
                                Longitude       = -85.801619,
                                Time            = 300,
                                TimeWindowStart = 171951,
                                TimeWindowEnd   = 172393 },

                new Address() { AddressString   = "1330 ALGONQUIN PKWY, Louisville, KY, 40208",
                                Latitude        = 38.219846,
                                Longitude       = -85.777344,
                                Time            = 300,
                                TimeWindowStart = 172393,
                                TimeWindowEnd   = 175337 },

                new Address() { AddressString   = "823 SUTCLIFFE, Louisville, KY, 40211",
                                Latitude        = 38.246956,
                                Longitude       = -85.811569,
                                Time            = 300,
                                TimeWindowStart = 175337,
                                TimeWindowEnd   = 176867 },

                new Address() { AddressString   = "4405 CHURCHMAN AVENUE #2, Louisville, KY, 40215",
                                Latitude        = 38.177768,
                                Longitude       = -85.792545,
                                Time            = 300,
                                TimeWindowStart = 176867,
                                TimeWindowEnd   = 178051 },

                new Address() { AddressString   = "3211 DUMESNIL ST #1, Louisville, KY, 40211",
                                Latitude        = 38.237789,
                                Longitude       = -85.807968,
                                Time            = 300,
                                TimeWindowStart = 178051,
                                TimeWindowEnd   = 179083 },

                new Address() { AddressString   = "3904 WEWOKA AVE, Louisville, KY, 40212",
                                Latitude        = 38.270367,
                                Longitude       = -85.813118,
                                Time            = 300,
                                TimeWindowStart = 179083,
                                TimeWindowEnd   = 181543 },

                new Address() { AddressString   = "660 SO. 42ND STREET, Louisville, KY, 40211",
                                Latitude        = 38.252865,
                                Longitude       = -85.822624,
                                Time            = 300,
                                TimeWindowStart = 181543,
                                TimeWindowEnd   = 184193 },

                new Address() { AddressString   = "3619  LENTZ  AVE, Louisville, KY, 40215",
                                Latitude        = 38.193249,
                                Longitude       = -85.785492,
                                Time            = 300,
                                TimeWindowStart = 184193,
                                TimeWindowEnd   = 185853 },

                new Address() { AddressString   = "4305  STOLTZ  CT, Louisville, KY, 40215",
                                Latitude        = 38.178707,
                                Longitude       = -85.787292,
                                Time            = 300,
                                TimeWindowStart = 185853,
                                TimeWindowEnd   = 187252 }

                #endregion
              };

            // Set parameters
            RouteParameters parameters = new RouteParameters()
            {
                AlgorithmType = AlgorithmType.CVRP_TW_MD,
                RouteName = "Multiple Depot, Multiple Driver, Time Window",
                StoreRoute = false,

                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                RouteTime = 60 * 60 * 7,
                RT = true,
                RouteMaxDuration = 86400 * 3,
                VehicleCapacity = "99",
                VehicleMaxDistanceMI = "99999",

                Optimize = Optimize.Time.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description(),
                TravelMode = TravelMode.Driving.Description(),
                Metric = Metric.Geodesic
            };

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                Addresses = addresses,
                Parameters = parameters
            };

            // Run the query
            string errorString;
            dataObject = route4Me.RunOptimization(optimizationParameters, out errorString);

            routeId_MultipleDepotMultipleDriverTimeWindow = (dataObject != null && dataObject.Routes != null && dataObject.Routes.Length > 0) ? dataObject.Routes[0].RouteID : null;

            Assert.IsNotNull(dataObject, "MultipleDepotMultipleDriverTimeWindowTest failed... " + errorString);

            DeleteRoutesTest(routeId_MultipleDepotMultipleDriverTimeWindow);
        }

        [TestMethod]
        public void SingleDepotMultipleDriverNoTimeWindowTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Prepare the addresses
            Address[] addresses = new Address[]
              {
                #region Addresses

                new Address() { AddressString   = "40 Mercer st, New York, NY",
                                IsDepot         = true,
                                Latitude        = 40.7213583,
                                Longitude       = -74.0013082,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york, ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Manhatten Island NYC",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "503 W139 St, NY,NY",
                                Latitude        = 40.7109062,
                                Longitude       = -74.0091848,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "203 grand st, new york, ny",
                                Latitude        = 40.7188990,
                                Longitude       = -73.9967320,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "119 Church Street",
                                Latitude        = 40.7137757,
                                Longitude       = -74.0088238,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "broadway street, new york",
                                Latitude        = 40.7191551,
                                Longitude       = -74.0020849,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Ground Zero, Vesey-Liberty-Church-West Streets New York NY 10038",
                                Latitude        = 40.7233126,
                                Longitude       = -74.0116602,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "226 ilyssa way staten lsland ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "185 franklin st.",
                                Latitude        = 40.7192099,
                                Longitude       = -74.0097670,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york city,",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "11 e. broaway 11038",
                                Latitude        = 40.7132060,
                                Longitude       = -73.9974019,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Brooklyn Bridge, NY",
                                Latitude        = 40.7053804,
                                Longitude       = -73.9962503,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "World Trade Center Site, NY",
                                Latitude        = 40.7114980,
                                Longitude       = -74.0122990,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York Stock Exchange, NY",
                                Latitude        = 40.7074242,
                                Longitude       = -74.0116342,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Wall Street, NY",
                                Latitude        = 40.7079825,
                                Longitude       = -74.0079781,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Trinity Church, NY",
                                Latitude        = 40.7081426,
                                Longitude       = -74.0120511,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "World Financial Center, NY",
                                Latitude        = 40.7104750,
                                Longitude       = -74.0154930,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Federal Hall, NY",
                                Latitude        = 40.7073034,
                                Longitude       = -74.0102734,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Flatiron Building, NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "South Street Seaport, NY",
                                Latitude        = 40.7069210,
                                Longitude       = -74.0036380,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Rockefeller Center, NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "FAO Schwarz, NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Woolworth Building, NY",
                                Latitude        = 40.7123903,
                                Longitude       = -74.0083309,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Met Life Building, NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "SOHO/Tribeca, NY",
                                Latitude        = 40.7185650,
                                Longitude       = -74.0120170,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "MacyГўв‚¬в„ўs, NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "City Hall, NY, NY",
                                Latitude        = 40.7127047,
                                Longitude       = -74.0058663,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Macy&amp;acirc;в‚¬в„ўs, NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "1452 potter blvd bayshore ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "55 Church St. New York, NY",
                                Latitude        = 40.7112320,
                                Longitude       = -74.0102680,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "55 Church St, New York, NY",
                                Latitude        = 40.7112320,
                                Longitude       = -74.0102680,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "79 woodlawn dr revena ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "135 main st revena ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "250 greenwich st, new york, ny",
                                Latitude        = 40.7131590,
                                Longitude       = -74.0118890,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "79 grand, new york, ny",
                                Latitude        = 40.7216958,
                                Longitude       = -74.0024352,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "World trade center\n",
                                Latitude        = 40.7116260,
                                Longitude       = -74.0107140,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "World trade centern",
                                Latitude        = 40.7132910,
                                Longitude       = -74.0118350,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "391 broadway new york",
                                Latitude        = 40.7183693,
                                Longitude       = -74.0027800,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Fletcher street",
                                Latitude        = 40.7063954,
                                Longitude       = -74.0056353,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "2 Plum LanenPlainview New York",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "50 Kennedy drivenPlainview New York",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "7 Crestwood DrivenPlainview New York",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "85 west street nyc",
                                Latitude        = 40.7096460,
                                Longitude       = -74.0146140,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York, New York",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "89 Reade St, New York City, New York 10013",
                                Latitude        = 40.7142970,
                                Longitude       = -74.0059660,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "100 white st",
                                Latitude        = 40.7172477,
                                Longitude       = -74.0014351,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "100 white st\n33040",
                                Latitude        = 40.7172477,
                                Longitude       = -74.0014351,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Canal st and mulberry",
                                Latitude        = 40.7170880,
                                Longitude       = -73.9986025,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "91-83 111st st\nRichmond hills ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "122-09 liberty avenOzone park ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "80-16 101 avenOzone park ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "6302 woodhaven blvdnRego park ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "39-02 64th stnWoodside ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York City, NY,",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Pine st",
                                Latitude        = 40.7069754,
                                Longitude       = -74.0089557,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Wall st",
                                Latitude        = 40.7079825,
                                Longitude       = -74.0079781,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "32 avenue of the Americas, NY, NY",
                                Latitude        = 40.7201140,
                                Longitude       = -74.0050920,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "260 west broadway, NY, NY",
                                Latitude        = 40.7206210,
                                Longitude       = -74.0055670,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Long island, ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "27 Carley ave\nHuntington ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "17 west neck RdnHuntington ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "206 washington st",
                                Latitude        = 40.7131577,
                                Longitude       = -74.0126091,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Cipriani new york",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Byshnell Basin. NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "89 Reade St, New York, New York 10013",
                                Latitude        = 40.7142970,
                                Longitude       = -74.0059660,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "250 Greenwich St, New York, New York 10007",
                                Latitude        = 40.7133000,
                                Longitude       = -74.0120000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "64 Bowery, New York, New York 10013",
                                Latitude        = 40.7165540,
                                Longitude       = -73.9962700,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "142-156 Mulberry St, New York, New York 10013",
                                Latitude        = 40.7192764,
                                Longitude       = -73.9973096,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "80 Spring St, New York, New York 10012",
                                Latitude        = 40.7226590,
                                Longitude       = -73.9981820,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "182 Duane street ny",
                                Latitude        = 40.7170879,
                                Longitude       = -74.0101210,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "182 Duane St, New York, New York 10013",
                                Latitude        = 40.7170879,
                                Longitude       = -74.0101210,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "462 broome street nyc",
                                Latitude        = 40.7225800,
                                Longitude       = -74.0008980,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "117 mercer street nyc",
                                Latitude        = 40.7239679,
                                Longitude       = -73.9991585,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Lucca antiques\n182 Duane St, New York, New York 10013",
                                Latitude        = 40.7167516,
                                Longitude       = -74.0087482,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Room and board\n105 Wooster street nyc",
                                Latitude        = 40.7229097,
                                Longitude       = -74.0021852,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Lucca antiquesn182 Duane St, New York, New York 10013",
                                Latitude        = 40.7167516,
                                Longitude       = -74.0087482,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Room and boardn105 Wooster street nyc",
                                Latitude        = 40.7229097,
                                Longitude       = -74.0021852,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Lucca antiques 182 Duane st new York ny",
                                Latitude        = 40.7170879,
                                Longitude       = -74.0101210,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Property\n14 Wooster street nyc",
                                Latitude        = 40.7229097,
                                Longitude       = -74.0021852,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "101 Crosby street nyc",
                                Latitude        = 40.7235730,
                                Longitude       = -73.9969540,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Room and board \n105 Wooster street nyc",
                                Latitude        = 40.7229097,
                                Longitude       = -74.0021852,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Propertyn14 Wooster street nyc",
                                Latitude        = 40.7229097,
                                Longitude       = -74.0021852,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Room and board n105 Wooster street nyc",
                                Latitude        = 40.7229097,
                                Longitude       = -74.0021852,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Mecox gardens\n926 Lexington nyc",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "25 sybil&apos;s crossing Kent lakes, ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "10149 ASHDALE LANE\t67\t67393253\t\t\tSANTEE\tCA\t92071\t\t280501691\t67393253\tIFI\t280501691\t05-JUN-10\t67393253",
                                Latitude        = 40.7143000,
                                Longitude       = -74.0067000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "193 Lakebridge Dr, Kings Paark, NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "219 west creek",
                                Latitude        = 40.7198564,
                                Longitude       = -74.0121098,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "14 North Moore Street\nNew York, ny",
                                Latitude        = 40.7196970,
                                Longitude       = -74.0066100,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "14 North Moore StreetnNew York, ny",
                                Latitude        = 40.7196970,
                                Longitude       = -74.0066100,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "14 North Moore Street New York, ny",
                                Latitude        = 40.7196970,
                                Longitude       = -74.0066100,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "30-38 Fulton St, New York, New York 10038",
                                Latitude        = 40.7077737,
                                Longitude       = -74.0043299,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "73 Spring Street Ny NY",
                                Latitude        = 40.7225378,
                                Longitude       = -73.9976742,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "119 Mercer Street Ny NY",
                                Latitude        = 40.7241390,
                                Longitude       = -73.9993110,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "525 Broadway Ny NY",
                                Latitude        = 40.7230410,
                                Longitude       = -73.9991650,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Church St",
                                Latitude        = 40.7154338,
                                Longitude       = -74.0075430,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "135 union stnWatertown ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "21101 coffeen stnWatertown ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "215 Washington stnWatertown ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "619 mill stnWatertown ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "3 canel st, new York, ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york city new york",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "50 grand street",
                                Latitude        = 40.7225780,
                                Longitude       = -74.0038019,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Orient ferry, li ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Hilton hotel river head li ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "116 park pl",
                                Latitude        = 40.7140565,
                                Longitude       = -74.0110155,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "long islans new york",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "1 prospect pointe niagra falls ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York City\tNY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "pink berry ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York City\t NY",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "10108",
                                Latitude        = 40.7143000,
                                Longitude       = -74.0067000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Ann st",
                                Latitude        = 40.7105937,
                                Longitude       = -74.0073715,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Hok 620 ave of Americas new York ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Som 14 wall st nyc",
                                Latitude        = 40.7076179,
                                Longitude       = -74.0107630,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York ,ny",
                                Latitude        = 40.7142691,
                                Longitude       = -74.0059729,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "52 prince st. 10012",
                                Latitude        = 40.7235840,
                                Longitude       = -73.9961170,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "451 broadway 10013",
                                Latitude        = 40.7205177,
                                Longitude       = -74.0009557,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Dover street",
                                Latitude        = 40.7087886,
                                Longitude       = -74.0008644,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Murray st",
                                Latitude        = 40.7148929,
                                Longitude       = -74.0113349,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "85 West St, New York, New York",
                                Latitude        = 40.7096460,
                                Longitude       = -74.0146140,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },
                //125left
                new Address() { AddressString   = "NYC",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "64 trinity place, ny, ny",
                                Latitude        = 40.7081649,
                                Longitude       = -74.0127168,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "150 broadway ny ny",
                                Latitude        = 40.7091850,
                                Longitude       = -74.0100330,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Pinegrove Dude Ranch 31 cherrytown Rd Kerhinkson Ny",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Front street",
                                Latitude        = 40.7063990,
                                Longitude       = -74.0045493,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "234 canal St new York, NY 10013",
                                Latitude        = 40.7177010,
                                Longitude       = -73.9999570,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "72 spring street, new york ny 10012",
                                Latitude        = 40.7225093,
                                Longitude       = -73.9976540,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "150 spring street, new york, ny 10012",
                                Latitude        = 40.7242393,
                                Longitude       = -74.0014922,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "580 broadway street, new york, ny 10012",
                                Latitude        = 40.7244210,
                                Longitude       = -73.9970260,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "42 trinity place, new york, ny 10007",
                                Latitude        = 40.7074000,
                                Longitude       = -74.0135510,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "baco ny",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Micro Tel Inn Alburn New York",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "20 Cedar Close",
                                Latitude        = 40.7068734,
                                Longitude       = -74.0078613,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "South street",
                                Latitude        = 40.7080184,
                                Longitude       = -73.9999414,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "47 Lafayette street",
                                Latitude        = 40.7159204,
                                Longitude       = -74.0027332,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Newyork",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Ground Zero, NY",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "GROUND ZERO NY",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "33400 SE Harrison",
                                Latitude        = 40.7188400,
                                Longitude       = -74.0103330,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york, new york",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "8 Greene St, New York, 10013",
                                Latitude        = 40.7206160,
                                Longitude       = -74.0027600,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "226 w 44st new york city",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "s street seaport 11 fulton st new york city",
                                Latitude        = 40.7069150,
                                Longitude       = -74.0033215,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "30 Rockefeller Plaza w 49th St New York City",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "30 Rockefeller Plaza 50th St New York City",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "S. Street Seaport 11 Fulton St. New York City",
                                Latitude        = 40.7069150,
                                Longitude       = -74.0033215,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "30 rockefeller plaza w 49th st, new york city",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "30 rockefeller plaza 50th st, new york city",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "11 fulton st, new york city",
                                Latitude        = 40.7069150,
                                Longitude       = -74.0033215,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york city ny",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Big apple",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Ny",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York new York",
                                Latitude        = 40.7143528,
                                Longitude       = -74.0059731,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "83-85 Chambers St, New York, New York 10007",
                                Latitude        = 40.7148130,
                                Longitude       = -74.0068890,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York",
                                Latitude        = 40.7145502,
                                Longitude       = -74.0071249,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "102 North End Ave NY, NY",
                                Latitude        = 40.7147980,
                                Longitude       = -74.0159690,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "57 Thompson St, New York, New York 10012",
                                Latitude        = 40.7241400,
                                Longitude       = -74.0035860,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york city",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071250,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "nyc, ny",
                                Latitude        = 40.7145502,
                                Longitude       = -74.0071249,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York NY",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071250,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "285 West Broadway New York, NY 10013",
                                Latitude        = 40.7208750,
                                Longitude       = -74.0046310,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "100 avenue of the americas New York, NY 10013",
                                Latitude        = 40.7233120,
                                Longitude       = -74.0043950,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "270 Lafeyette st New York, NY 10012",
                                Latitude        = 40.7238790,
                                Longitude       = -73.9965270,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "560 Broadway New York, NY 10012",
                                Latitude        = 40.7238540,
                                Longitude       = -73.9974980,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "42 Wooster St New York, NY 10013",
                                Latitude        = 40.7223860,
                                Longitude       = -74.0024220,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "42 Wooster StreetNew York, NY 10013-2230",
                                Latitude        = 40.7223633,
                                Longitude       = -74.0026240,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "504 Broadway, New York, NY 10012",
                                Latitude        = 40.7221444,
                                Longitude       = -73.9992714,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "426 Broome Street, New York, NY 10013",
                                Latitude        = 40.7213295,
                                Longitude       = -73.9987121,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "City hall, nyc",
                                Latitude        = 40.7122066,
                                Longitude       = -74.0055026,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "South street seaport, nyc",
                                Latitude        = 40.7069501,
                                Longitude       = -74.0030848,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Ground zero, nyc",
                                Latitude        = 40.7116410,
                                Longitude       = -74.0122530,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Ground zero",
                                Latitude        = 40.7116410,
                                Longitude       = -74.0122530,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Mulberry and canal, NYC",
                                Latitude        = 40.7170900,
                                Longitude       = -73.9985900,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "World Trade Center, NYC",
                                Latitude        = 40.7116670,
                                Longitude       = -74.0125000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "South Street Seaport",
                                Latitude        = 40.7069501,
                                Longitude       = -74.0030848,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Wall Street and Nassau Street, NYC",
                                Latitude        = 40.7071400,
                                Longitude       = -74.0106900,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Trinity Church, NYC",
                                Latitude        = 40.7081269,
                                Longitude       = -74.0125691,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Federal Hall National Memorial",
                                Latitude        = 40.7069515,
                                Longitude       = -74.0101638,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Little Italy, NYC",
                                Latitude        = 40.7196920,
                                Longitude       = -73.9977650,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York, NY",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071250,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York City, NY,",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071250,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york,ny",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071300,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Odeon cinema",
                                Latitude        = 40.7168300,
                                Longitude       = -74.0080300,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York City",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071300,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "52 broadway, ny,ny 1004",
                                Latitude        = 40.7065000,
                                Longitude       = -74.0123000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "52 broadway, ny,ny 10004",
                                Latitude        = 40.7065000,
                                Longitude       = -74.0123000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "22 beaver st, ny,ny 10004",
                                Latitude        = 40.7048200,
                                Longitude       = -74.0121800,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "54 pine st,ny,ny 10005",
                                Latitude        = 40.7068600,
                                Longitude       = -74.0084900,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "114 liberty st, ny,ny 10006",
                                Latitude        = 40.7097700,
                                Longitude       = -74.0122000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "215 canal st,ny,ny 10013",
                                Latitude        = 40.7174700,
                                Longitude       = -73.9989500,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york city ny",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071300,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "World Trade Center, New York, NY",
                                Latitude        = 40.7116700,
                                Longitude       = -74.0125000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Chinatown, New York, NY",
                                Latitude        = 40.7159600,
                                Longitude       = -73.9974100,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "101 murray street new york, ny",
                                Latitude        = 40.7152600,
                                Longitude       = -74.0125100,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "nyc",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071200,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "510 broadway new york",
                                Latitude        = 40.7223400,
                                Longitude       = -73.9990160,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "nyc",
                                Latitude        = 40.7145502,
                                Longitude       = -74.0071249,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Little Italy",
                                Latitude        = 40.7196920,
                                Longitude       = -73.9977647,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "463 Broadway, New York, NY",
                                Latitude        = 40.7210590,
                                Longitude       = -74.0006880,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "222 West Broadway, New York, NY",
                                Latitude        = 40.7193520,
                                Longitude       = -74.0064170,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "270 Lafayette street new York new york",
                                Latitude        = 40.7238790,
                                Longitude       = -73.9965270,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "New York, NY USA",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071250,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "97 Kenmare Street, New York, NY 10012",
                                Latitude        = 40.7214370,
                                Longitude       = -73.9969110,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "19 Beekman St, New York, New York 10038",
                                Latitude        = 40.7107540,
                                Longitude       = -74.0062870,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Soho",
                                Latitude        = 40.7241404,
                                Longitude       = -74.0020213,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Bergen, New York",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071250,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "478 Broadway, NY, NY",
                                Latitude        = 40.7213360,
                                Longitude       = -73.9997710,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "555 broadway, ny, ny",
                                Latitude        = 40.7238830,
                                Longitude       = -73.9982960,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "375 West Broadway, NY, NY",
                                Latitude        = 40.7235000,
                                Longitude       = -74.0026020,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "35 howard st, NY, NY",
                                Latitude        = 40.7195240,
                                Longitude       = -74.0010300,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Pier 17 NYC",
                                Latitude        = 40.7063660,
                                Longitude       = -74.0026890,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "120 Liberty St NYC",
                                Latitude        = 40.7097740,
                                Longitude       = -74.0124510,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "80 White Street, NY, NY",
                                Latitude        = 40.7178340,
                                Longitude       = -74.0020520,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Manhattan, NY",
                                Latitude        = 40.7144300,
                                Longitude       = -74.0061000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "22 read st, ny",
                                Latitude        = 40.7142010,
                                Longitude       = -74.0044910,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "130 Mulberry St, New York, NY 10013-5547",
                                Latitude        = 40.7182880,
                                Longitude       = -73.9977110,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "new york city, ny",
                                Latitude        = 40.7145500,
                                Longitude       = -74.0071250,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "10038",
                                Latitude        = 40.7092119,
                                Longitude       = -74.0033631,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "11 Wall St, New York, NY 10005-1905",
                                Latitude        = 40.7072900,
                                Longitude       = -74.0112010,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "89 Reade St, New York, New York 10007",
                                Latitude        = 40.7134560,
                                Longitude       = -74.0034990,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "265 Canal St, New York, NY 10013-6010",
                                Latitude        = 40.7188850,
                                Longitude       = -74.0009000,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "39 Broadway, New York, NY 10006-3003",
                                Latitude        = 40.7133450,
                                Longitude       = -73.9961320,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "25 beaver street new york ny",
                                Latitude        = 40.7051110,
                                Longitude       = -74.0120070,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "100 church street new york ny",
                                Latitude        = 40.7130430,
                                Longitude       = -74.0096370,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "69 Mercer St, New York, NY 10012-4440",
                                Latitude        = 40.7226490,
                                Longitude       = -74.0006100,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "111 Worth St, New York, NY 10013-4008",
                                Latitude        = 40.7159210,
                                Longitude       = -74.0034100,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "240-248 Broadway, New York, New York 10038",
                                Latitude        = 40.7127690,
                                Longitude       = -74.0076810,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "12 Maiden Ln, New York, NY 10038-4002",
                                Latitude        = 40.7094460,
                                Longitude       = -74.0095760,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "291 Broadway, New York, NY 10007-1814",
                                Latitude        = 40.7150000,
                                Longitude       = -74.0061340,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "55 Liberty St, New York, NY 10005-1003",
                                Latitude        = 40.7088430,
                                Longitude       = -74.0093840,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "Brooklyn Bridge, NY",
                                Latitude        = 40.7063440,
                                Longitude       = -73.9974390,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "wall street",
                                Latitude        = 40.7063889,
                                Longitude       = -74.0094444,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "south street seaport, ny",
                                Latitude        = 40.7069501,
                                Longitude       = -74.0030848,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "little italy, ny",
                                Latitude        = 40.7196920,
                                Longitude       = -73.9977647,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "47 Pine St, New York, NY 10005-1513",
                                Latitude        = 40.7067340,
                                Longitude       = -74.0089280,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "22 cortlandt street new york ny",
                                Latitude        = 40.7100820,
                                Longitude       = -74.0102510,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "105 reade street new york ny",
                                Latitude        = 40.7156330,
                                Longitude       = -74.0085220,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "2 lafayette street new york ny",
                                Latitude        = 40.7140310,
                                Longitude       = -74.0038910,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "53 crosby street new york ny",
                                Latitude        = 40.7219770,
                                Longitude       = -73.9982450,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "2 Lafayette St, New York, NY 10007-1307",
                                Latitude        = 40.7140310,
                                Longitude       = -74.0038910,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "105 Reade St, New York, NY 10013-3840",
                                Latitude        = 40.7156330,
                                Longitude       = -74.0085220,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "chinatown, ny",
                                Latitude        = 40.7159556,
                                Longitude       = -73.9974133,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "250 Broadway, New York, NY 10007-2516",
                                Latitude        = 40.7130180,
                                Longitude       = -74.0074700,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "156 William St, New York, NY 10038-2609",
                                Latitude        = 40.7097970,
                                Longitude       = -74.0055770,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "100 Church St, New York, NY 10007-2601",
                                Latitude        = 40.7130430,
                                Longitude       = -74.0096370,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null },

                new Address() { AddressString   = "33 Beaver St, New York, NY 10004-2736",
                                Latitude        = 40.7050980,
                                Longitude       = -74.0117200,
                                Time            = 0,
                                TimeWindowStart = null,
                                TimeWindowEnd   = null }

                #endregion
              };

            // Set parameters
            RouteParameters parameters = new RouteParameters()
            {
                AlgorithmType = AlgorithmType.CVRP_TW_MD,
                RouteName = "Single Depot, Multiple Driver, No Time Window",
                StoreRoute = false,

                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                RouteTime = 60 * 60 * 7,
                RT = true,
                RouteMaxDuration = 86400,
                VehicleCapacity = "20",
                VehicleMaxDistanceMI = "99999",
                Parts = 4,

                Optimize = Optimize.Time.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description(),
                TravelMode = TravelMode.Driving.Description(),
                Metric = Metric.Geodesic
            };

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                Addresses = addresses,
                Parameters = parameters
            };

            // Run the query
            string errorString;
            dataObject = route4Me.RunOptimization(optimizationParameters, out errorString);

            routeId_SingleDepotMultipleDriverNoTimeWindow = (dataObject != null && dataObject.Routes != null && dataObject.Routes.Length > 0) ? dataObject.Routes[0].RouteID : null;

            //Note: The addresses of this test aren't permited for api_key=11111111111111111111111111111111. 
            // If you put in the parameter api_key your valid key, the test will be finished successfuly.

            Assert.IsNull(dataObject, "SingleDepotMultipleDriverNoTimeWindowTest failed... " + errorString);

            //DeleteRoutesTest(routeId_SingleDepotMultipleDriverNoTimeWindow);
        }

        [TestMethod]
        public void MultipleDepotMultipleDriverWith24StopsTimeWindowTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Prepare the addresses
            Address[] addresses = new Address[]
              {
                #region Addresses

                new Address() { AddressString   = "3634 W Market St, Fairlawn, OH 44333",
                                IsDepot         = true,
                                Latitude        = 41.135762259364,
                                Longitude       = -81.629313826561,
                                Time            = 300,
                                TimeWindowStart = 28800,
                                TimeWindowEnd   = 29465 },

                new Address() { AddressString   = "1218 Ruth Ave, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.143505096435,
                                Longitude       = -81.46549987793,
                                Time            = 300,
                                TimeWindowStart = 29465,
                                TimeWindowEnd   = 30529 },

                new Address() { AddressString   = "512 Florida Pl, Barberton, OH 44203",
                                Latitude        = 41.003671512008,
                                Longitude       = -81.598461046815,
                                Time            = 300,
                                TimeWindowStart = 30529,
                                TimeWindowEnd   = 33479 },

                new Address() { AddressString   = "512 Florida Pl, Barberton, OH 44203",
                                Latitude        = 41.003671512008,
                                Longitude       = -81.598461046815,
                                Time            = 300,
                                TimeWindowStart = 33479,
                                TimeWindowEnd   = 33944 },

                new Address() { AddressString   = "3495 Purdue St, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.162971496582,
                                Longitude       = -81.479049682617,
                                Time            = 300,
                                TimeWindowStart = 33944,
                                TimeWindowEnd   = 34801 },

                new Address() { AddressString   = "1659 Hibbard Dr, Stow, OH 44224",
                                Latitude        = 41.194505989552,
                                Longitude       = -81.443351581693,
                                Time            = 300,
                                TimeWindowStart = 34801,
                                TimeWindowEnd   = 36366 },

                new Address() { AddressString   = "2705 N River Rd, Stow, OH 44224",
                                Latitude        = 41.145240783691,
                                Longitude       = -81.410247802734,
                                Time            = 300,
                                TimeWindowStart = 36366,
                                TimeWindowEnd   = 39173 },

                new Address() { AddressString   = "10159 Bissell Dr, Twinsburg, OH 44087",
                                Latitude        = 41.340042114258,
                                Longitude       = -81.421226501465,
                                Time            = 300,
                                TimeWindowStart = 39173,
                                TimeWindowEnd   = 41617 },

                new Address() { AddressString   = "367 Cathy Dr, Munroe Falls, OH 44262",
                                Latitude        = 41.148578643799,
                                Longitude       = -81.429229736328,
                                Time            = 300,
                                TimeWindowStart = 41617,
                                TimeWindowEnd   = 43660 },

                new Address() { AddressString   = "367 Cathy Dr, Munroe Falls, OH 44262",
                                Latitude        = 41.148579,
                                Longitude       = -81.42923,
                                Time            = 300,
                                TimeWindowStart = 43660,
                                TimeWindowEnd   = 46392 },

                new Address() { AddressString   = "512 Florida Pl, Barberton, OH 44203",
                                Latitude        = 41.003671512008,
                                Longitude       = -81.598461046815,
                                Time            = 300,
                                TimeWindowStart = 46392,
                                TimeWindowEnd   = 48089 },

                new Address() { AddressString   = "559 W Aurora Rd, Northfield, OH 44067",
                                Latitude        = 41.315116882324,
                                Longitude       = -81.558746337891,
                                Time            = 300,
                                TimeWindowStart = 48089,
                                TimeWindowEnd   = 48449 },

                new Address() { AddressString   = "3933 Klein Ave, Stow, OH 44224",
                                Latitude        = 41.169467926025,
                                Longitude       = -81.429420471191,
                                Time            = 300,
                                TimeWindowStart = 48449,
                                TimeWindowEnd   = 50152 },

                new Address() { AddressString   = "2148 8th St, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.136692047119,
                                Longitude       = -81.493492126465,
                                Time            = 300,
                                TimeWindowStart = 50152,
                                TimeWindowEnd   = 51682 },

                new Address() { AddressString   = "3731 Osage St, Stow, OH 44224",
                                Latitude        = 41.161357879639,
                                Longitude       = -81.42293548584,
                                Time            = 300,
                                TimeWindowStart = 51682,
                                TimeWindowEnd   = 54379 },

                new Address() { AddressString   = "3862 Klein Ave, Stow, OH 44224",
                                Latitude        = 41.167895123363,
                                Longitude       = -81.429973393679,
                                Time            = 300,
                                TimeWindowStart = 54379,
                                TimeWindowEnd   = 54879 },

                new Address() { AddressString   = "138 Northwood Ln, Tallmadge, OH 44278",
                                Latitude        = 41.085464134812,
                                Longitude       = -81.447411775589,
                                Time            = 300,
                                TimeWindowStart = 54879,
                                TimeWindowEnd   = 56613 },

                new Address() { AddressString   = "3401 Saratoga Blvd, Stow, OH 44224",
                                Latitude        = 41.148849487305,
                                Longitude       = -81.407363891602,
                                Time            = 300,
                                TimeWindowStart = 56613,
                                TimeWindowEnd   = 57052 },

                new Address() { AddressString   = "5169 Brockton Dr, Stow, OH 44224",
                                Latitude        = 41.195003509521,
                                Longitude       = -81.392700195312,
                                Time            = 300,
                                TimeWindowStart = 57052,
                                TimeWindowEnd   = 59004 },

                new Address() { AddressString   = "5169 Brockton Dr, Stow, OH 44224",
                                Latitude        = 41.195003509521,
                                Longitude       = -81.392700195312,
                                Time            = 300,
                                TimeWindowStart = 59004,
                                TimeWindowEnd   = 60027 },

                new Address() { AddressString   = "458 Aintree Dr, Munroe Falls, OH 44262",
                                Latitude        = 41.1266746521,
                                Longitude       = -81.445808410645,
                                Time            = 300,
                                TimeWindowStart = 60027,
                                TimeWindowEnd   = 60375 },

                new Address() { AddressString   = "512 Florida Pl, Barberton, OH 44203",
                                Latitude        = 41.003671512008,
                                Longitude       = -81.598461046815,
                                Time            = 300,
                                TimeWindowStart = 60375,
                                TimeWindowEnd   = 63891 },

                new Address() { AddressString   = "2299 Tyre Dr, Hudson, OH 44236",
                                Latitude        = 41.250511169434,
                                Longitude       = -81.420433044434,
                                Time            = 300,
                                TimeWindowStart = 63891,
                                TimeWindowEnd   = 65277 },

                new Address() { AddressString   = "2148 8th St, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.136692047119,
                                Longitude       = -81.493492126465,
                                Time            = 300,
                                TimeWindowStart = 65277,
                                TimeWindowEnd   = 68545 }

                #endregion
              };

            // Set parameters
            RouteParameters parameters = new RouteParameters()
            {
                AlgorithmType = AlgorithmType.CVRP_TW_MD,
                RouteName = "Multiple Depot, Multiple Driver with 24 Stops, Time Window",
                StoreRoute = false,

                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                RouteTime = 60 * 60 * 7,
                RouteMaxDuration = 86400,
                VehicleCapacity = "1",
                VehicleMaxDistanceMI = "10000",

                Optimize = Optimize.Distance.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description(),
                TravelMode = TravelMode.Driving.Description(),
                Metric = Metric.Geodesic
            };

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                Addresses = addresses,
                Parameters = parameters
            };

            // Run the query
            string errorString;
            dataObjectMDMD24 = route4Me.RunOptimization(optimizationParameters, out errorString);

            routeId_MultipleDepotMultipleDriverWith24StopsTimeWindow = (dataObjectMDMD24 != null && dataObjectMDMD24.Routes != null && dataObjectMDMD24.Routes.Length > 0) ? dataObjectMDMD24.Routes[0].RouteID : null;

            Assert.IsNotNull(dataObjectMDMD24, "MultipleDepotMultipleDriverWith24StopsTimeWindowTest failed... " + errorString);

            MoveDestinationToRouteTest();

            MergeRoutesTest();

            DeleteRoutesTest(routeId_MultipleDepotMultipleDriverWith24StopsTimeWindow);
        }

        public void MoveDestinationToRouteTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            Assert.IsNotNull(dataObjectMDMD24, "dataObjectMDMD24 is null...");

            Assert.IsTrue(dataObjectMDMD24.Routes.Length >= 2, "There is no 2 routes for moving a destination to other route...");

            DataObjectRoute route1 = dataObjectMDMD24.Routes[0];

            Assert.IsTrue(route1.Addresses.Length >= 2, "There is less than 2 addresses in the generated route...");

            int routeDestinationIdToMove = route1.Addresses[1].RouteDestinationId != null ? Convert.ToInt32(route1.Addresses[1].RouteDestinationId) : -1;

            Assert.IsTrue(routeDestinationIdToMove > 0, "Wrong destination_id to move: " + routeDestinationIdToMove);

            DataObjectRoute route2 = dataObjectMDMD24.Routes[1];

            Assert.IsTrue(route1.Addresses.Length >= 2, "There is less than 2 addresses in the generated route...");

            int afterDestinationIdToMoveAfter = route2.Addresses[1].RouteDestinationId != null ? Convert.ToInt32(route2.Addresses[1].RouteDestinationId) : -1;

            Assert.IsTrue(afterDestinationIdToMoveAfter > 0, "Wrong destination_id to move after: " + afterDestinationIdToMoveAfter);

            string errorString;

            bool result = route4Me.MoveDestinationToRoute(route2.RouteID, routeDestinationIdToMove, afterDestinationIdToMoveAfter, out errorString);

            Assert.IsTrue(result, "MoveDestinationToRouteTest failed... " + errorString);
        }

        public void MergeRoutesTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            Assert.IsNotNull(dataObjectMDMD24, "dataObjectMDMD24 is null...");

            Assert.IsTrue(dataObjectMDMD24.Routes.Length >= 2, "There is no 2 routes for moving a destination to other route...");

            DataObjectRoute route1 = dataObjectMDMD24.Routes[0];

            Assert.IsTrue(route1.Addresses.Length >= 2, "There is less than 2 addresses in the generated route...");

            DataObjectRoute route2 = dataObjectMDMD24.Routes[1];

            MergeRoutesQuery mergeRoutesParameters = new MergeRoutesQuery()
            {
                RouteIds = route1.RouteID + "," + route2.RouteID,
                DepotAddress = route1.Addresses[0].AddressString.ToString(),
                RemoveOrigin = false,
                DepotLat = route1.Addresses[0].Latitude,
                DepotLng = route1.Addresses[0].Longitude
            };

            string errorString;
            bool result = route4Me.MergeRoutes(mergeRoutesParameters, out errorString);

            Assert.IsTrue(result, "MergeRoutesTest failed... " + errorString);
        }

        [TestMethod]
        public void SingleDriverMultipleTimeWindowsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Prepare the addresses
            Address[] addresses = new Address[]
              {
                #region Addresses

                new Address() { AddressString   = "3634 W Market St, Fairlawn, OH 44333",
                                //all possible originating locations are depots, should be marked as true
                                //stylistically we recommend all depots should be at the top of the destinations list
                                IsDepot          = true, 
                                Latitude         = 41.135762259364,
                                Longitude        = -81.629313826561,
                        
                                TimeWindowStart  = null,
                                TimeWindowEnd    = null,
                                TimeWindowStart2 = null,
                                TimeWindowEnd2   = null,
                                Time             = null
                },

                new Address() { AddressString   = "1218 Ruth Ave, Cuyahoga Falls, OH 44221",
                                Latitude        = 41.135762259364,
                                Longitude       = -81.629313826561,

                                //together these two specify the time window of a destination
                                //seconds offset relative to the route start time for the open availability of a destination
                                TimeWindowStart  = 6 * 3600 + 00 * 60,
                                //seconds offset relative to the route end time for the open availability of a destination
                                TimeWindowEnd    = 6 * 3600 + 30 * 60,

                                // Second 'TimeWindowStart'
                                TimeWindowStart2 = 7 * 3600 + 00 * 60,
                                // Second 'TimeWindowEnd'
                                TimeWindowEnd2   = 7 * 3600 + 20 * 60,

                                //the number of seconds at destination
                                Time             = 300
                },

                new Address() { AddressString    = "512 Florida Pl, Barberton, OH 44203",
                                Latitude         = 41.003671512008,
                                Longitude        = -81.598461046815,
                                TimeWindowStart  = 7 * 3600 + 30 * 60,
                                TimeWindowEnd    = 7 * 3600 + 40 * 60,
                                TimeWindowStart2 = 8 * 3600 + 00 * 60,
                                TimeWindowEnd2   = 8 * 3600 + 10 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "512 Florida Pl, Barberton, OH 44203",
                                Latitude         = 41.003671512008,
                                Longitude        = -81.598461046815,
                                TimeWindowStart  = 8 * 3600 + 30 * 60,
                                TimeWindowEnd    = 8 * 3600 + 40 * 60,
                                TimeWindowStart2 = 8 * 3600 + 50 * 60,
                                TimeWindowEnd2   = 9 * 3600 + 00 * 60,
                                Time             = 100
                },

                new Address() { AddressString    = "3495 Purdue St, Cuyahoga Falls, OH 44221",
                                Latitude         = 41.162971496582,
                                Longitude        = -81.479049682617,
                                TimeWindowStart  = 9 * 3600 + 00 * 60,
                                TimeWindowEnd    = 9 * 3600 + 15 * 60,
                                TimeWindowStart2 = 9 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 9 * 3600 + 45 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "1659 Hibbard Dr, Stow, OH 44224",
                                Latitude         = 41.194505989552,
                                Longitude        = -81.443351581693,
                                TimeWindowStart  = 10 * 3600 + 00 * 60,
                                TimeWindowEnd    = 10 * 3600 + 15 * 60,
                                TimeWindowStart2 = 10 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 10 * 3600 + 45 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "2705 N River Rd, Stow, OH 44224",
                                Latitude         = 41.145240783691,
                                Longitude        = -81.410247802734,
                                TimeWindowStart  = 11 * 3600 + 00 * 60,
                                TimeWindowEnd    = 11 * 3600 + 15 * 60,
                                TimeWindowStart2 = 11 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 11 * 3600 + 45 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "10159 Bissell Dr, Twinsburg, OH 44087",
                                Latitude         = 41.340042114258,
                                Longitude        = -81.421226501465,
                                TimeWindowStart  = 12 * 3600 + 00 * 60,
                                TimeWindowEnd    = 12 * 3600 + 15 * 60,
                                TimeWindowStart2 = 12 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 12 * 3600 + 45 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "367 Cathy Dr, Munroe Falls, OH 44262",
                                Latitude         = 41.148578643799,
                                Longitude        = -81.429229736328,
                                TimeWindowStart  = 13 * 3600 + 00 * 60,
                                TimeWindowEnd    = 13 * 3600 + 15 * 60,
                                TimeWindowStart2 = 13 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 13 * 3600 + 45 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "367 Cathy Dr, Munroe Falls, OH 44262",
                                Latitude         = 41.148578643799,
                                Longitude        = -81.429229736328,
                                TimeWindowStart  = 14 * 3600 + 00 * 60,
                                TimeWindowEnd    = 14 * 3600 + 15 * 60,
                                TimeWindowStart2 = 14 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 14 * 3600 + 45 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "512 Florida Pl, Barberton, OH 44203",
                                Latitude         = 41.003671512008,
                                Longitude        = -81.598461046815,
                                TimeWindowStart  = 15 * 3600 + 00 * 60,
                                TimeWindowEnd    = 15 * 3600 + 15 * 60,
                                TimeWindowStart2 = 15 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 15 * 3600 + 45 * 60,
                                Time             = 300
                },

                new Address() { AddressString    = "559 W Aurora Rd, Northfield, OH 44067",
                                Latitude         = 41.315116882324,
                                Longitude        = -81.558746337891,
                                TimeWindowStart  = 16 * 3600 + 00 * 60,
                                TimeWindowEnd    = 16 * 3600 + 15 * 60,
                                TimeWindowStart2 = 16 * 3600 + 30 * 60,
                                TimeWindowEnd2   = 17 * 3600 + 00 * 60,
                                Time             = 50
                }

                #endregion
              };

            // Set parameters
            RouteParameters parameters = new RouteParameters()
            {
                AlgorithmType = AlgorithmType.TSP,
                StoreRoute = false,
                RouteName = "Single Driver Multiple TimeWindows 12 Stops",

                RouteDate = R4MeUtils.ConvertToUnixTimestamp(DateTime.UtcNow.Date.AddDays(1)),
                RouteTime = 5 * 3600 + 30 * 60,
                Optimize = Optimize.Distance.Description(),
                DistanceUnit = DistanceUnit.MI.Description(),
                DeviceType = DeviceType.Web.Description()
            };

            OptimizationParameters optimizationParameters = new OptimizationParameters()
            {
                Addresses = addresses,
                Parameters = parameters
            };

            // Run the query
            string errorString;
            dataObject = route4Me.RunOptimization(optimizationParameters, out errorString);

            routeId_SingleDriverMultipleTimeWindows = (dataObject != null && dataObject.Routes != null && dataObject.Routes.Length > 0) ? dataObject.Routes[0].RouteID : null;

            Assert.IsNotNull(dataObject, "SingleDriverMultipleTimeWindowsTest failed... " + errorString);

            DeleteRoutesTest(routeId_SingleDriverMultipleTimeWindows);
        }

        public void DeleteRoutesTest(string routeId)
        {
            List<string> routeIdsToDelete = new List<string>();

            if (routeId != null)
                routeIdsToDelete.Add(routeId);

            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            // Run the query
            string errorString;
            string[] deletedRouteIds = route4Me.DeleteRoutes(routeIdsToDelete.ToArray(), out errorString);

            Assert.IsInstanceOfType(deletedRouteIds, typeof(string[]), "DeleteRoutesTest failed... " + errorString);
        }
    }

    [TestClass]
    public class IndpedentTestsGroup
    {
        static string c_ApiKey = "11111111111111111111111111111111";

        [TestMethod]
        public void GetUsersTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            GenericParameters parameters = new GenericParameters()
            {
            };

            // Run the query
            string errorString;
            User[] dataObjects = route4Me.GetUsers(parameters, out errorString);

            Assert.IsInstanceOfType(dataObjects, typeof(User[]), "GetUsersTest failed... " + errorString);
        }

        [TestMethod]
        public void GetOptimizationsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            RouteParametersQuery queryParameters = new RouteParametersQuery()
            {
                Limit = 10,
                Offset = 5
            };

            // Run the query
            string errorString;
            DataObject[] dataObjects = route4Me.GetOptimizations(queryParameters, out errorString);

            Assert.IsInstanceOfType(dataObjects, typeof(DataObject[]), "GetOptimizationsTest failed... " + errorString);
        }

        [TestMethod]
        public void GetAddressBookContactsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AddressBookParameters addressBookParameters = new AddressBookParameters()
            {
                Limit = 10,
                Offset = 0
            };

            // Run the query
            uint total;
            string errorString;
            AddressBookContact[] contacts = route4Me.GetAddressBookLocation(addressBookParameters, out total, out errorString);

            Assert.IsInstanceOfType(contacts, typeof(AddressBookContact[]), "GetAddressBookContactsTest failed... " + errorString);
        }
    }

    [TestClass]
    public class AddressbookContactsGroup
    {
        static string c_ApiKey = "11111111111111111111111111111111";

        static AddressBookContact contact1, contact2;

        static List<int> lsRemoveContacts=new List<int>();

        [TestMethod]
        [TestInitialize]
        public void AddAddressBookContactsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AddressBookContact contact = new AddressBookContact()
            {
                first_name = "Test FirstName " + (new Random()).Next().ToString(),
                address_1 = "Test Address1 " + (new Random()).Next().ToString(),
                cached_lat = 38.024654,
                cached_lng = -77.338814
            };

            // Run the query
            string errorString;
            contact1 = route4Me.AddAddressBookContact(contact, out errorString);

            Assert.IsNotNull(contact1, "AddAddressBookContactsTest failed... " + errorString);

            int location1 = contact1.address_id != null ? Convert.ToInt32(contact1.address_id) : -1;
            if (location1 > 0) lsRemoveContacts.Add(location1);

            contact = new AddressBookContact()
            {
                first_name = "Test FirstName " + (new Random()).Next().ToString(),
                address_1 = "Test Address1 " + (new Random()).Next().ToString(),
                cached_lat = 38.024654,
                cached_lng = -77.338814
            };

            contact2 = route4Me.AddAddressBookContact(contact, out errorString);

            Assert.IsNotNull(contact2, "AddAddressBookContactsTest failed... " + errorString);

            int location2 = contact2.address_id != null ? Convert.ToInt32(contact2.address_id) : -1;
            if (location2 > 0) lsRemoveContacts.Add(location2);
        }

        [TestMethod]
        public void UpdateAddressBookContactTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            Assert.IsNotNull(contact1, "contact1 is null..");

            contact1.address_group = "Updated";
            // Run the query
            string errorString;
            AddressBookContact updatedContact = route4Me.UpdateAddressBookContact(contact1, out errorString);

            Assert.IsNotNull(updatedContact, "UpdateAddressBookContactTest failed... " + errorString);
        }

        [TestMethod]
        public void SearchLocationsByTextTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AddressBookParameters addressBookParameters = new AddressBookParameters
            {
                Query = "Test Address1",
                Offset = 0,
                Limit = 20
            };

            // Run the query
            uint total = 0;
            string errorString = "";
            AddressBookContact[] contacts = route4Me.GetAddressBookLocation(addressBookParameters, out total, out errorString);

            Assert.IsInstanceOfType(contacts, typeof(AddressBookContact[]), "SearchLocationsByTextTest failed... " + errorString);
        }

        [TestMethod]
        public void SearchLocationsByIDsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            Assert.IsNotNull(contact1, "contact1 is null..."); 
            Assert.IsNotNull(contact2, "contact2 is null...");

            string addresses = contact1.address_id + "," + contact2.address_id;

            AddressBookParameters addressBookParameters = new AddressBookParameters
            {
                AddressId = addresses
            };

            // Run the query
            uint total = 0;
            string errorString = "";
            AddressBookContact[] contacts = route4Me.GetAddressBookLocation(addressBookParameters, out total, out errorString);

            Assert.IsInstanceOfType(contacts, typeof(AddressBookContact[]), "SearchLocationsByIDsTest failed... " + errorString);
        }

        [TestMethod]
        public void GetSpecifiedFieldsSearchTextTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AddressBookParameters addressBookParameters = new AddressBookParameters
            {
                Query = "Test Address1",
                Fields = "first_name,address_email",
                Offset = 0,
                Limit = 20
            };

            // Run the query
            uint total = 0;
            string errorString = "";
            AddressBookContact[] contacts = route4Me.GetAddressBookLocation(addressBookParameters, out total, out errorString);

            Assert.IsInstanceOfType(contacts, typeof(AddressBookContact[]), "GetSpecifiedFieldsSearchTextTest failed... " + errorString);
        }

        [TestMethod]
        public void SearchRoutedLocationsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AddressBookParameters addressBookParameters = new AddressBookParameters
            {
                Display = "routed",
                Offset = 0,
                Limit = 20
            };

            // Run the query
            uint total = 0;
            string errorString = "";
            AddressBookContact[] contacts = route4Me.GetAddressBookLocation(addressBookParameters, out total, out errorString);

            Assert.IsInstanceOfType(contacts, typeof(AddressBookContact[]), "SearchRoutedLocationsTest failed... " + errorString);
        }

        [TestMethod]
        [TestCleanup]
        public void RemoveAddressBookContactsTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            List<string> lsRemLocations = new List<string>();
            if (lsRemoveContacts.Count > 0)
            {
                foreach (int loc1 in lsRemoveContacts) lsRemLocations.Add(loc1.ToString());

                string errorString;
                bool removed = route4Me.RemoveAddressBookContacts(lsRemLocations.ToArray(), out errorString);

                Assert.IsTrue(removed, "RemoveAddressBookContactsTest failed... " + errorString);
            }
        }
    }

    [TestClass]
    public class AvoidanseZonesGroup
    {
        static string c_ApiKey = "11111111111111111111111111111111";

        static List<string> lsAvoidanceZones = new List<string>();

        [TestMethod]
        [TestInitialize]
        public void AddAvoidanceZonesTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AvoidanceZoneParameters circleAvoidanceZoneParameters = new AvoidanceZoneParameters()
            {
                TerritoryName = "Test Circle Territory",
                TerritoryColor = "ff0000",
                Territory = new Territory()
                {
                    Type = TerritoryType.Circle.Description(),
                    Data = new string[] { "37.569752822786455,-77.47833251953125",
                                "5000"}
                }
            };

            string errorString;
            AvoidanceZone circleAvoidanceZone = route4Me.AddAvoidanceZone(circleAvoidanceZoneParameters, out errorString);

            if (circleAvoidanceZone!=null) lsAvoidanceZones.Add(circleAvoidanceZone.TerritoryId);

            Assert.IsNotNull(circleAvoidanceZone, "Add Circle Avoidance Zone test failed... " + errorString);

            AvoidanceZoneParameters polyAvoidanceZoneParameters = new AvoidanceZoneParameters()
            {
                TerritoryName = "Test Poly Territory",
                TerritoryColor = "ff0000",
                Territory = new Territory
                {
                    Type = TerritoryType.Poly.Description(),
                    Data = new string[] {
			            "37.569752822786455,-77.47833251953125",
			            "37.75886716305343,-77.68974800109863",
			            "37.74763966054455,-77.6917221069336",
			            "37.74655084306813,-77.68863220214844",
			            "37.7502255383101,-77.68125076293945",
			            "37.74797991274437,-77.67498512268066",
			            "37.73327960206065,-77.6411678314209",
			            "37.74430510679532,-77.63172645568848",
			            "37.76641925847049,-77.66846199035645"
		            }
                }
            };

            AvoidanceZone polyAvoidanceZone = route4Me.AddAvoidanceZone(polyAvoidanceZoneParameters, out errorString);

            Assert.IsNotNull(polyAvoidanceZone, "Add Polygon Avoidance Zone test failed... " + errorString);

            if (polyAvoidanceZone != null) lsAvoidanceZones.Add(polyAvoidanceZone.TerritoryId);

            AvoidanceZoneParameters rectAvoidanceZoneParameters = new AvoidanceZoneParameters
            {
                TerritoryName = "Test Rect Territory",
                TerritoryColor = "ff0000",
                Territory = new Territory
                {
                    Type = TerritoryType.Rect.Description(),
                    Data = new string[] {
			            "43.51668853502909,-109.3798828125",
			            "46.98025235521883,-101.865234375"
		            }
                }
            };

            AvoidanceZone rectAvoidanceZone = route4Me.AddAvoidanceZone(rectAvoidanceZoneParameters, out errorString);

            Assert.IsNotNull(rectAvoidanceZone, "Add Rectangular Avoidance Zone test failed... " + errorString);

            if (lsAvoidanceZones != null) lsAvoidanceZones.Add(rectAvoidanceZone.TerritoryId);
        }

        [TestMethod]
        public void GetAvoidanceZonesTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AvoidanceZoneQuery avoidanceZoneQuery = new AvoidanceZoneQuery()
            {

            };

            // Run the query
            string errorString;
            AvoidanceZone[] avoidanceZones = route4Me.GetAvoidanceZones(avoidanceZoneQuery, out errorString);

            Assert.IsInstanceOfType(avoidanceZones, typeof(AvoidanceZone[]), "GetAvoidanceZonesTest failed... " + errorString);
        }

        [TestMethod]
        public void GetAvoidanceZoneTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string territoryId = "";
            if (lsAvoidanceZones.Count > 0) territoryId = lsAvoidanceZones[0];
            AvoidanceZoneQuery avoidanceZoneQuery = new AvoidanceZoneQuery()
            {
                TerritoryId = territoryId
            };

            // Run the query
            string errorString;
            AvoidanceZone avoidanceZone = route4Me.GetAvoidanceZone(avoidanceZoneQuery, out errorString);

            Assert.IsNotNull(avoidanceZone, "GetAvoidanceZonesTest failed... " + errorString);
        }

        [TestMethod]
        public void UpdateAvoidanceZoneTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string territoryId = "";
            if (lsAvoidanceZones.Count > 0) territoryId = lsAvoidanceZones[0];

            AvoidanceZoneParameters avoidanceZoneParameters = new AvoidanceZoneParameters()
            {
                TerritoryId = territoryId,
                TerritoryName = "Test Territory Updated",
                TerritoryColor = "ff00ff",
                Territory = new Territory()
                {
                    Type = TerritoryType.Circle.Description(),
                    Data = new string[] { "38.41322259056806,-78.501953234",
                                "3000"}
                }
            };

            // Run the query
            string errorString;
            AvoidanceZone avoidanceZone = route4Me.UpdateAvoidanceZone(avoidanceZoneParameters, out errorString);

            Assert.IsNotNull(avoidanceZone, "UpdateAvoidanceZoneTest failed... " + errorString);
        }

        [TestMethod]
        [ClassCleanup]
        public static void RemoveAvoidanceZoneTest()
        {
            foreach (string territoryId in lsAvoidanceZones)
            {
                Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

                AvoidanceZoneQuery avoidanceZoneQuery = new AvoidanceZoneQuery()
                {
                    TerritoryId = territoryId
                };

                // Run the query
                string errorString;
                bool result = route4Me.DeleteAvoidanceZone(avoidanceZoneQuery, out errorString);

                Assert.IsTrue(result, "RemoveAvoidanceZoneTest failed... " + errorString);
            }
        }
    }

    [TestClass]
    public class TerritoriesGroup
    {
        static string c_ApiKey = "11111111111111111111111111111111";

        static List<string> lsTerritories = new List<string>();

        [TestMethod]
        [TestInitialize]
        public void AddTerritoriesTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AvoidanceZoneParameters circleTerritoryParameters = new AvoidanceZoneParameters()
            {
                TerritoryName = "Test Circle Territory",
                TerritoryColor = "ff0000",
                Territory = new Territory()
                {
                    Type = TerritoryType.Circle.Description(),
                    Data = new string[] { "37.569752822786455,-77.47833251953125",
                                "5000"}
                }
            };

            string errorString;
            TerritoryZone circleTerritory = route4Me.CreateTerritory(circleTerritoryParameters, out errorString);

            if (circleTerritory != null) lsTerritories.Add(circleTerritory.TerritoryId);

            Assert.IsNotNull(circleTerritory, "Add Circle Territory test failed... " + errorString);

            AvoidanceZoneParameters polyTerritoryParameters = new AvoidanceZoneParameters()
            {
                TerritoryName = "Test Poly Territory",
                TerritoryColor = "ff0000",
                Territory = new Territory
                {
                    Type = TerritoryType.Poly.Description(),
                    Data = new string[] {
			            "37.569752822786455,-77.47833251953125",
			            "37.75886716305343,-77.68974800109863",
			            "37.74763966054455,-77.6917221069336",
			            "37.74655084306813,-77.68863220214844",
			            "37.7502255383101,-77.68125076293945",
			            "37.74797991274437,-77.67498512268066",
			            "37.73327960206065,-77.6411678314209",
			            "37.74430510679532,-77.63172645568848",
			            "37.76641925847049,-77.66846199035645"
		            }
                }
            };

            TerritoryZone polyTerritory = route4Me.CreateTerritory(polyTerritoryParameters, out errorString);

            Assert.IsNotNull(polyTerritory, "Add Polygon Territory test failed... " + errorString);

            if (polyTerritory != null) lsTerritories.Add(polyTerritory.TerritoryId);

            AvoidanceZoneParameters rectTerritoryParameters = new AvoidanceZoneParameters
            {
                TerritoryName = "Test Rect Territory",
                TerritoryColor = "ff0000",
                Territory = new Territory
                {
                    Type = TerritoryType.Rect.Description(),
                    Data = new string[] {
			            "43.51668853502909,-109.3798828125",
			            "46.98025235521883,-101.865234375"
		            }
                }
            };

            TerritoryZone rectTerritory = route4Me.CreateTerritory(rectTerritoryParameters, out errorString);

            Assert.IsNotNull(rectTerritory, "Add Rectangular Avoidance Zone test failed... " + errorString);

            if (lsTerritories != null) lsTerritories.Add(rectTerritory.TerritoryId);
        }

        [TestMethod]
        public void GetTerritoriesTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            AvoidanceZoneQuery territoryQuery = new AvoidanceZoneQuery()
            {

            };

            // Run the query
            string errorString;
            AvoidanceZone[] territories = route4Me.GetTerritories(territoryQuery, out errorString);

            Assert.IsInstanceOfType(territories, typeof(AvoidanceZone[]), "GetTerritoriesTest failed... " + errorString);
        }

        [TestMethod]
        public void GetTerritoryTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string territoryId = "";
            if (lsTerritories.Count > 0) territoryId = lsTerritories[0];
            TerritoryQuery territoryQuery = new TerritoryQuery()
            {
                TerritoryId = territoryId
            };

            // Run the query
            string errorString;
            TerritoryZone territory = route4Me.GetTerritory(territoryQuery, out errorString);

            Assert.IsNotNull(territory, "GetTerritoryTest failed... " + errorString);
        }

        [TestMethod]
        public void UpdateTerritoryTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            string territoryId = "";
            if (lsTerritories.Count > 0) territoryId = lsTerritories[0];

            AvoidanceZoneParameters territoryParameters = new AvoidanceZoneParameters()
            {
                TerritoryId = territoryId,
                TerritoryName = "Test Territory Updated",
                TerritoryColor = "ff00ff",
                Territory = new Territory()
                {
                    Type = TerritoryType.Circle.Description(),
                    Data = new string[] { "38.41322259056806,-78.501953234",
                                "3000"}
                }
            };

            // Run the query
            string errorString;
            AvoidanceZone territory = route4Me.UpdateTerritory(territoryParameters, out errorString);

            Assert.IsNotNull(territory, "UpdateTerritoryTest failed... " + errorString);
        }

        [TestMethod]
        [ClassCleanup]
        public static void RemoveTerritoriesTest()
        {
            foreach (string territoryId in lsTerritories)
            {
                Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

                AvoidanceZoneQuery territoryQuery = new AvoidanceZoneQuery()
                {
                    TerritoryId = territoryId
                };

                // Run the query
                string errorString;
                bool result = route4Me.RemoveTerritory(territoryQuery, out errorString);

                Assert.IsTrue(result, "RemoveTerritoriesTest failed... " + errorString);
            }
        }
    }

    [TestClass]
    public class OrdersGroup
    {
        static string c_ApiKey = "11111111111111111111111111111111";

        static List<string> lsOrders = new List<string>();

        [TestMethod]
        [TestInitialize]
        public void CreateOrderTest()
        {
            Route4MeManager route4Me = new Route4MeManager(c_ApiKey);

            Order order = new Order()
            {
                address_1 = "Test Address1 " + (new Random()).Next().ToString(),
                address_alias = "Test AddressAlias " + (new Random()).Next().ToString(),
                cached_lat = 37.773972,
                cached_lng = -122.431297
            };

            // Run the query
            string errorString;
            Order resultOrder = route4Me.AddOrder(order, out errorString);

            Assert.IsNotNull(resultOrder, "CreateOrderTest failed... " + errorString);
        }

        [TestMethod]
        public void GetOrdersTest()
        {

        }
    }

    #region Types

    [DataContract]
    class AddressInfo : GenericParameters
    {
        [DataMember(Name = "route_destination_id")]
        public int DestinationId { get; set; }

        [DataMember(Name = "sequence_no")]
        public int SequenceNo { get; set; }

        [DataMember(Name = "is_depot")]
        public bool IsDepot { get; set; }
    }

    [DataContract]
    class AddressesOrderInfo : GenericParameters
    {
        [HttpQueryMemberAttribute(Name = "route_id", EmitDefaultValue = false)]
        public string RouteId { get; set; }

        [DataMember(Name = "addresses")]
        public AddressInfo[] Addresses { get; set; }
    }

    // Inherit from GenericParameters and add any JSON serializable content
    // Marked with attribute [DataMember]
    [DataContract]
    class MyAddressAndParametersHolder : GenericParameters
    {
        [DataMember]
        public Address[] addresses { get; set; } // Using the defined class "Address", can use user-defined class instead

        [DataMember]
        public RouteParameters parameters { get; set; } // Using the defined "RouteParameters", can use user-defined class instead
    }

    // Generic class for returned JSON holder
    [DataContract]
    class MyDataObjectGeneric
    {
        [DataMember(Name = "optimization_problem_id")]
        public string OptimizationProblemId { get; set; }

        [DataMember(Name = "state")]
        public int MyState { get; set; }

        [DataMember(Name = "addresses")]
        public Address[] Addresses { get; set; } // Using the defined class "Address", can use user-defined class instead

        [DataMember(Name = "parameters")]
        public RouteParameters Parameters { get; set; } // Using the defined "RouteParameters", can use user-defined class instead
    }
    #endregion
}