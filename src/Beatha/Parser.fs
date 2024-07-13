module Beatha.Parser

open FParsec

type Rule =
    { Birth: int list
      Survival: int list }

// https://conwaylife.com/wiki/Mirek%27s_Cellebration
let parseMCell : Parser<Core.Rule, unit> =
    many1 digit
    .>> pchar '/'
    .>>. many1 digit
    |>> fun (birth, survival) ->
        { Birth = birth |> List.map (fun c -> int c - int '0')
          Survival = survival |> List.map (fun c -> int c - int '0')}    

/// https://golly.sourceforge.io/
let parseGolly : Parser<Core.Rule, unit> =
    pchar 'B'
    >>. many1 digit
    .>> pchar '/'
    .>> pchar 'S'
    .>>. many digit
    |>> fun (born, survive) ->
        { Birth = born |> List.map (fun c -> int c - int '0')
          Survival = survive |> List.map (fun c -> int c - int '0') }
        
let parseRule =
    choice [ parseGolly; parseMCell ]

module Parse =
    let rule s =
        match run parseRule s with
        | Success (result, _, _) -> FSharp.Core.Ok result
        | Failure (msg, _, _) -> FSharp.Core.Error msg         