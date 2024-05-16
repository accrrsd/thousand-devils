using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class FortressLogic : BaseLogic
{
  public FortressLogic(Cell cell) : base(cell) { }

  public override bool CanAcceptThatPawn(Pawn pawn) {
    if (!base.CanAcceptThatPawn(pawn)) return false;
    bool canAcceptPawn = Cell.GetPawns().Count == 0 || (Cell.GetPawns().Count > 0 && pawn.OwnerPlayer == Cell.GetPawns()[0].OwnerPlayer);
    bool pawnDontHaveItem = pawn.CarryItem == PawnItems.None;
    return canAcceptPawn && pawnDontHaveItem;
  }
}