object SudokuSolver {
	private var stack = List()

	def initialize(val puzzle:Sudoku) {
		stack = List(puzzle)
	}

	def reduce() {
		
	}

	def solve() = {
		while (stack nonEmpty) {
			val current = stack.head
			stack = stack.tail

			if (current.isFeasible) {
				if (current.isComplete) return current;

				var (row,col) = current.nextCellToAssign()
				for (num <- current.domain(row,col))
					stack = current.clone().setCellValue(row,col,num) :: stack
			}
		}
		throw new Exception("Invalid Sudoku: No solution possible.")
	}
}