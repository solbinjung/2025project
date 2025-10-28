using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 드래그 중인 슬롯의 '인덱스'를 임시로 저장하는 static 클래스
public static class ItemDragHandler
{
    // 1. 드래그 중인 슬롯의 인덱스 (0, 1, 2...)
    public static int draggedSlotIndex = -1; // -1은 드래그 중이 아님을 의미

    // 2. 마우스를 따라다닐 아이콘
    public static Image dragIcon;
}