import scala.collection.mutable.BitSet
import scala.collection.mutable.HashSet

class Sudoku(val size: Int) {
	private val board = Array.ofDim[Int](size,size,size,size)
	private val domains = Array.fill[BitSet](size,size,size,size)(BitSet(1 to size*size :_*))
	private val unassignedCells = HashSet({for (r <- 0 until size*size; c <- 0 until size*size) yield (r,c)} :_*)

	private def index(row:Int, col:Int) = (row / size, row % size, col / size, col % size)
	private def cell(R:Int, r:Int, C:Int, c:Int) = (size*R + r, size*C + c)

	def this() { this(3) }

	def this(other: Sudoku) {
		this(other.size)

		for (R <- 0 until size; r <- 0 until size; C <- 0 until size; c <- 0 until size) {
			board(R)(r)(C)(c) = other.board(R)(r)(C)(c)
			domains(R)(r)(C)(c) = other.domains(R)(r)(C)(c).clone()

			val t = cell(R,r,C,c)
			if (!(other.unassignedCells contains t)) unassignedCells.remove(t)
		}
	}

	override def clone() = new Sudoku(this)

	// ===================================================================================

	def reset() {
		for (R <- 0 until size; r <- 0 until size; C <- 0 until size; c <- 0 until size) {
			board(R)(r)(C)(c) = 0
			domains(R)(r)(C)(c) = BitSet(1 to size*size :_*)
			unassignedCells.add(cell(R,r,C,c))
		}
	}

	def getCellValue(row:Int, col:Int) = {
		val t = index(row,col)
		board(t._1)(t._2)(t._3)(t._4);
	}

	def setCellValue(row:Int, col:Int, value:Int) {
		if (row < 0 || col < 0 || row >= size*size || col >= size*size)
			throw new IndexOutOfBoundsException("Row and column indices must be in range [0," + (size*size-1) + "].");

		if (!(unassignedCells contains ((row,col)) ))
			throw new IllegalAccessException("Cell (" + row + "," + col + ") already assigned. Cannot reassign." )

		if (value <= 0 || value > size*size)
			throw new IllegalArgumentException("Value must be in the range [1," + (size*size) + "].");

		val t = index(row,col)
		board(t._1)(t._2)(t._3)(t._4) = value

		unassignedCells -= ((row,col))

		for (i <- 0 until size; j <- 0 until size) {
			domains(t._1)(t._2)(i)(j) -= value
			domains(i)(j)(t._3)(t._4) -= value
			domains(t._1)(i)(t._3)(j) -= value
		}
	}

	def domainEliminate(row:Int, col:Int, value:Int) {
		if (value <= 0 || value > size*size)
			throw new IllegalArgumentException("Value must be in the range [1," + (size*size) + "].");

		val t = index(row,col)
		domains(t._1)(t._2)(t._3)(t._4) -= value
	}

	def domain(row:Int, col:Int) = {
		val t = index(row,col)
		domains(t._1)(t._2)(t._3)(t._4).toSet
	}

	def nextCellToAssign() = unassignedCells.head

	def neighborCells(row:Int, col:Int) = {
		val t = index(row,col) // (R,r,C,c)

		// Cells in same row: (R,r,_,_)
		val s1 = Set({for(i <- 0 until size; j <- 0 until size) yield cell(t._1,t._2,i,j)} :_*);

		// Cells in same column: (_,_,C,c)
		val s2 = Set({for(i <- 0 until size; j <- 0 until size) yield cell(i,j,t._3,t._4)} :_*);

		// Cells in same box: (R,_,C,_)
		val s3 = Set({for(i <- 0 until size; j <- 0 until size) yield cell(t._1,i,t._3,j)} :_*);

		(s1 | s2 | s3)-((row,col))
	}

	private def isValid:Boolean = {
		for (i <- 0 until size; j <- 0 until size) {
			val row = {for (C <- 0 until size; c <- 0 until size) yield board(i)(j)(C)(c)}.filter(_ > 0)
			if (row.size > row.toSet.size) return false

			val col = {for (R <- 0 until size; r <- 0 until size) yield board(R)(r)(i)(j)}.filter(_ > 0)
			if (col.size > col.toSet.size) return false

			val box = {for (r <- 0 until size; c <- 0 until size) yield board(i)(r)(j)(c)}.filter(_ > 0)
			if (box.size > box.toSet.size) return false
		}

		return true
	}

	def isFeasible = {for(R <- 0 until size; r <- 0 until size; C <- 0 until size; c <- 0 until size) yield domains(R)(r)(C)(c)}.forall(_.nonEmpty) && isValid

	def isComplete = unassignedCells.isEmpty
}
