using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using LogMagic;
using MarkdownTool.Model;
using MarkdownTool.Styles;

namespace MarkdownTool
{
   class XmlToMarkdownConverter
   {
      private static readonly ILog log = L.G(typeof(XmlToMarkdownConverter));
      private readonly string _inputFile;
      private readonly string _outputFile;
      private readonly XmlDocument _xmlDoc;
      private readonly StringBuilder _s = new StringBuilder();
      private static readonly Regex _wsRgx = new Regex("\\s+", RegexOptions.Multiline | RegexOptions.Compiled);

      public XmlToMarkdownConverter(string inputFile, string outputFile)
      {
         _xmlDoc = new XmlDocument();

         log.Trace("loading file...");
         using (FileStream fs = File.OpenRead(inputFile))
         {
            _xmlDoc.Load(fs);
         }

         _outputFile = outputFile;
      }

      public void Convert()
      {
         string libName = _xmlDoc.SelectSingleNode("//assembly/name").InnerText;

         log.Trace("extracting types...");
         ICollection<DocType> types = GetTypes();
         log.Trace("detecting namespaces...");
         ICollection<DocNamespace> namespaces = GetNamespaces(types);
         var lib = new DocLibrary { Name = libName, Namespaces = namespaces.ToArray() };

         log.Trace("converting...");
         IOutputStyle style = new SinglePageOutputStyle();
         style.Generate(lib, _s);

         log.Trace("writing to file...");
         if (File.Exists(_outputFile)) File.Delete(_outputFile);
         File.WriteAllText(_outputFile, _s.ToString());

         log.Trace("done.");
      }


      private ICollection<DocNamespace> GetNamespaces(ICollection<DocType> types)
      {
         return types
            .GroupBy(t => t.Namespace)
            .Select(g => new DocNamespace
            {
               Name = g.Key,
               Types = g.ToArray()
            })
            .ToList();
      }

      private ICollection<DocType> GetTypes()
      {
         var r = new List<DocType>();

         foreach(XmlNode node in GetMembers("T:"))
         {
            DocType dt = ToDocType(node);

            XmlNodeList typeParams = node.SelectNodes("typeparam");
            dt.TypeParameters = typeParams.Count == 0
               ? null
               : typeParams.Cast<XmlNode>().Select(n => ToDocPrimitive(n, useInnerTextAsSummary: true)).ToArray();

            XmlNodeList fields = GetMembers("F:" + dt.Name);
            dt.Fields = fields.Count == 0
               ? null
               : fields.Cast<XmlNode>().Select(n => ToDocPrimitive(n, prefixLength: 2)).ToArray();

            XmlNodeList properties = GetMembers("P:" + dt.Name);
            dt.Properties = properties.Count == 0
               ? null
               : properties.Cast<XmlNode>().Select(n => ToDocPrimitive(n, prefixLength: 2)).ToArray();

            XmlNodeList methods = GetMembers("M:" + dt.Name);
            dt.Methods = methods.Count == 0
               ? null
               : methods.Cast<XmlNode>().Select(n => ToDocMethod(n, dt)).ToArray();

            r.Add(dt);
         }

         return r;
      }

      private DocMethod ToDocMethod(XmlNode node, DocType parent)
      {
         DocPrimitive dp = ToDocPrimitive(node, 2);
         var m = new DocMethod
         {
            Name = dp.Name,
            Summary = dp.Summary,
            Parent = parent
         };

         XmlNodeList typeParams = node.SelectNodes("typeparam");
         m.TypeParameters = typeParams.Count == 0
            ? null
            : typeParams.Cast<XmlNode>().Select(n => ToDocPrimitive(n, useInnerTextAsSummary: true)).ToArray();

         XmlNodeList methodParams = node.SelectNodes("param");
         m.Parameters = methodParams.Count == 0
            ? null
            : methodParams.Cast<XmlNode>().Select(n => ToDocPrimitive(n, useInnerTextAsSummary: true)).ToArray();

         m.Returns = GetValue(node, "returns");

         return m;
      }

      private DocType ToDocType(XmlNode node)
      {
         DocPrimitive dp = ToDocPrimitive(node, 2);
         var dt = new DocType
         {
            Name = dp.Name,
            Summary = dp.Summary
         };
         return dt;
      }

      private DocPrimitive ToDocPrimitive(XmlNode node, int prefixLength = 0, bool useInnerTextAsSummary = false)
      {
         var dt = new DocPrimitive();
         dt.Name = node.Attributes["name"].Value.Substring(prefixLength);
         dt.Summary = useInnerTextAsSummary
            ? node.InnerText
            : GetValue(node, "summary");
         return dt;
      }

      private string GetValue(XmlNode node, string path)
      {
         XmlNode fn = node.SelectSingleNode(path);
         if (fn == null) return null;
         string s = fn.InnerText;

         s = _wsRgx.Replace(s, " ");
         s = s.Trim();

         return s;
      }

      private XmlNodeList GetMembers(string namePrefix)
      {
         return _xmlDoc.SelectNodes($"//members/member[starts-with(@name, \"{namePrefix}\")]");
      }
   }
}
