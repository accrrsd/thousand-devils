using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;
using ThousandDevils.features.GlobalUtils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class EarthquakeLogic : BaseLogic
{
  private readonly Color _selectedColor;
  private int _changedCellsCount = 2;
  private Cell _selectedCell;

  public EarthquakeLogic(Cell cell) : base(cell) {
    //Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
    _selectedColor = UtilsFunctions.GenerateColorFromRgb(247, 127, 0);
  }

  private void HighlightOpenedCells() {
    //todo Добавить проверку на то, лежат ли пердметы на клетке (Не подсвечивать их)
    List<Cell> highlightedCells = Cell.Field.SwitchHighlightWholeField(true,
      pCell => pCell.IsOpen && pCell.Type != CellType.Ocean && pCell.Type != CellType.Ship && pCell.Type != CellType.PossibleShip);

    //earthquake for work needed 2 cells, so we cancel logic if less then 2.
    if (highlightedCells.Count < 2) Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
  }

  private void SwitchCellsPlaces(Cell firstCell, Cell secondCell) {
    Field field = Cell.Field;
    Vector2I firstCellCords = firstCell.GridCords;
    Vector2I secondCellCords = secondCell.GridCords;
    Vector3 firstCellGlobalPos = firstCell.GlobalPosition;
    Vector3 secondCellGlobalPos = secondCell.GlobalPosition;

    field.UpdateCellGridCords(firstCell, secondCellCords);
    field.UpdateCellGridCords(secondCell, firstCellCords);

    firstCell.GlobalPosition = secondCellGlobalPos;
    secondCell.GlobalPosition = firstCellGlobalPos;
  }

  public override bool OnHighlightedCellClick(Cell highlightedCell) {
    if (_selectedCell == null) {
      highlightedCell.ChangeHighlightedBorderColor(_selectedColor);
      _selectedCell = highlightedCell;
      return true;
    }

    if (_selectedCell == highlightedCell) {
      highlightedCell.ChangeHighlightedBorderColor();
      _selectedCell = null;
      return true;
    }

    SwitchCellsPlaces(_selectedCell, highlightedCell);
    _selectedCell.ChangeHighlightedBorderColor();
    _selectedCell = null;
    _changedCellsCount--;

    if (_changedCellsCount == 0) {
      Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
      Cell.Field.ClearHighlightedCells();
    }

    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.ClearHighlightedCells();
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    HighlightOpenedCells();
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }
}
