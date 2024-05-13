using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.GlobalUtils;
using static ThousandDevils.features.Game.utils.Variables;
using static ThousandDevils.features.GlobalUtils.GdUtilsFunctions;
using static ThousandDevils.features.GlobalUtils.UtilsFunctions;

namespace ThousandDevils.features.Game.components.field.code;

public partial class Field : Node3D
{
  public Game.code.Game Game { get; private set; }
  public List<Cell> HighlightedCells { get; set; } = new();
  public List<Cell> Cells { get; private set; } = new();
  public Cell[][] CellsGrid { get; private set; }

  public override void _Ready() {
    Cells = GetChildsByType<Cell>(this);
    ArrangeCellsBasedOnMapPosition();
    ConvertCellListIntoGrid();
    AssociateListOfChilds(this, Cells);
  }

  public Cell GetCellFromCellsGrid(Vector2I gridCords) => CellsGrid[gridCords[0]][gridCords[1]];
  public Cell GetCellFromCellsGrid(int x, int y) => CellsGrid[x][y];

  [AssociateAttributes.ParentSetter]
  public void UpdateGame(Game.code.Game game) => Game = game;

  // todo Нужно срочно фиксить определение позиции у клеток, т.к работает через раз.
  private void ArrangeCellsBasedOnMapPosition() {
    Cells.Sort((cell1, cell2) => {
      int compareX = cell1.GlobalPosition[0].CompareTo(cell2.GlobalPosition[0]);
      return compareX != 0 ? compareX : cell1.GlobalPosition[2].CompareTo(cell2.GlobalPosition[2]);
    });
  }

  private void ConvertCellListIntoGrid() {
    float minX = Cells[0].GlobalPosition[0], maxX = minX;
    float minZ = Cells[0].GlobalPosition[2], maxZ = minZ;

    foreach (Vector3 globalPos in Cells.Select(cell => cell.GlobalPosition)) {
      minX = Math.Min(minX, globalPos[0]);
      maxX = Math.Max(maxX, globalPos[0]);
      minZ = Math.Min(minZ, globalPos[2]);
      maxZ = Math.Max(maxZ, globalPos[2]);
    }

    int width = (int)(maxX - minX);
    int height = (int)(maxZ - minZ);
    // magic idk how + 1 works, maybe it will broke on small maps.
    width = width / CellSize + 1;
    height = height / CellSize + 1;

    CellsGrid = ListTo2DArray(Cells, width, height, (cell, _, col, row) => cell.GridCords = new Vector2I(col, row));
  }

  public void UpdateCellGridCords(Cell cell, Vector2I newCords) {
    CellsGrid[newCords[0]][newCords[1]] = cell;
    cell.GridCords = newCords;
  }

  public void ResetHighlightedCells(List<Cell> CellsToRemove = null) {
    if (CellsToRemove?.Count > 0) {
      HighlightedCells.RemoveAll(cell => {
        if (!CellsToRemove.Contains(cell)) return false;
        cell.IsHighlighted = false;
        return true;
      });
    }
    else {
      HighlightedCells.ForEach(cell => cell.IsHighlighted = false);
      HighlightedCells.Clear();
    }
  }

  public void SwitchHighlightNeighbors(Cell cell, bool value, Predicate<Cell> predicate = null, bool affectInsteadOfReplace = false) {
    int x = cell.GridCords[0];
    int y = cell.GridCords[1];
    List<Cell> affectedCells = new();
    for (int dx = -1; dx <= 1; dx++)
      for (int dy = -1; dy <= 1; dy++) {
        int neighborX = x + dx;
        int neighborY = y + dy;
        if (!IsIn2DArrayBounds(neighborX, neighborY, CellsGrid)) continue;
        Cell currentCell = CellsGrid[neighborX][neighborY];
        if (currentCell == cell) continue;
        if (predicate != null && !predicate(currentCell)) continue;
        currentCell.IsHighlighted = value;
        affectedCells.Add(currentCell);
      }

    if (value) {
      if (affectInsteadOfReplace) HighlightedCells.AddRange(affectedCells);
      else {
        ResetHighlightedCells();
        HighlightedCells = affectedCells;
      }
    }
    else ResetHighlightedCells(affectInsteadOfReplace ? affectedCells : null);
  }

  public void SwitchHighlightByCords(bool value, List<Vector2I> cords, bool affectInsteadOfReplace = false) {
    List<Cell> affectedCells = new();
    foreach (Vector2I cord in cords.Where(cord => IsIn2DArrayBounds(cord[0], cord[1], CellsGrid))) {
      CellsGrid[cord[0]][cord[1]].IsHighlighted = value;
      affectedCells.Add(CellsGrid[cord[0]][cord[1]]);
    }

    if (value) {
      if (affectInsteadOfReplace) HighlightedCells.AddRange(affectedCells);
      else HighlightedCells = affectedCells;
    }
    else {
      if (affectInsteadOfReplace) HighlightedCells.RemoveAll(cell1 => affectedCells.Contains(cell1));
      else HighlightedCells.Clear();
    }
  }
}
