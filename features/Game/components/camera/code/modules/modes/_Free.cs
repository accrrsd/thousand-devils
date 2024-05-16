﻿using System;
using Godot;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.camera.code.modules.modes;

public class FreeMode : BaseMode
{
  private Cell _firstCellToTp;
  private Vector2 _lookAngles = Vector2.Zero;
  private bool _showMouse;
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

    Vector3 direction = UpdateDirection();
    if (direction.LengthSquared() > 0) _velocity = direction * Acceleration * (float)delta;
    Camera.Translate(_velocity * (float)delta * Speed);
  }

  public override void OnInput(InputEvent @event) {
    if (@event is InputEventMouseMotion mouseMotion && !_showMouse)
      _lookAngles -= mouseMotion.Relative / MouseSensitivity;
    UpdateCameraSpeed();
    if (Input.IsActionJustPressed("free_cam_lock")) {
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
        Pawn pawn = _firstCellToTp.GetPawns()[0];
        pawn?.MoveToCell(cell, true);
        _firstCellToTp = null;
      }
    }
  }

  public override void OnReady() {
    if (!_showMouse) Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  private Vector3 UpdateDirection() {
    Vector3 dir = new();
    if (Input.IsActionPressed("free_cam_forward")) dir += Vector3.Forward;
    if (Input.IsActionPressed("free_cam_backward")) dir += Vector3.Back;
    if (Input.IsActionPressed("free_cam_left")) dir += Vector3.Left;
    if (Input.IsActionPressed("free_cam_right")) dir += Vector3.Right;
    if (Input.IsActionPressed("free_cam_up")) dir += Vector3.Up;
    if (Input.IsActionPressed("free_cam_down")) dir += Vector3.Down;
    if (dir == Vector3.Zero) _velocity = Vector3.Zero;
    return dir.Normalized();
  }

  private void UpdateCameraSpeed() {
    if (Input.IsActionJustPressed("free_cam_increase_speed")) Speed += Speed / 2;
    if (Input.IsActionJustPressed("free_cam_decrease_speed")) Speed -= Speed / 2;
    if (Speed < 0.5) Speed = 0.5f;
    if (Speed > 1000.0) Speed = 1000.0f;
  }
}