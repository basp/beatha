module Tests

open Xunit
open FsUnit.Xunit
open Beatha.Core
open Beatha.Parser

let makeWrappedGrid: GridFactory<bool> = fun arr -> WrapGrid(arr)

let makeBoundedGrid: GridFactory<bool> = fun arr -> Grid(arr)

[<Fact>]
let ``Wrapped grid wraps around horizontally`` () =
    let grid = Array2D.create 3 3 false |> makeWrappedGrid
    grid[{ Row = 0; Column = 3 }] <- Some true
    grid[{ Row = 0; Column = 0 }] |> should equal <| Some true

[<Fact>]
let ``Wrapped grid wraps around vertically`` () =
    let grid = Array2D.create 3 3 false |> makeWrappedGrid
    grid[{ Row = 3; Column = 0 }] <- Some true
    grid[{ Row = 0; Column = 0 }] |> should equal <| Some true
    
[<Fact>]
let ``Bounded grid allows out of bounds row index`` () =
    let grid = Array2D.create 3 3 false |> makeBoundedGrid
    grid[{ Row = 5; Column = 0 }] <- Some true
    grid[{ Row = 5; Column = 0 }] |> should equal None 
    
[<Fact>]
let ``Bounded grid allows out of bounds colum index`` () =
    let grid = Array2D.create 3 3 false |> makeBoundedGrid
    grid[{ Row = 0; Column = 5 }] <- Some true
    grid[{ Row = 0; Column = 5 }] |> should equal None
    
[<Fact>]
let ``Neighbors of the Moore neighborhood exclude the given position`` () =
    // https://en.wikipedia.org/wiki/Moore_neighborhood
    let pos = { Row = 1; Column = 1 }
    let expected =
        [| { Row = 0; Column = 0 }
           { Row = 0; Column = 1 }
           { Row = 0; Column = 2 }
           { Row = 1; Column = 0 }
           { Row = 1; Column = 2 }
           { Row = 2; Column = 0 }
           { Row = 2; Column = 1 }
           { Row = 2; Column = 2 }
        |]        
    neighbors2 Moore pos |> should equal expected

[<Fact>]
let ``Neighbors of the Von Neumann neighborhood exclude the given position`` () =
    // https://en.wikipedia.org/wiki/Von_Neumann_neighborhood
    let pos = { Row = 1; Column = 1; }
    let expected =
        [| { Row = 0; Column = 1 }
           { Row = 1; Column = 0 }
           { Row = 1; Column = 2 }
           { Row = 2; Column = 1 }
        |]
    neighbors2 VonNeumann pos |> should equal expected
    
[<Fact>]
let ``Default neighbors are in the Moore neighborhood`` () =
    let pos = { Row = 1; Column = 1 }
    neighbors pos |> should equal <| neighbors2 Moore pos
    
[<Fact>]
let ``Parse rule string in Golly format`` () =
    match Parse.rule "B123/S456" with
    | Ok a ->
        a |> should equal
            { Birth = [1; 2; 3]
              Survival = [4; 5; 6] }
    | Error msg -> Assert.Fail(msg)
    
[<Fact>]
let ``Parse rule string in MCell format`` () =
    match Parse.rule "123/456" with
    | Ok a ->
        a |> should equal
            { Birth = [1; 2; 3]
              Survival = [4; 5; 6] }
    | Error msg -> Assert.Fail(msg)
    
[<Fact>]
let ``Calculate alive neighbors in Moore neighborhood`` () =
    let grid = Array2D.create 3 3 true |> makeBoundedGrid
    grid[{ Row = 1; Column = 0 }] <- Some false
    grid
    |> countAliveNeighbors2 Moore { Row = 1; Column = 1 }
    |> should equal 7  
    
[<Fact>]
let ``Calculate alive neighbors in Von Neumann neighborhood`` () =
    let grid = Array2D.create 3 3 true |> makeBoundedGrid
    grid[{ Row = 1; Column = 0 }] <- Some false
    grid
    |> countAliveNeighbors2 VonNeumann { Row = 1; Column = 1 }
    |> should equal 3

[<Fact>]
let ``Calculate alive neighbors uses Moore neighborhood by default`` () =
    let grid = Array2D.create 3 3 true |> makeBoundedGrid
    grid[{ Row = 1; Column = 0 }] <- Some false
    let pos = { Row = 1; Column = 1 }
    countAliveNeighbors pos grid 
    |> should equal
    <| countAliveNeighbors2 Moore pos grid

[<Fact>]    
let ``Offset a list of positions by a number of rows`` () =
    let positions =
        [ (0, 0)
          (1, 1)
          (2, 2) ]
        |> List.map (fun (row, col) ->
            { Row = row; Column = col })
    let expected =
        [ { Row = 5; Column = 0 }
          { Row = 6; Column = 1 }
          { Row = 7; Column = 2 } ]
    positions
    |> offsetRows 5
    |> should equal expected
    
[<Fact>]    
let ``Offset a list of positions by a number of columns`` () =
    let positions =
        [ (0, 0)
          (1, 1)
          (2, 2) ]
        |> List.map (fun (row, col) ->
            { Row = row; Column = col })
    let expected =
        [ { Row = 0; Column = 5 }
          { Row = 1; Column = 6 }
          { Row = 2; Column = 7 } ]
    positions
    |> offsetCols 5
    |> should equal expected
    
    
[<Fact>]    
let ``Offset a list of positions by a number of rows and columns`` () =
    let positions =
        [ (0, 0)
          (1, 1)
          (2, 2) ]
        |> List.map (fun (row, col) ->
            { Row = row; Column = col })
    let expected =
        [ { Row = 5; Column = 5 }
          { Row = 6; Column = 6 }
          { Row = 7; Column = 7 } ]
    positions
    |> offset 5 5
    |> should equal expected
