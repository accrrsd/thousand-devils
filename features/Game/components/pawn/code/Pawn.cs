using Godot;
using GameSpace.CellSpace;
using GameSpace.Constants;
namespace GameSpace.PawnSpace;

public interface IPawn
{
  public bool CanMove { get; set; }
  public PawnItems CarryItem { get; set; }
  public PawnType Type { get; set; }

  public Cell PrevCell { get; }
  public Cell CurrentCell { get; }
}

public partial class Pawn : Node3D, IPawn
{
  public bool CanMove { get; set; } = true;
  public PawnItems CarryItem { get; set; } = PawnItems.None;
  public PawnType Type { get; set; } = PawnType.Basic;

  // needed for calculating movement vectors
  public Cell PrevCell { get; private set; } = null;
  public Cell CurrentCell { get; private set; }
}