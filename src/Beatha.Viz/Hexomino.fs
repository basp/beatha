namespace Beatha

module Hexomino =
    let stairstep pos gen =
        [ (1, 2)
          (2, 2)
          (2, 3)
          (3, 3)
          (3, 4)
          (4, 4) ]
        |> Gaia.revive pos gen
        gen
