using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace MI.Editor.HotKey
{
    // Hierarchy에서 호버 중인 GameObject의 ActiveSelf를 단축키로 토글
    // 단축키 변경: Edit > Shortcuts > "MI/Toggle Hovered Active"
    [InitializeOnLoad]
    public static class MIHierarchyToggleActive
    {
        private const string SHORTCUT_ID = "MI/Toggle Hovered Active";

        // 마우스가 현재 올라가 있는 Hierarchy 항목의 instanceID
        private static int _hoveredInstanceID;

        static MIHierarchyToggleActive()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
        }

        // 호버 중인 항목의 instanceID 추적
        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            if (Event.current == null)
            {
                _hoveredInstanceID = 0;
                return;
            }

            if (selectionRect.Contains(Event.current.mousePosition))
                _hoveredInstanceID = instanceID;
        }

        [Shortcut(SHORTCUT_ID, KeyCode.A)]
        private static void ToggleHoveredObjectActive()
        {
            // 1순위: 호버 중인 오브젝트
            GameObject go = EditorUtility.InstanceIDToObject(_hoveredInstanceID) as GameObject;

            // 2순위: 현재 선택된 오브젝트
            if (go == null)
                go = Selection.activeGameObject;

            if (go == null)
                return;

            Undo.RecordObject(go, "Toggle Active Self");
            go.SetActive(!go.activeSelf);
            EditorUtility.SetDirty(go);
        }
    }
}
