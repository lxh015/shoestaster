namespace St.Specification
{
    public interface ISpecificationParser<TCriteria>
    {
        TCriteria Parse<T>(ISpecification<T> specification);
    }
}
