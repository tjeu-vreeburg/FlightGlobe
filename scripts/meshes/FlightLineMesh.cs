using System;
using System.Collections.Generic;
using FlightGlobe.Data;
using FlightGlobe.Utilities;
using Godot;

namespace FlightGlobe
{
    public partial class FlightLineMesh : MeshInstance3D
    {
        public Flight[] Flights { private get; set; }
        public DateTime DateTime { private get; set; }
        private DeltaTimer deltaTimer = new();

        public override void _Process(double delta)
        {
            if (deltaTimer.IsWaiting(delta)) return;

            UpdateMesh();
        }

        private void UpdateMesh()
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();

            for (var i = 0; i < Flights.Length; i++)
            {
                var flight = Flights[i];
                if (flight.IsIdle(DateTime)) continue;

                var flightPath = flight.Path;
                var startIndex = vertices.Count;

                foreach (var vertex in flightPath)
                {
                    vertices.Add(vertex.Normalized() * vertex.Length());
                }

                for (var j = 0; j < flightPath.Length - 1; j++)
                {
                    indices.Add(startIndex + j);
                    indices.Add(startIndex + j + 1);
                }
            }

            Mesh?.Dispose();

            if (vertices.Count > 0)
            {
                var arrays = new Godot.Collections.Array();
                arrays.Resize((int)Mesh.ArrayType.Max);
                arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
                arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();

                var arrayMesh = new ArrayMesh();
                arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, arrays);

                Mesh = arrayMesh;
            }

            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = Colors.Aquamarine,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            };
        }
    }
}