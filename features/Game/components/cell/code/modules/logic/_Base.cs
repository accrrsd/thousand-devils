using GameSpace.FieldSpace;

namespace GameSpace.CellSpace.modules.logic;

// todo Если мы не будем делать общих переменных и методов, то можно заменить на интерфейс.
public abstract class BaseLogic
{
  protected Cell Cell { get; private set; }
  protected BaseLogic(Cell cell)
  {
    Cell = cell;
  }
}