package sudoku

object SudokuSolver {
	private var stack = List[Sudoku]()
	private var reduced = false
	private var solved = false

	def initialize(puzzle:Sudoku) {
		stack = puzzle :: Nil
		reduced = false
		solved = false
	}

	def reduce() {
		
	}

	def solve():Sudoku = {
		while (stack nonEmpty) {
			val current = stack.head
			stack = stack.tail

			if (current.isFeasible) {
				if (current.isComplete) return current

				val (row,col) = current.nextCellToAssign()
				for (num <- current.domain(row,col)) {
					val next = current.clone()
					next.setCellValue(row,col,num)
					stack = next :: stack
				}
			}
		}
		throw new Exception("Invalid Sudoku: No solution possible.")
	}
}