using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;

    [SerializeField] private Image hpFillImage;

    void Update()
    {
        float hpPercent = (float)_playerStats.CurrentHp / _playerStats.MaxHp;
        float mpPercent = (float)_playerStats.CurrentMp / _playerStats.MaxMp;

        hpBar.value = hpPercent;
        mpBar.value = mpPercent;

        // HP바 색상 변경s
        if (hpPercent > 0.5f) // 50% 이상
            hpFillImage.color = Color.green;
        else if (hpPercent > 0.2f) // 20% ~ 50%
            hpFillImage.color = Color.yellow;
        else if (hpPercent > 0f) // 0%~ 20%
            hpFillImage.color = Color.red;
        else // 0%
            hpFillImage.color = Color.white;
    }

}
