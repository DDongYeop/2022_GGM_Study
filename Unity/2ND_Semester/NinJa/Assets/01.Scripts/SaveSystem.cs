using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public Vector2 PlayerPos;
}

public class SaveSystem : MonoBehaviour
{
    private SaveData saveData;
    private string savePath;
    private string saveFileName = "/SaveFIle.txt";
    private PlayerController player;

    private void Start() 
    {
        saveData = new SaveData();
        savePath = Application.dataPath + "/SaveData/";

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
    }

    [ContextMenu ("저장")]
    public void Save()
    {
        player = FindObjectOfType<PlayerController>();
        saveData.PlayerPos = player.transform.position;
        
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath+saveFileName, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    [ContextMenu ("로드")]
    public void Load()
    {
        if (File.Exists(savePath + saveFileName))
        {
            string json = File.ReadAllText(savePath+saveFileName);
            saveData = JsonUtility.FromJson<SaveData>(json);

            player = FindObjectOfType<PlayerController>();
            player.transform.position = saveData.PlayerPos;

            Debug.Log("로드 완료");
        }
        else
        {
            Debug.Log("저장 파일이 없습니다");
        }
    }
}
