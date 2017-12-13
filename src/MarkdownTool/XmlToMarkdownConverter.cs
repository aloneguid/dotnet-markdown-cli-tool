using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using MarkdownTool.Model;

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

         _s.AppendLine("# Library " + libName);
         _s.AppendLine();

         ICollection<DocType> types = GetTypes();
         _s.AppendLine("## Types");
         _s.AppendLine();
         AddTypeTable(types);

         File.WriteAllText(_outputFile, _s.ToString());
      }

      private void AddTypeTable(ICollection<DocType> types)
      {
         _s.AppendLine("|Name|Summary|");
         _s.AppendLine("|----|-------|");

         foreach(DocType dt in types)
         {
            _s.Append("|`");
            _s.Append(dt.Name);
            _s.Append("`|");
            _s.Append(dt.Summary);
            _s.AppendLine("|");
         }
      }

      private ICollection<DocType> GetTypes()
      {
         var r = new List<DocType>();

         foreach(XmlNode node in GetMembers("T:"))
         {
            var dt = new DocType();
            dt.Name = node.Attributes["name"].Value.Substring(2);
            dt.Summary = GetValue(node, "summary");

            XmlNodeList typeParams = node.SelectNodes("typeparam");
            foreach(XmlNode typeParam in typeParams)
            {
               string name = typeParam.Attributes["name"].Value;
               string summary = typeParam.InnerText;
            }


            r.Add(dt);
         }

         return r;
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
