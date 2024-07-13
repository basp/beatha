module Beatha.Core

[<Struct>]
type Position = { Row: int; Column: int }

[<Interface>]
type IGrid<'a> =
    abstract member Rows: int    
    abstract member Columns: int    
    abstract member Item: Position -> 'a option with get, set    
    abstract member Array: 'a array2d
    
[<Sealed>]
type Grid<'a> =
    new: 'a array2d -> Grid<'a>
    interface IGrid<'a>
    
[<Sealed>]
type WrapGrid<'a> =
    new: 'a array2d -> WrapGrid<'a>
    interface IGrid<'a>

type GridFactory<'a> = 'a array2d -> IGrid<'a>

type Generation = IGrid<bool>

type Neighborhood = Moore | VonNeumann    

val neighbors: Position -> Position array

val neighbors2: Neighborhood -> Position -> Position array