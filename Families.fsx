#!/usr/bin/env fsharpi
#r @"AdventProvider.dll"

open AdventProvider
open Advent.Provided

type JesusGenerations = Family<"Genealogy.txt">

printfn "%s" (JesusGenerations.Raw.Substring(0, 20) + "...")

let descendentOfAbraham (_ : #JesusGenerations.IAbraham) = true
let descendentOfDavid 
        (_ : 
            #JesusGenerations
                .Abraham
                .Isaac
                .Jacob
                .Judah
                .Perez
                .Hezron
                .Ram
                .Amminadab
                .Nahshon
                .Salmon
                .Boaz
                .Obed
                .Jesse
                .IDavid) = true


// This compiles...
printfn "%A" <| descendentOfAbraham ({ new JesusGenerations.Abraham.IIsaac })

// So does this:
printfn "%A" <|
    descendentOfDavid 
        ({ new JesusGenerations
                .Abraham
                .Isaac
                .Jacob
                .Judah
                .Perez
                .Hezron
                .Ram
                .Amminadab
                .Nahshon
                .Salmon
                .Boaz
                .Obed
                .Jesse
                .David
                .Solomon
                .Rehoboam
                .IAbijah })

// This doesn't - how cool is that?
(* printfn "%A" <|
    descendentOfDavid ({ new JesusGenerations.Abraham.IIsaac }) *)


