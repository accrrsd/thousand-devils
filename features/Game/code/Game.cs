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
  private bool _basicReady;
  public TurnModule TurnModule { get; private set; }
  public List<Player> Players { get; set; } = new();
  public Field Field { get; private set; }

  public Camera Camera { get; private set; }

  public bool BasicReady {
    get => _basicReady;
    private set {
      _basicReady = value;
      if (_basicReady) BasicWasReady?.Invoke();
    }
  }

  private event Action BasicWasReady;

  [AssociateAttributes.ChildSetter]
  public void UpdateField(Field field) => Field = field;

  [AssociateAttributes.ChildSetter]
  public void UpdateCamera(Camera camera) => Camera = camera;

  public override void _Ready() {
    base._Ready();
    AssociateParentAndChild(this, GdUtilsFunctions.GetFirstChildByType<Field>(this));
    AssociateParentAndChild(this, GdUtilsFunctions.GetFirstChildByType<Camera>(this));
    TurnModule = new TurnModule(this);
    BasicReady = true;

    FillPlayers();
    UpdatePlayersTurn(TurnModule.ActivePlayerIndex);
    TurnModule.OnActivePlayerIndexChange += UpdatePlayersTurn;
  }

  //Calls callback when game gets based variables setup

  public void AskForBasicReady(Action callback) {
    if (BasicReady) {
      callback();
    }
    else {
      void Wrapper() {
        callback();
        BasicWasReady -= Wrapper;
      }

      BasicWasReady += Wrapper;
    }
  }

  private List<Player> SortPlayersClockwise(List<Player> players) {
    int minPlayerY = players.Min(player => player.Ship.GridCords[1]);

    List<Player> minPlayers = players.Where(player => player.Ship.GridCords[1] == minPlayerY).ToList();

    Player originPlayer = minPlayers.MinBy(player => player.Ship.GridCords[0]);

    List<Tuple<Player, double, double>> playerPolarAngleDistance = new();

    foreach (Player player in players) {
      int dx = player.Ship.GridCords[0] - originPlayer.Ship.GridCords[0];
      int dy = player.Ship.GridCords[1] - originPlayer.Ship.GridCords[1];
      playerPolarAngleDistance.Add(new Tuple<Player, double, double>(player, Math.Atan2(dy, dx), Math.Sqrt(dx * dx + dy * dy)));
    }

    return playerPolarAngleDistance.OrderBy(player => player.Item2).ThenBy(player => player.Item3).Select(player => player.Item1).ToList();
  }

  private void UpdatePlayersTurn(int activePlayerIndex) {
    for (int i = 0; i < Players.Count; i++) Players[i].IsTurn = i == activePlayerIndex;
  }

  private void FillPlayers() {
    if (Field == null || Field.Cells.Count == 0) throw new MyExceptions.EmptyExportException("Field is null or not have cells");
    // late todo Тут мы должны заполнять игроков по необходимому количеству (которое будет передваться из начала игры), а не по количеству возможных спавнов (оно лишь ограничивает возможное количество игроков).
    foreach (Cell cell in Field.Cells.Where(cell => cell.Type == CellType.PossibleShip)) {
      if (cell.Logic is not PossibleShipLogic currentCellLogic) continue;
      Cell shipCell = DefaultShipNodeScene.Instantiate<Cell>();
      currentCellLogic.ReplaceByShip(shipCell);
      Players.Add(new Player(shipCell, this));
    }
  }
}