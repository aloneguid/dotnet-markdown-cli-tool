using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownTool.Model
{
   class LocationPin
   {
      public LocationPin(string path, string entry)
      {
         Path = path;
         Entry = entry;
      }

      public string Path { get; }
      public string Entry { get; }
   }
}
