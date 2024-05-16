using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class RumTrapLogic : BaseLogic
{
  public RumTrapLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    pawn.CanMove = false;

    void Wrapper(int __) {
      pawn.CanMove = true;
      Cell.Field.Game.TurnModule.OnCircleChange -= Wrapper;
    }

    Cell.Field.Game.TurnModule.OnCircleChange += Wrapper;
  }
}