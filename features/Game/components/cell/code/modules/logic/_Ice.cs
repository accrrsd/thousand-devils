using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using static ThousandDevils.features.GlobalUtils.UtilsFunctions;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class IceLogic : BaseLogic
{
  public IceLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Vector2I direction = (Cell.GridCords - pawn.PrevCell.GridCords).Sign();
    if (direction == Vector2I.Zero) return;
    Vector2I newGridPos = Cell.GridCords + direction;
    Cell targetCell = Cell.Field.GetCellFromCellsGrid(newGridPos);
    if (targetCell.CanAcceptPawns && IsIn2DArrayBounds(newGridPos, Cell.Field.CellsGrid))
      pawn.MoveToCell(targetCell, false);
  }
}