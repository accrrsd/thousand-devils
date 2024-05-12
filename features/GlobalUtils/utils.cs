using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace ThousandDevils.features.GlobalUtils;

public static class UtilsFunctions
{
  private static readonly Random Random = new();

  public static T GetRandomEnumValue<T>() where T : struct, Enum {
    if (Enum.GetValues(typeof(T)).Length == 0) throw new ArgumentException("Enum type has no values");
    return (T)Enum.GetValues(typeof(T)).GetValue(Random.Next(Enum.GetValues(typeof(T)).Length))!;
  }

  public static T GetRandomEnumValueExcluding<T>(params T[] excludedValues) where T : struct, Enum {
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
  public static T[][] ListTo2DArray<T>(List<T> list, int rows, int cols, Action<T, int, int, int> onEachFunc = null) {
    if (list.Count != rows * cols)
      throw new ArgumentException("List size does not match array dimensions " + "Array size: " + cols * rows +
                                  " List size: " + list.Count + " Cols: " + cols + " Rows: " + rows);

    T[][] arr = new T[rows][];
    for (int i = 0; i < rows; i++) arr[i] = new T[cols];

    int index = 0;
    for (int i = 0; i < rows; i++)
      for (int j = 0; j < cols; j++) {
        onEachFunc?.Invoke(list[index], index, i, j);
        arr[i][j] = list[index];
        index++;
      }

    return arr;
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

  /// <summary>
  ///   Prop parent class to child class or child class to parent class
  /// </summary>
  /// <param name="parentClass"></param>
  /// <param name="childClass"></param>
  /// <param name="parentInChild">if true - prop parent class to child class</param>
  public static bool AssociateClassesByAttribute<T1, T2>(T1 parentClass, T2 childClass, bool parentInChild) where T1 : class where T2 : class {
    IEnumerable<MethodInfo> methods = parentInChild
      ? GetMethodsWithPredicate(childClass.GetType(), p => p.GetCustomAttribute<MyAttributes.ParentSetterAttribute>() != null)
      : GetMethodsWithPredicate(parentClass.GetType(), p => p.GetCustomAttribute<MyAttributes.ChildSetterAttribute>() != null);
    List<MethodInfo> setterMethods = methods
      .Where(mInfo => mInfo.GetParameters()[0].ParameterType == (parentInChild ? parentClass.GetType() : childClass.GetType()))
      .ToList();
    if (setterMethods.Count == 0) return false;
    setterMethods[0].Invoke(parentInChild ? childClass : parentClass, new object[] { parentInChild ? parentClass : childClass });
    return true;
  }

  ///<summary>Prop parent class to multiple child classes</summary>
  public static bool AssociateListOfChilds<T1, T2>(T1 parentClass, List<T2> childClasses) where T1 : class where T2 : class {
    IEnumerable<MethodInfo> methods =
      GetMethodsWithPredicate(childClasses[0].GetType(), p => p.GetCustomAttribute<MyAttributes.ParentSetterAttribute>() != null);
    List<MethodInfo> parentSettersInChild = methods.Where(mInfo => mInfo.GetParameters()[0].ParameterType == parentClass.GetType()).ToList();
    if (parentSettersInChild.Count == 0) return false;
    foreach (T2 childClass in childClasses) parentSettersInChild[0].Invoke(childClass, new object[] { parentClass });
    return true;
  }

  //Prop both, parent and child class connect to each other by attribute mark.
  public static bool AssociateParentAndChild<T1, T2>(T1 parentClass, T2 childClass) where T1 : class where T2 : class =>
    AssociateClassesByAttribute(parentClass, childClass, true) && AssociateClassesByAttribute(parentClass, childClass, false);
}
