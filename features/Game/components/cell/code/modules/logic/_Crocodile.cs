using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class CrocodileLogic : BaseLogic
{
  public CrocodileLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
    //example scene replacement from logic
    Cell.ReplaceCellMapScene(CellMapName.Visible, GD.Load<PackedScene>("res://features/Game/components/cell/scenes/visible_maps/Crocodile_map.tscn"));
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    pawn.MoveToCell(pawn.PrevCell);
    Cell.CanAcceptPawns = false;
  }
}