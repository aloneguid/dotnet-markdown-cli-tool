using Config.Net;
using LogMagic;
using NetBox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarkdownTool
{
   class Program
   {
      private static readonly ILog log = L.G(typeof(Program));

      static int Main(string[] args)
      {
         IParameters p = new ConfigurationBuilder<IParameters>()
            .UseCommandLineArgs(
               new KeyValuePair<string, int>(nameof(IParameters.InputPath), 1),
               new KeyValuePair<string, int>(nameof(IParameters.OutputPath), 2)
            )
            .Build();

         L.Config.WriteTo.PoshConsole("{message}{error}");

         log.Trace(".NET Markdown CLI v{0}", typeof(Program).FileVersion());

         int r = Validate(p);
         if (r != 0) return r;

         new XmlToMarkdownConverter(p.InputPath, p.OutputPath, p.SourceSearchPath, p.SourceBasePath).Convert();

         return 0;
      }

      private static int Validate(IParameters p)
      {
         if(p.InputPath == null)
         {
            log.Trace("input path is missing");
            return 1;
         }

         if(!File.Exists(p.InputPath))
         {
            log.Trace("input file {0} does not exist");
            return 2;
         }

         if(p.OutputPath == null)
         {
            log.Trace("output path is required");
            return 3;
         }

         DirectoryInfo dir = new FileInfo(p.OutputPath).Directory;
         if(!dir.Exists) dir.Create();

         return 0;
      }
   }
}
