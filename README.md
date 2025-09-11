# ✈️ FlightGlobe

> A stunning 3D visualization of global flight patterns built with Godot Engine

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white) ![FlightGlobe Banner](https://img.shields.io/badge/Godot-4.x-blue?style=for-the-badge&logo=godot-engine) ![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

FlightGlobe is a real-time 3D simulation that visualizes aircraft movements across the globe. Watch as planes traverse their flight paths in a mesmerizing display of global aviation traffic, all rendered on a beautiful 3D earth model.
<img width="400" height="400" alt="image" src="https://github.com/user-attachments/assets/84140e2e-04af-4edf-bce3-e4919f05b47d" />
<img width="400" height="400" alt="image" src="https://github.com/user-attachments/assets/bc409243-0744-45a9-ad1b-e7899aae858a" />



## Features

- **Real-time Flight Simulation**: Animated aircraft following realistic flight paths
- **3D Globe Visualization**: Beautiful earth model with proper surface mapping
- **Multi-directional Flight Paths**: Aircraft can travel in both directions along routes
- **Performance Optimized**: Efficient MultiMesh rendering for thousands of simultaneous flights
- **Customizable Speed**: Adjustable time multiplier for faster or slower simulations
- **Dynamic Flight Duration**: Each flight has realistic timing based on actual route distances

## Demo
> Experience the beauty of global aviation in real-time 3D

### Key Visual Elements
- **Aircraft Models**: Textured 3D representations of planes
- **Flight Trajectories**: Smooth interpolated paths between waypoints  
- **Globe Surface**: High-quality earth texture mapping
- **Real-time Animation**: Fluid movement with proper aircraft orientation

## Getting Started

### Prerequisites

- [Godot Engine 4.x](https://godotengine.org/download) with C# support
- .NET SDK 6.0 or later
- Visual Studio, Visual Studio Code, or JetBrains Rider (recommended)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/tjeu-vreeburg/FlightGlobe.git
   cd FlightGlobe
   ```
2. Open in Godot
   - Launch Godot Engine
   - Click "Import" and select the project.godot file
   - Let Godot import all assets
3. Build the project
   - Go to Project > Tools > C# > Create C# Solution
   - Build the solution in your preferred IDE
4. Run the simulation
   - Press F5 in Godot or click the play button
   - Enjoy watching global flight patterns come to life!

## 📊 Performance
> The aim of this project was to use the CPU as much as possible to handle heavy computations,
  I am aware that using the GPU will significantly improve performance.

**FlightGlobe is optimized to handle:**
- 1000+ simultaneous flights on modern hardware
- Smooth 60 FPS performance with proper LOD
- Minimal memory footprint through efficient MultiMesh usage
- Scalable architecture for adding more feature
