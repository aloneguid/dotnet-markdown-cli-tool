using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownTool.Model
{
    class DocPrimitive
    {
      public string Name { get; set; }

      public string Summary { get; set; }

      public string Namespace
      {
         get
         {
            int i = Name.LastIndexOf('.');
            if (i == -1) return Name;

            return Name.Substring(0, i);
         }
      }

      public string NameWithoutNamespace
      {
         get
         {
            int i = Name.LastIndexOf('.');
            if (i == -1) return Name;
            return Name.Substring(i + 1);
         }
      }




   }
}
