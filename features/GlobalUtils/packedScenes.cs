using Godot;
using static ThousandDevils.features.GlobalUtils.Paths;

namespace ThousandDevils.features.GlobalUtils;

public static class LoadedPackedScenes
{
  public static readonly PackedScene DefaultPawnNodeScene = GD.Load<PackedScene>(DefaultPawnNodePath);
  public static readonly PackedScene DefaultShipNodeScene = GD.Load<PackedScene>(DefaultShipNodePath);
}