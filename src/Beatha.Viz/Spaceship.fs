namespace Beatha

module Spaceship =
    let glider pos gen =
        [ (1, 3)
          (2, 1)
          (2, 3)
          (3, 2)
          (3, 3) ]
        |> Gaia.revive pos gen
        gen


