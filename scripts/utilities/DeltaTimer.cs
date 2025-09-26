using Godot;

namespace FlightGlobe.Utilities
{
    public partial class DeltaTimer : Node
    {
        private float updateTimer = 0.0f;
        private const float UPDATE_INTERVAL = 1f / 60f;

        public bool IsWaiting(double deltaTime)
        {
            updateTimer += (float)deltaTime;
            if (updateTimer < UPDATE_INTERVAL) return true;

            updateTimer = 0f;

            return false;
        }
    }
}