namespace Beatha

module Methuselah =
    let rPentomino pos gen =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 2)
          (3, 2) ]
        |> Gaia.revive pos gen
        gen
        
    let diehard pos gen =
        [ (1, 7)
          (2, 1)
          (2, 2)
          (3, 2)
          (3, 6)
          (3, 7)
          (3, 8) ]
        |> Gaia.revive pos gen
        gen
        
    let acorn pos gen =
        [ (1, 2)
          (2, 4)
          (3, 1)
          (3, 2)
          (3, 5)
          (3, 6)
          (3, 7) ]
        |> Gaia.revive pos gen
        gen

