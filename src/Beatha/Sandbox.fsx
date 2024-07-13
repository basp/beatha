#load "Core.fs"

open Beatha.Core

let grid =
    Array2D.create 5 5 false
    |> WrapGrid
    :> Generation 

grid[{ Row = 1; Column = 1 }] <- Some true
grid[{ Row = 1; Column = 3 }] <- Some true
grid[{ Row = 3; Column = 1 }] <- Some true
grid[{ Row = 3; Column = 3 }] <- Some true

let rule = {
    Birth = []
    Survival = [] }

grid |> mapLivingNeighbors2