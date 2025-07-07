namespace DiscViewer.Commands;

public abstract class Command
{
    public abstract string Name { get; }
    public abstract string Usage { get; }
    public abstract string Description { get; }
    public abstract Task Execute(string[] args);
}