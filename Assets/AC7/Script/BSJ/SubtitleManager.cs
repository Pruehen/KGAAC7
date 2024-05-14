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
    /// 키를 받아 자막을 반환하는 메서드
    /// 
    /// Hit1,경보! 우측 엔진에 피격당했다!
    ///Hit2,"피격 상태 심각, 즉시 지원 요청한다!"
    ///MissileLaunch,폭스 2
    ///Kill1,목표 적중! 다음 목표로 이동!
    ///Kill2,"적군 격파 확인, 작전 성공적!"
    ///Kill3,정확한 타격! 목표 파괴 완료!
    ///StartMission1,"임무 시작, 모두 집중하고 조심하자!"
    ///StartMission2,"전투 준비 완료, 행운을 빕니다!"
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetSubtitle(string key)
    {
        return _subtitleDictionary[key];
    }


}
