namespace Beatha

module Tetromino =
    let t pos gen =
        [ (1, 2)
          (2, 1)
          (2, 2)
          (2, 3) ]
        |> Gaia.revive pos gen
        gen
