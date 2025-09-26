using System;
using System.Collections.Generic;
using FlightGlobe.Data;
using Godot;

namespace FlightGlobe.Loaders
{
    public partial class RouteLoader : Node
    {
        public static Flight[] CreateFlightsFromRoutes(List<Route> routes, float speedMultiplier, float radius, float radiusOffset, int segments)
        {
            var random = new Random();
            var flights = new Flight[routes.Count];
            for (int i = 0; i < routes.Count; i++)
            {
                var route = routes[i];
                flights[i] = new Flight
                {
                    Route = route,
                    Airplane = route.Airplane,
                    Path = route.GetCirclePath(radius, radiusOffset, segments),
                    Schedules =
                    [
                        new Schedule
                        {
                            DepartureDay = (DayOfWeek)random.Next(0, 7),
                            DepartureTime = new TimeOnly(random.Next(0, 24), random.Next(0, 60))
                        },
                        new Schedule
                        {
                            DepartureDay = (DayOfWeek)random.Next(0, 7),
                            DepartureTime = new TimeOnly(random.Next(0, 24), random.Next(0, 60))
                        },
                        new Schedule
                        {
                            DepartureDay = (DayOfWeek)random.Next(0, 7),
                            DepartureTime = new TimeOnly(random.Next(0, 24), random.Next(0, 60))
                        }
                    ]
                };
            }
            return flights;
        }

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

                usedPairs.Add(routeKey);
                routes.Add(new Route
                {
                    Origin = origin,
                    Destination = destination,
                    Airplane = randomAirplane,
                });
            }

            return routes;
        }
    }
}