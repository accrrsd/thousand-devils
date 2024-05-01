namespace ThousandDevils.features.Game.components.cell.code.modules.logic;

// todo Если мы не будем делать общих переменных и методов, то можно заменить на интерфейс.
public abstract class BaseLogic
{
  protected BaseLogic(Cell cell)
  {
    Cell = cell;
  }

  protected Cell Cell { get; private set; }
}
