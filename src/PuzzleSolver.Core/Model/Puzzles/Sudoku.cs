namespace PuzzleSolver.Core.Model.Puzzles;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PuzzleSolver.Core.Extensions;
using PuzzleSolver.Core.Model.Constraints;

public class Sudoku : BasePuzzle<int,Sudoku>
{
    private static IEnumerable<int> Range(int m) => Enumerable.Range(0, m);

    public int N { get; private set; }

    private Variable<int>[] _board;

    private List<Constraint<int>> _constraints;

    public override IEnumerable<Variable<int>> Variables => _board;

    public override IEnumerable<Constraint<int>> Constraints => _constraints;

    public Sudoku(int n = 3)
    {
        HashSet<int> domain = [..Enumerable.Range(1,n*n)];
        Initialize(n, Enumerable.Range(0, n*n*n*n).Select(i => new Variable<int>($"Sudoku-{n}:{Coord2(i)}", domain)));
    }

    private Sudoku(int n, IEnumerable<Variable<int>> vars) => Initialize(n, vars);

    [MemberNotNull(nameof(_board), nameof(_constraints))]
    private void Initialize(int n, IEnumerable<Variable<int>> vars)
    {
        this.N = n;
        this._board = [.. vars];
        this._constraints = [.. GenerateConstraints()];
    }
    public override Sudoku Clone() => new(this.N, this._board.Select(v => v.Clone()));

    private IEnumerable<Constraint<int>> GenerateConstraints()
    {
        // Generate row constraints
        for (int row = 0; row < N * N; row++)
            yield return new Distinct<int>(Range(N*N).Select(col => _board[Index(row, col)]));

        // Generate column constraints
        for (int col = 0; col < N * N; col++)
            yield return new Distinct<int>(Range(N*N).Select(row => _board[Index(row, col)]));

        // Generate box constraints
        for (int box = 0; box < N * N; box++)
            yield return new Distinct<int>(Range(N * N).Select(cell => _board[IndexFromBox(box, cell)]));
    }

    // array index -> (row, column)
    private (int row, int col) Coord2(int index) => (index / (N * N), index % (N * N));

    // (row, column) -> array index
    private int Index(int row, int col) => row * N * N + col;

    // (box, cell) -> array index
    private int IndexFromBox (int box, int cell)
    {
        (int r0, int r1, int c0, int c1) = (box / N, cell / N, box % N, cell % N);
        (int r, int c) = (r0 * N + r1, c0 * N + c1);
        return Index(r, c);
    }

    public override string ToString() => _board.Select(v => ToChar(v.Value))
                                               .Chunk(N).Select(chars => chars.ToJoinedString(" "))
                                               .Chunk(N).Select(strs => strs.ToJoinedString(" | "))
                                               .Chunk(N).Select(rows => rows.ToJoinedString("\n"))
                                               .ToJoinedString($"\n{Enumerable.Repeat(new string([.. Enumerable.Repeat('-', 2*N-1)]), N).ToJoinedString(" + ")}\n");

    private static char ToChar(int? v) => v switch
    {
        null => '.',
        > 0 and <= 9 => (char)('0' + (int)v),
        >= 10 => (char)('A' + ((int)v - 10)),
        _ => throw new ArgumentOutOfRangeException(nameof(v))
    };

    public static Sudoku Parse(string s, int n = 3)
    {
        var result = new Sudoku(n);
        IEnumerable<char> cells = s.Where(c => c == '.' || Char.IsLetterOrDigit(c) && c != '0').Take(n*n*n*n);
        foreach((Variable<int> var, char c) in result.Variables.Zip(cells))
        {
            if (Char.IsDigit(c))
                result[var] = c - '0';
            else if (Char.IsLetter(c))
                result[var] = 10 + c - (Char.IsUpper(c) ? 'A' : 'a');
        }
        return result;
    }
}
