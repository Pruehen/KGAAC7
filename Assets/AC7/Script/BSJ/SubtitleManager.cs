using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ����:
/// ���� Subtitle�Ŵ����� ��ġ�ϰ� Text���� �Ҵ���
/// ����������� �ڸ��� Ű�� �Ѱ��ָ� ���� �Ѱ��ش�
/// </summary>
public class SubtitleManager : SceneSingleton<SubtitleManager>
{
    [SerializeField] private TextAsset _subtitleCsvAsset;
    private Dictionary<string, string> _subtitleDictionary = new Dictionary<string, string>();
    private void Awake()
    {
        Init();
        foreach(string text in _subtitleDictionary.Keys )
        {
            Debug.Log( "Key :" + text + "    Var :" + _subtitleDictionary[text]);
        }
    }

    private void Init()
    {
        string[,] result = CSVReader.SplitCsvGrid(_subtitleCsvAsset.text);
        for (int i = 0; i < result.GetLength(1); i++)
        {
            if (result[0, i] != null && result[1, i] != null)
            {
                _subtitleDictionary.Add(result[0, i], result[1, i]);
            }
                
        }
    }

    /// <summary>
    /// Ű�� �޾� �ڸ��� ��ȯ�ϴ� �޼���
    /// 
    /// Hit1,�溸! ���� ������ �ǰݴ��ߴ�!
    ///Hit2,"�ǰ� ���� �ɰ�, ��� ���� ��û�Ѵ�!"
    ///MissileLaunch,���� 2
    ///Kill1,��ǥ ����! ���� ��ǥ�� �̵�!
    ///Kill2,"���� ���� Ȯ��, ���� ������!"
    ///Kill3,��Ȯ�� Ÿ��! ��ǥ �ı� �Ϸ�!
    ///StartMission1,"�ӹ� ����, ��� �����ϰ� ��������!"
    ///StartMission2,"���� �غ� �Ϸ�, ����� ���ϴ�!"
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetSubtitle(string key)
    {
        return _subtitleDictionary[key];
    }


}
