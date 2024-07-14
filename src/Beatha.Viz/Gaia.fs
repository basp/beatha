namespace Beatha

open Beatha.Core

module Gaia =
    let offsetPosition (pos: Position) (offset: int * int)  =
        let rowOffset, colOffset = offset
        { Row = rowOffset + pos.Row; Column = colOffset + pos.Column }

    let revive (pos: Position) (gen: Generation) (offsets: (int * int) list) =
        offsets
        |> List.map (offsetPosition pos)
        |> List.iter (fun pos -> gen[pos] <- Some true)

