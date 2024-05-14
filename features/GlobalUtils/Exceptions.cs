using System;

namespace ThousandDevils.features.GlobalUtils;

public static class MyExceptions
{
  /// <summary>Exception for editor export parameters</summary>
  public class EmptyExportException : Exception
  {
    public EmptyExportException() : base("Invalid editor export parameter") { }
    public EmptyExportException(string message) : base(message) { }
  }

  public class NotExistingElemException : Exception
  {
    public NotExistingElemException(string message) : base(message) { }
    public NotExistingElemException() : base("Element does not exists!") { }
    public NotExistingElemException(Type elemType) : base(elemType + " Element does not exists!") { }
  }
}