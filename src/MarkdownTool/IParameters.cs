using System;
using System.Collections.Generic;
using System.Text;
using Config.Net;

namespace MarkdownTool
{
   public interface IParameters
   {
      [Option(Alias = "in")]
      string InputPath { get; }

      [Option(Alias = "out")]
      string OutputPath { get; }

      string SourceSearchPath { get; }

      string SourceBasePath { get; }
   }
}
