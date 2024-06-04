using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;
using ThousandDevils.features.GlobalUtils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class PlaneLogic : BaseLogic
{
  private Color _selectedColor;

  public PlaneLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
    _selectedColor = UtilsFunctions.GenerateColorFromRgb(247, 127, 0);
  }

  private void HighlightOpenedCells() {
    List<Cell> highlightedCells = Cell.Field.SwitchHighlightWholeField(true, pCell => pCell != Cell && pCell.Type != CellType.Ocean && pCell.Type != CellType.Ship);
    Cell.ChangeHighlightedBorderColor(_selectedColor);
    Cell.UpdateHighlight(true);
  }

  public override bool OnHighlightedCellClick(Cell highlightedCell) {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = null;
    if (highlightedCell == Cell) {
      Cell.ChangeHighlightedBorderColor();
      Cell.Field.ClearHighlightedCells();
      return false;
    }
    Pawn pawn = Cell.GetPawns()[0];
    if (!highlightedCell.IsOpen && pawn.CarryItem != PawnItems.None) {
      //todo Тут пешка дропает свой предмет (оставляет на текущей клетке)
    }
    pawn.MoveToCell(highlightedCell);
    //remove OnPawnWasAdded only after plane was used
    Cell.PawnWasAdded -= OnPawnWasAdded;
    return true;
  }

  //todo На тест

  // Пешка заходит на самолет, на этом ходу и следующем на следующем ходу может выбрать клетки, на которые отправится, все по всему полю
  // После того как самолетом воспользуются, он недоступен до конца игры и становится обычной клеткой.
  // Пешку может выпнуть враждебная пешка, в таком случае она по своему обыкновению отправляется на корабль.

  private void OnPawnWasAdded(Cell _, Pawn pawn) {
    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    Cell.Field.ClearHighlightedCells();
    HighlightOpenedCells();
  }
}
