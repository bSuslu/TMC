using Cysharp.Threading.Tasks;

namespace _Project.Core.Systems.SaveSystem.Interfaces
{
    public interface ISaveService
    {
        // Async methods (NEW - önerilen)
        UniTask<bool> SaveAsync<T>(string relativePath, T data, bool overwrite = true, bool encrypted = false);
        UniTask<(bool success, T data)> TryLoadAsync<T>(string relativePath, bool encrypted = false);
        UniTask<T> LoadAsync<T>(string relativePath, bool encrypted = false);
        UniTask DeleteAsync(string relativePath);
        UniTask DeleteAllAsync();
        UniTask<bool> ExistsAsync(string relativePath);
    }
} 