using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using MarkdownTool.Model;
using MarkdownTool.Styles;

namespace MarkdownTool
{
   class XmlToMarkdownConverter
   {
      private readonly string _inputFile;
      private readonly string _outputFile;
      private readonly XmlDocument _xmlDoc;
      private readonly StringBuilder _s = new StringBuilder();
      private static readonly Regex _wsRgx = new Regex("\\s+", RegexOptions.Multiline | RegexOptions.Compiled);

      public XmlToMarkdownConverter(string inputFile, string outputFile)
      {
         _xmlDoc = new XmlDocument();

         using (FileStream fs = File.OpenRead(inputFile))
         {
            _xmlDoc.Load(fs);
         }

         _outputFile = outputFile;
      }

      public void Convert()
      {
         string libName = _xmlDoc.SelectSingleNode("//assembly/name").InnerText;

         ICollection<DocType> types = GetTypes();
         ICollection<DocNamespace> namespaces = GetNamespaces(types);
         var lib = new DocLibrary { Name = libName, Namespaces = namespaces.ToArray() };


         IOutputStyle style = new SinglePageOutputStyle();
         style.Generate(lib, _s);

         File.WriteAllText(_outputFile, _s.ToString());
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
            DocType dt = ToDocType(node, 2);
            XmlNodeList typeParams = node.SelectNodes("typeparam");
            dt.TypeParameters = typeParams.Count == 0
               ? null
               : typeParams.Cast<XmlNode>().Select(n => ToDocType(n, useInnerTextAsSummary: true)).ToArray();

            r.Add(dt);
         }

         return r;
      }

      private DocType ToDocType(XmlNode node, int prefixLength = 0, bool useInnerTextAsSummary = false)
      {
         var dt = new DocType();
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
