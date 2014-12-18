#!/usr/bin/env fsharpi

(** Load up/reference some things we'll be needing. *)

#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.FscHelper

(** Set up our source files for our provider *)
let sourceFiles = [
        @"paket-files/fsprojects/FSharp.TypeProviders.StarterPack/src/ProvidedTypes.fsi"
        @"paket-files/fsprojects/FSharp.TypeProviders.StarterPack/src/ProvidedTypes.fs"
        "Parser.fs"
        "AdventProvider.fsx"
    ]

(** Build the provider! *)
Target "BuildProvider" (fun _ ->
    trace "Building the build"
    sourceFiles
    |> Fsc (fun p -> { p with Output = "AdventProvider.dll"; FscTarget = Library })
)

(** Build the test app **)
Target "BuildApp" (fun _ ->
    trace "Building the app"
    [ "Families.fsx" ]
    |> Fsc (fun p -> { p with Output = "App.exe"; FscTarget = Exe; References = ["AdventProvider.dll"] })
)

"BuildProvider"
 ==> "BuildApp"

Run "BuildApp"
