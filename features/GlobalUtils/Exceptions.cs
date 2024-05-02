using System;

namespace ThousandDevils.features.GlobalUtils;

public static class MyExceptions
{
  /// <summary>
  ///   Exception for editor export parameters
  /// </summary>
  public class EmptyExportException : Exception
  {
    public EmptyExportException() : base("Invalid editor export parameter") { }
    public EmptyExportException(string message) : base(message) { }
  }
}
