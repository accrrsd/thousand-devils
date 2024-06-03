using System;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class RumLogic : BaseLogic
{
  private bool _containsRum = true;
  private readonly Random _random = new();
  private readonly int _rumCount;

  public RumLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
    _rumCount = _random.Next(1, 4);
  }
    private void OnPawnWasAdded(Cell _, Pawn pawn) {
    if (_containsRum) pawn.OwnerPlayer.RumCount += _rumCount;
    _containsRum = false;
  }
}