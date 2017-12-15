using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MarkdownTool.Model;

namespace MarkdownTool
{
   class SourceTree
   {
      private static readonly LogMagic.ILog log = LogMagic.L.G(typeof(SourceTree));
      private static Regex NamespaceClassRgx = new Regex(@"^namespace\s+(.*)[\s\S]*class\s+([\da-zA-Z]*)\s", RegexOptions.Multiline | RegexOptions.Compiled);
      private readonly string _rootPath;
      private readonly Dictionary<string, LocationPin> _entityToLocation = new Dictionary<string, LocationPin>();

      public SourceTree(string rootPath)
      {
         _rootPath = rootPath;
      }

      public Dictionary<string, LocationPin> Build()
      {
         log.Trace("discovering source files...");
         string[] files = Directory.GetFiles(_rootPath, "*.cs", SearchOption.AllDirectories);
         log.Trace("found {0} files.", files.Length);

         log.Trace("scanning...");
         foreach(string file in files)
         {
            ProcessFile(file);
         }
         log.Trace("found {0} useful entries.", _entityToLocation.Count);
         return _entityToLocation;
      }

      private void ProcessFile(string path)
      {
         string txt = File.ReadAllText(path);

         PutEntries(path, NamespaceClassRgx, txt);
      }

      private void PutEntries(string path, Regex rgx, string text)
      {
         path = path.Substring(_rootPath.Length).Trim('/', '\\');
         MatchCollection matches = rgx.Matches(text);
         foreach(Match m in matches)
         {
            string ns = m.Groups[1].Value;
            string cn = m.Groups[2].Value;
            string full = ns.Trim() + "." + cn.Trim();


            var pin = new LocationPin(path, full);

            _entityToLocation[full] = pin;
         }
      }
   }
}
