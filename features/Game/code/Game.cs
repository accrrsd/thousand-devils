using System.Collections.Generic;
using GameSpace.PlayerSpace;
namespace GameSpace;

public class Game
{
  // todo make player cell (ship), and make player_spawn_cell, if player exist in that list - player_spawn_cell replaced by player_cell
  public Dictionary<Player, int> Players { get; set; }
}