using System;
using System.Collections.Generic;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class RumTrapLogic : BaseLogic
{
  private readonly List<Tuple<Pawn, int>> _stuckPawns = new();


  public RumTrapLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

//todo Та же проблема что с стандартным ромом.
  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    if (pawn == null) return;
    if (_stuckPawns.Count == 0) Cell.Field.Game.TurnModule.OnCircleChange += Wrapper;

    _stuckPawns.Add(new Tuple<Pawn, int>(pawn, Cell.Field.Game.TurnModule.CurrentCircle));
    pawn.CanMove = false;

    void Wrapper(int currentCircle) {
      foreach (Tuple<Pawn, int> pawnAndCurrentCircle in _stuckPawns) {
        if (pawnAndCurrentCircle.Item2 + 2 > currentCircle) return;
        pawnAndCurrentCircle.Item1.CanMove = true;
      }

      if (_stuckPawns.Count == 0) Cell.Field.Game.TurnModule.OnCircleChange -= Wrapper;
    }
  }
}