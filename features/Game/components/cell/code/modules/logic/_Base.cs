using System.Collections.Generic;
using System.Linq;
using ThousandDevils.features.Game.components.pawn.code;

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

  protected void PawnFight(Cell _, Pawn pawn) {
    List<Pawn> EnemyPawns = Cell.GetPawns().Where(p => p.OwnerPlayer != pawn.OwnerPlayer).ToList();
    if (EnemyPawns.Count > 0) {
      EnemyPawns[0].MoveToCell(EnemyPawns[0].OwnerPlayer.Ship, false);
      if (EnemyPawns.Count > 1) pawn.MoveToCell(pawn.OwnerPlayer.Ship, false);
    }
  }

  // can be rewritten, modified or (canceled by -= ) in childs
  protected void DiscoverCell(Cell _, Pawn _2) => Cell.IsOpen = true;
}
