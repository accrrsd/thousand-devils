using System;
using Godot;
using Godot.Collections;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.cell.code.modules.logic;

namespace ThousandDevils.features.Game.components.camera.code.modules.modes;

public class BaseMode
{
  protected readonly Camera Camera;

  //todo (IF WE HAVE MORE THAN 1 CAMERA MODE) add logic for sharing firstCell as static, because of change camera mode when cell is selected
  private Cell _firstCell;

  protected BaseMode(Camera camera) {
    Camera = camera;
  }

  public BaseLogic ForcedByCellLogic { get; set; }

  public virtual void OnProcess(double delta) { }
  public virtual void OnInput(InputEvent @event) { }
  public virtual void OnReady() { }

  public event Action<Node> OnRayCast;

  protected Node ShootRay(int rayLength = 1000) {
    Vector2 mousePos = Camera.GetViewport().GetMousePosition();
    Vector3 from = Camera.ProjectRayOrigin(mousePos);
    Vector3 to = from + Camera.ProjectRayNormal(mousePos) * rayLength;
    PhysicsDirectSpaceState3D space = Camera.GetWorld3D().DirectSpaceState;
    PhysicsRayQueryParameters3D rayQuery = new() {
      From = from,
      To = to
    };
    Dictionary res = space.IntersectRay(rayQuery);
    if (res.Count == 0 || !res.ContainsKey("collider")) return null;
    StaticBody3D staticBody = (StaticBody3D)res["collider"];
    Node node = staticBody.GetParent();
    if (node is null) return null;
    OnRayCast?.Invoke(node);
    return node;
  }

  protected virtual void CameraLogic(Cell targetCell) {
    //if forced by some cell logic, click on non highlighted cell do nothing
    if (ForcedByCellLogic != null) {
      _firstCell = null;
      if (Camera.Game.Field.HighlightedCells.Contains(targetCell)) ForcedByCellLogic.OnHighlightCellClick(targetCell);
    }
    //click on first cell
    else if (_firstCell == null) {
      //if first cell is accepted
      if (targetCell.Logic.OnCellClick())
        _firstCell = targetCell;
      else
        GD.Print("Cords: ", targetCell.GridCords, " Logic: ", targetCell.Logic.GetType().Name);
    }
    //click on highlighted cell
    else if (Camera.Game.Field.HighlightedCells.Contains(targetCell)) {
      _firstCell.Logic.OnHighlightCellClick(targetCell);
      Camera.Game.Field.SwitchHighlightNeighbors(_firstCell, false);
      _firstCell = null;
    }
    //click on any cell on map
    else {
      Camera.Game.Field.SwitchHighlightNeighbors(_firstCell, false);
      _firstCell = null;
    }
  }
}