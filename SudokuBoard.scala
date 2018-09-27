import scala.collection.mutable.BitSet

class SudokuBoard(val size: Int) {
	private var board = Array.ofDim[Int](size,size,size,size)
	private var domains = Array.ofDim[BitSet](size,size,size,size)

	for (i <- 0 until size; j <- 0 until size; k <- 0 until size; l <- 0 until size)
		domains(i)(j)(k)(l) = BitSet(1 to size*size :_*)

	def this() {
		this(3)
	}


}