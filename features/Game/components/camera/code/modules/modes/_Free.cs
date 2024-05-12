using System;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.cell.code.modules.logic;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.camera.code.modules.modes;

public class FreeMode : BaseMode
{
  private Cell _firstCell;
  private Vector2 _lookAngles = Vector2.Zero;
  private bool _showMouse;
  private Vector3 _velocity = Vector3.Zero;

  public FreeMode(Camera camera) : base(camera) { }

  public FreeMode(Camera camera, float speed, float mouseSensitivity, float acceleration) : base(camera) {
    UpdateCameraStats(speed, mouseSensitivity, acceleration);
  }

  public float Speed { get; private set; } = 5.0f;
  public float MouseSensitivity { get; private set; } = 300.0f;
  public float Acceleration { get; private set; } = 25.0f;

  public void UpdateCameraStats(float speed, float mouseSensitivity, float acceleration) {
    Speed = speed;
    MouseSensitivity = mouseSensitivity;
    Acceleration = acceleration;
  }

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
    if (@event is InputEventMouseMotion mouseMotion)
      _lookAngles -= mouseMotion.Relative / MouseSensitivity;
    UpdateCameraSpeed();
    if (Input.IsActionJustPressed("free_cam_lock")) UpdateCameraLock();
    if (Input.IsActionJustPressed("lmb_click")) {
      Node rayCastRes = ShootRay();
      if (ProcessDefaultRayCast && rayCastRes is Cell cell) DefaultCameraLogic(cell);
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

  private void UpdateCameraLock() {
    _showMouse = !_showMouse;
    Input.MouseMode = _showMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
  }

  private void UpdateCameraSpeed() {
    if (Input.IsActionJustPressed("free_cam_increase_speed")) Speed += Speed / 2;
    if (Input.IsActionJustPressed("free_cam_decrease_speed")) Speed -= Speed / 2;
    if (Speed < 0.5) Speed = 0.5f;
  }

  private bool ClickOnHighlightedCell(Cell targetCell) {
    if (!Camera.Game.Field.HighlightedCells.Contains(targetCell)) return false;
    Camera.Game.Field.SwitchHighlightNeighbors(_firstCell, false);
    if (targetCell.Type == CellType.Ocean && _firstCell.Type is CellType.Ship) {
      ((ShipLogic)_firstCell.Logic).SwitchPlacesWithCell(targetCell);
      return true;
    }

    if (targetCell.CanAcceptPawns) {
      Pawn currentPawn = _firstCell.GetPawns().FirstOrDefault(p => p.OwnerPlayer == Camera.Game.TurnModule.GetActivePlayer());
      if (currentPawn == null) return false;
      currentPawn.MoveToCell(targetCell);
      return true;
    }

    return false;
  }

  private void DefaultCameraLogic(Cell targetCell) {
    // if click on player that have turn
    if (targetCell.GetPawns().Any(pawn => pawn.OwnerPlayer.IsTurn)) {
      if (targetCell.Type == CellType.Ship)
        Camera.Game.Field.SwitchHighlightNeighbors(targetCell, true);
      else
        Camera.Game.Field.SwitchHighlightNeighbors(targetCell, true, pCell => pCell.Type != CellType.Ocean);
      _firstCell = targetCell;
      return;
    }

    // if click on highlighted cell
    if (ClickOnHighlightedCell(targetCell)) {
      _firstCell = null;
    }

    // if click on non highlighted cell
    else if (_firstCell != null) {
      Camera.Game.Field.SwitchHighlightNeighbors(_firstCell, false);
      _firstCell = null;
    }
    else {
      // if any click on map
      GD.Print(targetCell.GridCords);
    }
  }
}
