using System;
using System.Collections.Generic;
using Mission;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class DeckEditorUIManager : MonoBehaviour
{
    private MissionDeckManager missionDeckManager;

    [TabGroup("Mission Informations")]
    [field: SerializeField] private TMP_Text _missionNameText;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private Image _missionComplishedIcon;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private Image _completingRewardsIcon;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private TMP_Text _completingRewardsCountText;
    [TabGroup("Mission Informations")]
    [TabGroup("Mission Informations")]
    [field: SerializeField] private List<Image> StarsIcons;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private Image _starsRewardIcon;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private TMP_Text _starsRewardText;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private Image _goldRewardIcon;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private TMP_Text _goldRewardText;
    [TabGroup("Mission Informations")]
    [field: SerializeField] private TMP_Text secondaryObjectivesText;

    private void Awake()
    {
        missionDeckManager = MissionDeckManager.Instance;
    }

    public void LoadMissionData()
    {
        _missionNameText.text = missionDeckManager.MissionData.MissionName;

        _missionComplishedIcon.gameObject.SetActive(missionDeckManager.MissionData.MissionComplished);
        _completingRewardsIcon.gameObject.SetActive(missionDeckManager.MissionData.MissionComplished);
        _completingRewardsCountText.gameObject.SetActive(!missionDeckManager.MissionData.MissionComplished);

        for (int i = 0; i < missionDeckManager.MissionData.StarObtained; i++)
        {
            StarsIcons[i].gameObject.SetActive(true);
        }

        _starsRewardIcon.gameObject.SetActive(missionDeckManager.MissionData.StarObtained == 3);

        _starsRewardText.text = (3 - missionDeckManager.MissionData.StarObtained).ToString();

        _goldRewardIcon.gameObject.SetActive(missionDeckManager.MissionData.GoldCardToFound == 0);
        _goldRewardText.text = missionDeckManager.MissionData.GoldCardToFound.ToString();

        secondaryObjectivesText.text = missionDeckManager.MissionData.SecondaryMission;
    }
}