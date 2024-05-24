using System;
using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;
using ThousandDevils.features.GlobalUtils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class EarthquakeLogic : BaseLogic
{
  private int _changedCellsCount = 2;
  private Cell _selectedCell = null;
  private Color _selectedColor;

  public EarthquakeLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
    _selectedColor = UtilsFunctions.GenerateColorFromRgb(247, 127, 0);
  }

  private void HighlightOpenedCells() {
    List<Vector2I> highlightedCellsCords = new();

    for (int x = 0; x < Cell.Field.FieldSize.Item1; x++) {
      for (int z = 0; z < Cell.Field.FieldSize.Item2; z++) {
        Cell currentCell = Cell.Field.GetCellFromCellsGrid(x, z);
        bool opened = currentCell.IsOpen;
        if (!opened) continue;
        // todo НЕ РАБОТАЕТ БЛЯТЬ !1!!! (Пиздец, нахуй, блять)
        // todo Поглядеть мол не работает Cell.Field.HighlightedCells (Жопа)
        // todo Для Шипера (ДаНьКа ака Дантесс ака dAИ0И_B_Kedax2009.png ака Дано ака Д-Д-Даня) Изменять Cell.Field.HighlightedCells в Cell.IsHighlighted
        // if (currentCell.Type == CellType.Ocean) continue;
        if (currentCell.Type == CellType.Ship) continue;
        // if (Cell.Type == CellType.PossibleShip) continue;
        if (currentCell.GetPawns().Count > 0) continue;
        // todo Сделать проверку на то, лежат ли предметы на клетке (Хуй)
        highlightedCellsCords.Add(currentCell.GridCords);
      }
    }

    if (highlightedCellsCords.Count <= 1) {
      Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
      return;
    }

    Cell.Field.SwitchHighlightByCords(true, highlightedCellsCords);
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
      highlightedCell.ChangeHighlightedBorderColor(default);
      _selectedCell = null;
      return true;
    }

    SwitchCellsPlaces(_selectedCell, highlightedCell);
    _selectedCell.ChangeHighlightedBorderColor(default);
    _selectedCell = null;
    _changedCellsCount--;
    
    if (_changedCellsCount == 0) {
      Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
      Cell.Field.ClearHighlighedCells();
    }
    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.ClearHighlighedCells();
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    HighlightOpenedCells();
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }
}