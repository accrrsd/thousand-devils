using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.components.player.code;

//todo НЕ РАБОТАЕТ БЛЯТЬ
namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class DreamFloraLogic : BaseLogic
{
  private readonly Dictionary<Player, List<Pawn>> _originalOwnersAndPawns = new();

  public DreamFloraLogic(Cell cell) : base(cell) {
    // todo Нужно сделать всплывающее окно, что нужно играть за других игроков (Отключение в настройках)
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
    Cell.Field.Game.TurnModule.OnCircleChange += CancelEffect;
  }

  public void OnPawnWasAdded(Cell _, Pawn pawn) {
    List<Player> players = Cell.Field.Game.Players;
    List<Player> changedPlayers = players;
    changedPlayers.Add(players[0]);
    changedPlayers.RemoveAt(0);
    GD.Print(changedPlayers.Count);
    for (int i = 0; i < changedPlayers.Count; i++) {
      Player originalPlayer = players[i];
      _originalOwnersAndPawns.Add(originalPlayer, originalPlayer.ControlledPawns);
      Player changedPlayer = changedPlayers[i];
      changedPlayer.ControlledPawns = originalPlayer.ControlledPawns;
      changedPlayer.ControlledPawns.ForEach(p => p.OwnerPlayer = changedPlayer);
    }

    Cell.Field.Game.Players = changedPlayers;
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }

  public void CancelEffect(int currentTurn) {
    foreach (KeyValuePair<Player, List<Pawn>> kvp in _originalOwnersAndPawns) {
      kvp.Value.ForEach(p => p.OwnerPlayer = kvp.Key);
      kvp.Key.ControlledPawns = kvp.Value;
    }

    Cell.Field.Game.Players = _originalOwnersAndPawns.Keys.ToList();
    Cell.Field.Game.TurnModule.OnCircleChange -= CancelEffect;
  }
}

// 1 - 2 - 3 - 4 - 5 - 6
// 2 - 3 - 4 - 5 - 6 - 1