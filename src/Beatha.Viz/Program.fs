namespace Beatha

open Beatha.Core
open Beatha.Parser
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing

module Viz =
    
    let example () =
        let factory : GridFactory<bool> =
            fun arr -> WrapGrid(arr)
            
        // This string represents the Conway rule.
        let rule =
            match (Parse.rule "B3/S23") with
            | Ok a -> a
            | Error msg -> failwith msg             
        
        // Preload the evaluator with the factory (for convenience).
        let eval = factory |> (makeEvaluator rule)        
        
        let (rows, cols) = (5, 5)
        let (width, height) = (800, 800)
        
        let mutable current =
            Array2D.create rows cols false
            |> factory                   
        
        // Setup basic blinker.
        current[ { Row = 1; Column = 2 } ] <- Some true
        current[ { Row = 2; Column = 2 } ] <- Some true
        current[ { Row = 3; Column = 2 } ] <- Some true
        
        // Simulate and visualize.
        Window.Init(width, height, "Beatha Viz")        
        Time.SetTargetFPS 60
        
        let dx = float32 width / float32 cols
        let dy = float32 height / float32 rows
        
        let mutable frameCount = 0L
        
        while (not <| Window.ShouldClose()) do
            frameCount <- frameCount + 1L
            
            if (frameCount % 60L = 0) then
                current <- eval current

            Graphics.BeginDrawing()
            do
                Graphics.ClearBackground(Color.RayWhite)
                for row in [0..(current.Rows - 1)] do
                    for col in [0..(current.Columns - 1)] do
                        let pos = { Row = row; Column = col }
                        match current[pos] with
                        | Some a when a ->
                            let px = int <| round (float32 col * dx)
                            let py = int <| round (float32 row * dy)
                            Graphics.DrawRectangle(
                                px,
                                py,
                                int dx,
                                int dy,
                                Color.DarkBlue)
                        | _ -> ()
                Graphics.DrawFPS(10, 10)                
            Graphics.EndDrawing()
        Window.Close()
            
    let [<EntryPoint>] main _ =
        example ()
        0
