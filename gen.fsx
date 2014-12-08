#!/usr/bin/env fsharpi


(** Load up/reference some things we'll be needing. *)

#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.FscHelper


(** Set up our source files for our provider *)
let sourceFiles = [
        @"paket-files/fsprojects/FSharp.TypeProviders.StarterPack/src/ProvidedTypes.fsi"
        @"paket-files/fsprojects/FSharp.TypeProviders.StarterPack/src/ProvidedTypes.fs"
        "AdventProvider.fs"
    ]

(** Build the provider! *)
Target "Build" (fun _ ->
    trace "Building the build"
    sourceFiles
    |> Fsc (fun p -> { p with Output = "AdventProvider.dll"; FscTarget = Library })
)

Run "Build"
