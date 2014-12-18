module Parser
open System.Text.RegularExpressions

type Person =
    {
        Name : string
        Heir : Person option
        Others : string list
    }

let namesRegex = Regex(@"(?<name>[A-Z][a-z]+)", RegexOptions.Compiled)

let ParseToNames line =
    namesRegex.Matches(line)
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Groups.["name"].Value)
    |> Seq.filter (fun n -> n <> "King" && n <> "Messiah" && n <> "Babylon")
    |> Seq.toList
    |> function h::t -> h, t | [] -> failwith "No blank lines!"

let rec NamesToPerson names =
    match names with
    | [] -> None
    | (father,others)::t ->
        let heir =
            match t with
            | [] -> None
            | (heir, _)::_ -> Some heir
        Some {
            Name = father
            Heir = NamesToPerson t
            Others =
                others
                |> List.filter
                    (fun c ->
                        match heir with
                        | Some h -> c <> h
                        | None -> true)
        }

let Parse lines =
    lines
    |> List.map ParseToNames
    |> NamesToPerson
