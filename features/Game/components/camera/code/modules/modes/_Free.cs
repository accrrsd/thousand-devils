using System;
using Godot;
using ThousandDevils.features.Game.components.cell.code;

namespace ThousandDevils.features.Game.components.camera.code.modules.modes;

public class FreeMode : BaseMode
{
  private Cell _firstCellToTp;
  private Vector2 _lookAngles = Vector2.Zero;
  private bool _showMouse = true;
  private Vector3 _velocity = Vector3.Zero;

  public FreeMode(Camera camera) : base(camera) { }

  public float Speed { get; private set; } = 5.0f;
  public float MouseSensitivity { get; } = 300.0f;
  public float Acceleration { get; } = 25.0f;

  public override void OnProcess(double delta) {
    if (!_showMouse) {
      _lookAngles[1] = (float)Math.Clamp(_lookAngles[1], Math.PI / -2, Math.PI / 2);
      Camera.Rotation = new Vector3(_lookAngles[1], _lookAngles[0], 0);
    }

    Vector3 direction = GetDirection();
    if (direction.LengthSquared() > 0) _velocity = direction * Acceleration * (float)delta;
    Camera.Translate(_velocity * (float)delta * Speed);
  }

  public override void OnInput(InputEvent @event) {
    if (@event is InputEventMouseMotion mouseMotion && !_showMouse)
      _lookAngles -= mouseMotion.Relative / MouseSensitivity;
    UpdateCameraSpeed();
    if (Input.IsActionJustPressed("cam_free_lock")) {
      _showMouse = !_showMouse;
      Input.MouseMode = _showMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
    }

    if (Input.IsActionJustPressed("lmb_click")) {
      Node rayCastRes = ShootRay();
      if (rayCastRes is Cell cell) CameraLogic(cell);
    }
    else if (Input.IsActionJustPressed("rmb_click")) {
      Node rayCastRes = ShootRay();
      if (rayCastRes is not Cell cell) return;
      if (_firstCellToTp == null) {
        _firstCellToTp = cell;
      }
      else {
        if (_firstCellToTp.GetPawns().Count > 0) _firstCellToTp.GetPawns()[0].MoveToCell(cell, true);
        _firstCellToTp = null;
      }
    }
  }

  public override void OnReady() {
    _lookAngles = new Vector2(Camera.Rotation[1], Camera.Rotation[0]);
    if (!_showMouse) Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  private Vector3 GetDirection() {
    Vector3 dir = new();
    if (Input.IsActionPressed("cam_free_forward")) dir += Vector3.Forward;
    if (Input.IsActionPressed("cam_free_backward")) dir += Vector3.Back;
    if (Input.IsActionPressed("cam_free_left")) dir += Vector3.Left;
    if (Input.IsActionPressed("cam_free_right")) dir += Vector3.Right;
    if (Input.IsActionPressed("cam_free_up")) dir += Vector3.Up;
    if (Input.IsActionPressed("cam_free_down")) dir += Vector3.Down;
    if (dir == Vector3.Zero) _velocity = Vector3.Zero;
    return dir.Normalized();
  }

  private void UpdateCameraSpeed() {
    if (Input.IsActionJustPressed("wheel_up")) Speed += Speed / 2;
    if (Input.IsActionJustPressed("wheel_down")) Speed -= Speed / 2;
    if (Speed < 0.5) Speed = 0.5f;
    if (Speed > 1000.0) Speed = 1000.0f;
  }
}