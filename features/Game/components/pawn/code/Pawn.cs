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

  private StandardMaterial3D BaseMaterial { get; set; }

  public override void _Ready() {
    CurrentCell = GetParent<Cell>();
  }

  public bool MoveToCell(Cell targetCell, bool increaseTurn = false) {
    if (!CanMove || !targetCell.CanAcceptPawns || !targetCell.Logic.CanAcceptThatPawn(this)) return false;

    CurrentCell.RemovePawn(this);
    PrevCell = CurrentCell;

    CurrentCell = targetCell;
    targetCell.AddPawn(this);

    if (increaseTurn) CurrentCell.Field.Game.TurnModule.CurrentTurn++;
    return true;
  }

  public void UpdatePawnColor(Color newColor) {
    MeshInstance3D modelInstance = GetNode<MeshInstance3D>("Model");
    BaseMaterial ??= modelInstance.GetSurfaceOverrideMaterial(0) as StandardMaterial3D ?? new StandardMaterial3D();
    BaseMaterial.AlbedoColor = newColor;
    modelInstance.SetSurfaceOverrideMaterial(0, BaseMaterial);
  }

  public void Die() {
    CurrentCell.RemovePawn(this);
    OwnerPlayer.ControlledPawns.Remove(this);
    QueueFree();
  }
}