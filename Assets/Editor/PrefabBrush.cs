#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Tilemaps;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "PrefabBrush", menuName = "Brushes/Prefab Brush")]
#if UNITY_EDITOR
[CustomGridBrush(false, true, false, "Prefab Brush")]
#endif
public class PrefabBrush : GridBrushBase
{
    public GameObject prefab;

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
#if UNITY_EDITOR
        if (prefab == null || brushTarget == null)
            return;

        Vector3 worldPos = gridLayout.CellToWorld(position);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = worldPos;
        instance.transform.SetParent(brushTarget.transform);
        Undo.RegisterCreatedObjectUndo(instance, "Paint Prefab");
#endif
    }

    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
#if UNITY_EDITOR
        Transform erased = GetObjectInCell(gridLayout, brushTarget.transform, position);
        if (erased != null)
        {
            Undo.DestroyObjectImmediate(erased.gameObject);
        }
#endif
    }

    private Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        Vector3 min = grid.CellToWorld(position);
        Vector3 max = grid.CellToWorld(position + Vector3Int.one);
        Bounds bounds = new Bounds((min + max) * 0.5f, max - min);

        foreach (Transform child in parent)
        {
            if (bounds.Contains(child.position))
                return child;
        }
        return null;
    }
}
