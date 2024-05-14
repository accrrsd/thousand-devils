using System;
using System.Collections.Generic;
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
    int numberOfDirections = _random.Next(1, 9); // 1-8 number
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
    if (!(targetCell.CanAcceptPawns && IsIn2DArrayBounds(newPos, Cell.Field.CellsGrid))) {
      pawn.Die();
      return;
    }

    pawn.MoveToCell(targetCell);
  }

  private void MultipleDirectionLogic(Pawn pawn) {
    Cell.Field.SwitchHighlightByCords(true, _possibleDirections);

    Cell targetCell = null;
    if (targetCell.Logic is ArrowLogic) pawn.MoveToCell(targetCell, false);
    pawn.MoveToCell(targetCell);

    //todo Сейчас проблема в том, что MoveToCell() выполняется в камере и если нужна какая-то дополнительная логика, то из камеры нужно вернуть это каким-то образом сюда (каллбек может быть)

    // todo Отследить куда нажал игрок и получить клетку, чтобы потом ее передать в MoveToCell()
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    if (_possibleDirections.Count == 1) OneDirectionLogic(pawn);
    else MultipleDirectionLogic(pawn);
  }
}
