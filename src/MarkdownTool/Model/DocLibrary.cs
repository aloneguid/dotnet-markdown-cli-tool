using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownTool.Model
{
   class DocLibrary
   {
      public string Name { get; set; }

      public DocNamespace[] Namespaces { get; set; }

      public static string Anchor(string s)
      {
         return s
            .ToLower()
            .Replace(" ", "-");
      }
   }
}
