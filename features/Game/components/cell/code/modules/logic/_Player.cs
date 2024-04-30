using GameSpace.FieldSpace;
using Godot;

namespace GameSpace.CellSpace.modules.logic;

public class PlayerLogic : BaseLogic
{
  public PlayerLogic(Cell cell) : base(cell) { }
  public void SwitchPlacesWithCell(Cell otherCell)
  {
    Field Field = Cell.Field;
    Vector2I currentCords = Cell.GridCords;
    Vector2I otherCellCords = otherCell.GridCords;
    Vector3 currentGlobalPos = Cell.GlobalPosition;
    Vector3 otherCellGlobalPos = otherCell.GlobalPosition;

    Field.UpdateCellGridCords(Cell, otherCellCords);
    Field.UpdateCellGridCords(otherCell, currentCords);

    Cell.GlobalPosition = otherCellGlobalPos;
    otherCell.GlobalPosition = currentGlobalPos;
  }
}