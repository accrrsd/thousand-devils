using System;
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
}