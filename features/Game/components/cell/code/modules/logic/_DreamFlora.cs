using System.Collections.Generic;
using System.Linq;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.components.player.code;

//todo НЕ РАБОТАЕТ БЛЯТЬ
namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class DreamFloraLogic : BaseLogic
{
  private readonly Dictionary<Player, List<Pawn>> OriginalOwnersAndPawns = new();

  public DreamFloraLogic(Cell cell) : base(cell) {
    // todo Нужно сделать всплывающее окно, что нужно играть за других игроков (Отключение в настройках)
    // Карты, деньги, два ствола
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
    Cell.Field.Game.TurnModule.OnCircleChange += CancelEffect;
  }

  public void OnPawnWasAdded(Cell _, Pawn pawn) {
    List<Player> Players = Cell.Field.Game.Players;
    List<Player> ChangedPlayers = Players;
    ChangedPlayers.Add(Players[0]);
    ChangedPlayers.RemoveAt(0);
    for (int i = 0; i < ChangedPlayers.Count; i++) {
      Player originalPlayer = Players[i];
      OriginalOwnersAndPawns.Add(originalPlayer, originalPlayer.ControlledPawns);
      Player changedPlayer = ChangedPlayers[i];
      originalPlayer.ControlledPawns.ForEach(pawn => pawn.OwnerPlayer = changedPlayer);
    }

    Cell.Field.Game.Players = ChangedPlayers;
    Cell.PawnWasAdded -= OnPawnWasAdded;
  }

  public void CancelEffect(int currentTurn) {
    foreach (KeyValuePair<Player, List<Pawn>> kvp in OriginalOwnersAndPawns) kvp.Key.ControlledPawns = kvp.Value;
    Cell.Field.Game.Players = OriginalOwnersAndPawns.Keys.Select(key => key).ToList();
    Cell.Field.Game.TurnModule.OnCircleChange -= CancelEffect;
  }
}

// 1 - 2 - 3 - 4 - 5 - 6
// 2 - 3 - 4 - 5 - 6 - 1