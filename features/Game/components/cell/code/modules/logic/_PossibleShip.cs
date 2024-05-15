namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class PossibleShipLogic : BaseLogic
{
  public PossibleShipLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
  }

  public void ReplaceByShip(Cell shipCell) {
    shipCell.Position = Cell.Position;
    shipCell.SetField(Cell.Field);
    shipCell.Field.UpdateCellGridCords(shipCell, Cell.GridCords);
    Cell.GetParent().AddChild(shipCell);
    Cell.QueueFree();
  }
}