using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class RumTrapLogic : BaseLogic
{
  private int _currentCircle;

  public RumTrapLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  //todo В текущей логике будет ошибка, если на клетке стояла пешка и добавилась еще одна. Это реально и можно пофиксить через list pawn, currentCircle

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    pawn.CanMove = false;
    _currentCircle = Cell.Field.Game.TurnModule.CurrentCircle;

    void Wrapper(int newCircleNumber) {
      if (_currentCircle + 2 > newCircleNumber) return;
      pawn.CanMove = true;
      _currentCircle = 0;
      Cell.Field.Game.TurnModule.OnCircleChange -= Wrapper;
    }

    Cell.Field.Game.TurnModule.OnCircleChange += Wrapper;
  }
}
