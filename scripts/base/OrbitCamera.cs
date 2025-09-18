using Godot;
using Godot.Collections;

namespace FlightGlobe.Base
{
	public partial class OrbitCamera : Node3D
	{
		[Export] private Camera3D camera;
		[Export] private Node3D horizontal;
		[Export] private Node3D vertical;

		[Export] private float rotationSpeed = 5.0f;
		[Export] private float zoomSpeed = 8.0f;
		[Export] private Vector2 rotationBounds = new(-75.0f, 75.0f);
		[Export] private Vector2 zoomBounds = new(2.5f, 75.0f);

		private float initialDistance;
		private float horizontalRotation;
		private float verticalRotation;

		private bool leftButtonDown = false;
		private bool rightButtonDown = false;

		public float TargetZoom { get; set; } = 25.0f;
		public float Zoom { get; private set; } = 25.0f;
		private float initialZoom;

		private float targetHorizontalRotation;
		private float targetVerticalRotation;

		private Dictionary<long, Vector2> touchPoints = [];

		[Signal]
		public delegate void OnZoomSignalEventHandler(float zoom);

		public override void _Process(double delta)
		{
			targetVerticalRotation = Mathf.Clamp(targetVerticalRotation, rotationBounds.X, rotationBounds.Y);
			verticalRotation = (float)Mathf.Lerp(verticalRotation, targetVerticalRotation, rotationSpeed * delta);
			horizontalRotation = (float)Mathf.Lerp(horizontalRotation, targetHorizontalRotation, rotationSpeed * delta);

			TargetZoom = Mathf.Clamp(TargetZoom, zoomBounds.X, zoomBounds.Y);
			Zoom = (float)Mathf.Lerp(Zoom, TargetZoom, zoomSpeed * delta);
			camera.Fov = Zoom;

			var horizontalDegrees = horizontal.RotationDegrees;
			horizontalDegrees.Y = horizontalRotation;
			horizontal.RotationDegrees = horizontalDegrees;

			var verticalDegrees = vertical.RotationDegrees;
			verticalDegrees.X = verticalRotation;
			vertical.RotationDegrees = verticalDegrees;
		}

		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseButton inputEventMouseButton)
			{
				leftButtonDown = inputEventMouseButton.IsPressed() && inputEventMouseButton.ButtonIndex == MouseButton.Left;
				rightButtonDown = inputEventMouseButton.IsPressed() && inputEventMouseButton.ButtonIndex == MouseButton.Right;

				if (inputEventMouseButton.ButtonIndex == MouseButton.WheelDown)
				{
					TargetZoom -= 0.1f * zoomSpeed;
					EmitSignal(SignalName.OnZoomSignal, TargetZoom);
				}

				if (inputEventMouseButton.ButtonIndex == MouseButton.WheelUp)
				{
					TargetZoom += 0.1f * zoomSpeed;
					EmitSignal(SignalName.OnZoomSignal, TargetZoom);
				}
			}

			if (@event is InputEventMouseMotion inputEventMouseMotion)
			{
				if (leftButtonDown)
				{
					var rotationScale = TargetZoom / 15.0f;
					targetHorizontalRotation += -(inputEventMouseMotion.Relative.X * 0.1f * rotationScale);
					targetVerticalRotation += -(inputEventMouseMotion.Relative.Y * 0.1f * rotationScale);
				}
			}
		}
	}
}