using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class JunglesLogic : BaseLogic
{
  public JunglesLogic(Cell cell) : base(cell) { }

  protected override void PawnFight(Cell _, Pawn pawn) { }

  public override bool CanAcceptThatPawn(Pawn pawn) {
    if (!base.CanAcceptThatPawn(pawn)) return false;
    return pawn.CarryItem == PawnItems.None;
  }
}