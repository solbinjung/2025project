using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;

    [SerializeField] private Image hpFillImage;
    [SerializeField] private Image mpFillImage;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;

    void Update()
    {
        hpBar.value = (float)_playerStats.CurrentHp / _playerStats.MaxHp;
        mpBar.value = (float)_playerStats.CurrentMp / _playerStats.MaxMp;

        if (hpBar.value > 0)
            hpFillImage.color = new Color32(147, 0, 0, 255);
        else
            hpFillImage.color = new Color32(28, 22, 21, 255);

        if (mpBar.value > 0)
            mpFillImage.color = new Color32(0, 11, 93, 255);
        else
            mpFillImage.color = new Color32(28, 22, 21, 255);

        hpText.text = _playerStats.CurrentHp + "/" + _playerStats.MaxHp;
        mpText.text = _playerStats.CurrentMp + "/" + _playerStats.MaxMp;
    }
}
