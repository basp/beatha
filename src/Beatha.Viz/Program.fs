namespace Beatha

open System
open Beatha.Core
open Beatha.Parser
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

type Operator =
    { Init: Position -> Generation -> Generation
      Dimension: int * int }

module Soup =
    let random seed (gen: Generation) =
        let rng = Random(seed)
        for row in [0..(gen.Rows - 1)] do
            for col in [0..(gen.Columns - 1)] do
                let roll = rng.NextDouble()
                if roll < 0.5 then
                    gen[ { Row = row; Column = col } ] <- Some true
        gen

module Spaceship =
    let glider pos gen =
        [ (1, 3)
          (2, 1)
          (2, 3)
          (3, 2)
          (3, 3) ]
        |> Gaia.revive pos gen
        gen
       
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
        
module Viz =        

    let example () =
        
        // Specifies the update interval in frames.
        // For example, 60 is an update of the cell grid roughly every second.
        // Setting N to 1 means updating every frame, this is probably not a
        // good idea.
        let N = 2L        

        // Setup the grid factory to produce a specific grid instance.
        let factory : GridFactory<bool> =
            fun arr -> WrapGrid(arr)
            
        // https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
        let conway = "B3/S23"
        
        // https://en.wikipedia.org/wiki/Highlife_(cellular_automaton)
        // let highlife = "B36/S23"
        
        // https://en.wikipedia.org/wiki/Life_without_Death
        // let lifeWithoutDeath = "B3/S012345678" 
           
        let rule =
            match (Parse.rule conway) with
            | Ok a -> a
            | Error msg -> failwith msg             
        
        // Create the evaluator, curry the factory for convenience.
        let eval = factory |> (makeEvaluator rule)

        // Setup grid and viewport dimensions.
        let rows, cols = (200, 200)
        let width, height = (800, 800)
        
        // Calculate the horizontal and vertical stride for drawing the cells
        // across the viewport.
        let dx = float32 width / float32 cols
        let dy = float32 height / float32 rows

        /// let centerPos = { Row = (rows / 2); Column = (cols / 2) }
        let pos = { Row = 100; Column = 100 }
        
        // Setup the initial population.
        let mutable current =
            Array2D.create rows cols false
            |> factory
            // |> Oscillator.queenBeeShuttle pos
            |> Heptomino.pi pos
            
        // We'll update every N frames so keep track of frame count.        
        let mutable frameCount = 0L
        let mutable generation = 0
        let mutable alive = 0       
        
        // We don't want all this gunk in the main drawing loop so we will
        // keep it in its own function that we can call later.
        let drawCells () =
            alive <- 0
            for row in [0..(current.Rows - 1)] do
                for col in [0..(current.Columns - 1)] do
                    let pos = { Row = row; Column = col }
                    match current[pos] with
                    | Some a when a ->
                        alive <- alive + 1
                        let px = int <| round (float32 col * dx)
                        let py = int <| round (float32 row * dy)
                        let color = Color.SkyBlue
                        Graphics.DrawRectangle(
                            px + 1,
                            py + 1,
                            int dx - 2,
                            int dy - 2,
                            color)
                    | _ -> ()
                
        // Start simulation.
        Window.Init(width, height, "Beatha Viz")        
        Time.SetTargetFPS 60        
        while (not <| Window.ShouldClose()) do
            // Update portion of the game loop.
            frameCount <- frameCount + 1L
            
            // We only have to update every N frames. 
            if (frameCount % N = 0) && (alive > 0) then
                generation <- generation + 1
                current <- eval current

            // Drawing portion of the game loop.
            Graphics.BeginDrawing()
            do
                Graphics.ClearBackground(Color.Black)
                drawCells ()
                Graphics.DrawFPS(10, 10)
                Graphics.DrawText(
                    $"Generation: {generation}",
                    10,
                    32,
                    20,
                    Color.White)
                Graphics.DrawText(
                    $"Alive: {alive}",
                    10,
                    56,
                    20,
                    Color.White)                
            Graphics.EndDrawing()            
        Window.Close()
            
    let [<EntryPoint>] main _ =
        example ()
        0
