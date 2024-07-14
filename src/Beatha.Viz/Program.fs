namespace Beatha

open System
open System.Numerics
open Beatha.Core
open Beatha.Parser
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

module Gaia =
    let offsetPosition (pos: Position) (offset: int * int)  =
        let rowOffset, colOffset = offset
        { Row = rowOffset + pos.Row; Column = colOffset + pos.Column }

    let revive (pos: Position) (gen: Generation) (offsets: (int * int) list) =
        offsets
        |> List.map (offsetPosition pos)
        |> List.iter (fun pos -> gen[pos] <- Some true)

type Operator =
    { Init: Position -> Generation -> Generation
      Dimension: int * int }

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

module Spaceship =
    let glider pos gen =
        [ (1, 3)
          (2, 1)
          (2, 3)
          (3, 2)
          (3, 3) ]
        |> Gaia.revive pos gen
        gen

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
        
// Cool examples
// B3/S01234678 with (200, 200) and (800, 800) wrapped acorn.
        
module Viz =        

    /// Basic example of how to setup a visualization.
    let example () =
        
        // Specifies the update interval in frames.
        // For example, 60 is an update of the cell grid roughly every second.
        // Setting N to 1 means updating every frame, this is probably not a
        // good idea.
        let N = 5L        

        // Setup the grid factory to produce a specific grid instance.
        let factory : GridFactory<bool> =
            fun arr -> WrapGrid(arr)
            
        // https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
        let conway = "B3/S23"
        
        // https://en.wikipedia.org/wiki/Highlife_(cellular_automaton)
        let highlife = "B36/S23"
        
        // https://en.wikipedia.org/wiki/Life_without_Death
        let lifeWithoutDeath = "B3/S012345678" 
        
        // If we leave the 4 out we get a square-ish shape. If we leave the
        // five out we get a circular-ish shape.
        let lifeEpidemic = "B3/S01243678"
           
        let rule =
            match (Parse.rule lifeWithoutDeath) with
            | Ok a -> a
            | Error msg -> failwith msg             
        
        // Create the evaluator, curry the factory for convenience.
        let eval = factory |> (makeEvaluator rule)

        // Setup grid and viewport dimensions.
        // let rows, cols = (200, 200)
        let rows, cols = (200, 200)
        let width, height = (800, 800)
        
        // Calculate the horizontal and vertical stride for drawing the cells
        // across the viewport.
        let dx = float32 width / float32 cols
        let dy = float32 height / float32 rows

        let centerPos = { Row = (rows / 2); Column = (cols / 2) }
        
        // Setup the initial population.
        let mutable current =
            Array2D.create rows cols false
            |> factory
            |> Spaceship.glider centerPos
            
        let countAlive (gen: Generation) =
            let mutable n = 0
            for row in [0 .. (gen.Rows - 1)] do
                for col in [0 .. (gen.Columns - 1)] do
                    let pos = { Row = row; Column = col }
                    if isAlive pos gen then
                        n <- n + 1                    
            n

        // We'll update every N frames so keep track of frame count.        
        let mutable frameCount = 0L
        let mutable generation = 0
        let mutable alive = countAlive current       
        
        let rng = Random()
        
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
                        // let i = 0.8f + rng.NextSingle() * 0.2f
                        // let cv = Vector4(i * 0.6f, i * 0.9f, i, 1f)
                        // let color = Color.FromNormalized(cv)
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
            // Start by updating the frame counter.
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
