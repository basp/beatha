namespace Beatha

open System
open Beatha.Core

module Soup =
    let random seed (gen: Generation) =
        let rng = Random(seed)
        for row in [0..(gen.Rows - 1)] do
            for col in [0..(gen.Columns - 1)] do
                let roll = rng.NextDouble()
                if roll < 0.5 then
                    gen[ { Row = row; Column = col } ] <- Some true
        gen

