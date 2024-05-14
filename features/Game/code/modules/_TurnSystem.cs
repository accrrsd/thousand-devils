using System;
using ThousandDevils.features.Game.components.player.code;

namespace ThousandDevils.features.Game.code.modules;

public class TurnModule
{
  private readonly Game _game;
  private int _activePlayerIndex;
  private int _currentTurn = 1;
  private int _currentCircle = 1;

  public TurnModule(Game game) {
    _game = game;
    OnTurnChange += UpdateActivePlayerIndex;
    OnTurnChange += CalculateCurrentCircle;
  }

  public int CurrentTurn {
    get => _currentTurn;
    set {
      _currentTurn = value;
      OnTurnChange?.Invoke(value);
    }
  }
  
  private void CalculateCurrentCircle(int currentTurn) {
    int calculatedCircle = currentTurn / _game.Players.Count;
    if (calculatedCircle != CurrentCircle) CurrentCircle = calculatedCircle;
  }
  
  public int CurrentCircle {
    get => _currentCircle;
    set {
      _currentCircle = value;
      OnCircleChange?.Invoke(value);
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

  //props current turn
  public event Action<int> OnTurnChange;
  
  //props current circle
  public event Action<int> OnCircleChange;

  //props current player index
  public event Action<int> OnActivePlayerIndexChange;
}
