﻿/// Contains methods to handle lockfiles.
module Paket.LockFile

open System
open System.IO

/// [omit]
let format (resolved : PackageResolution) = 
    // TODO: implement conflict handling
    let sources = 
        resolved
        |> Seq.map (fun x ->
            match x.Value with
            | Resolved d -> 
                match d.Referenced.VersionRange with
                | Specific v -> d.Referenced.Source,d.Referenced.Name,v
            )
        |> Seq.groupBy (fun (s,_,_) -> s)

    let all = 
        [ yield "NUGET"
          for source, packages in sources do
              yield "  remote: " + source
              yield "  specs:"
              for _, name, version in packages do
                  yield sprintf "    %s (%s)" name <| version.ToString() ]
    
    String.Join(Environment.NewLine, all)

/// Parses a lockfile from lines
let Parse(lines : string seq) = 
    let lines = 
        lines
        |> Seq.filter (fun line -> String.IsNullOrWhiteSpace line |> not)
        |> Seq.map (fun line -> line.Replace(" ", ""))
        |> Seq.skip 1
    
    let remote = ref "http://nuget.org/api/v2"
    [ for line in lines do
          if line.StartsWith("remote:") then remote := line.Replace("remote:", "")
          elif line.StartsWith("specs:") then ()
          else 
              let spec = line.Replace(")", "")
              let splitted = spec.Split('(')
              yield { SourceType = "nuget"
                      Source = !remote
                      Name = splitted.[0]
                      VersionRange = VersionRange.Exactly splitted.[1] } ]

/// Updates the lockfile with the analyzed dependencies from the packageFile.
let Update(packageFile, lockFile) = 
    let cfg = Config.ReadFromFile packageFile
    let resolution = cfg.Resolve(Nuget.NugetDiscovery)
    File.WriteAllText(lockFile, format resolution)
    printfn "Lockfile written to %s" lockFile