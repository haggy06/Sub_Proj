using UnityEngine;

using System.IO;

public enum DataType
{
    SettingData,
    PlayData,
    EctData,

}
public static class FileSaveLoader<T> where T : class
{
    public static void SaveData(string fileName, T nData)
    {
        Debug.Log(fileName + " ������ �����մϴ�.");

        File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), JsonUtility.ToJson(nData));
    }

    public static bool TryLoadData(string fileName, out T data)
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName))) // ��ο� ������ ������ ���
        {
            Debug.Log("����� " + fileName + " ������ �������� �� �����߽��ϴ�.");

            data = JsonUtility.FromJson<T>(File.ReadAllText(Path.Combine(Application.persistentDataPath, fileName)));
            return true;
        }
        else // ��ο� ������ �������� ���� ���
        {
            Debug.Log("����� " + fileName + " ������ �������� �ʽ��ϴ�.");

            data = null;
            return false;
        }
    }

    public static void DelectData(string fileName)
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName.ToString()))) // ��ο� ������ ������ ���
        {
            File.Delete(Path.Combine(Application.persistentDataPath, fileName.ToString()));

            Debug.Log(fileName + " ������ �����߽��ϴ�.");
        }
        else // ��ο� ������ �������� ���� ���
        {
            Debug.Log(fileName + " ������ �������� �ʽ��ϴ�.");
        }
    }
}
