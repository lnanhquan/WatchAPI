namespace WatchAPI.Models.Base;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public int Version { get; protected set; } = 0;

    protected void IncreaseVersion()
    {
        Version++;
    }
}
