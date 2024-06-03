using System;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class RumLogic : BaseLogic
{
  private readonly Random _random = new();
  private readonly int _rumCount;

  public RumLogic(Cell cell) : base(cell) {
    Cell.WasDiscovered += OnCellWasDiscovered;
    _rumCount = _random.Next(1, 4);
  }

//todo Обратить внимание и обсудить это.
  private void OnCellWasDiscovered(Cell _, Pawn pawn) {
    if (pawn == null) return;
    pawn.OwnerPlayer.RumCount += _rumCount;
  }
}