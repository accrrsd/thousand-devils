using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.cell.code.modules.logic;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.camera.code.modules.modes;

public class VerticalPinnedMode : BaseMode
{
  private Cell _firstCell;
  private Vector3 _velocity = Vector3.Zero;

  public VerticalPinnedMode(Camera camera) : base(camera) { }

  public VerticalPinnedMode(Camera camera, float speed, float acceleration) : base(camera) {
    Speed = speed;
    Acceleration = acceleration;
  }

  public float Speed { get; set; } = 5.0f;
  public float Acceleration { get; } = 25.0f;

  private Vector3 GetDirection() {
    Vector3 dir = new();
    if (Input.IsActionPressed("free_cam_forward")) dir += Vector3.Forward;
    if (Input.IsActionPressed("free_cam_backward")) dir += Vector3.Back;
    if (Input.IsActionPressed("free_cam_left")) dir += Vector3.Left;
    if (Input.IsActionPressed("free_cam_right")) dir += Vector3.Right;
    if (dir == Vector3.Zero) _velocity = Vector3.Zero;
    return dir.Normalized();
  }


  public override void OnProcess(double delta) {
    base.OnProcess(delta);
    Vector3 direction = GetDirection();


    // if (direction.LengthSquared() > 0) _velocity = direction * Acceleration * (float)delta;
    // Vector3 currentSpeed = _velocity * (float)delta * Speed;
    // Vector3 newRes = new()
    // Camera.Translate(new Vector3(currentSpeed[0], 0, currentSpeed[2]));
  }

  public override void OnInput(InputEvent @event) {
    base.OnInput(@event);
    if (Input.IsActionJustPressed("lmb_click")) {
      Node rayCastRes = ShootRay();
      if (ProcessDefaultRayCast && rayCastRes is Cell cell) DefaultCameraLogic(cell);
    }
  }

  //Todo В двух тудушках ниже нужно учитывать что в камере у нас есть ProcessDefaultRayCast.

  // TODO Это можно вынести в базу логики клетки, чтобы логика была такая же, как в ArrowLogic.
  private bool ClickOnHighlightedCell(Cell targetCell) {
    if (!Camera.Game.Field.HighlightedCells.Contains(targetCell)) return false;
    Camera.Game.Field.SwitchHighlightNeighbors(_firstCell, false);
    if (targetCell.Type == CellType.Ocean && _firstCell.Type is CellType.Ship) {
      ((ShipLogic)_firstCell.Logic).SwitchPlacesWithCell(targetCell);
      return true;
    }

    if (targetCell.CanAcceptPawns) {
      Pawn currentPawn = _firstCell.GetPawns().FirstOrDefault(p => p.OwnerPlayer == Camera.Game.TurnModule.GetActivePlayer());
      if (currentPawn == null || targetCell.Logic.CanAcceptThatPawn(currentPawn)) return false;
      currentPawn.MoveToCell(targetCell);
      return true;
    }

    return false;
  }

  // Todo Если выполнить todo сверху, можно упростить и вынести в базовый класс режимов Камеры.
  private void DefaultCameraLogic(Cell targetCell) {
    // if click on player that have turn
    Pawn currentPawn = targetCell.GetPawns().FirstOrDefault(p => p.OwnerPlayer.IsTurn);
    if (currentPawn != null) _firstCell = currentPawn.HighlightMove();

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