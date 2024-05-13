using System;

namespace ThousandDevils.features.GlobalUtils;

public static class AssociateAttributes
{
  [AttributeUsage(AttributeTargets.Method)]
  public class ChildSetterAttribute : Attribute
  { }

  [AttributeUsage(AttributeTargets.Method)]
  public class ParentSetterAttribute : Attribute
  { }
}
