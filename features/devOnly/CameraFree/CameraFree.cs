using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.cell.code.modules.logic;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.devOnly.CameraFree;

public partial class CameraFree : Camera3D
{
  // dev only variables - for player movement
  private Cell _firstCell;
  private List<Cell> _highlightedNeighbors = new();
  private Vector2 _lookAngles = Vector2.Zero;
  private bool _showMouse;

  private Vector3 _velocity = Vector3.Zero;
  [Export] public float Speed { get; set; } = 5.0f;
  [Export] public float MouseSensitivity { get; set; } = 300.0f;
  [Export] public float Acceleration { get; set; } = 25.0f;
  [Export] public int RayLength { get; set; } = 1000;

  // maybe dev only
  [Export] public Field Field { get; private set; }

  public override void _Ready()
  {
    if (!_showMouse) Input.MouseMode = Input.MouseModeEnum.Captured;
  }

  public override void _Process(double delta)
  {
    if (!_showMouse)
    {
      _lookAngles[1] = (float)Math.Clamp(_lookAngles[1], Math.PI / -2, Math.PI / 2);
      Rotation = new Vector3(_lookAngles[1], _lookAngles[0], 0);
    }

    Vector3 direction = UpdateDirection();
    if (direction.LengthSquared() > 0) _velocity = direction * Acceleration * (float)delta;
    Translate(_velocity * (float)delta * Speed);
  }

  public override void _Input(InputEvent @event)
  {
    base._Input(@event);
    if (@event is InputEventMouseMotion mouseMotion)
      // todo fix mouse twitching
      _lookAngles -= mouseMotion.Relative / MouseSensitivity;
    UpdateCameraSpeed();
    if (Input.IsActionJustPressed("free_cam_lock")) UpdateCameraLock();
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
    if (dir == Vector3.Zero) _velocity = Vector3.Zero;
    return dir.Normalized();
  }

  private void UpdateCameraLock()
  {
    _showMouse = !_showMouse;
    Input.MouseMode = _showMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
  }

  private void UpdateCameraSpeed()
  {
    if (Input.IsActionJustPressed("free_cam_increase_speed")) Speed += Speed / 2;
    if (Input.IsActionJustPressed("free_cam_decrease_speed")) Speed -= Speed / 2;
    if (Speed < 0.5) Speed = 0.5f;
  }

  // dev function for click on the cells
  private void ShootRay()
  {
    if (!_showMouse) return;
    Vector2 mousePos = GetViewport().GetMousePosition();
    Vector3 from = ProjectRayOrigin(mousePos);
    Vector3 to = from + ProjectRayNormal(mousePos) * RayLength;
    PhysicsDirectSpaceState3D space = GetWorld3D().DirectSpaceState;
    PhysicsRayQueryParameters3D rayQuery = new()
    {
      From = from,
      To = to
    };
    Dictionary res = space.IntersectRay(rayQuery);
    if (res.Count == 0 || !res.ContainsKey("collider")) return;
    StaticBody3D staticBody = (StaticBody3D)res["collider"];
    Node node = staticBody.GetParent();
    if (node is not Cell cell) return;

    // cell.IsOpen = !cell.IsOpen; -- dev checking maps visibility

    // DEV ONLY below - player movement
    // if click on player that have turn
    if (cell.GetPawns().Any(pawn => pawn.OwnerPlayer.IsTurn))
    {
      _highlightedNeighbors = cell.Type == CellType.Ship ? Field.HighlightNeighbors(cell, true) : Field.HighlightNeighbors(cell, true, pCell => pCell.Type != CellType.Ocean);
      _firstCell = cell;
      return;
    }

    // if click on highlighted cell
    if (_highlightedNeighbors.Contains(cell))
    {
      Field.HighlightNeighbors(_firstCell, false);
      if (cell.Type == CellType.Ocean && _firstCell.Type is CellType.Ship) ((ShipLogic)_firstCell.Logic).SwitchPlacesWithCell(cell);
      else _firstCell.GetPawns()[0].MoveToCell(cell);

      _firstCell = null;
      _highlightedNeighbors = new List<Cell>();
    }
    // if click on non highlighted cell
    else if (_firstCell != null)
    {
      Field.HighlightNeighbors(_firstCell, false);
      _firstCell = null;
      _highlightedNeighbors = new List<Cell>();
    }
    else
    {
      // if any click on map
      GD.Print(cell.GridCords);
    }
  }
}
