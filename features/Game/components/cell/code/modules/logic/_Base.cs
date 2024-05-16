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
      enemyPawns[0].MoveToCell(enemyPawns[0].OwnerPlayer.Ship, false);
      if (enemyPawns.Count > 1) pawn.MoveToCell(pawn.OwnerPlayer.Ship, false);
    }
  }

  // can be rewritten, modified or (canceled by -= ) in childs
  protected virtual void DiscoverCell(Cell _, Pawn _2) => Cell.IsOpen = true;

  public virtual bool CanAcceptThatPawn(Pawn pawn) => pawn != null;
}