using System;
using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.GlobalUtils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class EarthquakeLogic : BaseLogic
{
  private int _changedCellsCount = 2;
  private List<Cell> _selectedCells = new ();
  private Color _selectedColor;

  public EarthquakeLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
    _selectedColor = UtilsFunctions.GenerateColorFromRgb(247, 127, 0);
  }

  private void HighlightClosedCells() {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    List<Vector2I> highlightedCellsCords = new();

    for (int x = 0; x < Cell.Field.FieldSize.Item1; x++) {
      for (int z = 0; z < Cell.Field.FieldSize.Item2; z++) {
        Cell currentCell = Cell.Field.GetCellFromCellsGrid(x, z);
        bool opened = currentCell.IsOpen;
        if (!opened) continue;
        // if (currentCell.Type == utils.CellType.Ocean) continue;
        if (currentCell.GetPawns().Count > 0) continue;
        // todo Сделать проверку на то, лежат ли предметы на клетке
        highlightedCellsCords.Add(currentCell.GridCords);
      }
    }

    if (highlightedCellsCords.Count == 0) {
      Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
      return;
    }

    Cell.Field.SwitchHighlightByCords(true, highlightedCellsCords);
  }
  
  private void SwitchCellsPlaces(List<Cell> cells) {
    Field field = Cell.Field;
    Cell firstCell = cells[0];
    Cell secondCell = cells[1];
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
    //if user cancel selection
    // if (_selectedCells.Contains(highlightedCell)) {
    //   highlightedCell.ChangeHighlightedBorderColor(default);
    //   _selectedCells.Remove(highlightedCell);
    //   return true;
    // }
    //if user click on first two cells
    
    //todo НЕ РАБОТАЕТ БЛЯТЬ
    if (_selectedCells.Count > 2) return false;
    _selectedCells.Add(highlightedCell);
    _selectedCells[0].ChangeHighlightedBorderColor(_selectedColor);
    if (_selectedCells.Count > 1) {
      SwitchCellsPlaces(_selectedCells);
      foreach (Cell cell in _selectedCells) cell.ChangeHighlightedBorderColor(default); 
      _selectedCells.Clear();
      _changedCellsCount--;
      if (_changedCellsCount == 0) {
        Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
        Cell.Field.ClearHighlighedCells();
      }
    }
    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.ClearHighlighedCells();
    HighlightClosedCells();
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }
}