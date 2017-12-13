using System;
using System.Collections.Generic;
using System.Text;
using MarkdownTool.Model;

namespace MarkdownTool.Styles
{
   interface IOutputStyle
   {
      void Generate(DocLibrary library, StringBuilder output);
   }
}
