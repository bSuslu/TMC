namespace TMC._Project.Core.Systems.SaveSystem.Interfaces
{
    public interface ISaveService
    {
        bool Save<T>(string relativePath,T data, bool overwrite = false, bool encrypted = false);
        bool TryLoad<T>(string relativePath, out T data, bool encrypted = false);
        T Load<T>(string relativePath, bool encrypted = false);
        void Delete(string relativePath);
        void DeleteAll();
    }
} 