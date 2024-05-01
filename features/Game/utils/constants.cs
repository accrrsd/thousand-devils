namespace ThousandDevils.features.Game.utils;

public enum CellType
{
  Random,
  Empty,
  Arrow,
  Ocean,
  Ship,
  PossibleShip
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
