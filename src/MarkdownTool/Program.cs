using Config.Net;
using LogMagic;
using NetBox.Extensions;
using System;
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
               nameof(IParameters.InputPath).PairedWith(1),
               nameof(IParameters.OutputDir).PairedWith(2)
            )
            .Build();

         L.Config.WriteTo.PoshConsole("{message}{error}");

         log.Trace(".NET Markdown CLI v{0}", typeof(Program).FileVersion());

         int r = Validate(p);
         if (r != 0) return r;

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

         if(p.OutputDir == null)
         {
            log.Trace("output directory is required");
            return 3;
         }

         if(!Directory.Exists(p.OutputDir))
         {
            Directory.CreateDirectory(p.OutputDir);
         }

         return 0;
      }
   }
}
