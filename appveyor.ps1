### AppVeyor specific code

$gv = $env:APPVEYOR_BUILD_VERSION
if($gv -eq $null)
{
   $gv = "1.0.0"
}

function Update-ProjectVersion($File)
{
   $v = $gv

   $xml = [xml](Get-Content $File.FullName)

   if($xml.Project.PropertyGroup.Count -eq $null)
   {
      $pg = $xml.Project.PropertyGroup
   }
   else
   {
      $pg = $xml.Project.PropertyGroup[0]
   }

   $parts = $v -split "\."
   $bv = $parts[2]
   if($bv.Contains("-")) { $bv = $bv.Substring(0, $bv.IndexOf("-"))}
   $fv = "{0}.{1}.{2}.0" -f $parts[0], $parts[1], $bv
   $av = "{0}.0.0.0" -f $parts[0]
   $pv = $v

   $pg.Version = $pv
   $pg.FileVersion = $fv
   $pg.AssemblyVersion = $av

   Write-Host "$($File.Name) => fv: $fv, av: $av, pkg: $pv"

   $xml.Save($File.FullName)
}

Invoke-Expression "nuget restore src/MarkdownTool.sln"

# Update versioning information
Get-ChildItem *.csproj -Recurse | % {
   Update-ProjectVersion $_
}
