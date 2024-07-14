namespace Beatha

module Heptomino =
    let pi pos gen =
        [ (1, 1)
          (1, 2)
          (1, 3)
          (2, 1)
          (2, 3)
          (3, 1)
          (3, 3) ]
        |> Gaia.revive pos gen
        gen

