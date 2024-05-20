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
  public Tuple<int, int> FieldSize { get; private set; }

  public override void _Ready() {
    Cells = GetChildsByType<Cell>(this);
    ConvertCellListIntoGrid();
    AssociateListOfChilds(this, Cells);
  }

  public Cell GetCellFromCellsGrid(Vector2I gridCords) => CellsGrid[gridCords[0]][gridCords[1]];
  public Cell GetCellFromCellsGrid(int x, int y) => CellsGrid[x][y];

  [AssociateAttributes.ParentSetter]
  public void UpdateGame(Game.code.Game game) => Game = game;

  private void ConvertCellListIntoGrid() {
    float maxX = int.MinValue, maxZ = int.MinValue;
    float minX = int.MaxValue, minZ = int.MaxValue;

    foreach (Vector3 globalPos in Cells.Select(cell => cell.GlobalPosition)) {
      float x = globalPos[0] / CellSize;
      float z = globalPos[2] / CellSize;
      maxX = Math.Max(maxX, x);
      maxZ = Math.Max(maxZ, z);
      minX = Math.Min(minX, x);
      minZ = Math.Min(minZ, z);
    }

    float offsetX = 0 - minX;
    float offsetZ = 0 - minZ;

    float sizeX = maxX - minX + 1;
    float sizeZ = maxZ - minZ + 1;

    FieldSize = new Tuple<int, int>((int)sizeX, (int)sizeZ);

    CellsGrid = new Cell[(int)sizeX][];
    for (int i = 0; i < (int)sizeX; i++) CellsGrid[i] = new Cell[(int)sizeZ];

    foreach (Cell cell in Cells) {
      float x = cell.GlobalPosition[0] / CellSize;
      float z = cell.GlobalPosition[2] / CellSize;
      float newX = x + offsetX;
      float newZ = z + offsetZ;
      CellsGrid[(int)newX][(int)newZ] = cell;
      cell.GridCords = new Vector2I((int)newX, (int)newZ);
    }
  }

  public void UpdateCellGridCords(Cell cell, Vector2I newCords) {
    CellsGrid[newCords[0]][newCords[1]] = cell;
    cell.GridCords = newCords;
  }

  public void ManageOneHighlightedCell(bool value, Cell cell, bool deleteOlds = true) {
    cell.IsHighlighted = value;
    if (value){
      HighlightedCells.Add(cell);
    }
    else{
      if (HighlightedCells.Contains(cell)) HighlightedCells.Remove(cell);
    }
      if (deleteOlds) ClearHighlighedCells();
  }

  private void ManageHighlightCells(bool value, List<Cell> newCells, bool deleteOlds = true) {
    //Delete newCells from HighlightedCells
    if (!deleteOlds && !value) {
      if (newCells.Count == 0) return;
      HighlightedCells.RemoveAll(cell => {
        if (!newCells.Contains(cell)) return false;
        cell.IsHighlighted = false;
        return true;
      });
      return;
    }

    //if delete olds, we delete it anyway, but if value - we fill with new cells
    if (deleteOlds) ClearHighlighedCells();

    if (!value) return;
    foreach (Cell cell in newCells) cell.IsHighlighted = true;
    HighlightedCells.AddRange(newCells);
  }

  public void ClearHighlighedCells() {
    foreach (Cell cell in HighlightedCells) cell.IsHighlighted = false;
    HighlightedCells.Clear();
  }

  public List<Cell> SwitchHighlightNeighbors(Cell cell, bool value, Predicate<Cell> predicate = null, bool deleteOlds = true) {
    int x = cell.GridCords[0];
    int y = cell.GridCords[1];
    List<Cell> affectedCells = new();
    for (int dx = -1; dx <= 1; dx++)
      for (int dy = -1; dy <= 1; dy++) {
        int neighborX = x + dx;
        int neighborY = y + dy;
        if (!IsIn2DArrayBounds(neighborX, neighborY, CellsGrid)) continue;
        Cell targetCell = CellsGrid[neighborX][neighborY];
        if (targetCell == cell) continue;
        if (predicate == null || predicate(targetCell)) affectedCells.Add(targetCell);
      }

    ManageHighlightCells(value, affectedCells, deleteOlds);
    return affectedCells;
  }

  public List<Cell> SwitchHighlightByCords(bool value, List<Vector2I> cords, Predicate<Cell> predicate = null, bool deleteOlds = true) {
    List<Cell> affectedCells = cords.Where(cord => {
      if (!IsIn2DArrayBounds(cord[0], cord[1], CellsGrid)) return false;
      return predicate == null || predicate(CellsGrid[cord[0]][cord[1]]);
    }).Select(cord => CellsGrid[cord[0]][cord[1]]).ToList();
    ManageHighlightCells(value, affectedCells, deleteOlds);
    return affectedCells;
  }
}
