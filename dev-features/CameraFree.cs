using CellSpace;
using Godot;
using System;
using System.Linq;

public partial class CameraFree : Camera3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float MouseSensitivity = 300.0f;
	[Export]
	public float Acceleration = 25.0f;
	[Export]
	public int rayLength = 1000;

	private Vector3 _velocity = Vector3.Zero;
	private Vector2 _lookAngles = Vector2.Zero;
	private bool showMouse = false;

	public override void _Ready()
	{
		if (!showMouse) Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
		if (!showMouse)
		{
			_lookAngles[1] = (float)Math.Clamp(_lookAngles[1], Math.PI / -2, Math.PI / 2);
			Rotation = new Vector3(_lookAngles[1], _lookAngles[0], 0);
		}
		Vector3 direction = UpdateDirection();
		if (direction.LengthSquared() > 0)
		{
			_velocity = direction * Acceleration * (float)delta;
		}
		Translate(_velocity * (float)delta * Speed);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseMotion mouseMotion)
		{
			_lookAngles -= mouseMotion.Relative / MouseSensitivity;
		}
		UpdateCameraLock();
		UpdateCameraSpeed();
		if (Input.IsActionJustPressed("lmb_click")) ShootRay();
	}

	private Vector3 UpdateDirection()
	{
		Vector3 dir = new();
		if (Input.IsActionPressed("free_cam_forward")) dir += Vector3.Forward;
		if (Input.IsActionPressed("free_cam_backward")) dir += Vector3.Back;
		if (Input.IsActionPressed("free_cam_left")) dir += Vector3.Left;
		if (Input.IsActionPressed("free_cam_right")) dir += Vector3.Right;
		if (Input.IsActionPressed("free_cam_up")) dir += Vector3.Up;
		if (Input.IsActionPressed("free_cam_down")) dir += Vector3.Down;
		if (dir == Vector3.Zero)
		{
			_velocity = Vector3.Zero;
		}
		return dir.Normalized();
	}

	private void UpdateCameraLock()
	{
		if (Input.IsActionJustPressed("free_cam_lock")) showMouse = !showMouse;
		if (!showMouse)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	private void UpdateCameraSpeed()
	{
		if (Input.IsActionJustPressed("free_cam_increase_speed")) Speed += Speed / 2;
		if (Input.IsActionJustPressed("free_cam_decrease_speed")) Speed -= Speed / 2;
	}

	// dev function for click on the cells
	private void ShootRay()
	{
		if (!showMouse) return;
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector3 from = ProjectRayOrigin(mousePos);
		Vector3 to = from + ProjectRayNormal(mousePos) * rayLength;
		PhysicsDirectSpaceState3D space = GetWorld3D().DirectSpaceState;
		PhysicsRayQueryParameters3D ray_query = new()
		{
			From = from,
			To = to
		};
		Godot.Collections.Dictionary res = space.IntersectRay(ray_query);
		if (res.Count == 0) return;
		if (res.ContainsKey("collider"))
		{
			StaticBody3D staticBody = (StaticBody3D)res["collider"];
			Cell cell = staticBody.GetParent() as Cell;
			cell.IsOpen = !cell.IsOpen;
		}
	}
}
