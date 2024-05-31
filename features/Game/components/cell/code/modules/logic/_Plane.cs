using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class PlaneLogic : BaseLogic
{
  public PlaneLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void HighlightOpenedCells() {
    List<Vector2I> highlightedCellsCords = new();

    for (int x = 0; x < Cell.Field.FieldSize.Item1; x++) {
      for (int z = 0; z < Cell.Field.FieldSize.Item2; z++) {
        Cell currentCell = Cell.Field.GetCellFromCellsGrid(x, z);
        bool opened = currentCell.IsOpen;
        if (!opened) continue;
        if (currentCell.Type == CellType.Ocean) continue;
        highlightedCellsCords.Add(currentCell.GridCords);
      }
    }

    Cell.Field.SwitchHighlightByCords(true, highlightedCellsCords);
  }

  // public override bool OnHighlightedCellClick(Cell highlightedCell) {
  //   pawn.MoveToCell(highlightedCell);
  //   return true;
  // }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    Cell.Field.ClearHighlightedCells();
    HighlightOpenedCells();
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }
}