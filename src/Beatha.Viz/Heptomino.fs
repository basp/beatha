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

    let b pos gen =
        [ (1, 1)
          (1, 2)
          (2, 2)
          (2, 3)
          (3, 1)
          (3, 2)
          (4, 1) ]
        |> Gaia.revive pos gen
        gen