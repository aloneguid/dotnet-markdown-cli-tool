using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownTool.Model
{
   class DocMethod : DocGenericPrimitive
   {
      public DocType Parent { get; set; }

      public DocPrimitive[] Parameters { get; set; }

      public string Returns { get; set; }

      public string MethodNameWithoutParametersAndNamespace
      {
         get
         {
            string n = Name;

            //cut-off parameters
            int idx = n.IndexOf("(");
            if (idx != -1) n = n.Substring(0, idx);

            //cut-off namespace
            idx = n.LastIndexOf(".");
            if (idx != -1) n = n.Substring(idx + 1);

            return n;
         }
      }

      public string[] GetParameterTypesInName()
      {
         int idx = Name.IndexOf("(");
         if (idx == -1) return null;

         string s = Name.Substring(idx + 1).Trim('(', ')');
         string[] r = s.Split(',');
         return r;
      }

      public string GetBeautifulParameterString()
      {
         var b = new StringBuilder();
         b.Append(MethodNameWithoutParametersAndNamespace);
         string[] ptn = GetParameterTypesInName();

         b.Append("(");

         if (ptn != null)
         {
            int npi = 0;
            int gpi = 0;
            for (int i = 0; i < ptn.Length; i++)
            {
               if(i != 0)
               {
                  b.Append(", ");
               }

               string name = ptn[i];
               string oname = name.StartsWith("`")
                  ? TypeParameters?[gpi++].Name
                  : Parameters?[npi++].Name;

               if(name.EndsWith("@"))
               {
                  name = name.TrimEnd('@');
                  b.Append("*out* ");
               }
               b.Append(name);

               if (oname != null)
               {
                  b.Append(" ");
                  b.Append(oname);
               }
            }
         }

         b.Append(")");

         return b.ToString();
      }

   }
}
