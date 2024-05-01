using Godot;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.pawn.code;
using static ThousandDevils.features.GlobalUtils.LoadedPackedScenes;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class ShipLogic : BaseLogic
{
  public ShipLogic(Cell cell) : base(cell)
  {
    Cell.IsOpen = true;
    // late todo: 3 by default - but we need to think about it as parameter in editor but only if cell type is ship
    Cell.AddPawn(DefaultPawnNodeScene.Instantiate<Pawn>(), false);
    Cell.AddPawn(DefaultPawnNodeScene.Instantiate<Pawn>(), false);
    Cell.AddPawn(DefaultPawnNodeScene.Instantiate<Pawn>(), false);
  }

  public void SwitchPlacesWithCell(Cell otherCell)
  {
    Field field = Cell.Field;
    Vector2I currentCords = Cell.GridCords;
    Vector2I otherCellCords = otherCell.GridCords;
    Vector3 currentGlobalPos = Cell.GlobalPosition;
    Vector3 otherCellGlobalPos = otherCell.GlobalPosition;

    field.UpdateCellGridCords(Cell, otherCellCords);
    field.UpdateCellGridCords(otherCell, currentCords);

    Cell.GlobalPosition = otherCellGlobalPos;
    otherCell.GlobalPosition = currentGlobalPos;

    field.Game.TurnSystem.CurrentTurn++;
  }
}
