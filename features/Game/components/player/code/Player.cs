using System.Collections.Generic;
using GameSpace.PawnSpace;
using Godot;

namespace GameSpace.PlayerSpace;

public interface IPlayer
{
  Color PawnColor { get; set; }
  int MoneyCount { get; set; }
  int RomeCount { get; set; }
  int MaxPawns { get; set; }
  List<Pawn> ControlledPawns { get; set; }
}

public partial class Player : Node3D, IPlayer
{
  public Color PawnColor { get; set; }
  public int MoneyCount { get; set; }
  public int RomeCount { get; set; }
  public int MaxPawns { get; set; }
  public List<Pawn> ControlledPawns { get; set; }
}
