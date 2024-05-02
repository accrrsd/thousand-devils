using System;
using ThousandDevils.features.Game.components.player.code;

namespace ThousandDevils.features.Game.code.modules;

public class TurnModule
{
  private readonly Game _game;
  private int _activePlayerIndex;
  private int _currentTurn = 1;

  public TurnModule(Game game) {
    _game = game;
    OnTurnChange += UpdateActivePlayerIndex;
  }

  public int CurrentTurn {
    get => _currentTurn;
    set {
      _currentTurn = value;
      OnTurnChange?.Invoke(CurrentTurn);
    }
  }

  public int ActivePlayerIndex {
    get => _activePlayerIndex;
    set {
      _activePlayerIndex = value;
      OnActivePlayerIndexChange?.Invoke(_activePlayerIndex);
    }
  }

  private void UpdateActivePlayerIndex(int currentTurn) {
    ActivePlayerIndex = ActivePlayerIndex + 1 > _game.Players.Count - 1 ? 0 : ActivePlayerIndex + 1;
  }

  public Player GetActivePlayer() => _game.Players[ActivePlayerIndex];

  public event Action<int> OnTurnChange;
  public event Action<int> OnActivePlayerIndexChange;
}
