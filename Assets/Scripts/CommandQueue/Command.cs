using System;

public class Command : IComparable<Command>
{
    public float delay;
    public string message;
    public int priority;
    public bool noFurtherExecution = false;
    public Action<Action> action;

    public int CompareTo(Command c)
    {
        return c.priority.CompareTo(this.priority);
    }
}
