namespace Core.Behaviour.ObjectPool
{
    public enum ObjectPoolHandlingMode
    {
        CreateInstances,
        ExpandPool,
        RefillPool,
        ReuseExisting
    }
}