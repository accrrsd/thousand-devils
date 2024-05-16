using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using static ThousandDevils.features.GlobalUtils.AssociateAttributes;

namespace ThousandDevils.features.GlobalUtils;

public static class UtilsFunctions
{
  private static readonly Random Random = new();

  public static Color GenerateColorFromRgb(float r, float g, float b, float a = 255) =>
    new(MathF.Round(r / 255, 2), MathF.Round(g / 255, 2), MathF.Round(b / 255, 2), MathF.Round(a / 255, 2));

  public static Color GenerateRandomRgbColor(bool generateAlpha = false) =>
    GenerateColorFromRgb(Random.Next(0, 256), Random.Next(0, 256), Random.Next(0, 256), generateAlpha ? Random.Next(0, 256) : 255);

  public static T GetRandomEnumValueExcluding<T>(params T[] excludedValues) where T : struct, Enum {
    if (!typeof(T).IsEnum) throw new ArgumentException($"{typeof(T)} is not an enum type");
    if (Enum.GetValues<T>().Length == 0) throw new ArgumentException("Enum type has no values");
    T[] resultValues = Enum.GetValues<T>().Where(x => !excludedValues.Contains(x)).ToArray();
    int randomIndex = Random.Next(resultValues.Length);
    return resultValues[randomIndex];
  }

  public static bool IsIn2DArrayBounds<T>(int x, int y, T[][] arr) => x >= 0 && x < arr.Length && y >= 0 && y < arr[0].Length;
  public static bool IsIn2DArrayBounds<T>(Vector2I vector, T[][] arr) => IsIn2DArrayBounds(vector[0], vector[1], arr);

  public static PropertyInfo GetPropertyWithPredicate(Type type, Predicate<PropertyInfo> predicate) {
    PropertyInfo[] properties = type.GetProperties();
    return Array.Find(properties, predicate);
  }

  private static IEnumerable<MethodInfo> GetMethodsWithPredicate(Type type, Predicate<MethodInfo> predicate) {
    MethodInfo[] methods = type.GetMethods();
    return methods.Where(p => predicate(p));
  }

  /// <summary>Prop parent class to child class or child class to parent class</summary>
  /// <param name="parentClass"></param>
  /// <param name="childClass"></param>
  /// <param name="parentInChild">if true - prop parent class to child class</param>
  public static bool AssociateClassesByAttribute<T1, T2>(T1 parentClass, T2 childClass, bool parentInChild) where T1 : class where T2 : class {
    List<MethodInfo> neededMethods = GetMethodsWithPredicate(parentInChild ? childClass.GetType() : parentClass.GetType(),
      pMethod =>
        pMethod.GetCustomAttribute(parentInChild ? typeof(ParentSetterAttribute) : typeof(ChildSetterAttribute)) !=
        null && pMethod.GetParameters()[0].ParameterType == (parentInChild ? parentClass.GetType() : childClass.GetType())).ToList();
    if (neededMethods.Count == 0) return false;

    neededMethods[0].Invoke(parentInChild ? childClass : parentClass, new object[] { parentInChild ? parentClass : childClass });
    return true;
  }

  /// <summary>Prop parent class to multiple child classes</summary>
  public static bool AssociateListOfChilds<T1, T2>(T1 parentClass, List<T2> childClasses) where T1 : class where T2 : class {
    List<MethodInfo> neededMethods = GetMethodsWithPredicate(childClasses[0].GetType(),
      pMethod => pMethod.GetCustomAttribute<ParentSetterAttribute>() != null &&
                 pMethod.GetParameters()[0].ParameterType == parentClass.GetType()).ToList();
    if (neededMethods.Count == 0) return false;
    foreach (T2 childClass in childClasses) neededMethods[0].Invoke(childClass, new object[] { parentClass });
    return true;
  }

  /// <summary>Prop both, parent and child class connect to each other by attribute mark.</summary>
  public static bool AssociateParentAndChild<T1, T2>(T1 parentClass, T2 childClass) where T1 : class where T2 : class =>
    AssociateClassesByAttribute(parentClass, childClass, true) && AssociateClassesByAttribute(parentClass, childClass, false);
}