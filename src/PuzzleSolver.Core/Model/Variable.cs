namespace PuzzleSolver.Core.Model;

using System;
using System.Collections.Generic;

public class Variable<T>(string name, HashSet<T> domain)
    where T : struct
{
    public string Name { get; } = name;

    public HashSet<T> Domain { get; private set; } = [.. domain];

    // Domain Reduction
    public void DomainReduce(Predicate<T> isFeasible) => this.Domain.RemoveWhere(v => !isFeasible(v));

    public T? Value { 
        get; 
        set
        {
            if (value == null)
            {
                Domain = domain;
            } 
            else if (domain.Contains((T)value))
            {
                Domain = new HashSet<T>([(T)value]);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            field = value;
        }
    }

    public bool IsAssigned() => this.Value.HasValue;

    public bool Matches(Variable<T> other) => this.Name == other.Name;

    public Variable<T> Clone()
    {
        Variable<T> other = new Variable<T>(this.Name, domain);
        if (this.IsAssigned())
        {
            other.Value = this.Value;
        }
        else
        {
            other.Domain.IntersectWith(this.Domain);
        }
        return other;
    }
}
