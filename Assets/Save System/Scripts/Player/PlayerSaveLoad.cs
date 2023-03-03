using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class PlayerSaveLoad
{
    public static void SaveData(Vector3 position, int scene, int saveNum, bool hasGrappler)
    {
        //Establishes the base of the file saving (formatter, file position, and usable stream)
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data" + saveNum.ToString();
        FileStream stream = new FileStream(path, FileMode.Create);

        //Creates a PlayerData class with the info fed into this function, and saves it in a file in the path
        PlayerData data = new PlayerData(position, scene, System.DateTime.Now.ToString(), hasGrappler);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer(int num)
    {
        //Establishes a path based on the number fed into this
        string path = Application.persistentDataPath + "/player.data" + num.ToString();
        
        if(File.Exists(path))
        {
            //If a file exists here, find the PlayerData class and return it
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        } else
        {
            //If no file exists, return nothing
            return null;
        }
    }
}
