using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownTool.Model
{
   class DocType
   {
      private static readonly Regex GenericCountRgx = new Regex("`(\\d+)");

      public string Name { get; set; }

      public string Summary { get; set; }

      public DocType[] TypeParameters { get; set; }

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

      public string SanitisedName
      {
         get
         {
            Match m = GenericCountRgx.Match(Name);
            if(m.Success && TypeParameters != null)
            {
               string replacement = "<";
               for(int i = 0; i < TypeParameters.Length; i++)
               {
                  if (i != 0) replacement += ", ";
                  replacement += TypeParameters[i].Name;
               }
               replacement += ">";

               return GenericCountRgx.Replace(Name, replacement);
            }

            return Name;
         }
      }
   }
}
