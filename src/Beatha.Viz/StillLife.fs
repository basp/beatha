namespace Beatha

module StillLife =
    let block pos gen =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 2) ]
        |> Gaia.revive pos gen
        gen
        
    let beehive pos gen =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 4)
          (3, 2)
          (3, 3) ]
        |> Gaia.revive pos gen
        gen
        
    let loaf pos gen =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 4)
          (3, 2)
          (3, 4)
          (4, 3) ]
        |> Gaia.revive pos gen
        gen
    
    let boat pos gen =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 3)
          (3, 2) ]
        |> Gaia.revive pos gen
        gen
        
    let tub pos gen =
        [ (1, 2)
          (2, 1)
          (2, 3)
          (3, 2) ]
        |> Gaia.revive pos gen
        gen

