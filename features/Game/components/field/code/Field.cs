using Godot;
using System;
using System.Collections.Generic;
using GameSpace.CellSpace;
using static UtilsSpace.GdUtilsFunctions;
using static UtilsSpace.UtilsFunctions;
using static GameSpace.Constants.Variables;

namespace GameSpace.FieldSpace;

public partial class Field : Node3D
{
	public List<Cell> Cells { get; private set; } = new List<Cell>();

	public Cell[][] CellsGrid { get; set; }

	private void ArrangeCellsBasedOnMapPosition()
	{
		Cells.Sort((cell1, cell2) =>
		{
			int compareX = cell1.GlobalPosition[0].CompareTo(cell2.GlobalPosition[0]);
			if (compareX != 0)
			{
				return compareX;
			}
			return cell1.GlobalPosition[2].CompareTo(cell2.GlobalPosition[2]);
		});
	}

	private void ConvertCellListIntoGrid()
	{
		float minX = Cells[0].GlobalPosition[0], maxX = minX;
		float minZ = Cells[0].GlobalPosition[2], maxZ = minZ;

		foreach (Cell cell in Cells)
		{
			minX = Math.Min(minX, cell.GlobalPosition[0]);
			maxX = Math.Max(maxX, cell.GlobalPosition[0]);
			minZ = Math.Min(minZ, cell.GlobalPosition[2]);
			maxZ = Math.Max(maxZ, cell.GlobalPosition[2]);
		}

		int width = (int)(maxX - minX);
		int height = (int)(maxZ - minZ);
		// idk how + 1 works, maybe it will broke on small maps.
		width = width / CELL_SIZE + 1;
		height = height / CELL_SIZE + 1;

		CellsGrid = ListTo2DArray(Cells, width, height, (Cell cell, int index, int i, int j) => cell.UpdateGridCords(new Vector2I(i, j)));
	}

	public override void _Ready()
	{
		Cells = FindChildsByType<Cell>(this);
		ArrangeCellsBasedOnMapPosition();
		ConvertCellListIntoGrid();
		foreach (Cell cell in Cells) cell.SetField(this);
	}

	public void UpdateCellGridCords(Cell cell, Vector2I newCords)
	{
		CellsGrid[newCords[0]][newCords[1]] = cell;
		cell.UpdateGridCords(newCords);
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
		List<Cell> neighbors = new List<Cell>();
		for (int dx = -1; dx <= 1; dx++)
		{
			for (int dy = -1; dy <= 1; dy++)
			{
				int neighborX = x + dx;
				int neighborY = y + dy;

				if (IsInGridBounds(neighborX, neighborY))
				{
					Cell currentCell = CellsGrid[neighborX][neighborY];
					if (predicate == null || predicate(currentCell))
					{
						currentCell.IsHighlighted = value;
						neighbors.Add(currentCell);
					}
				}
			}
		}
		return neighbors;
	}

	private bool IsInGridBounds(int x, int y)
	{
		return x >= 0 && x < CellsGrid.Length && y >= 0 && y < CellsGrid[0].Length;
	}
}