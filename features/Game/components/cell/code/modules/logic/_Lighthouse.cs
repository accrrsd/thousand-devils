using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class LightHouseLogic : BaseLogic
{
  private int _openedCellsCount = 4;

  public LightHouseLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void HighlightClosedCells() {
    if (Cell.Field.SwitchHighlightWholeField(true, pCell => !pCell.IsOpen).Count > 0) return;
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
  }

  public override bool OnHighlightedCellClick(Cell highlightedCell) {
    highlightedCell.IsOpen = true;
    highlightedCell.UpdateHighlight(false);
    _openedCellsCount--;
    if (_openedCellsCount == 0 || Cell.Field.GetHighlightedCells().Count == 0) {
      Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
      Cell.Field.ClearHighlightedCells();
    }

    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.ClearHighlightedCells();
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    HighlightClosedCells();
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }
}