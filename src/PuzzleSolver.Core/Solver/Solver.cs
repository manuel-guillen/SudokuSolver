namespace PuzzleSolver.Core.Solver;

using System;
using System.Collections.Generic;
using System.Reflection;
using PuzzleSolver.Core.Model;

/// <summary>
/// Static factory class for creating Solver instances with type inference.
/// </summary>
public static class Solver
{
    /// <summary>
    /// Creates a new ISolver instance with automatic type inference from the provided puzzle.
    /// This method uses reflection to determine the TValue type from the puzzle's base class.
    /// </summary>
    /// <typeparam name="TPuzzle">The puzzle type that inherits from BasePuzzle</typeparam>
    /// <param name="puzzle">The puzzle instance to solve</param>
    /// <returns>A new Solver instance configured for the given puzzle</returns>
    public static ISolver<TPuzzle> CreateInstance<TPuzzle>(TPuzzle puzzle)
        where TPuzzle : class
    {
        Type? baseType = typeof(TPuzzle);
        while (baseType?.IsGenericType is false)
        {
            baseType = baseType.BaseType;
        }
        if (baseType is { Name: string name } && name.StartsWith(nameof(BasePuzzle<,>)) && baseType.GetGenericArguments() is [Type valueType, Type puzzleType])
        {
            Type solverType = typeof(DFSSolver<,>).MakeGenericType(puzzleType, valueType);
            return (ISolver<TPuzzle>)Activator.CreateInstance(solverType,
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                [puzzle],
                null)!;
        }
        throw new InvalidOperationException($"{typeof(TPuzzle)} is not of the generic type BasePuzzle");
    }

    private class DFSSolver<TPuzzle, TValue> : ISolver<TPuzzle>
        where TValue : struct
        where TPuzzle : BasePuzzle<TValue, TPuzzle>
    {
        public TPuzzle Puzzle { get; private init; }

        private TPuzzle? _solution = null;

        private readonly Stack<TPuzzle> _stack;

        internal DFSSolver(TPuzzle puzzle)
        {
            this.Puzzle = puzzle;
            _stack = new Stack<TPuzzle>([puzzle]);
        }

        public bool TrySolve(out TPuzzle? solution)
        {
            // Check cached solution / if has already been solved
            if (_solution is not null)
            {
                solution = _solution;
                return true;
            }

            while (_stack.TryPop(out TPuzzle? current))
            {
                if (current.IsFeasible())
                {
                    if (current.IsComplete())
                    {
                        _solution = current;
                        solution = _solution;
                        return true;
                    }

                    Variable<TValue> var = current.GetNextVariableToAssign();
                    foreach (TValue value in var.Domain)
                        _stack.Push(current.WithVariableAssigned(var, value));
                }
            }

            // If while loop exited (no return called within loop), no solution was found
            solution = null;
            return false;
        }
    }
}
