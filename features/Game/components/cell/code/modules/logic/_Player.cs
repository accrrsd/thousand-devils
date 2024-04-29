using Godot;

namespace GameSpace.CellSpace.modules.logic;

public class PlayerLogic : BaseLogic
{
  public PlayerLogic(Cell cell) : base(cell) { }
  public void SwitchPlacesWithCell(Cell otherCell)
  {
    Vector2I currentCords = Cell.GridCords;
    Vector2I otherCellCords = otherCell.GridCords;
    Vector3 currentGlobalPos = Cell.GlobalPosition;
    Vector3 otherCellGlobalPos = otherCell.GlobalPosition;

    Cell.Field.UpdateCellGridCords(Cell, otherCellCords);
    otherCell.Field.UpdateCellGridCords(otherCell, currentCords);

    Cell.GlobalPosition = otherCellGlobalPos;
    otherCell.GlobalPosition = currentGlobalPos;
  }
}