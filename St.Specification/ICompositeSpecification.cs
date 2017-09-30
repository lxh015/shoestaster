namespace St.Specification
{
    /// <summary>
    /// 实现类复合规范。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICompositeSpecification<T> : ISpecification<T>
    {
        ISpecification<T> Left { get; }

        ISpecification<T> Right { get; }
    }
}