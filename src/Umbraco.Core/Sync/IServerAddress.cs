using Umbraco.Core.Models.EntityBase;

namespace Umbraco.Core.Sync
{
    /// <summary>
    /// An interface exposing a server address to use for server syncing
    /// </summary>
    internal interface IServerAddress
    {
        string ServerAddress { get; }
    }
}