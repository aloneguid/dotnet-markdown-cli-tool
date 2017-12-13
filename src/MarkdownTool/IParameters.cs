using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownTool
{
   public interface IParameters
   {
      string InputPath { get; }

      string OutputDir { get; }
   }
}
