namespace Core.Behaviour.ObjectPool
{
    public enum ObjectPoolOverflowHandlingMode
    {
        CreateInstances,
        ExpandPool,
        RefillPool,
        ReuseExisting
    }
}