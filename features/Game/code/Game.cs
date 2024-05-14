using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.code.modules;
using ThousandDevils.features.Game.components.camera.code;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.cell.code.modules.logic;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.player.code;
using ThousandDevils.features.Game.utils;
using ThousandDevils.features.GlobalUtils;
using static ThousandDevils.features.GlobalUtils.LoadedPackedScenes;
using static ThousandDevils.features.GlobalUtils.UtilsFunctions;

namespace ThousandDevils.features.Game.code;

public partial class Game : Node3D
{
  public TurnModule TurnModule { get; private set; }
  public List<Player> Players { get; set; } = new();
  public Field Field { get; private set; }
  public Camera Camera { get; private set; }

  [AssociateAttributes.ChildSetter]
  private void UpdateField(Field field) => Field = field;

  [AssociateAttributes.ChildSetter]
  private void UpdateCamera(Camera camera) => Camera = camera;

  public override void _Ready() {
    base._Ready();
    AssociateParentAndChild(this, GdUtilsFunctions.GetFirstChildByType<Field>(this));
    AssociateParentAndChild(this, GdUtilsFunctions.GetFirstChildByType<Camera>(this));
    FillPlayers();
    Players = SortPlayersClockwise(Players);
    TurnModule = new TurnModule(this);
    UpdatePlayersTurn(TurnModule.ActivePlayerIndex);
    TurnModule.OnActivePlayerIndexChange += UpdatePlayersTurn;
  }
  
  // todo нужно сделать передачу хода по часовой стрелке.
  private List<Player> SortPlayersClockwise(List<Player> players) {
    int minPlayerY = 0;
    for (int i = 0; i < players.Count; i++) {
      if (players[i].Ship.GridCords[1] < minPlayerY) minPlayerY = players[i].Ship.GridCords[1];
    }
    List<Player> minPlayers = new();
    foreach (Player player in players) {
      if (player.Ship.GridCords[1] == minPlayerY) minPlayers.Add(player);
    }

    Player originPlayer = minPlayers[0];
    if (minPlayers.Count > 0) {
      foreach (Player player in players) {
        if (player.Ship.GridCords[0] > originPlayer.Ship.GridCords[0]) originPlayer = player;
      }
    }

    Tuple<Player, double, double> CalculatePolarAngleAndDistance(Player player) {
      int dx = player.Ship.GridCords[0] - originPlayer.Ship.GridCords[0];
      int dy = player.Ship.GridCords[1] - originPlayer.Ship.GridCords[1];
      return new Tuple<Player, double, double>(player, Math.Atan2(dy, dx), Math.Sqrt(dx * dx + dy * dy));
    }

    List<Tuple<Player, double, double>> PlayerPolarAngleDistance = new();
    foreach (Player player in players) {
      PlayerPolarAngleDistance.Add(CalculatePolarAngleAndDistance(player));
    }
    
    PlayerPolarAngleDistance.Sort((piecePPD1, piecePPD2) => 
       piecePPD1.Item2.CompareTo(piecePPD2.Item2) != 0 ? 
         piecePPD1.Item2.CompareTo(piecePPD2.Item2) :
         piecePPD1.Item3.CompareTo(piecePPD2.Item3)
    );
    return PlayerPolarAngleDistance.Select(piecePPD => piecePPD.Item1).ToList();
  }

  private void UpdatePlayersTurn(int activePlayerIndex) {
    for (int i = 0; i < Players.Count; i++) Players[i].IsTurn = i == activePlayerIndex;
  }

  private void FillPlayers() {
    if (Field == null || Field.Cells.Count == 0) throw new MyExceptions.EmptyExportException("Field is null or not have cells to spawn ships in");
    // late todo Тут мы должны заполнять игроков по необходимому количеству (которое будет передваться из начала игры), а не по количеству возможных спавнов (оно лишь ограничивает возможное количество игроков).
    foreach (Cell cell in Field.Cells.Where(cell => cell.Type == CellType.PossibleShip)) {
      if (cell.Logic is not PossibleShipLogic currentCellLogic) continue;
      Cell shipCell = DefaultShipNodeScene.Instantiate<Cell>();
      currentCellLogic.ReplaceByShip(shipCell);
      Players.Add(new Player(shipCell, this));
    }
  }
}
