using GameSpace.CellSpace;
using Godot;
using System.Collections.Generic;

namespace GameSpace.FieldSpace;

public partial class Field : Node3D
{
	public List<Cell> Cells { get; private set; } = new List<Cell>();

	private void ArrangeCellsBasedOnMapPosition()
	{
		Cells.Sort((cell1, cell2) =>
		{
			int compareX = cell1.Position[0].CompareTo(cell2.Position[0]);
			if (compareX != 0)
			{
				return compareX;
			}

			int compareY = cell1.Position[1].CompareTo(cell2.Position[1]);
			return compareY;
		});
	}

	public override void _Ready()
	{
		ArrangeCellsBasedOnMapPosition();
	}
}
