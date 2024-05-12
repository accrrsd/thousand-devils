namespace ThousandDevils.features.Game.utils;

public enum CellType
{
  Random,
  Empty,
  Arrow,
  Trap,
  Ice,
  Ocean,
  Ship,
  PossibleShip
}

public enum CameraModeType
{
  Free,
  Fixed
}

public enum PawnItems
{
  None,
  Money,
  Chest
}

public enum PawnType
{
  Basic,
  Friday,
  Missioner
}

public static class Variables
{
  public const int CellSize = 10;
}
