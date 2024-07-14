namespace Beatha

module Misc =
    let preHoneyFarm pos gen =
        [ (1, 2)
          (1, 3)
          (1, 4)
          (2, 1)
          (2, 4)
          (3, 2)
          (3, 3) ]
        |> Gaia.revive pos gen
        gen
        
    let queenBee pos gen =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 3)
          (3, 4)
          (4, 1)
          (4, 4)
          (5, 4)
          (6, 1)
          (6, 3)
          (7, 1)
          (7, 2) ]
        |> Gaia.revive pos gen
        gen
        
    let twinBees pos gen =
        [ () ]

