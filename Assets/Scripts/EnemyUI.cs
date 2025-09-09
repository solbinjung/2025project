using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private EnemyStats _enemyStats;
    [SerializeField] private Slider hpBar;

    [SerializeField] private Image hpFillImage;

    void Update()
    {
        hpBar.value = (float)_enemyStats.CurrentHp / _enemyStats.MaxHp;

        if(hpBar.value > 0)
            hpFillImage.color = Color.red;
        else
            hpFillImage.color = Color.white;

    }
}
