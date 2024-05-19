using System;
using System.Collections.Generic;
using Godot;

namespace ThousandDevils.features.GlobalUtils;

public static class GdUtilsFunctions
{
  public static Vector3 ClampVector3(Vector3 vector, Vector3 min, Vector3 max) =>
    new(
      Math.Clamp(vector[0], min[0], max[0]),
      Math.Clamp(vector[1], min[1], max[1]),
      Math.Clamp(vector[2], min[2], max[2])
    );

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