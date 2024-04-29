using GameSpace.CellSpace;
using Godot;
using System;
using System.Collections.Generic;
using static UtilsSpace.GdUtilsFunctions;
using static UtilsSpace.UtilsFunctions;
using static GameSpace.Constants.Variables;

namespace GameSpace.FieldSpace;

public partial class Field : Node3D
{
	// todo recursive find cells and add to list
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

			int compareZ = cell1.GlobalPosition[2].CompareTo(cell2.GlobalPosition[2]);
			return compareZ;
		});
	}

	private void ConvertCellListIntoGrid()
	{
	}

	public override void _Ready()
	{
		Cells = FindChildsByType<Cell>(this);
		ArrangeCellsBasedOnMapPosition();
		ConvertCellListIntoGrid();
	}
}
