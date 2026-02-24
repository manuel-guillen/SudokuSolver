namespace PuzzleSolver.Core.Model;

using System;
using System.Collections.Generic;
using System.Linq;

public abstract class BasePuzzle<TValue, TSelf> 
    where TValue : struct 
    where TSelf : BasePuzzle<TValue,TSelf>
{    
    public abstract IEnumerable<Variable<TValue>> Variables { get; }
    
    public abstract IEnumerable<Constraint<TValue>> Constraints { get; }
    
    public abstract TSelf Clone();
    
    public abstract override string ToString();

    public TSelf WithVariableAssigned(Variable<TValue> variable, TValue value)
    {
        TSelf clone = this.Clone();
        clone[variable] = value;
        return clone;
    }
    
    public Variable<TValue> GetNextVariableToAssign() => this.Variables.Where(v => !v.IsAssigned()).MinBy(v => v.Domain.Count) ?? throw new InvalidOperationException("Method cannot be called in the absence of unassigned variables");

    /// <summary>
    /// Determines whether the current puzzle state is not violating any constraints.
    /// </summary>
    /// <returns>False if the puzzle state is violating at least one of the puzzle's constraints; otherwise, <see langword="true"/>.</returns>
    public bool IsValid() => !this.Constraints.Any(c => c.IsViolated());
    
    /// <summary>
    /// Determines whether the current puzzle state is feasible for solving, meaning there are no variables that
    /// no longer have feasible values in their domain and there are no constraints being violated
    /// </summary>
    /// <returns>True if all variables have non-empty domains and no constraints are violated; otherwise, false.</returns>
    public bool IsFeasible() => this.Variables.All(v => v.Domain.Count != 0) && IsValid();

    /// <summary>
    /// Determines whether all variables in the collection have been assigned values.
    /// </summary>
    /// <returns>True if every variable is assigned; otherwise, false.</returns>
    public bool IsComplete() => this.Variables.All(v => v.IsAssigned());
    
    public TValue? this[Variable<TValue> var]
    {
        get => this.Variables.FirstOrDefault(v => v.Matches(var))?.Value;
        set {
            Variable<TValue>? variable = this.Variables.FirstOrDefault(v => v.Matches(var));
            if (variable != null)
            {
                variable.Value = value;
                PerformConstraintPropagation(variable);
            }
        } 
    }

    private void PerformConstraintPropagation(Variable<TValue> assignedVariable)
    {
        foreach (var constraint in this.Constraints.Where(c => c.Contains(assignedVariable)))
        {
            constraint.PropagateIn(assignedVariable);
        }
    }
}