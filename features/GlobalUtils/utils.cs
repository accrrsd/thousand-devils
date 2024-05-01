using System;
using System.Collections.Generic;
using System.Linq;

namespace ThousandDevils.features.GlobalUtils;

public static class UtilsFunctions
{
  private static readonly Random Random = new();

  public static T GetRandomEnumValue<T>() where T : struct, Enum
  {
    if (Enum.GetValues(typeof(T)).Length == 0) throw new ArgumentException("Enum type has no values");
    return (T)Enum.GetValues(typeof(T)).GetValue(Random.Next(Enum.GetValues(typeof(T)).Length))!;
  }

  public static T GetRandomEnumValueExcluding<T>(params T[] excludedValues) where T : struct, Enum
  {
    if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T)} is not an enum type");

    T[] resultValues = Enum.GetValues<T>().Where(x => !excludedValues.Contains(x)).ToArray();
    int randomIndex = Random.Next(resultValues.Length);
    return resultValues[randomIndex];
  }

  /// <param name="list"></param>
  /// <param name="onEachFunc">
  ///   Addition function to call each elem: fist param - elem, second index in list, third - column,
  ///   fourth - row
  /// </param>
  /// <param name="cols"></param>
  /// <param name="rows"></param>
  public static T[][] ListTo2DArray<T>(List<T> list, int rows, int cols, Action<T, int, int, int> onEachFunc = null)
  {
    if (list.Count != rows * cols)
      throw new ArgumentException("List size does not match array dimensions " + "Array size: " + cols * rows +
                                  " List size: " + list.Count + " Cols: " + cols + " Rows: " + rows);

    T[][] arr = new T[rows][];
    for (int i = 0; i < rows; i++) arr[i] = new T[cols];

    int index = 0;
    for (int i = 0; i < rows; i++)
      for (int j = 0; j < cols; j++)
      {
        onEachFunc?.Invoke(list[index], index, i, j);
        arr[i][j] = list[index];
        index++;
      }

    return arr;
  }

  public static bool IsIn2DArrayBounds<T>(int x, int y, T[][] arr)
  {
    return x >= 0 && x < arr.Length && y >= 0 && y < arr[0].Length;
  }
}
