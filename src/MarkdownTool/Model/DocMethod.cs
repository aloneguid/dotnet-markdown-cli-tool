using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownTool.Model
{
   class DocMethod : DocGenericPrimitive
   {
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
   }
}
