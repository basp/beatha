#r "nuget: FParsec"

#load "Core.fs"
#load "Parser.fs"

open Beatha.Core
open Beatha.Parser

let factory = fun arr ->
    WrapGrid(arr) :> IGrid<bool>

let mutable current =
    Array2D.create 5 5 false
    |> factory

// Blinker
current[{ Row = 1; Column = 2 }] <- Some true
current[{ Row = 2; Column = 2 }] <- Some true
current[{ Row = 3; Column = 2 }] <- Some true

let rule =
    match (Parse.rule "B3/S23") with
    | Ok a -> a
    | Error msg -> failwith msg 

let eval : Evaluator = makeEvaluator rule

for _ in [1..4] do
    printfn $"%A{current.Array}"
    current <- current |> eval factory
