using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class OgreLogic : BaseLogic
{
  public OgreLogic(Cell cell) : base(cell) {
    Cell.PawnWasAdded += OnPawnWasAdded;
    // Cell.ReplaceCellMapScene(CellMapName.Visible, GD.Load<PackedScene>("res://features/Game/components/cell/scenes/visible_maps/Crocodile_map.tscn"));
  }

  // private void

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    pawn.Die();
    // Cell.CanAcceptPawns = false;
  }
}