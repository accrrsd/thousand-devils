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

		foreach (var cell in Cells)
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

		CellsGrid = ListTo2DArray(Cells, width, height);
		for (int i = 0; i < CellsGrid.Length; i++) for (int j = 0; j < CellsGrid[i].Length; j++) CellsGrid[i][j].UpdateGridCords(new Vector2(i, j));
	}

	public override void _Ready()
	{
		Cells = FindChildsByType<Cell>(this);
		ArrangeCellsBasedOnMapPosition();
		ConvertCellListIntoGrid();
	}
}