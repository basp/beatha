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

/// Convenience alias for bool grids.
type Generation = IGrid<bool>  
    
/// Determines what cells are considered neighbors.
type Neighborhood = Moore | VonNeumann    

let northWest neighborhood pos =
    match neighborhood with
    | Moore -> Some { Row = pos.Row - 1; Column = pos.Column - 1 }
    | VonNeumann -> None
    
let north neighborhood pos =
    let pos' = { pos with Row = pos.Row - 1 }
    match neighborhood with
    | Moore -> Some pos'
    | VonNeumann -> Some pos'

let northEast neighborhood pos =
    match neighborhood with
    | Moore -> Some { Row = pos.Row - 1; Column = pos.Column + 1 }
    | VonNeumann -> None
    
let east neighborhood pos =
    let pos' = { pos with Column = pos.Column + 1 }
    match neighborhood with
    | Moore -> Some pos'
    | VonNeumann -> Some pos'
    
let southEast neighborhood pos =
    match neighborhood with
    | Moore -> Some { Row = pos.Row + 1; Column = pos.Column + 1 }
    | VonNeumann -> None
    
let south neighborhood pos =
    let pos' = { pos with Row = pos.Row + 1 }
    match neighborhood with
    | Moore -> Some pos'
    | VonNeumann -> Some pos'
    
let southWest neighborhood pos =
    match neighborhood with
    | Moore -> Some { Row = pos.Row + 1; Column = pos.Column - 1 }
    | VonNeumann -> None
    
let west neighborhood pos =
    let pos' = { pos with Column = pos.Column - 1 }
    match neighborhood with
    | Moore -> Some pos'
    | VonNeumann -> Some pos'

// Offsets for a Moore neighborhood.
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

// Offsets for a Von Neumann neighborhood.
let vonNeumann =
    [| { Row = -1; Column = 0 }
       { Row = 0; Column = -1 }
       { Row = 0; Column = 1 }
       { Row = 1; Column = 0 }
    |]      
    
/// Returns an array of neighbor offsets for the given neighborhood.
let neighborOffsets neighborhood =
    match neighborhood with
    | Moore -> moore
    | VonNeumann -> vonNeumann
    
/// Given a neighborhood and position, returns a list of neighboring positions.
let neighbors2 neighborhood pos =
    let offsets = neighborOffsets neighborhood
    let offsetPosition offset =
        { Row = offset.Row + pos.Row
          Column = offset.Column + pos.Column }
    offsets |> Array.map offsetPosition
    
/// Same as neighbors2 but always uses Moore neighborhood.
let neighbors = neighbors2 Moore

/// Given a neighborhood and position, returns the number of alive neighbors.
let countAliveNeighbors2 neighborhood pos (gen: Generation) =
    pos
    |> neighbors2 neighborhood
    |> Array.map (fun p -> gen[p])
    |> Array.where (fun opt -> opt |> Option.defaultValue false)
    |> Array.length

/// Same as countAliveNeighbors2 but always uses Moore neighborhood.
let countAliveNeighbors pos gen = countAliveNeighbors2 Moore pos gen

/// Returns a 2D array where each position contains the number of alive
/// neighbors corresponding to the same position in the given generation.
let mapLivingNeighbors2 neighborhood (gen: Generation) =
    gen.Array
    |> Array2D.mapi (fun row col _ ->
        let pos = { Row = row; Column = col }
        gen |> countAliveNeighbors2 neighborhood pos)

/// Same as mapLivingNeighbors2 but always uses Moore neighborhood.
let mapLivingNeighbors = mapLivingNeighbors2 Moore

/// Given a position and a generation, returns true if that position
/// corresponds to a living cell and false otherwise.
let isAlive pos (gen: Generation) =
    match gen[pos] with
    | Some a -> a
    | None -> false

/// A specification about how cells are born and survive.
type Rule =
    {
      /// If a dead cell has a number of alive neighbors that occurs in this
      /// list, it will be (re)born.
      Birth: int list
      
      /// If an alive cell has a number of alive neighbors that occurs in this
      /// list, it will survive.
      Survival: int list
    }

/// An evaluator function applies a game rule to a given generation in order
/// to produce a new successive generation. A grid factory function is used to
/// transform the raw array into one of the supported grid implementations.
type Evaluator = GridFactory<bool> -> Generation -> Generation

/// Creates an evaluator function based on the given rule.
let makeEvaluator rule : Evaluator =
    let b = rule.Birth
    let s = rule.Survival
    fun (factory: GridFactory<bool>) gen ->
        gen
        |> mapLivingNeighbors
        |> Array2D.mapi (fun row col aliveNeighbors ->
            let pos = { Row = row; Column = col }
            let areWeAlive = gen |> isAlive pos
            match (areWeAlive, aliveNeighbors) with
            | true, n -> s |> List.contains n
            | false, n -> b |> List.contains n)
        |> factory        