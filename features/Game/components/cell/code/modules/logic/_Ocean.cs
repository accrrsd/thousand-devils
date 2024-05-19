using System.Collections.Generic;
using System.Linq;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class OceanLogic : BaseLogic
{
  public OceanLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded -= DiscoverCell;
    Cell.PawnWasAdded += OnPawnWasAdded;
    Cell.IsOpen = true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    pawn.CarryItem = PawnItems.None;
  }

  protected override bool HighlightPawnMoves(Pawn pawn) {
    if (!base.HighlightPawnMoves(pawn)) return false;
    List<Cell> highlightedCells = Cell.Field.SwitchHighlightNeighbors(Cell, true,
      pCell => (pCell.Type == CellType.Ocean || pCell.Type == CellType.Ship) && pCell.CanAcceptPawns && pCell.Logic.CanAcceptThatPawn(pawn));
    return highlightedCells.Count != 0;
  }

  protected override void PawnFight(Cell _, Pawn pawn) {
    List<Pawn> enemyPawns = Cell.GetPawns().Where(p => p.OwnerPlayer != pawn.OwnerPlayer).ToList();

    if (enemyPawns.Count > 0) {
      enemyPawns[0].Die();
      if (enemyPawns.Count > 1) pawn.Die();
    }
  }
}

// todo Должна быть дичайшая обработка океана, пешка за бортом!