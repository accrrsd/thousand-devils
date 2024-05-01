using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.code.modules;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.cell.code.modules.logic;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.player.code;
using ThousandDevils.features.Game.utils;
using static ThousandDevils.features.GlobalUtils.LoadedPackedScenes;

namespace ThousandDevils.features.Game.code;

public partial class Game : Node3D
{
  public TurnSystem TurnSystem { get; private set; }
  public List<Player> Players { get; set; } = new();
  [Export] public Field Field { get; private set; }

  public override void _Ready()
  {
    base._Ready();
    FillPlayers();
    TurnSystem = new TurnSystem(this);
    UpdatePlayersTurn(TurnSystem.ActivePlayerIndex);
    TurnSystem.OnActivePlayerIndexChange += UpdatePlayersTurn;
  }

  private void UpdatePlayersTurn(int activePlayerIndex)
  {
    for (int i = 0; i < Players.Count; i++) Players[i].IsTurn = i == activePlayerIndex;
    GD.Print("Player 1: ", Players[0].IsTurn);
    GD.Print("Player 2: ", Players[1].IsTurn);
  }

  private void FillPlayers()
  {
    if (Field == null || Field.Cells.Count == 0) throw new ArgumentException("field is null or not have cells to spawn ships in");
    // late todo Тут мы должны заполнять игроков по необходимому количеству, а не по количеству возможных спавнов.
    foreach (Cell cell in Field.Cells.Where(cell => cell.Type == CellType.PossibleShip))
    {
      if (cell.Logic is not PossibleShipLogic currentCellLogic) continue;
      Cell shipCell = DefaultShipNodeScene.Instantiate<Cell>();
      currentCellLogic.ReplaceByShip(shipCell);
      Players.Add(new Player(shipCell, this));
    }
  }
}
