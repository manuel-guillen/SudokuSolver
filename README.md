# PuzzleSolver
A simple constraint propagation + domain reduction implementation for solving a Constraint Satisfaction Problem (CSP) puzzles (ex. Sudoku)

## Example Usage

The below example is the single main program file (using top-level statements) of a C# console application. Taking a dependency on the `PuzzleSolver.Core` module, 
we import the namespaces for the puzzle models and for the solver. We make use of a string parsing `Sudoku` static method in the example below, which only focuses on
digits, letters, and `.` (to denote empty). The rest of the characters are to make the string look nice.

#### Example

```csharp
using PuzzleSolver.Core.Model.Puzzles;
using PuzzleSolver.Core.Solver;

string s = """
9 . . | 7 . 1 | 8 . .
. . 4 | 5 . . | . . 2
. . 6 | . . 3 | . . .
------+-------+------
2 7 9 | . . 5 | . 6 .
. . 5 | . . 8 | 7 . .
. . . | . . . | . . .
------+-------+------
4 . . | 2 9 . | . 3 1
. 2 . | . . . | . 8 .
3 . . | . . . | . . .
""";

Sudoku puzzle = Sudoku.Parse(s);
Console.WriteLine($"{puzzle}\n");

ISolver<Sudoku> solver = Solver.CreateInstance(puzzle);
if (solver.TrySolve(out Sudoku? solution))
    Console.WriteLine($"SOLUTION:\n{solution}");
```
