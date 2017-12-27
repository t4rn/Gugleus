namespace Gugleus.Core.Repositories
{
    public abstract class AbstractRepository
    {
        protected readonly string _connStr;

        public AbstractRepository(string connectionString)
        {
            _connStr = connectionString;
        }
    }
}
