# Beatha
This is a F# library that focuses on *life-like* cellular automata.

## Getting started
A basic simulation involves a few things:
* A 2D grid world space
* Initial setup
* Step function (evaluator)

### Creating the world space
A *world* is represented by a `IGrid<bool>` (or `Generation`) type. This is a 
two-dimensional grid of cells. We can create such a grid by wrapping it around
a 2D array.
```
let gen = Array2D.create 5 5 false :> Generation
```

Often we need to convert between a raw array and a grid so we use 
a `GridFactory<'a>` function. 
```
 let factory : GridFactory<bool> = 
    fun arr -> Grid(arr)
```

We can then re-use this function while we setup the rest of the pipeline.

### Gane rules
The rules of the game are determined by the `Rule` value.


### Initial setup

### Step function

# Future
* A 2D grid should also be able to calculate its neighborhood positions by combining the neighborhoods of all alive cells.
* This way we don't have have to loop over every cell in the grid, we can just loop over the cumulative neighborhood.
* A better way to deal with time values, currently this is more or less done from the draw function which is not ideal.
* An optimized grid could keep track of the alive cells so when we ask for a neighborhood we don't need to loop over the entire grid.

# Finds
* `Soup.random 3` on a wrapped grid of 180 rows and 320 columns takes about 5000 generations to stabilize into a star field.
* 