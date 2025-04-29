namespace StorageHub.Infrastructure
{
    public class DatabaseSettings
    {
        public string Provider { get; set; } // Connection string to the database
        public SqliteSettings Sqlite { get; set; }     // Name of the database
        public SqlServerSettings SqlServer { get; set; }   // Name of the collection in the database
        public InMemorySettings InMemory { get; set; } // Name of the file collection in the database
    }
    public class SqliteSettings
    {
        public string ConnectionString { get; set; }   // Name of the collection in the database

    }

    public class SqlServerSettings
    {
        public string ConnectionString { get; set; }   // Name of the collection in the database

    }
    public class InMemorySettings
    {
        public string DatabaseName { get; set; }   // Name of the collection in the database

    }
}