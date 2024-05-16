using System;
using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;
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

  private void MultipleDirectionLogic(Pawn pawn) {
    //todo Сделать проверку могут ли клетки принимать пешку, и если нет, убивать ее. КОНТРАРГУМЕНТ - КЛЕТКА НЕ БУДЕТ ОБНАРУЖЕНА ПРИВЫЧНЫМ СПОСОБОМ, НО ЕСЛИ ЧТО МЕТОД Я УЖЕ НАПИСАЛ.
    // List<Cell> affected = Cell.Field.SwitchHighlightByCords(true, _possibleDirections.Select(direction => Cell.GridCords + direction).ToList());
    // Cell.Field.Game.Camera.CurrentMode.ForcedByCellLogic = this;
    // GD.Print(affected);

    Cell.Field.Game.Camera.CurrentMode.ForcedByCellLogic = this;
    Cell.Field.SwitchHighlightNeighbors(Cell, true);
  }

  public override bool HighlightPawnMoves(Pawn pawn) {
    if (_possibleDirections.Count == 1) OneDirectionLogic(pawn);
    else MultipleDirectionLogic(pawn);
    //true because if not affected - we kill pawn, so its always accepted.
    return true;
  }

  public override bool OnHighlightCellClick(Cell highlightedCell) {
    Pawn currentPawn = Cell.GetPawns()[0];
    if (highlightedCell.Type is CellType.Arrow) return currentPawn.MoveToCell(highlightedCell);
    if (!currentPawn.MoveToCell(highlightedCell, true))
      //error handle (if arrow try to move pawn to unavailable cell (non arrow cell))
      currentPawn.Die();
    Cell.Field.Game.Camera.CurrentMode.ForcedByCellLogic = null;
    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.SwitchHighlightNeighbors(Cell, true);
    // HighlightPawnMoves(pawn);
  }
}