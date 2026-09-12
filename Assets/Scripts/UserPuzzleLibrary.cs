using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UserPuzzleLibrary
{
    public static List<string> LoadPuzzles(string saveKey)
    {
        string json =  PlayerPrefs.GetString(saveKey, "");

        if (string.IsNullOrEmpty(json))
            return new List<string>();

        return JsonUtility
            .FromJson<UserPuzzleCollection>(json)
            .puzzles;
    }

    public static void SavePuzzles(string saveKey, List<string> puzzles)
    {
        UserPuzzleCollection collection = new UserPuzzleCollection();

        collection.puzzles = puzzles;

        string json = JsonUtility.ToJson(collection);

        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }
}