using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace VerticalSlice.Infra;

public class RedisServices : IServiceSetup
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<RedisConnection>();
    }
}

public class RedisConnection
{
    public class Config
    {
        public string ConnectionString { get; set; } = "localhost:6379";
    }

    private readonly Config _config; 
    private Lazy<ConnectionMultiplexer> _lazyConnection =>
        new (() => ConnectionMultiplexer.Connect(_config.ConnectionString+",abortConnect=false"));

    public RedisConnection(IOptions<Config> config)
    {
        _config = config.Value;
    }
    
    public bool IsConnected()
    {
        return _lazyConnection.Value.IsConnected;
    }
    
    public IDatabase Database => _lazyConnection.Value.GetDatabase();
}