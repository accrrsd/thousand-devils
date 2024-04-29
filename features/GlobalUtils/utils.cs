using System;
using System.Collections.Generic;
using System.Linq;
namespace UtilsSpace;

public static class UtilsFunctions
{
  private static readonly Random random = new();

  public static T GetRandomEnumValue<T>() where T : struct, Enum
  {
    return (T)Enum.GetValues(typeof(T)).GetValue(random.Next(Enum.GetValues(typeof(T)).Length));
  }

  public static T GetRandomEnumValueExcluding<T>(params T[] excludedValues) where T : struct, Enum
  {
    if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T)} is not an enum type");

    T[] resultValues = Enum.GetValues<T>().Where(x => !excludedValues.Contains(x)).ToArray();
    int randomIndex = random.Next(resultValues.Length);
    return resultValues[randomIndex];
  }

  public static T[][] ListTo2DArray<T>(List<T> list, int rows, int cols)
  {
    if (list.Count != rows * cols) throw new ArgumentException("List size does not match array dimensions " + "Array size: " + cols * rows + "List size: " + list.Count + "Cols: " + cols + "Rows: " + rows);

    T[][] arr = new T[rows][];
    for (int i = 0; i < rows; i++)
    {
      arr[i] = new T[cols];
    }

    int index = 0;
    for (int i = 0; i < rows; i++)
    {
      for (int j = 0; j < cols; j++)
      {
        arr[i][j] = list[index];
        index++;
      }
    }

    return arr;
  }
}