using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSettingsProcess
{
    //A static class that manually saves all of the settings data

    public static void SaveSettings(SettingsData data)
    {
        //Establishes the basics of the save system (formatter, path, and file stream)
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        //Creates a new SavedSettings class to serialize and saves it in a file
        SavedSettings dataToSave = new SavedSettings(data);

        formatter.Serialize(stream, dataToSave);
        stream.Close();
    }

    public static SavedSettings LoadSettings()
    {
        //Establishes the file path
        string path = Application.persistentDataPath + "/settings.data";

        if(File.Exists(path))
        {
            //If a file exists, find its SavedSettings class and return it
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SavedSettings newData = formatter.Deserialize(stream) as SavedSettings;

            stream.Close();

            return newData;
        } else
        {
            //If no file exists, return nothing
            return null;
        }
    }
}
