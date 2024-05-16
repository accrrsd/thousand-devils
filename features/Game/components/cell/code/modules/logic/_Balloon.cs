using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class BalloonLogic : BaseLogic
{
  public BalloonLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    pawn.MoveToCell(pawn.OwnerPlayer.Ship);
  }
}