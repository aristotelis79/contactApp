namespace ContactApp.Data.Entities
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract partial class BaseEntity<T> where T : struct
    {
        public T Id { get; set; }
    }
}
