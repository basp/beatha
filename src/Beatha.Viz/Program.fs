namespace Beatha

open System
open System.Numerics
open Beatha.Core
open Beatha.Parser
open Raylib_CSharp
open Raylib_CSharp.Colors
open Raylib_CSharp.Rendering
open Raylib_CSharp.Windowing
       
module Viz =              
    type DrawActiveCell = WithMargin | Full | Dont

    let example () =        
        // Specifies the update interval in frames.
        // For example, 60 is an update of the cell grid roughly every second.
        // Setting N to 1 means updating every frame, this is probably not a
        // good idea.
        let N = 2L
        
        let mode = Dont

        let factory : GridFactory<bool> = fun arr -> WrapGrid(arr)
            
        // https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life
        let conway = "B3/S23"
        
        // https://en.wikipedia.org/wiki/Highlife_(cellular_automaton)
        let highlife = "B36/S23"
        
        // https://en.wikipedia.org/wiki/Life_without_Death
        let lifeWithoutDeath = "B3/S012345678" 
           
        let rule =
            match (Parse.rule highlife) with
            | Ok a -> a
            | Error msg -> failwith msg             
        
        // Create the evaluator, curry the factory for convenience.
        let eval = factory |> (makeEvaluator rule)

        // Setup grid and viewport dimensions.
        let rows, cols = (180, 320)
        let width, height = (1280, 720)
        
        // Calculate the horizontal and vertical stride for drawing the cells
        // across the viewport.
        let dx = float32 width / float32 cols
        let dy = float32 height / float32 rows

        /// let centerPos = { Row = (rows / 2); Column = (cols / 2) }
        let pos = { Row = 18; Column = 10 }
        
        // Setup the initial population.
        let mutable current =
            Array2D.create rows cols false
            |> factory
            // |> Oscillator.queenBeeShuttle pos
            |> Soup.random 7
            
        // We'll use this to keep track of which cells have recently
        // been visited.
        let timeArray = Array2D.create rows cols 0f
            
        // We'll update every N frames so keep track of frame count.        
        let mutable frameCount = 0L
        let mutable generation = 0
        let mutable alive = 0       
        
        // We don't want all this gunk in the main drawing loop so we will
        // keep it in its own function that we can call later.
        let drawCells () =
            
            // Actually, keeping track of alive cells here is kinda smelly as
            // well as the whole time business.
            alive <- 0
            for row in [0..(current.Rows - 1)] do
                for col in [0..(current.Columns - 1)] do
                    let pos = { Row = row; Column = col }
                    let px = int <| round (float32 col * dx)
                    let py = int <| round (float32 row * dy)
                    
                    // Draw the background to indicate whether this cell
                    // has been recently visited.
                    // let colorBG = Vector4(0.5f, 0.75f, 0.9f, 1f)
                    let colorBG = Vector4(0.75f, 0.125f, 0.2f, 1f)
                    let t = timeArray[row, col]
                    Graphics.DrawRectangle(
                        px,
                        py,
                        int dx,
                        int dy,
                        Color.FromNormalized(colorBG * t))
                    
                    // See if this cell is alive and draw it if so.
                    match current[pos] with
                    | Some a when a ->
                        // It's a bit hacky to update the time from the draw
                        // function so best look for a better way to organize
                        // this. It is pretty efficient though.
                        timeArray[row, col] <- 1f
                        // Keep count of alive cells (for display purposes).
                        alive <- alive + 1                        
                        let color = Color.White                        
                        let drawWithMargin n =
                            Graphics.DrawRectangle(
                                px + n,
                                py + n,
                                int dx - (2 * n),
                                int dy - (2 * n),
                                color)                                                    
                        let drawWithoutMargin () =
                            Graphics.DrawRectangle(
                                px,
                                py,
                                int dx,
                                int dy,
                                color)                            
                        match mode with
                        | WithMargin -> drawWithMargin 1
                        | Full -> drawWithoutMargin ()
                        | _ -> ()
                    | _ -> ()
                
        // Start simulation.
        Window.Init(width, height, "Beatha Viz")        
        Time.SetTargetFPS 60        
        while (not <| Window.ShouldClose()) do
            let deltaTime = Time.GetFrameTime()
            let simTime = Time.GetTime()
            // Update portion of the game loop.
            frameCount <- frameCount + 1L
            
            // We only have to update every N frames. 
            if (frameCount % N = 0) && (alive > 0) then
                generation <- generation + 1
                current <- eval current
            
            let fade = 0.025f
            timeArray
            |> Array2D.iteri (fun row col t ->
                timeArray[row, col] <- max (t - deltaTime * fade) 0f) 
            
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
                let t = TimeSpan.FromSeconds(simTime)
                Graphics.DrawText(
                    $"Time: {t}",
                    10,
                    80,
                    20,
                    Color.White)                
            Graphics.EndDrawing()            
        Window.Close()
            
    let [<EntryPoint>] main _ =
        example ()
        0
