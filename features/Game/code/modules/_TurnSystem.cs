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

  //todo нужно сделать передачу хода по часовой стрелке.
  private void UpdateActivePlayerIndex(int currentTurn) {
    ActivePlayerIndex = ActivePlayerIndex + 1 > _game.Players.Count - 1 ? 0 : ActivePlayerIndex + 1;
  }

  public Player GetActivePlayer() => _game.Players[ActivePlayerIndex];

  //props current turn 
  public event Action<int> OnTurnChange;

  //props current player index
  public event Action<int> OnActivePlayerIndexChange;
}
