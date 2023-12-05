public static class SaveDataManager
{
    public static void SaveJsonData(ISaveable saveable)
    {
#if !DEDICATED_SERVER
        if (FileManager.WriteToFile(saveable.FileNameToUseForData(), saveable.ToJson()))
        {
        }
#endif
    }

    public static void LoadJsonData(ISaveable saveable)
    {
#if !DEDICATED_SERVER
        if (FileManager.LoadFromFile(saveable.FileNameToUseForData(), out var json))
        {
            saveable.LoadFromJson(json);
        }
#endif
    }
}

public interface ISaveable
{
    string ToJson();
    void LoadFromJson(string a_Json);
    string FileNameToUseForData();
}