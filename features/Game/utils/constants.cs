namespace ThousandDevils.features.Game.utils;

public enum CellType
{
  Random,
  Arrow,
  Trap,
  Balloon,
  Ogre,
  Crocodile,
  Ice,
  Rum,
  RumTrap,
  Ocean,
  Ship,
  PossibleShip,
  Jungles,
  Fortress,
  Horse,
  Cannon,
  LightHouse,
  DreamFlora,
  Earthquake
}

public enum CameraModeType
{
  Free,
  VerticalPinned
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

public enum CellMapName
{
  Visible,
  Invisible
}
