namespace MarkdownTool.Model
{
   class DocType : DocGenericPrimitive
   {
      public DocPrimitive[] Fields { get; set; }

      public DocPrimitive[] Properties { get; set; }

      public DocMethod[] Methods { get; set; }
   }
}
