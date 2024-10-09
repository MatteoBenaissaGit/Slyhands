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
    
    public void LoadMissionData()
    {
        _missionNameText.text = MissionDeckManager.Instance.MissionData.MissionName;

        _missionComplishedIcon.gameObject.SetActive(MissionDeckManager.Instance.MissionData.MissionComplished);
        _completingRewardsIcon.gameObject.SetActive(MissionDeckManager.Instance.MissionData.MissionComplished);
        _completingRewardsCountText.gameObject.SetActive(!MissionDeckManager.Instance.MissionData.MissionComplished);

        for (int i = 0; i < MissionDeckManager.Instance.MissionData.StarObtained; i++)
        {
            StarsIcons[i].gameObject.SetActive(true);
        }

        _starsRewardIcon.gameObject.SetActive(MissionDeckManager.Instance.MissionData.StarObtained == 3);

        _starsRewardText.text = (3 - MissionDeckManager.Instance.MissionData.StarObtained).ToString();

        _goldRewardIcon.gameObject.SetActive(MissionDeckManager.Instance.MissionData.GoldCardToFound == 0);
        _goldRewardText.text = MissionDeckManager.Instance.MissionData.GoldCardToFound.ToString();

        secondaryObjectivesText.text = MissionDeckManager.Instance.MissionData.SecondaryMission;
    }
}