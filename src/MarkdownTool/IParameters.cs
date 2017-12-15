using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownTool
{
   public interface IParameters
   {
      string InputPath { get; }

      string OutputPath { get; }

      string SourceSearchPath { get; }

      string SourceBasePath { get; }
   }
}
