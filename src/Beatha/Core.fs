module Beatha.Core

type [<Struct>] Position = { Row: int; Column: int }

/// The interface for life-like automata grids.
type [<Interface>] IGrid<'a> =
    
    /// Gets the number of rows in the grid.
    abstract member Rows: int
    
    /// Gets the number of columns in the grid.
    abstract member Columns: int
    
    /// Gets or sets the value at given position.
    abstract member Item: Position -> 'a option with get, set
    
    /// Gets a reference to the underlying array.
    abstract member Array: 'a array2d
    
/// Provides option based indexing for a bounded 2D grid.
type [<Sealed>] Grid<'a>(arr: 'a array2d) =
    let rows = Array2D.length1 arr
    
    let cols = Array2D.length2 arr        
    
    let validRow row = row >= 0 && row < rows        
    
    let validCol col = col >= 0 && col < cols
    
    interface IGrid<'a> with
        member _.Array = arr
        
        member _.Rows = rows
        
        member _.Columns = cols
        
        member _.Item
            with get pos =
                if validRow pos.Row && validCol pos.Column then
                    Some arr[pos.Row, pos.Column]
                else None
            and set pos value =
                match value with
                | Some a ->
                    if validRow pos.Row && validCol pos.Column then
                        arr[pos.Row, pos.Column] <- a
                    else ()
                | None -> ()
    
/// Provides wrap-around indexing for a 2D grid.
type [<Sealed>] WrapGrid<'a>(arr: 'a array2d) =
    let rows = Array2D.length1 arr
    
    let cols = Array2D.length2 arr

    let wrap v vMin vMax =
        let modV = v % vMax
        if v < vMin then vMax + modV
        else modV
        
    let wrapRow row = wrap row 0 rows        
    
    let wrapCol col = wrap col 0 cols
    
    interface IGrid<'a> with        
        member _.Array = arr
        
        member _.Rows = rows
        
        member _.Columns = cols
        
        member _.Item
            with get pos =
                let row' = wrapRow pos.Row
                let col' = wrapCol pos.Column
                Some arr[row', col']
            and set pos value =
                match value with
                | Some a ->
                    let row' = wrapRow pos.Row
                    let col' = wrapCol pos.Column
                    arr[row', col'] <- a
                | None -> ()

/// Constructs a grid from a 2D array.
type GridFactory<'a> = 'a array2d -> IGrid<'a>

type Generation = IGrid<bool>  
    
type Neighborhood = Moore | VonNeumann    
    
let neighbors2 neighborhood pos =
    let moore =
        [| { Row = -1; Column = -1 }
           { Row = -1; Column = 0 }
           { Row = -1; Column = 1 }
           { Row = 0; Column = -1 }
           { Row = 0; Column = 1 }
           { Row = 1; Column = -1 }
           { Row = 1; Column = 0 }
           { Row = 1; Column = 1 }
        |]
    let vonNeumann =
        [| { Row = -1; Column = 0 }
           { Row = 0; Column = -1 }
           { Row = 0; Column = 1 }
           { Row = 1; Column = 0 }
        |]      
    let offsets =
        match neighborhood with
        | Moore -> moore
        | VonNeumann -> vonNeumann
    let offsetPosition offset =
        { Row = offset.Row + pos.Row
          Column = offset.Column + pos.Column }
    offsets |> Array.map offsetPosition
    
let neighbors = neighbors2 Moore

let countAliveNeighbors2 neighborhood pos (gen: Generation) =
    pos
    |> neighbors2 neighborhood
    |> Array.map (fun p -> gen[p])
    |> Array.where (fun opt -> opt |> Option.defaultValue false)
    |> Array.length

let countAliveNeighbors = countAliveNeighbors2 Moore

let mapLivingNeighbors2 neighborhood (gen: Generation) =
    gen.Array
    |> Array2D.mapi (fun row col _ ->
        let pos = { Row = row; Column = col }
        gen |> countAliveNeighbors2 neighborhood pos)

let mapLivingNeighbors = mapLivingNeighbors2 Moore

let isAlive (gen: Generation) pos =
    match gen[pos] with
    | Some a -> a
    | None -> false

type Rule =
    { Birth: int list
      Survival: int list }

type Evaluator = GridFactory<bool> -> Generation -> Generation

