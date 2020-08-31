namespace ZenVortex
{
    internal interface IBindable<in TData>
    {
        void UpdateData(TData data);
    }
}