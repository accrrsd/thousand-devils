using System;
using System.Collections.Generic;
using Godot;

namespace ThousandDevils.features.GlobalUtils;

public static class GdUtilsFunctions
{
  public static List<T> FindChildsByType<T>(Node parent, Func<T, bool> predicate = null) where T : Node
  {
    List<T> childs = new();
    foreach (Node child in parent.GetChildren())
    {
      if (child is T childT && (predicate == null || predicate(childT))) childs.Add(childT);
      childs.AddRange(FindChildsByType(child, predicate));
    }

    return childs;
  }

  // todo check if it works
  public static T FindFirstChildByType<T>(Node parent, Func<T, bool> predicate = null) where T : Node
  {
    foreach (Node child in parent.GetChildren())
      if (child is T childT && (predicate == null || predicate(childT)))
        return childT;
    return null;
  }

  // todo check if it works
  public static T FindFirstParentByType<T>(Node child, Func<T, bool> predicate = null) where T : Node
  {
    while (child != null)
    {
      if (child is T parentT && (predicate == null || predicate(parentT))) return parentT;
      child = child.GetParent();
    }

    return null;
  }

  public static List<T> FindParentsByType<T>(Node child, Func<T, bool> predicate = null) where T : Node
  {
    List<T> parents = new();
    while (child != null)
    {
      if (child is T parentT && (predicate == null || predicate(parentT))) parents.Add(parentT);
      child = child.GetParent();
    }

    return parents;
  }
}
