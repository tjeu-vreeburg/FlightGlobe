using Godot;

namespace FlightGlobe.areas
{
    public partial class AirportCollisionArea : Area3D
    {
        public int AirportIndex { get; set; }

        [Signal]
        public delegate void AirportClickedEventHandler(int airportIndex, Vector2 screenPosition);

        public override void _Ready()
        {
            var collisionShape = new CollisionShape3D
            {
                Shape = new SphereShape3D
                {
                    Radius = 0.02f,
                },
            };

            AddChild(collisionShape);
            InputEvent += OnInputEvent;
        }

        public void UpdateTransform(Transform3D transform)
        {
            GlobalTransform = transform;
        }

        private void OnInputEvent(Node camera, InputEvent @event, Vector3 eventPosition, Vector3 normal, long shapeIdx)
        {
            if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                EmitSignal(SignalName.AirportClicked, AirportIndex, mouseButton.GlobalPosition);
            }
        }
    }
}