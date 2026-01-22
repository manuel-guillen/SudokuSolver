namespace PuzzleSolver.Core.Model.Constraints;

using System.Collections.Generic;
using System.Linq;

public class Distinct<T>(IEnumerable<Variable<T>> variables) : Constraint<T> where T : struct
{
    public override IEnumerable<Variable<T>> Variables { get; } = variables.ToList();

    public override bool IsViolated()
    {
        HashSet<T> seen = [];
        return this.Variables.Where(v => v.IsAssigned()).Select(v => (T)v.Value!).Any(v => !seen.Add(v));
    }

    public override void PropagateIn(Variable<T> assignedVar)
    {
        foreach(Variable<T> var in this.Variables.Where(v => !v.Matches(assignedVar)))
        {
            var.DomainReduce(val => !val.Equals(assignedVar.Value));
        }
    }
}
