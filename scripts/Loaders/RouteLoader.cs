using System;
using System.Collections.Generic;
using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.Loaders
{
    public partial class RouteLoader : Node
    {
         public static List<Route> GetRoutes(Airport[] airports, Airplane[] airplanes, int maxRoutes)
        {
            var routes = new List<Route>();
            var usedPairs = new HashSet<string>();
            var random = new Random();

            var maxPossibleRoutes = airports.Length * (airports.Length - 1);
            var totalRoutes = Math.Min(maxRoutes, maxPossibleRoutes);

            while (routes.Count < totalRoutes)
            {
                var originIndex = random.Next(airports.Length);
                var destIndex = random.Next(airports.Length);

                if (originIndex == destIndex) continue;

                var origin = airports[originIndex];
                var destination = airports[destIndex];

                string routeKey = origin.IATA + "->" + destination.IATA;

                if (usedPairs.Contains(routeKey)) continue;

                var randomAirplane = airplanes[random.Next(airplanes.Length)];
                var randomDirection = (Direction) random.Next(2);

                usedPairs.Add(routeKey);
                routes.Add(new Route
                {
                    Origin = origin,
                    Destination = destination,
                    Airplane = randomAirplane,
                    Direction = randomDirection,
                });
            }

            return routes;
        }
    }
}