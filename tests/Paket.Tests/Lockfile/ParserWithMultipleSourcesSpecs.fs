module Paket.LockFile.ParserWithMultipleSourcesSpecs

open Paket
open NUnit.Framework
open FsUnit
open TestHelpers

let lockFile = """NUGET
  remote: http://nuget.org/api/v2
  specs:
    Castle.Windsor (2.1)
    Castle.Windsor-log4net (3.3)
    log (1.2)
    log4net (1.1)
  remote: http://nuget.org/api/v3
  specs:
    Rx-Core (2.1)
    Rx-Main (2.0)"""

[<Test>]
let ``should parse lock file``() = 
    let strict,result,_ = LockFile.Parse(toLines lockFile)
    let result = result |> Seq.toArray

    result.Length |> shouldEqual 6
    strict |> shouldEqual false

    result.[0].Source |> shouldEqual (Nuget "http://nuget.org/api/v2")
    result.[0].Name |> shouldEqual "Castle.Windsor"
    result.[0].Version |> shouldEqual (SemVer.parse "2.1")

    result.[1].Source |> shouldEqual (Nuget "http://nuget.org/api/v2")
    result.[1].Name |> shouldEqual "Castle.Windsor-log4net"
    result.[1].Version |> shouldEqual (SemVer.parse "3.3")
    
    result.[4].Source |> shouldEqual (Nuget "http://nuget.org/api/v3")
    result.[4].Name |> shouldEqual "Rx-Core"
    result.[4].Version |> shouldEqual (SemVer.parse "2.1")

    result.[5].Source |> shouldEqual (Nuget "http://nuget.org/api/v3")
    result.[5].Name |> shouldEqual "Rx-Main"
    result.[5].Version |> shouldEqual (SemVer.parse "2.0")