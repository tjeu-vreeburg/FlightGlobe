using Godot;

namespace FlightGlobe.areas
{
    public partial class FlightCollisionArea : Area3D
    {
        public int FlightIndex { get; set; }

        [Signal]
        public delegate void FlightClickedEventHandler(int flightIndex, Vector2 screenPosition);

        public override void _Ready()
        {
            var collisionShape = new CollisionShape3D
            {
                Shape = new BoxShape3D
                {
                    Size = new Vector3(0.2f, 0.2f, 0.01f),
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
                EmitSignal(SignalName.FlightClicked, FlightIndex, mouseButton.GlobalPosition);
            }
        }
    }
}