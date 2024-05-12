using Godot;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.player.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.pawn.code;

public partial class Pawn : Node3D
{
  public bool CanMove { get; set; } = true;
  public PawnItems CarryItem { get; set; } = PawnItems.None;
  public PawnType Type { get; set; } = PawnType.Basic;

  // needed for calculating movement vectors
  public Cell PrevCell { get; private set; }
  public Cell CurrentCell { get; private set; }
  public Player OwnerPlayer { get; set; }

  public override void _Ready() {
    CurrentCell = GetParent<Cell>();
  }

  public void MoveToCell(Cell targetCell, bool increaseTurn = true) {
    if (!CanMove || !targetCell.CanAcceptPawns) return;

    CurrentCell.RemovePawn(this);
    PrevCell = CurrentCell;

    CurrentCell = targetCell;
    targetCell.AddPawn(this);

    if (increaseTurn) CurrentCell.Field.Game.TurnModule.CurrentTurn++;
  }

  public void Die() {
    CurrentCell.RemovePawn(this);
    QueueFree();
  }
}
