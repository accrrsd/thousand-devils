using Godot;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.player.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.pawn.code;

public partial class Pawn : Node3D
{
  private Player _ownerPlayer;
  public bool CanMove { get; set; } = true;
  public PawnItems CarryItem { get; set; } = PawnItems.None;
  public PawnType Type { get; set; } = PawnType.Basic;

  // needed for calculating movement vectors
  public Cell PrevCell { get; private set; }
  public Cell CurrentCell { get; private set; }

  public Player OwnerPlayer {
    get => _ownerPlayer;
    set {
      _ownerPlayer = value;
      UpdatePawnColor(_ownerPlayer.ColorTheme);
    }
  }

  private StandardMaterial3D BaseMaterial { get; set; }

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

  private void UpdatePawnColor(Color newColor) {
    MeshInstance3D modelInstance = GetNode<MeshInstance3D>("Model");
    BaseMaterial ??= modelInstance.GetSurfaceOverrideMaterial(0) as StandardMaterial3D ?? new StandardMaterial3D();
    BaseMaterial.AlbedoColor = newColor;
    modelInstance.SetSurfaceOverrideMaterial(0, BaseMaterial);
  }

  public Cell HighlightMove() {
    if (!CanMove) return null;
    if (CurrentCell.Type == CellType.Ship)
      CurrentCell.Field.SwitchHighlightNeighbors(CurrentCell, true, targetCell => {
        if (!targetCell.CanAcceptPawns) return false;
        int dx = CurrentCell.GridCords[0] - targetCell.GridCords[0];
        int dy = CurrentCell.GridCords[1] - targetCell.GridCords[1];
        Vector2I vectorToTargetCell = new(dx, dy);
        return vectorToTargetCell[0] != vectorToTargetCell[1];
      });
    else CurrentCell.Field.SwitchHighlightNeighbors(CurrentCell, true, pCell => pCell.Type != CellType.Ocean && pCell.CanAcceptPawns);

    return CurrentCell;
  }

  public void Die() {
    CurrentCell.RemovePawn(this);
    OwnerPlayer.ControlledPawns.Remove(this);
    QueueFree();
  }
}