using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class LightHouseLogic : BaseLogic
{
  private int _openedCellsCount = 4;

  public LightHouseLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void HighlightClosedCells() {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    List<Vector2I> highlightedCellsCords = new();

    for (int x = 0; x < Cell.Field.FieldSize.Item1; x++) {
      for (int z = 0; z < Cell.Field.FieldSize.Item2; z++) {
        Cell currentCell = Cell.Field.GetCellFromCellsGrid(x, z);
        bool opened = currentCell.IsOpen;
        if (opened) continue;
        highlightedCellsCords.Add(currentCell.GridCords);
      }
    }

    if (highlightedCellsCords.Count == 0) {
      Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
      return;
    }

    Cell.Field.SwitchHighlightByCords(true, highlightedCellsCords);
  }

  public override bool OnHighlightedCellClick(Cell highlightedCell) {
    highlightedCell.IsOpen = true;
    highlightedCell.IsHighlighted = false;
    _openedCellsCount--;
    if (_openedCellsCount == 0) {
      Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
      Cell.Field.ClearHighlighedCells();
    }

    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.ClearHighlighedCells();
    HighlightClosedCells();
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }
}