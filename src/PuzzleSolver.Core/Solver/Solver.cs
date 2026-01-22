namespace PuzzleSolver.Core.Solver;

using System;
using System.Collections.Generic;
using PuzzleSolver.Core.Model;

/// <summary>
/// Static factory class for creating Solver instances with type inference.
/// </summary>
public static class Solver
{
    /// <summary>
    /// Creates a new Solver instance with automatic type inference from the provided puzzle.
    /// This method uses reflection to determine the TValue type from the puzzle's base class.
    /// </summary>
    /// <typeparam name="TPuzzle">The puzzle type that inherits from BasePuzzle</typeparam>
    /// <param name="puzzle">The puzzle instance to solve</param>
    /// <returns>A new Solver instance configured for the given puzzle</returns>
    public static ISolver<TPuzzle> CreateInstance<TPuzzle>(TPuzzle puzzle)
        where TPuzzle : class
    {
        // Get the base type that implements BasePuzzle<TValue, TPuzzle>
        var baseType = typeof(TPuzzle);
        while (baseType != null && !baseType.IsGenericType)
        {
            baseType = baseType.BaseType;
        }
        
        if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name.StartsWith("BasePuzzle"))
        {
            var genericArgs = baseType.GetGenericArguments();
            if (genericArgs.Length == 2)
            {
                var valueType = genericArgs[0]; // TValue
                var puzzleType = genericArgs[1]; // TPuzzle
                
                // Create the generic Solver type
                var solverType = typeof(DFSSolver<,>).MakeGenericType(puzzleType, valueType);
                
                // Create an instance using the internal constructor
                return (ISolver<TPuzzle>)Activator.CreateInstance(solverType, 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, 
                    null, 
                    [puzzle],
                    null)!;
            }
        }
        
        throw new InvalidOperationException($"Unable to determine generic types for puzzle type {typeof(TPuzzle)}");
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
