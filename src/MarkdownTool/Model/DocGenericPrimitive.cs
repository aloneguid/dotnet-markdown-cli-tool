using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownTool.Model
{
   class DocGenericPrimitive : DocPrimitive
   {
      private static readonly Regex GenericCountRgx = new Regex("`(\\d+)");

      public DocPrimitive[] TypeParameters { get; set; }

      public string SanitisedName => Sanitise(Name);

      public string SanitisedNameWithoutNamespace => Sanitise(NameWithoutNamespace);

      public string Sanitise(string s)
      {
         Match m = GenericCountRgx.Match(s);
         if (m.Success && TypeParameters != null)
         {
            string replacement = "<";
            for (int i = 0; i < TypeParameters.Length; i++)
            {
               if (i != 0) replacement += ", ";
               replacement += TypeParameters[i].Name;
            }
            replacement += ">";

            return GenericCountRgx.Replace(s, replacement);
         }

         return s;

      }
   }
}