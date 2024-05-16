using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using static ThousandDevils.features.GlobalUtils.UtilsFunctions;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class HorseLogic : BaseLogic
{
  private readonly List<Vector2I> _possibleDirections = new()
  {
    new  Vector2I(-1, -2),
    new  Vector2I(1, -2),
    new  Vector2I(-2, -1),
    new  Vector2I(2, -1),
    new  Vector2I(-2, 1),
    new  Vector2I(2, 1),
    new  Vector2I(-1, 2),
    new  Vector2I(1, 2)
  };

  public HorseLogic(Cell cell) : base(cell) {
    
  }
}