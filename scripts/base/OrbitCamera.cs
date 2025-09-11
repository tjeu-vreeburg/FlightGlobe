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

		private float zoom = 25.0f;
		private float targetZoom = 25.0f;
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

			targetZoom = Mathf.Clamp(targetZoom, zoomBounds.X, zoomBounds.Y);
			zoom = (float)Mathf.Lerp(zoom, targetZoom, zoomSpeed * delta);
			camera.Fov = zoom;

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
					targetZoom -= 0.1f * zoomSpeed;
					EmitSignal(SignalName.OnZoomSignal, targetZoom);
				}

				if (inputEventMouseButton.ButtonIndex == MouseButton.WheelUp)
				{
					targetZoom += 0.1f * zoomSpeed;
					EmitSignal(SignalName.OnZoomSignal, targetZoom);
				}
			}

			if (@event is InputEventMouseMotion inputEventMouseMotion)
			{
				if (leftButtonDown)
				{
					var rotationScale = targetZoom / 15.0f;
					targetHorizontalRotation += -(inputEventMouseMotion.Relative.X * 0.1f * rotationScale);
					targetVerticalRotation += -(inputEventMouseMotion.Relative.Y * 0.1f * rotationScale);
				}
			}
		}
	}
}