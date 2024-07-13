namespace Beatha

open System
open Beatha.Core
open Beatha.Parser
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

[<AutoOpen>]
module Utils =
    let offsetPosition pos offset  =
        let rowOffset, colOffset = offset
        { Row = rowOffset + pos.Row; Column = colOffset + pos.Column }

    let revive pos (gen: Generation) (offsets: (int * int) list) =
        offsets
        |> List.map (offsetPosition pos)
        |> List.iter (fun pos -> gen[pos] <- Some true)

type Operator =
    { Init: Position -> Generation -> Generation
      Dimension: int * int }

module Oscillator =
    let blinker pos (gen: Generation) =
        [ (1, 2)
          (2, 2)
          (3, 2) ]
        |> revive pos gen
        gen   

    let toad pos (gen: Generation) =
        [ (2, 2)
          (2, 3)
          (2, 4)
          (3, 1)
          (3, 2)
          (3, 3) ]
        |> revive pos gen
        gen

module StillLife =
    let block pos (gen: Generation) =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 2) ]
        |> revive pos gen
        gen
        
    let beehive pos (gen: Generation) =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 4)
          (3, 2)
          (3, 3) ]
        |> revive pos gen
        gen
        
    let loaf pos (gen: Generation) =
        [ (1, 2)
          (1, 3)
          (2, 1)
          (2, 4)
          (3, 2)
          (3, 4)
          (4, 3) ]
        |> revive pos gen
        gen
    
    let boat pos (gen: Generation) =
        [ (1, 1)
          (1, 2)
          (2, 1)
          (2, 3)
          (3, 2) ]
        |> revive pos gen
        gen
        
    let tub pos (gen: Generation) =
        [ (1, 2)
          (2, 1)
          (2, 3)
          (3, 2) ]
        |> revive pos gen
        gen
        
module Viz =        
        
    let example () =
        let factory : GridFactory<bool> =
            fun arr -> WrapGrid(arr)
            
        // This string represents the Conway rule.
        let rule =
            match (Parse.rule "B3/S23") with
            | Ok a -> a
            | Error msg -> failwith msg             
        
        // Curry the factory for convenience.
        let eval = factory |> (makeEvaluator rule)        
        
        let rows, cols = (5, 5)
        let width, height = (800, 800)
        
        let mutable current =
            Array2D.create rows cols false
            |> factory
            |> StillLife.tub { Row = 0; Column = 0 }
        
        // Simulate and visualize.
        Window.Init(width, height, "Beatha Viz")        
        Time.SetTargetFPS 60
        
        let dx = float32 width / float32 cols
        let dy = float32 height / float32 rows
        
        let mutable frameCount = 0L
        
        // We don't want all this gunk in the main drawing loop.
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
                
        while (not <| Window.ShouldClose()) do
            // Update portion of the game loop.
            frameCount <- frameCount + 1L
            if (frameCount % 30L = 0) then
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
