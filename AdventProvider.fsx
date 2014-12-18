module AdventProvider
#if INTERACTIVE
#load "paket-files/fsprojects/FSharp.TypeProviders.StarterPack/src/ProvidedTypes.fsi"
#load "paket-files/fsprojects/FSharp.TypeProviders.StarterPack/src/ProvidedTypes.fs"
#load "Parser.fs"
#endif

open System.Reflection
open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open Parser

[<TypeProvider>]
type AdventProvider (cfg : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces ()

    let ns = "Advent.Provided"
    let asm = Assembly.GetExecutingAssembly()
    let tempAsmPath = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), ".dll")
    let tempAsm = ProvidedAssembly tempAsmPath

    let t = ProvidedTypeDefinition(asm, ns, "Family", Some typeof<obj>, IsErased = false)
    let parameters = [ProvidedStaticParameter("Genealogy", typeof<string>)]

    do
        t.DefineStaticParameters(
            parameters,
            fun typeName args ->
                let genealogy = args.[0] :?> string
                let inputFile = 
                    System.IO.Path.Combine(cfg.ResolutionFolder, genealogy)
                let raw =
                    System.IO.File.ReadAllLines inputFile
                let input =
                    raw
                    |> Seq.toList
                    |> Parse

                let g = ProvidedTypeDefinition(
                            asm, 
                            ns, 
                            typeName, 
                            Some typeof<obj>, 
                            IsErased = false)

                let s = ProvidedProperty("Raw", typeof<string>, IsStatic = true)
                let rawStr = String.concat "\n" raw
                s.GetterCode <- fun _ -> Expr.Value rawStr
                g.AddMember s

                let rec personToType (father : ProvidedTypeDefinition) (fatherInterfaces : System.Type list) (person : Person) =
                    let t = ProvidedTypeDefinition(person.Name, Some typeof<obj>, IsErased = false)
                    let parentInterface =
                        match fatherInterfaces with
                        | [] -> None
                        | h::_ -> Some h
                    let i = ProvidedTypeDefinition("I" + person.Name, None, IsErased = false)
                    i.SetAttributes (TypeAttributes.Public ||| TypeAttributes.Interface ||| TypeAttributes.Abstract)
                    fatherInterfaces
                    |> List.iter i.AddInterfaceImplementation
                    father.AddMembers [t;i]
                    match person.Heir with
                    | Some p -> personToType t (i :> System.Type::fatherInterfaces) p
                    | None -> ()

                match input with
                | Some p ->
                    personToType g [] p
                | None ->
                    ()

                tempAsm.AddTypes [g]
                
                g
            )

    do
        this.RegisterRuntimeAssemblyLocationAsProbingFolder cfg
        tempAsm.AddTypes [t]
        this.AddNamespace(ns, [t])

[<assembly:TypeProviderAssembly>]
do ()

