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

