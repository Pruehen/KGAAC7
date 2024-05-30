using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 사용법:
/// 씬에 Subtitle매니저를 배치하고 Text에셋 할당후
/// 가져오고싶은 자막의 키를 넘겨주면 값을 넘겨준다
/// </summary>
public class SubtitleManager : SceneSingleton<SubtitleManager>
{
    //[SerializeField] private TextAsset _subtitleCsvAsset;
    //private Dictionary<string, string> _subtitleDictionary = new Dictionary<string, string>();
    //[SerializeField] private TMPro.TMP_Text _text;

    ////대사 지속시간 계산법 _baseDuration + text.length * addtionalTimeRatio
    ////예                        2        +  10 * .15f  10글자면 3.5초
    //private float _baseDuration = 1f;//기본 지속시간
    //private float _addtionalDurationRatio = .05f;
    //private float _duration;

    ////테스트용 코드
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
    ///// 키를 받아 자막을 출력하는 메서드
    ///// 
    ///*
    //    ID,English,Original
    //    Hit1,"Alert! The right engine has been hit!","경보! 우측 엔진에 피격당했다!"
    //    Hit2,"The damage is severe, request immediate support!","피격 상태 심각, 즉시 지원 요청한다!"
    //    MissileLaunch,"Fox 2","폭스 2"
    //    Kill1,"Target hit! Moving to the next target!","목표 적중! 다음 목표로 이동!"
    //    Kill2,"Enemy down confirmed, operation successful!","적군 격파 확인, 작전 성공적!"
    //    Kill3,"Accurate strike! Target destroyed!","정확한 타격! 목표 파괴 완료!"
    //    StartMission1,"Mission start, everyone stay focused and be careful!","임무 시작, 모두 집중하고 조심하자!"
    //    StartMission2,"Combat ready, good luck!","전투 준비 완료, 행운을 빕니다!"
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
    ///// 키를 받아 지정된 시간동안 자막을 출력하는 메서드
    ///// 
    ///*
    //    ID,English,Original
    //    Hit1,"Alert! The right engine has been hit!","경보! 우측 엔진에 피격당했다!"
    //    Hit2,"The damage is severe, request immediate support!","피격 상태 심각, 즉시 지원 요청한다!"
    //    MissileLaunch,"Fox 2","폭스 2"
    //    Kill1,"Target hit! Moving to the next target!","목표 적중! 다음 목표로 이동!"
    //    Kill2,"Enemy down confirmed, operation successful!","적군 격파 확인, 작전 성공적!"
    //    Kill3,"Accurate strike! Target destroyed!","정확한 타격! 목표 파괴 완료!"
    //    StartMission1,"Mission start, everyone stay focused and be careful!","임무 시작, 모두 집중하고 조심하자!"
    //    StartMission2,"Combat ready, good luck!","전투 준비 완료, 행운을 빕니다!"
    // */
    ///// 
    ///// </summary>
    ///// <param name="key">키</param>
    ///// <param name="time">지속시간</param>
    //public void ShowSubtitleWithTime(string key, float time)
    //{
    //    Debug.Assert(_subtitleDictionary.ContainsKey(key), "Wrong subtitle key");
    //    _text.text = $"<< {_subtitleDictionary[key]} >>";

    //    _duration = time;

    //    _text.enabled = true;
    //}

}
