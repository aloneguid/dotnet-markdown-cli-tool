using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkdownTool.Model;

namespace MarkdownTool.Styles
{
   class SinglePageOutputStyle : IOutputStyle
   {
      public void Generate(DocLibrary library, StringBuilder output)
      {
         output.AppendLine("# Library " + library.Name);
         output.AppendLine();

         output.AppendLine("## Namespaces");
         output.AppendLine();

         foreach(DocNamespace ns in library.Namespaces.OrderBy(ns => ns.Name))
         {
            GenerateTitle(ns, output);
         }

         output.AppendLine();

         foreach (DocNamespace ns in library.Namespaces.OrderBy(ns => ns.Name))
         {
            Generate(ns, output);
         }
      }

      private void GenerateTitle(DocNamespace ns, StringBuilder output)
      {
         output.Append(" - [");
         output.Append(ns.Name);
         output.Append("](###");
         output.Append(ns.Name);
         output.Append(") *(");
         output.Append(ns.Types.Length);
         output.Append(")*");
         output.AppendLine();
      }

      private void Generate(DocNamespace ns, StringBuilder output)
      {
         output.Append("### ");
         output.Append(ns.Name);
         output.AppendLine();
         output.AppendLine();

         foreach(DocType dt in ns.Types)
         {
            Generate(dt, output);
         }
      }

      private void Generate(DocType dt, StringBuilder output)
      {
         output.Append("#### ");
         output.Append(dt.NameWithoutNamespace);
         output.AppendLine();
         output.AppendLine();

         output.Append(dt.Summary);
         output.AppendLine();
         output.AppendLine();
      }
   }
}
