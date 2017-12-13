using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkdownTool.Model;
using NetBox.Extensions;

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
            GenerateType(dt, output);
         }
      }

      private void GenerateType(DocType dt, StringBuilder output)
      {
         output.Append("#### ");
         output.Append(dt.SanitisedNameWithoutNamespace.HtmlEncode());
         output.AppendLine();
         output.AppendLine();

         output.Append(dt.Summary);
         output.AppendLine();
         output.AppendLine();

         if(dt.TypeParameters != null)
         {
            foreach(DocPrimitive tp in dt.TypeParameters.Where(t => !string.IsNullOrEmpty(t.Summary)))
            {
               output.Append(" - **");
               output.Append(tp.Name);
               output.Append("** - *");
               output.Append(tp.Summary);
               output.Append("*");
               output.AppendLine();
            }
         }

         if(dt.Fields != null)
         {
            output.Append("##### Fields");
            output.AppendLine();
            output.AppendLine();
            output.AppendLine("|Name|Summary|");
            output.AppendLine("|----|-------|");

            foreach (DocPrimitive f in dt.Fields.OrderBy(f => f.NameWithoutNamespace))
            {
               GenerateField(f, output);
            }
         }

         if (dt.Properties != null)
         {
            output.Append("##### Properties");
            output.AppendLine();
            output.AppendLine();
            output.AppendLine("|Name|Summary|");
            output.AppendLine("|----|-------|");

            foreach (DocPrimitive f in dt.Properties.OrderBy(f => f.NameWithoutNamespace))
            {
               GenerateField(f, output);
            }
         }

         if(dt.Methods != null)
         {
            output.Append("##### Methods");
            output.AppendLine();
            output.AppendLine();
            output.AppendLine("|Name|Summary|Returns|");
            output.AppendLine("|----|-------|-------|");

            foreach (DocMethod m in dt.Methods.OrderBy(m => m.MethodNameWithoutParametersAndNamespace))
            {
               GenerateMethod(m, output);
            }
         }

      }

      private void GenerateMethod(DocMethod m, StringBuilder output)
      {
         output.Append("|");
         output.Append(m.MethodNameWithoutParametersAndNamespace);
         output.Append("|");
         output.Append(m.Summary);
         output.Append("|");
         output.Append("?");
         output.Append("|");
         output.AppendLine();
      }

      private void GenerateField(DocPrimitive dt, StringBuilder output)
      {
         output.Append("|");
         output.Append(dt.NameWithoutNamespace);
         output.Append("|");
         output.Append(dt.Summary);
         output.Append("|");
         output.AppendLine();
      }
   }
}
