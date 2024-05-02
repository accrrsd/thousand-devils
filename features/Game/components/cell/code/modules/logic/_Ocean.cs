namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class OceanLogic : BaseLogic
{
  public OceanLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded -= DiscoverCell;
    Cell.IsOpen = true;
    Cell.CanAcceptPawns = false;
  }
}
