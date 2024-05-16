using System.Collections.Generic;
using System.Linq;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class BaseLogic
{
  public BaseLogic(Cell cell) {
    Cell = cell;
    //default behavior
    Cell.PawnWasAdded += DiscoverCell;
    Cell.PawnWasAdded += PawnFight;
  }

  protected Cell Cell { get; }

  protected virtual void PawnFight(Cell _, Pawn pawn) {
    List<Pawn> enemyPawns = Cell.GetPawns().Where(p => p.OwnerPlayer != pawn.OwnerPlayer).ToList();
    if (enemyPawns.Count > 0) {
      enemyPawns[0].MoveToCell(enemyPawns[0].OwnerPlayer.Ship);
      if (enemyPawns.Count > 1) pawn.MoveToCell(pawn.OwnerPlayer.Ship);
    }
  }

  // this method called when pawn get into cell for discover it.
  protected virtual void DiscoverCell(Cell _, Pawn _2) => Cell.IsOpen = true;

  public virtual bool CanAcceptThatPawn(Pawn pawn) => pawn != null;

  /// <summary>This method is called when user clicks on this cell. When is done with true - cell was set as first in camera</summary>
  /// <returns>true if cell was accepted</returns>
  public virtual bool OnCellClick() {
    Pawn currentPawn = Cell.GetPawns().FirstOrDefault(p => p.OwnerPlayer.IsTurn && p.CanMove);
    return currentPawn != null && HighlightPawnMoves(currentPawn);
  }

  /// <summary>This method highlights moves for selected pawn. When is done with false, basically OnCellClick would be cancelled</summary>
  /// <returns>true if any cells was affected</returns>
  public virtual bool HighlightPawnMoves(Pawn pawn) {
    return Cell.Field.SwitchHighlightNeighbors(Cell, true, pCell => pCell.Type != CellType.Ocean && pCell.CanAcceptPawns && pCell.Logic.CanAcceptThatPawn(pawn))
      .Count > 0;
  }

  /// <summary>
  ///   This method is called when user clicks on highlighted cell when that cell selected (in camera). When is done with true - camera can apply addition logic (not implemented yet)
  /// </summary>
  /// <returns>True if highlight cell was accepted</returns>
  public virtual bool OnHighlightCellClick(Cell highlightedCell) {
    Pawn currentPawn = Cell.GetPawns().FirstOrDefault(p => p.OwnerPlayer.IsTurn && p.CanMove);
    //move to cell already check if can accept and if logic can accept
    return currentPawn != null && currentPawn.MoveToCell(highlightedCell, true);
  }
}