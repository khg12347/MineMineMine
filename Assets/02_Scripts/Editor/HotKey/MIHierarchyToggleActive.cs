using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace MI.Editor.HotKey
{
    /// <summary>
    /// Hierarchy 창에서 마우스가 올라간 GameObject의 ActiveSelf를 단축키로 토글합니다.
    /// 단축키는 Edit > Shortcuts... > "MI/Toggle Hovered Active" 에서 변경 가능합니다.
    /// </summary>
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

        /// <summary>
        /// Hierarchy 각 항목이 렌더링될 때 호출됩니다.
        /// 마우스 위치와 항목의 Rect를 비교해 현재 호버 중인 instanceID를 추적합니다.
        /// </summary>
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
