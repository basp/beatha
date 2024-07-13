namespace Beatha

open System
open Beatha.Core
open Beatha.Parser
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

module Utils =
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
        |> Utils.revive pos gen
        gen   

    let toad pos gen =
        [ (2, 2)
          (2, 3)
          (2, 4)
          (3, 1)
          (3, 2)
          (3, 3) ]
        |> Utils.revive pos gen
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
        |> Utils.revive pos gen
        gen

module Spaceship =
    let glider pos gen =
        [ (1, 3)
          (2, 1)
          (2, 3)
          (3, 2)
          (3, 3) ]
        |> Utils.revive pos gen
        gen

module StillLife =
    let block pos gen =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 2) ]
        |> Utils.revive pos gen
        gen
        
    let beehive pos gen =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 4)
          (3, 2)
          (3, 3) ]
        |> Utils.revive pos gen
        gen
        
    let loaf pos gen =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 4)
          (3, 2)
          (3, 4)
          (4, 3) ]
        |> Utils.revive pos gen
        gen
    
    let boat pos gen =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 3)
          (3, 2) ]
        |> Utils.revive pos gen
        gen
        
    let tub pos gen =
        [ (1, 2)
          (2, 1)
          (2, 3)
          (3, 2) ]
        |> Utils.revive pos gen
        gen
        
module Methuselah =
    let rPentomino pos gen =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 2)
          (3, 2) ]
        |> Utils.revive pos gen
        gen
        
module Viz =        

    /// Basic example of how to setup a visualization.
    let example () =
        let factory : GridFactory<bool> =
            fun arr -> WrapGrid(arr)
            
        let conway = "B3/S23"
        
        // Specifies the update interval in frames.
        // For example, 60 is an update of the cell grid roughly every second.
        let N = 60L        
           
        // This string represents the Conway rule.
        let rule =
            match (Parse.rule conway) with
            | Ok a -> a
            | Error msg -> failwith msg             
        
        // Curry the factory for convenience.
        let eval = factory |> (makeEvaluator rule)        
        
        let rows, cols = (100, 100)
        let width, height = (800, 800)
        
        // Calculate the horizontal and vertical stride for drawing the cells
        // across the viewport.
        let dx = float32 width / float32 cols
        let dy = float32 height / float32 rows

        // Setup the initial population.
        let mutable current =
            Array2D.create rows cols false
            |> factory
            |> Methuselah.rPentomino { Row = 50; Column = 50 }

        // We'll update every N frames so keep track of frame count.        
        let mutable frameCount = 0L

        // We don't want all this gunk in the main drawing loop so we will
        // keep it in its own function that we can call later.
        let drawCells () =
            for row in [0..(current.Rows - 1)] do
                for col in [0..(current.Columns - 1)] do
                    let pos = { Row = row; Column = col }
                    match current[pos] with
                    | Some a when a ->
                        let px = int <| round (float32 col * dx)
                        let py = int <| round (float32 row * dy)
                        Graphics.DrawRectangle(
                            px + 1,
                            py + 1,
                            int dx - 2,
                            int dy - 2,
                            Color.DarkBlue)
                    | _ -> ()
                
        // Start simulation.
        Window.Init(width, height, "Beatha Viz")        
        Time.SetTargetFPS 60       
        while (not <| Window.ShouldClose()) do
            // Update portion of the game loop.
            // Start by updating the frame counter.
            frameCount <- frameCount + 1L
            
            // We only have to update every N frames. 
            if (frameCount % N = 0) then
                current <- eval current

            // Drawing portion of the game loop.
            Graphics.BeginDrawing()
            do
                Graphics.ClearBackground(Color.RayWhite)
                drawCells ()
                Graphics.DrawFPS(10, 10)                
            Graphics.EndDrawing()
        Window.Close()
            
    let [<EntryPoint>] main _ =
        example ()
        0
