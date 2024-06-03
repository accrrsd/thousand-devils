using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class PlaneLogic : BaseLogic
{
  public PlaneLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
    Cell.PawnWasAdded += OnPawnWasAdded;
  }

  private void HighlightOpenedCells(){
    List<Cell> highlightedCells = Cell.Field.SwitchHighlightWholeField(true, pCell => pCell != Cell && pCell.Type != CellType.Ocean && pCell.Type!=CellType.Ship);
    Cell.ChangeHighlightedBorderColor(SelectedColor);
    Cell.UpdateHighlight(true);
  }

// todo Пешка заходит на самолет, на этом ходу и следующем на следующем ходу может выбрать клетки, на которые отправится, все по всему полю
//todo После того как самолетом воспользуются, он недоступен до конца игры и становится обычной клеткой.
//todo Пешку может выпнуть враждебная пешка, в таком случае она по своему обыкновению отправляется на корабль.

  // private void HighlightOpenedCells() { 
  //   List<Vector2I> highlightedCellsCords = new();

  //   for (int x = 0; x < Cell.Field.FieldSize.Item1; x++) {
  //     for (int z = 0; z < Cell.Field.FieldSize.Item2; z++) {
  //       Cell currentCell = Cell.Field.GetCellFromCellsGrid(x, z);
  //       bool opened = currentCell.IsOpen;
  //       if (!opened) continue;
  //       if (currentCell.Type == CellType.Ocean) continue;
  //       highlightedCellsCords.Add(currentCell.GridCords);
  //     }
  //   }

  //   Cell.Field.SwitchHighlightByCords(true, highlightedCellsCords);
  // }

  // public override bool OnHighlightedCellClick(Cell highlightedCell) {
  //   pawn.MoveToCell(highlightedCell);
  //   return true;
  // }

  public override bool OnHighlightedCellClick(Cell highlightedCell){
    Pawn pawn = Cell.GetPawns()[0];
    if (!highlightedCell.IsOpen && pawn.CarryItem!=PawnItems.None){
      //todo Тут пешка дропает свой предмет (оставляет на текущей клетке)
      pawn.MoveToCell(highlightedCell);
    }
    else{
      Cell.GetPawns()[0].MoveToCell(highlightedCell);
    }
    Cell.PawnWasAdded -= OnPawnWasAdded;
    return true;
  }

  private void OnPawnWasAdded(Cell _, Pawn pawn) {

    Cell.Field.Game.Camera.CurrentMode.RedirectClickToCellLogic = this;
    Cell.Field.ClearHighlightedCells();
    HighlightOpenedCells();
    // Cell.PawnWasAdded -= OnPawnWasAdded;
  }
}