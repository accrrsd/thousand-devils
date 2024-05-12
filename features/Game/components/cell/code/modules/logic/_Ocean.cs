namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class OceanLogic : BaseLogic
{
  public OceanLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded -= DiscoverCell;
    Cell.IsOpen = true;
    // todo Должна быть дичайшая обработка океана, пешка за бортом!
    // Cell.CanAcceptPawns = false;
  }
}
