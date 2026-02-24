namespace PuzzleSolver.Core.Solver;

public interface ISolver<TPuzzle>
{
    public TPuzzle Puzzle { get; }

    public bool TrySolve(out TPuzzle? solution);
}
