namespace Beatha

module Oscillator =
    let blinker pos gen =
        [ (1, 2)
          (2, 2)
          (3, 2) ]
        |> Gaia.revive pos gen
        gen   

    let toad pos gen =
        [ (2, 2)
          (2, 3)
          (2, 4)
          (3, 1)
          (3, 2)
          (3, 3) ]
        |> Gaia.revive pos gen
        gen
        
    let beacon pos gen =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 2)
          (3, 3)
          (3, 4)
          (4, 3)
          (4, 4) ]
        |> Gaia.revive pos gen
        gen
        
    let clock pos gen =
        [ (1, 3)
          (2, 1)
          (2, 3)
          (3, 2)
          (3, 4)
          (4, 2) ]
        |> Gaia.revive pos gen
        gen
        
    let pentadecathlon pos gen =
        [ (5, 4)
          (5, 5)
          (4, 6)
          (6, 6)
          (5, 7)
          (5, 8)
          (5, 9)
          (5, 10)
          (4, 11)
          (6, 11)
          (5, 12)
          (5, 13) ]
        |> Gaia.revive pos gen
        gen

    let queenBeeShuttle pos gen =
        [ (0, 9)
          (1, 7)
          (1, 9)
          (2, 6)
          (2, 8)
          (2, 20)
          (2, 21)
          (3, 0)
          (3, 1)
          (3, 5)
          (3, 8)
          (3, 20)
          (3, 21)
          (4, 0)
          (4, 1)
          (4, 6)
          (4, 8)
          (5, 7)
          (5, 9)
          (6, 9) ]
        |> Gaia.revive pos gen
        gen

