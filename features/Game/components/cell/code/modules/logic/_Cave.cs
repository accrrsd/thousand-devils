using System.Collections.Generic;
using System.Linq;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;
public class CaveLogic : BaseLogic
{
  public CaveLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
    Cell.PawnWasRemoved += OnPawnWasRemoved;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    List<Cell> highlightedCells = Cell.Field.SwitchHighlightWholeField(true,
      pCell => pCell != Cell && pCell.IsOpen && pCell.Type == CellType.Cave);

    if (highlightedCells.Count == 0) {
      pawn.CanMove = false;
      return;
    }
    if (highlightedCells.Count == 1) {
      // without additional check because of block logic (pawn would already stay in first highlighted cell)
      Pawn otherPawn = highlightedCells[0].GetPawns()[0];
      pawn.CanMove = false;
      return;
    }
  }

  protected override void PawnFight(Cell _, Pawn pawn) {
    List<Pawn> enemyPawns = Cell.GetPawns().Where(p => p.OwnerPlayer != pawn.OwnerPlayer).ToList();
    if (enemyPawns.Count > 0) {
      enemyPawns[0].MoveToCell(enemyPawns[0].OwnerPlayer.Ship);
      pawn.MoveToCell(pawn.PrevCell);
    }
  }

  //Cell can accept only if empty, or if pawn try to move from another cave
  public override bool CanAcceptThatPawn(Pawn newPawn) {
    if (base.CanAcceptThatPawn(newPawn)) return false;
    if (Cell.GetPawns().Count == 0) return true;
    if (Cell.GetPawns().Count > 0 &&
    (newPawn.CurrentCell.Type == CellType.Cave || Cell.GetPawns()[0].OwnerPlayer == newPawn.OwnerPlayer)) return true;
    return false;
  }

  private void OnPawnWasRemoved(Cell _, Pawn pawn) {
    pawn.CanMove = true;
  }
}
