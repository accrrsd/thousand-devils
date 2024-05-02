using System;
using System.Collections.Generic;
using Godot;

namespace ThousandDevils.features.GlobalUtils;

public static class GdUtilsFunctions
{
  public static List<T> GetChildsByType<T>(Node parent, Predicate<T> predicate = null) where T : Node {
    List<T> childs = new();
    foreach (Node child in parent.GetChildren()) {
      if (child is T childT && (predicate == null || predicate(childT))) childs.Add(childT);
      childs.AddRange(GetChildsByType(child, predicate));
    }

    return childs;
  }

  public static T GetFirstChildByType<T>(Node parent, Predicate<T> predicate = null) where T : Node {
    foreach (Node child in parent.GetChildren()) {
      if (child is T childT && (predicate == null || predicate(childT))) return childT;
      T foundChild = GetFirstChildByType(child, predicate);
      if (foundChild != null) return foundChild;
    }

    return null;
  }

  public static T GetFirstParentByType<T>(Node parent, Predicate<T> predicate = null) where T : Node {
    while (parent != null) {
      if (parent is T parentT && (predicate == null || predicate(parentT))) return parentT;
      parent = parent.GetParent();
    }

    return null;
  }

  public static List<T> GetParentsByType<T>(Node parent, Predicate<T> predicate = null) where T : Node {
    List<T> parents = new();
    while (parent != null) {
      if (parent is T parentT && (predicate == null || predicate(parentT))) parents.Add(parentT);
      parent = parent.GetParent();
    }

    return parents;
  }

  public static T UnpackSceneWithDefault<T>(PackedScene targetScene, PackedScene defaultScene) where T : Node =>
    targetScene?.Instantiate<T>() ?? defaultScene.Instantiate<T>();
}
