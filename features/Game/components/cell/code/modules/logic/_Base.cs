using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class BaseLogic
{
  public BaseLogic(Cell cell) {
    Cell = cell;
    //default behavior
    Cell.PawnWasAdded += DiscoverCell;
  }

  protected Cell Cell { get; }

  // can be rewritten, modified or (canceled by -= ) in childs
  protected void DiscoverCell(Cell _, Pawn _2) => Cell.IsOpen = true;
}
