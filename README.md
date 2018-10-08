# SudokuSolver
A simple artificial intelligence program for solving a Sudoku puzzle.

## How to Use

In order to run a main method using the Scala build tool `sbt`, create a `Main.scala` file in the `\src\main\scala` directory. In this file, create an `Main` object extending `App` to create a main method object.

To use the `SudokuSolver` object, use the `Sudoku.parseString` method to parse a Sudoku string into a `Sudoku` object, or use the `Sudoku.parseFile` method to parse a text file containing a Sudoku board string. Initialize the `SudokuSolver`object with the `Sudoku` object, and run `solve` to get a solved `Sudoku`

#### Example

```sh
import sudoku._

object Main extends App {
	val s = ". 2 . | . . . | . . ."   + "\n" +
		". . . | 6 . . | . . 3"   + "\n" +
		". 7 4 | . 8 . | . . ."   + "\n" +
		"------+-------+------"   + "\n" +
		". . . | . . 3 | . . 2"   + "\n" +
		". 8 . | . 4 . | . 1 ."   + "\n" +
		"6 . . | 5 . . | . . ."   + "\n" +
		"------+-------+------"   + "\n" +
		". . . | . 1 . | 7 8 ."   + "\n" +
		"5 . . | . . 9 | . . ."   + "\n" +
		". . . | . . . | . 4 ."

	val b = Sudoku.parseString(s)
	SudokuSolver.initialize(b)
	println(SudokuSolver.solve())
}
```
