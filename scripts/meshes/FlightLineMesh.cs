using System.Collections.Generic;
using FlightGlobe.Data;
using Godot;
using Godot.Collections;

namespace FlightGlobe
{
    public partial class FlightLineMesh : MeshInstance3D
    {
        public Flight[] Flights { get; set; }

        public override void _Ready()
        {
            var vertices = new List<Vector3>();
            var indices = new List<int>();

            for (var i = 0; i < Flights.Length; i++)
            {
                var flight = Flights[i];
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

            var arrays = new Array();
            arrays.Resize((int)Mesh.ArrayType.Max);
            arrays[(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
            arrays[(int)Mesh.ArrayType.Index] = indices.ToArray();

            var arrayMesh = new ArrayMesh();
            arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Lines, arrays);

            Mesh = arrayMesh;
            MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = Colors.Aquamarine,
                ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            };
        }
    }
}