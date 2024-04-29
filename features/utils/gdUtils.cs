using System;
using System.Collections.Generic;
using Godot;

namespace UtilsSpace;

public static class GdUtilsFunctions
{
  public static List<T> FindChildsByType<T>(Node parent, Func<T, bool> predicate = null) where T : Node
  {
    List<T> children = new();

    foreach (var child in parent.GetChildren())
    {
      if (child is T childT)
      {
        if (predicate == null || predicate(childT))
        {
          children.Add(childT);
        }
      }

      children.AddRange(FindChildsByType<T>(child, predicate));
    }

    return children;
  }
}