using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ganweisoft.IoTCenter.Module.IdentityServer.Models.Authorize;
using Ganweisoft.IoTCenter.Module.IdentityServer.Services.Store;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Ganweisoft.IoTCenter.Module.IdentityServer.ServicesImpl.Store;

public class DefaultAuthorizationCodeStore : IAuthorizationCodeStore
{
    /// <summary>
    /// The logger.
    /// </summary>
    protected ILogger<DefaultAuthorizationCodeStore> Logger { get; }

    /// <summary>
    /// The MemoryCache
    /// </summary>
    protected readonly IMemoryCache MemoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAuthorizationCodeStore"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="memoryCache"></param>
    /// <exception cref="System.ArgumentNullException">grantType</exception>
    public DefaultAuthorizationCodeStore(ILogger<DefaultAuthorizationCodeStore> logger, 
        IMemoryCache memoryCache)
    {
        Logger = logger;
        MemoryCache = memoryCache;
    }

    protected virtual string GetUniqueKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[16];
        rng.GetBytes(bytes);
        var id = BitConverter.ToString(bytes).Replace("-", "");
        
        return id;
    }

    /// <summary>
    /// Gets the hashed key.
    /// </summary>
    /// <param name="input">The value.</param>
    /// <returns></returns>
    protected virtual string GetHashKey(string input)
    {
        if (input.IsNullOrEmpty()) return string.Empty;

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    protected virtual async Task<AuthorizationCode> GetItemAsync(string key)
    {
        var hashedKey = GetHashKey(key);

        await Task.CompletedTask;
        
        var grant = MemoryCache.Get<AuthorizationCode>(hashedKey);
        if (grant != null)
        {
            return grant;
        }

        Logger.LogDebug("grant with value: {key} not found in store.", key);

        return default(AuthorizationCode);
    }

    /// <summary>
    /// Creates the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected virtual async Task<string> CreateItemAsync(AuthorizationCode item)
    {
        var handle = GetUniqueKey();
        await StoreItemAsync(handle, item);
        return handle;
    }

    /// <summary>
    /// Stores the item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    protected virtual async Task StoreItemAsync(string key, AuthorizationCode item)
    {
        key = GetHashKey(key);

        MemoryCache.Set(key, item, TimeSpan.FromSeconds(item.Lifetime));
            
        await Task.CompletedTask;
    }

    /// <summary>
    /// Removes the item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    protected virtual async Task RemoveItemAsync(string key)
    {
        key = GetHashKey(key);
        
        MemoryCache.Remove(key);
            
        await Task.CompletedTask;
    }
    
    /// <summary>
    /// Stores the authorization code asynchronous.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns></returns>
    public async Task<string> StoreAuthorizationCodeAsync(AuthorizationCode code)
    {
        return await CreateItemAsync(code);
    }

    /// <summary>
    /// Gets the authorization code asynchronous.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns></returns>
    public Task<AuthorizationCode> GetAuthorizationCodeAsync(string code)
    {
        return GetItemAsync(code);
    }

    /// <summary>
    /// Removes the authorization code asynchronous.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns></returns>
    public Task RemoveAuthorizationCodeAsync(string code)
    {
        return RemoveItemAsync(code);
    }
}