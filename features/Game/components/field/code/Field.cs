using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.cell.code;
using static ThousandDevils.features.Game.utils.Variables;
using static ThousandDevils.features.GlobalUtils.GdUtilsFunctions;
using static ThousandDevils.features.GlobalUtils.UtilsFunctions;

namespace ThousandDevils.features.Game.components.field.code;

public partial class Field : Node3D
{
  [Export] public Game.code.Game Game { get; private set; }
  public List<Cell> Cells { get; set; } = new();
  public Cell[][] CellsGrid { get; set; }

  public override void _Ready()
  {
    Cells = FindChildsByType<Cell>(this);
    ArrangeCellsBasedOnMapPosition();
    ConvertCellListIntoGrid();
    foreach (Cell cell in Cells) cell.SetField(this);
  }

  //late todo Для корректной работы требуется чтобы "поле" было прижато одним своим краем к globalPosition = new VectorZero(). В будущем нужно будет это учитывать.
  private void ArrangeCellsBasedOnMapPosition()
  {
    Cells.Sort((cell1, cell2) =>
    {
      int compareX = cell1.GlobalPosition[0].CompareTo(cell2.GlobalPosition[0]);
      return compareX != 0 ? compareX : cell1.GlobalPosition[2].CompareTo(cell2.GlobalPosition[2]);
    });
  }

  private void ConvertCellListIntoGrid()
  {
    float minX = Cells[0].GlobalPosition[0], maxX = minX;
    float minZ = Cells[0].GlobalPosition[2], maxZ = minZ;

    foreach (Vector3 globalPos in Cells.Select(cell => cell.GlobalPosition))
    {
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

  public void UpdateCellGridCords(Cell cell, Vector2I newCords)
  {
    CellsGrid[newCords[0]][newCords[1]] = cell;
    cell.GridCords = newCords;
  }

  // dev only all below

  // Возвращает подсвеченных соседей
  public void HighlightSelected(Cell cell, bool value)
  {
    int x = cell.GridCords[0];
    int y = cell.GridCords[1];
    CellsGrid[x][y].IsHighlighted = value;
  }

  public List<Cell> HighlightNeighbors(Cell cell, bool value, Func<Cell, bool> predicate = null)
  {
    int x = cell.GridCords[0];
    int y = cell.GridCords[1];
    List<Cell> neighbors = new();
    for (int dx = -1; dx <= 1; dx++)
      for (int dy = -1; dy <= 1; dy++)
      {
        int neighborX = x + dx;
        int neighborY = y + dy;

        if (!IsIn2DArrayBounds(neighborX, neighborY, CellsGrid)) continue;
        Cell currentCell = CellsGrid[neighborX][neighborY];
        if (currentCell == cell) continue;
        if (predicate != null && !predicate(currentCell)) continue;
        currentCell.IsHighlighted = value;
        neighbors.Add(currentCell);
      }

    return neighbors;
  }
}
