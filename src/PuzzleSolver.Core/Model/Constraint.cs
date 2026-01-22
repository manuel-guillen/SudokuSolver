namespace PuzzleSolver.Core.Model;

using System.Collections.Generic;
using System.Linq;

public abstract class Constraint<T> where T : struct
{
    public abstract IEnumerable<Variable<T>> Variables { get; }

    public bool Contains(Variable<T> var) => this.Variables.Any(v => v.Matches(var));

    public abstract bool IsViolated();

    public abstract void PropagateIn(Variable<T> assignedVar);

}
