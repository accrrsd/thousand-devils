using System;
using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.cell.code;
using ThousandDevils.features.Game.components.cell.code.modules.logic;
using ThousandDevils.features.Game.components.pawn.code;

namespace ThousandDevils.features.Game.components.player.code;

public class Player
{
  public Player(Cell shipCell, Game.code.Game game) {
    if (shipCell.Logic is not ShipLogic) throw new ArgumentException("Cell must have Logic.Ship");
    Ship = shipCell;
    foreach (Pawn pawn in Ship.GetPawns()) UpdatePawnControl(pawn);
  }

  public Game.code.Game Game { get; set; }
  public Color ColorTheme { get; set; }
  public Cell Ship { get; }

  public int MoneyCount { get; set; }
  public int RumCount { get; set; }
  public bool IsTurn { get; set; }

  public int MaxPawns { get; set; }
  public List<Pawn> ControlledPawns { get; set; } = new();

  public void UpdatePawnControl(Pawn newPawn) {
    ControlledPawns.Add(newPawn);
    newPawn.OwnerPlayer = this;
  }
}
