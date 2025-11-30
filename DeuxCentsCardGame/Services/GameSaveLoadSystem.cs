using DeuxCentsCardGame.Models;

namespace DeuxCentsCardGame.Services
{
    /// Handles saving and loading game snapshots to/from disk.
    /// Supports both async (for Unity) and sync operations.
    public class GameSaveLoadSystem : IGameSaveLoadSystem
    {
        private readonly string _saveDirectory;
        private const string QUICK_SAVE_NAME = "quicksave";
        private const string SAVE_EXTENSION = ".deuxsave";

        public GameSaveLoadSystem(string? customSaveDirectory = null)
        {
            // Use custom directory or default to AppData
            _saveDirectory = customSaveDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DeuxCentsCardGame",
                "Saves"
            );

            // Ensure save directory exists
            Directory.CreateDirectory(_saveDirectory);
        }

        #region Async Operations (Unity-Friendly)

        /// Saves a game snapshot asynchronously.
        public async Task<bool> SaveGameAsync(GameSnapshot snapshot, string saveName)
        {
            if (snapshot == null || string.IsNullOrWhiteSpace(saveName))
                return false;

            try
            {
                string filePath = GetSaveFilePath(saveName);
                byte[] data = snapshot.Serialize();

                await File.WriteAllBytesAsync(filePath, data);

                // Also save metadata file
                await SaveMetadataAsync(snapshot, saveName);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }

        /// Loads a game snapshot asynchronously.
        public async Task<GameSnapshot?> LoadGameAsync(string saveName)
        {
            if (string.IsNullOrWhiteSpace(saveName))
                return null;

            try
            {
                string filePath = GetSaveFilePath(saveName);

                if (!File.Exists(filePath))
                    return null;

                byte[] data = await File.ReadAllBytesAsync(filePath);
                return GameSnapshot.Deserialize(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
                return null;
            }
        }

        /// Deletes a save file asynchronously.
        public async Task<bool> DeleteSaveAsync(string saveName)
        {
            if (string.IsNullOrWhiteSpace(saveName))
                return false;

            try
            {
                string filePath = GetSaveFilePath(saveName);
                string metadataPath = GetMetadataFilePath(saveName);

                await Task.Run(() =>
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    if (File.Exists(metadataPath))
                        File.Delete(metadataPath);
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting save: {ex.Message}");
                return false;
            }
        }

        /// Gets information about all saved games.
        public async Task<List<SaveFileInfo>> GetAllSavesAsync()
        {
            try
            {
                var saveFiles = Directory.GetFiles(_saveDirectory, $"*{SAVE_EXTENSION}");
                var saveInfoList = new List<SaveFileInfo>();

                foreach (var filePath in saveFiles)
                {
                    var metadataPath = GetMetadataFilePath(Path.GetFileNameWithoutExtension(filePath));
                    
                    if (File.Exists(metadataPath))
                    {
                        var json = await File.ReadAllTextAsync(metadataPath);
                        var metadata = System.Text.Json.JsonSerializer.Deserialize<SaveFileMetadata>(json);
                        
                        if (metadata != null)
                        {
                            saveInfoList.Add(new SaveFileInfo
                            {
                                SaveName = Path.GetFileNameWithoutExtension(filePath),
                                DisplayName = metadata.DisplayName,
                                Timestamp = metadata.Timestamp,
                                RoundNumber = metadata.RoundNumber,
                                GameState = metadata.GameState,
                                TeamOneScore = metadata.TeamOneScore,
                                TeamTwoScore = metadata.TeamTwoScore,
                                FileSizeBytes = new FileInfo(filePath).Length
                            });
                        }
                    }
                }

                return saveInfoList.OrderByDescending(s => s.Timestamp).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting saves: {ex.Message}");
                return new List<SaveFileInfo>();
            }
        }

        /// Quick save to a predefined slot.
        public async Task<bool> QuickSaveAsync(GameSnapshot snapshot)
        {
            return await SaveGameAsync(snapshot, QUICK_SAVE_NAME);
        }

        /// Quick load from the predefined slot.
        public async Task<GameSnapshot?> QuickLoadAsync()
        {
            return await LoadGameAsync(QUICK_SAVE_NAME);
        }

        #endregion

        #region Synchronous Operations

        /// Saves a game snapshot synchronously.
        public bool SaveGameSync(GameSnapshot snapshot, string saveName)
        {
            if (snapshot == null || string.IsNullOrWhiteSpace(saveName))
                return false;

            try
            {
                string filePath = GetSaveFilePath(saveName);
                byte[] data = snapshot.Serialize();

                File.WriteAllBytes(filePath, data);

                // Also save metadata file
                SaveMetadataSync(snapshot, saveName);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving game: {ex.Message}");
                return false;
            }
        }

        /// Loads a game snapshot synchronously.
        public GameSnapshot? LoadGameSync(string saveName)
        {
            if (string.IsNullOrWhiteSpace(saveName))
                return null;

            try
            {
                string filePath = GetSaveFilePath(saveName);

                if (!File.Exists(filePath))
                    return null;

                byte[] data = File.ReadAllBytes(filePath);
                return GameSnapshot.Deserialize(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Helper Methods

        private string GetSaveFilePath(string saveName)
        {
            return Path.Combine(_saveDirectory, $"{saveName}{SAVE_EXTENSION}");
        }

        private string GetMetadataFilePath(string saveName)
        {
            return Path.Combine(_saveDirectory, $"{saveName}.meta");
        }

        private async Task SaveMetadataAsync(GameSnapshot snapshot, string saveName)
        {
            var metadata = new SaveFileMetadata
            {
                DisplayName = snapshot.Description ?? saveName,
                Timestamp = snapshot.Timestamp,
                RoundNumber = snapshot.RoundNumber,
                GameState = snapshot.CurrentState.ToString(),
                TeamOneScore = snapshot.Scores.TeamOneTotalPoints,
                TeamTwoScore = snapshot.Scores.TeamTwoTotalPoints
            };

            string metadataPath = GetMetadataFilePath(saveName);
            string json = System.Text.Json.JsonSerializer.Serialize(metadata, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(metadataPath, json);
        }

        private void SaveMetadataSync(GameSnapshot snapshot, string saveName)
        {
            var metadata = new SaveFileMetadata
            {
                DisplayName = snapshot.Description ?? saveName,
                Timestamp = snapshot.Timestamp,
                RoundNumber = snapshot.RoundNumber,
                GameState = snapshot.CurrentState.ToString(),
                TeamOneScore = snapshot.Scores.TeamOneTotalPoints,
                TeamTwoScore = snapshot.Scores.TeamTwoTotalPoints
            };

            string metadataPath = GetMetadataFilePath(saveName);
            string json = System.Text.Json.JsonSerializer.Serialize(metadata, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(metadataPath, json);
        }

        #endregion
    }

    /// Metadata stored alongside save files for quick display.
    [Serializable]
    public class SaveFileMetadata
    {
        public string DisplayName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int RoundNumber { get; set; }
        public string GameState { get; set; } = string.Empty;
        public int TeamOneScore { get; set; }
        public int TeamTwoScore { get; set; }
    }

    /// Information about a save file for UI display.
    public class SaveFileInfo
    {
        public string SaveName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int RoundNumber { get; set; }
        public string GameState { get; set; } = string.Empty;
        public int TeamOneScore { get; set; }
        public int TeamTwoScore { get; set; }
        public long FileSizeBytes { get; set; }

        public string FormattedTimestamp => Timestamp.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
        public string FileSizeKB => $"{FileSizeBytes / 1024.0:F2} KB";
    }
}