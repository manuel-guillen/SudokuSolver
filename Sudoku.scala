import scala.io.Source

import scala.collection.mutable.BitSet
import scala.collection.mutable.HashSet

// ===================================================

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

	override def toString = 
		(for (R <- 0 until size) yield
			(for (r <- 0 until size) yield
				(for (C <- 0 until size) yield
					(for (c <- 0 until size) yield board(R)(r)(C)(c)).mkString(" ")
				).mkString(" | ")
			).mkString("\n")
		).mkString("\n" + Array.fill[String](size)("-" * (2*size-1)).mkString("-+-") + "\n").
		replaceAll("0",".")

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
		domains(t._1)(t._2)(t._3)(t._4) = BitSet(value)

		unassignedCells -= ((row,col))

		for (n <- neighborCells(row,col)) domainEliminate(n._1,n._2,value)
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

	def nextCellToAssign() = unassignedCells.minBy(x => {val t = index(x._1,x._2); domains(t._1)(t._2)(t._3)(t._4).size})

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

object Sudoku {
	def apply(arr: Array[Array[Int]]):Sudoku = {
		val size = scala.math.sqrt(arr.length).toInt
		var sudoku = new Sudoku(size)

		for (r <- arr.indices; c <- arr(r).indices if arr(r)(c) > 0) sudoku.setCellValue(r,c,arr(r)(c))

		return sudoku
	}

	def parseFile(filename: String) = Sudoku(Source.fromFile(filename).getLines.toArray.filterNot(_ contains "-").map(_.replaceAll("\\.","0").split(" ").filterNot(_ contains "|").map(_.toInt)))
}
