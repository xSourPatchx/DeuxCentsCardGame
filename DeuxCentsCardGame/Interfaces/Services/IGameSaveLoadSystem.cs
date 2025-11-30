using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    public interface IGameSaveLoadSystem
    {
        Task<bool> SaveGameAsync(GameSnapshot snapshot, string saveName);
        Task<GameSnapshot?> LoadGameAsync(string saveName);
        Task<bool> DeleteSaveAsync(string saveName);
        Task<List<SaveFileInfo>> GetAllSavesAsync();
        Task<bool> QuickSaveAsync(GameSnapshot snapshot);
        Task<GameSnapshot?> QuickLoadAsync();
        bool SaveGameSync(GameSnapshot snapshot, string saveName);
        GameSnapshot? LoadGameSync(string saveName);
    }
}