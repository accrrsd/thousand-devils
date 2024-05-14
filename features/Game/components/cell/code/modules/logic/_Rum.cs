using System;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class RumLogic : BaseLogic
{
  private readonly Random _random = new();
  private readonly int _rumCount;
  public RumLogic(Cell cell) : base(cell) {
    Cell.wasDiscovered += OnCellWasDiscovered; 
    _rumCount = _random.Next(1, 4);
  }

  private void OnCellWasDiscovered(Cell _, Pawn pawn) {
    pawn.OwnerPlayer.RumCount += _rumCount;
  }
}
