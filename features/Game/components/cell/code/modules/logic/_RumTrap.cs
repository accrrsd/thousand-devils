using System;
using System.Collections.Generic;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class RumTrapLogic : BaseLogic
{
  private List<Tuple<Pawn, int>> _stuckPawns = new();
  

  public RumTrapLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    if (_stuckPawns.Count == 0) Cell.Field.Game.TurnModule.OnCircleChange += Wrapper;
    
    _stuckPawns.Add(new Tuple<Pawn, int>(pawn, Cell.Field.Game.TurnModule.CurrentCircle));
    pawn.CanMove = false;
    
    void Wrapper(int currentCircle) {
      foreach (Tuple<Pawn, int> pawnAndСurrentCircle in _stuckPawns) {
        if (pawnAndСurrentCircle.Item2 + 2 > currentCircle) return;
        pawnAndСurrentCircle.Item1.CanMove = true;
      }
      if (_stuckPawns.Count == 0) Cell.Field.Game.TurnModule.OnCircleChange -= Wrapper;
    }
  }
}
