using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class TrapLogic : BaseLogic
{
  public TrapLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    if (Cell.GetPawns().Count == 1) {
      pawn.CanMove = false;
      return;
    }

    foreach (Pawn pawnInCell in Cell.GetPawns()) pawnInCell.CanMove = true;
  }
}