namespace Beatha

/// Contains shapes that work well with the Highlife automaton.
module Highlife =
    let replicator pos gen =
        [ (1, 3)
          (1, 4)
          (1, 5)
          (2, 2)
          (2, 5)
          (3, 1)
          (3, 5)
          (4, 1)
          (4, 4)
          (5, 1)
          (5, 2)
          (5, 3) ]
        |> Gaia.revive pos gen
        gen

