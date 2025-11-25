using Cysharp.Threading.Tasks;

namespace _Project.Core.Systems.LoadingSystem.Interfaces
{
    public interface IAsyncService
    {
        UniTask InitializeAsync();
    }
}