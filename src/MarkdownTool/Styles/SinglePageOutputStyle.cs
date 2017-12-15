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
      private readonly Dictionary<string, LocationPin> _entryToLocation;
      private readonly string _pathPrefix;

      public SinglePageOutputStyle(Dictionary<string, LocationPin> entryToLocation, string pathPrefix)
      {
         _entryToLocation = entryToLocation;
         _pathPrefix = pathPrefix;
      }

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
         output.Append("](##");
         output.Append(ns.Name);
         output.Append(") *(");
         output.Append(ns.Types.Length);
         output.Append(")*");
         output.AppendLine();
      }

      private void Generate(DocNamespace ns, StringBuilder output)
      {
         output.Append("## ");
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
         string typeLink = GetEntityLink(dt.Name);

         output.Append("### ");
         if (typeLink != null)
         {
            output.Append("[");
         }
         output.Append(dt.SanitisedNameWithoutNamespace.HtmlEncode());
         if(typeLink != null)
         {
            output.Append("](");
            output.Append(typeLink);
            output.Append(")");
         }
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
            output.Append("#### Fields");
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
            output.Append("#### Properties");
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
            output.Append("#### Methods");
            output.AppendLine();
            output.AppendLine();

            output.AppendLine("|Name|Summary|");
            output.AppendLine("|----|-------|");
               

            foreach (DocMethod m in dt.Methods.OrderBy(m => m.MethodNameWithoutParametersAndNamespace))
            {
               GenerateMethodShort(m, output);
            }
         }

      }

      private void GenerateMethodShort(DocMethod m, StringBuilder output)
      {
         output.Append("|");

         output.Append(m.GetBeautifulParameterString());

         output.Append("|");

         if(!string.IsNullOrEmpty(m.Summary))
         {
            output.Append(m.Summary);
         }

         output.Append("|");
         output.AppendLine();
      }

      private void GenerateMethod(DocMethod m, StringBuilder output)
      {
         //output.Append("**");
         output.Append("|**");

         output.Append(m.GetBeautifulParameterString());

         output.Append("**|");

         if (!string.IsNullOrEmpty(m.Summary))
         {
            output.Append(m.Summary);
         }

         output.Append("|");

         if (!string.IsNullOrEmpty(m.Returns))
         {
            output.Append(m.Returns);
         }

         output.Append("|");

         output.AppendLine();
         output.AppendLine();


         /*if (m.Parameters != null || m.TypeParameters != null)
         {
            if(m.TypeParameters != null)
            {
               foreach(DocPrimitive p in m.TypeParameters)
               {
                  AppendListItem(output, p);
               }
            }

            if (m.Parameters != null)
            {
               foreach (DocPrimitive p in m.Parameters)
               {
                  AppendListItem(output, p);
               }
            }
         }*/
      }

      private void AppendListItem(StringBuilder output, DocPrimitive p)
      {
         if (!string.IsNullOrEmpty(p.Summary))
         {
            output.Append(" - **");
            output.Append(p.Name);
            output.Append("**");
            output.Append(" - *");
            output.Append(p.Summary);
            output.Append("*<br>");
         }
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

      private string GetEntityLink(string entityName)
      {
         if (_entryToLocation == null || _pathPrefix == null) return null;

         if (!_entryToLocation.TryGetValue(entityName, out LocationPin pin)) return null;

         return _pathPrefix + "/" + pin.Path.Replace('\\', '/');
      }
   }
}
