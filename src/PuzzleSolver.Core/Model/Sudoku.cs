namespace PuzzleSolver.Core.Model;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public class Sudoku(int n = 3)
{
    #region Internal State

    /// <summary>
    /// Dimension of Sudoku board, where the board is an N×N arrangement of blocks, each block being an N×N arrangement of cells 
    /// </summary>
    public int N { get; } = n;

    /// <summary>
    /// The state of the value of the board cells
    /// </summary>
    private readonly int?[] _board = new int?[n*n*n*n];

    /// <summary>
    /// The state of the domains of the board cells
    /// </summary>
    private readonly HashSet<int>[] _domains = Enumerable.Range(1, n*n*n*n).Select(_ => Enumerable.Range(1, n*n).ToHashSet()).ToArray();

    #endregion

    #region Private Helper Methods

    // Converters between 1D and 2D cell indices
    private int Index(int row, int col) => row * N * N + col;
    private (int row, int col) Coord(int index) => (index / (N * N), index % (N * N));

    #endregion

    #region Public Members

    /// <summary>
    /// Enumerates the coordinates of the unassigned cells on the board
    /// </summary>
    public IEnumerable<(int row, int col)> Unassigned => _board.Select((cell,index) => (cell,index)).Where(t => !t.cell.HasValue).Select(t => Coord(t.index));  

    /// <summary>
    /// Gets or sets the value of a cell in the board at the specified row and column indices.
    /// </summary>
    /// <remarks>When setting a value, the specified cell must be unassigned, and the value must be within the
    /// valid range. Setting a value will also update the internal state of the board, including the domains of
    /// neighboring cells.</remarks>
    /// <param name="row">The zero-based row index of the cell. Must be in the range [0, <c>N * N</c>).</param>
    /// <param name="col">The zero-based column index of the cell. Must be in the range [0, <c>N * N</c>).</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="row"/> or <paramref name="col"/> are outside the valid board coordinates.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the cell at the specified row and column is already assigned</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the value being set not in the valid range [1, <c>N * N</c>].</exception>
    public int? this[int row, int col] 
    {
        get => _board[Index(row,col)];
        set { 
            if (row < 0 || col < 0 || row >= N*N || col >= N*N) throw new IndexOutOfRangeException($"Row and column indices must be in range [0,{ N * N })");
            if (!Unassigned.Contains((row, col))) throw new InvalidOperationException($"Cell ({row},{col}) already assigned. Cannot reassign");
            if (value == null || value <= 0 || value > N * N) throw new ArgumentOutOfRangeException($"Value must be in the range of [1, {N * N}]");

            _board[Index(row,col)] = value;
            _domains[Index(row, col)].RemoveWhere(_ => _ != value);

            // TODO: Eliminate value from domains of neighbors
        }
    }

    #endregion
}
