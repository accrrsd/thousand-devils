using System;
using Godot;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;
using static ThousandDevils.features.GlobalUtils.LoadedPackedScenes;

namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

public class ShipLogic : BaseLogic
{
  public ShipLogic(Cell cell) : base(cell) {
    Cell.IsOpen = true;
    // late todo: 3 by default - but we need to think about it as parameter in editor but only if cell type is ship
    Cell.AddPawn(DefaultPawnNodeScene.Instantiate<Pawn>(), false);
    Cell.AddPawn(DefaultPawnNodeScene.Instantiate<Pawn>(), false);
    Cell.AddPawn(DefaultPawnNodeScene.Instantiate<Pawn>(), false);
  }

  public void SwitchPlacesWithCell(Cell otherCell) {
    Field field = Cell.Field;
    Vector2I currentCords = Cell.GridCords;
    Vector2I otherCellCords = otherCell.GridCords;
    Vector3 currentGlobalPos = Cell.GlobalPosition;
    Vector3 otherCellGlobalPos = otherCell.GlobalPosition;

    field.UpdateCellGridCords(Cell, otherCellCords);
    field.UpdateCellGridCords(otherCell, currentCords);

    Cell.GlobalPosition = otherCellGlobalPos;
    otherCell.GlobalPosition = currentGlobalPos;

    field.Game.TurnModule.CurrentTurn++;
  }

  public override bool HighlightPawnMoves(Pawn pawn) {
    return Cell.Field.SwitchHighlightNeighbors(Cell, true, targetCell => {
      if (targetCell.Type == CellType.Ocean) return true;
      if (!targetCell.CanAcceptPawns || !targetCell.Logic.CanAcceptThatPawn(pawn)) return false;
      int dx = Cell.GridCords[0] - targetCell.GridCords[0];
      int dy = Cell.GridCords[1] - targetCell.GridCords[1];
      return Math.Abs(dx) != Math.Abs(dy);
    }).Count > 0;
  }

  public override bool OnHighlightCellClick(Cell highlightedCell) {
    if (highlightedCell.Type != CellType.Ocean) return base.OnHighlightCellClick(highlightedCell);
    SwitchPlacesWithCell(highlightedCell);
    return true;
  }
}