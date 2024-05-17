using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class HorseLogic : BaseLogic
{
  private readonly List<Vector2I> _possibleDirections = new() {
    new Vector2I(-1, -2),
    new Vector2I(1, -2),
    new Vector2I(-2, -1),
    new Vector2I(2, -1),
    new Vector2I(-2, 1),
    new Vector2I(2, 1),
    new Vector2I(-1, 2),
    new Vector2I(1, 2)
  };

  public HorseLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void HighlightCellsWithDirection(Pawn pawn) {
    List<Cell> highlightedCells = Cell.Field.SwitchHighlightByCords(true, _possibleDirections.Select(direction => Cell.GridCords + direction).ToList(),
      pCell => !pCell.IsOpen || (pCell.CanAcceptPawns && pCell.Logic.CanAcceptThatPawn(pawn)));
    if (highlightedCells.Count != 0) return;
    pawn.Die();
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
  }

  public override bool OnHighlightedCellClick(Cell highlightedCell) {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
    Cell.Field.SwitchHighlightNeighbors(Cell, false);
    if (Cell.GetPawns().Count == 0) return false;
    Pawn currentPawn = Cell.GetPawns()[0];
    highlightedCell.Logic.OnThisCellClickAsHighlighted(Cell);
    //always false for turn increase because we already increase it when get here.
    if (!currentPawn.MoveToCell(highlightedCell))
      currentPawn.Die();
    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.ClearHighlighedCells();
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    HighlightCellsWithDirection(pawn);
  }
}