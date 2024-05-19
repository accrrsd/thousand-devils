using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using static ThousandDevils.features.GlobalUtils.UtilsFunctions;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class ArrowLogic : BaseLogic
{
  private readonly List<Vector2I> _possibleDirections = new();
  private readonly Random _random = new();
  private bool _isPresent;

  public ArrowLogic(Cell cell) : base(cell) {
    GenerateRandomArrowType();
    Cell.PawnWasAdded += OnPawnWasAdded;
    Cell.Field.Game.TurnModule.OnTurnChange += ResetIsPresent;
  }

  private void ResetIsPresent(int currentTurn) => _isPresent = false;

  private void GenerateRandomArrowType() {
    int numberOfDirections = _random.Next(1, 9); //1-8 number
    for (int i = 0; i < numberOfDirections; i++) {
      Vector2I randomVector = new(_random.Next(-1, 2), _random.Next(-1, 2));
      _possibleDirections.Add(randomVector);
    }
  }

  //todo Подсветил сам себя после телепортации

  private void OneDirectionLogic(Pawn pawn) {
    if (_isPresent) {
      pawn.Die();
      return;
    }

    _isPresent = true;
    Vector2I newPos = Cell.GridCords + _possibleDirections[0];
    Cell targetCell = Cell.Field.GetCellFromCellsGrid(newPos);
    if (IsIn2DArrayBounds(newPos, Cell.Field.CellsGrid) && pawn.MoveToCell(targetCell)) return;
    pawn.Die();
  }

  private void HighlightCellsWithDirection(Pawn pawn) {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    //predicate for highlight checks for closet cells or if they can accept pawn
    List<Cell> highlightedCells = Cell.Field.SwitchHighlightByCords(true, _possibleDirections.Select(direction => Cell.GridCords + direction).ToList(),
      pCell => !pCell.IsOpen || (pCell.CanAcceptPawns && pCell.Logic.CanAcceptThatPawn(pawn)));
    if (highlightedCells.Count != 0) return;
    pawn.Die();
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
  }

  //multiple directions logic here
  public override bool OnHighlightedCellClick(Cell highlightedCell) {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
    Cell.Field.SwitchHighlightNeighbors(Cell, false);
    if (Cell.GetPawns().Count == 0) return false;
    Pawn currentPawn = Cell.GetPawns()[0];
    highlightedCell.Logic.OnThisCellClickAsHighlighted(Cell);
    //always false for turn increase because we already increase it when get here.
    if (!currentPawn.MoveToCell(highlightedCell))
      currentPawn.Die();
    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.ClearHighlighedCells();
    if (_possibleDirections.Count == 1) OneDirectionLogic(pawn);
    else HighlightCellsWithDirection(pawn);
  }
}