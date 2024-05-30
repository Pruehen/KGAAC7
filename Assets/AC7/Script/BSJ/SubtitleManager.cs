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
    //[SerializeField] private TextAsset _subtitleCsvAsset;
    //private Dictionary<string, string> _subtitleDictionary = new Dictionary<string, string>();
    //[SerializeField] private TMPro.TMP_Text _text;

    ////��� ���ӽð� ���� _baseDuration + text.length * addtionalTimeRatio
    ////��                        2        +  10 * .15f  10���ڸ� 3.5��
    //private float _baseDuration = 1f;//�⺻ ���ӽð�
    //private float _addtionalDurationRatio = .05f;
    //private float _duration;

    ////�׽�Ʈ�� �ڵ�
    //[SerializeField] private bool _test;
    //[SerializeField] private string _testKey;

    //private void Awake()
    //{
    //    Init();
    //    foreach(string text in _subtitleDictionary.Keys )
    //    {
    //        Debug.Log( "Key :" + text + "    Var :" + _subtitleDictionary[text]);
    //    }
    //}

    //private void Init()
    //{
    //    string[,] result = CSVReader.SplitCsvGrid(_subtitleCsvAsset.text);
    //    for (int i = 0; i < result.GetLength(1); i++)
    //    {
    //        if (result[0, i] != null && result[1, i] != null)
    //        {
    //            _subtitleDictionary.Add(result[0, i], result[1, i]);
    //        }
                
    //    }
    //}

    //private void Update()
    //{
    //    if(_duration > 0f)
    //    {
    //        _duration -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        _text.enabled = false;
    //    }

    //    if(_test == true)
    //    {
    //        ShowSubtitle(_testKey);
    //        _test = false;
    //    }
    //}

    ///// <summary>
    ///// Ű�� �޾� �ڸ��� ����ϴ� �޼���
    ///// 
    ///*
    //    ID,English,Original
    //    Hit1,"Alert! The right engine has been hit!","�溸! ���� ������ �ǰݴ��ߴ�!"
    //    Hit2,"The damage is severe, request immediate support!","�ǰ� ���� �ɰ�, ��� ���� ��û�Ѵ�!"
    //    MissileLaunch,"Fox 2","���� 2"
    //    Kill1,"Target hit! Moving to the next target!","��ǥ ����! ���� ��ǥ�� �̵�!"
    //    Kill2,"Enemy down confirmed, operation successful!","���� ���� Ȯ��, ���� ������!"
    //    Kill3,"Accurate strike! Target destroyed!","��Ȯ�� Ÿ��! ��ǥ �ı� �Ϸ�!"
    //    StartMission1,"Mission start, everyone stay focused and be careful!","�ӹ� ����, ��� �����ϰ� ��������!"
    //    StartMission2,"Combat ready, good luck!","���� �غ� �Ϸ�, ����� ���ϴ�!"
    // */
    ///// 
    ///// </summary>
    ///// <param name="key"></param>
    //public void ShowSubtitle(string key)
    //{
    //    Debug.Assert(_subtitleDictionary.ContainsKey(key), "Wrong subtitle key");
    //    _text.text = _subtitleDictionary[key];

    //    _duration = _baseDuration + _text.text.Length * _addtionalDurationRatio;

    //    _text.enabled = true;
    //}
    ///// <summary>
    ///// Ű�� �޾� ������ �ð����� �ڸ��� ����ϴ� �޼���
    ///// 
    ///*
    //    ID,English,Original
    //    Hit1,"Alert! The right engine has been hit!","�溸! ���� ������ �ǰݴ��ߴ�!"
    //    Hit2,"The damage is severe, request immediate support!","�ǰ� ���� �ɰ�, ��� ���� ��û�Ѵ�!"
    //    MissileLaunch,"Fox 2","���� 2"
    //    Kill1,"Target hit! Moving to the next target!","��ǥ ����! ���� ��ǥ�� �̵�!"
    //    Kill2,"Enemy down confirmed, operation successful!","���� ���� Ȯ��, ���� ������!"
    //    Kill3,"Accurate strike! Target destroyed!","��Ȯ�� Ÿ��! ��ǥ �ı� �Ϸ�!"
    //    StartMission1,"Mission start, everyone stay focused and be careful!","�ӹ� ����, ��� �����ϰ� ��������!"
    //    StartMission2,"Combat ready, good luck!","���� �غ� �Ϸ�, ����� ���ϴ�!"
    // */
    ///// 
    ///// </summary>
    ///// <param name="key">Ű</param>
    ///// <param name="time">���ӽð�</param>
    //public void ShowSubtitleWithTime(string key, float time)
    //{
    //    Debug.Assert(_subtitleDictionary.ContainsKey(key), "Wrong subtitle key");
    //    _text.text = $"<< {_subtitleDictionary[key]} >>";

    //    _duration = time;

    //    _text.enabled = true;
    //}

}
