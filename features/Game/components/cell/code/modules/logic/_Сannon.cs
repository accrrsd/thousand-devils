using System;
using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class СannonLogic : BaseLogic
{
    private Tuple<int, int> _direction;
    private readonly Random _random = new();
  public СannonLogic(Cell cell) : base(cell) {
    GenerateRandomArrowType();
    Cell.PawnWasAdded += OnPawnWasAdded;
  }
  private void GenerateRandomArrowType() {
    _direction = new Tuple<int, int>(_random.Next(0, 2), _random.Next(0, 2));
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    int fieldSizeX = Cell.Field.CellsGrid.Length;
    int fieldSizeY = Cell.Field.CellsGrid[Cell.GridCords[0]].Length;
    
    if (_direction.Item1 == 0) 
      pawn.MoveToCell(Cell.Field.CellsGrid[fieldSizeX * _direction.Item2][Cell.GridCords[1]], false);
    if (_direction.Item1 == 1) 
      pawn.MoveToCell(Cell.Field.CellsGrid[Cell.GridCords[0]][fieldSizeY * _direction.Item2], false);
    
  }
}