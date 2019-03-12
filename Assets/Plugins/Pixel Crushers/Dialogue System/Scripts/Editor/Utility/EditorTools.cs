// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using UnityEditor.Graphs;

namespace PixelCrushers.DialogueSystem
{

    public static class EditorTools
    {

        public static DialogueDatabase selectedDatabase = null;

        public static GUIStyle textAreaGuiStyle
        {
            get
            {
                if (m_textAreaGuiStyle == null)
                {
                    m_textAreaGuiStyle = new GUIStyle(EditorStyles.textArea);
                    m_textAreaGuiStyle.fixedHeight = 0;
                    m_textAreaGuiStyle.stretchHeight = true;
                    m_textAreaGuiStyle.wordWrap = true;
                }
                return m_textAreaGuiStyle;
            }
        }

        private static GUIStyle m_textAreaGuiStyle = null;

        public static DialogueDatabase FindInitialDatabase()
        {
            var dialogueSystemController = Object.FindObjectOfType<DialogueSystemController>();
            return (dialogueSystemController == null) ? null : dialogueSystemController.initialDatabase;
        }

        public static void SetInitialDatabaseIfNull()
        {
            if (selectedDatabase == null)
            {
                selectedDatabase = FindInitialDatabase();
            }
        }

        public static void DrawReferenceDatabase()
        {
            selectedDatabase = EditorGUILayout.ObjectField(new GUIContent("Reference Database", "Database to use for pop-up menus. Assumes this database will be in memory at runtime."), selectedDatabase, typeof(DialogueDatabase), true) as DialogueDatabase;
        }

        public static void DrawReferenceDatabase(Rect rect)
        {
            selectedDatabase = EditorGUI.ObjectField(rect, new GUIContent("Reference Database", "Database to use for pop-up menus. Assumes this database will be in memory at runtime."), selectedDatabase, typeof(DialogueDatabase), true) as DialogueDatabase;
        }

        public static void DrawSerializedProperty(SerializedObject serializedObject, string propertyName)
        {
            serializedObject.Update();
            var property = serializedObject.FindProperty(propertyName);
            if (property == null) return;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        public static Styles.Color StringToStylesColor(string s)
        {
            switch (s)
            {
                case "Aqua":
                    return Styles.Color.Aqua;
                case "Blue":
                    return Styles.Color.Blue;
                case "Gray":
                    return Styles.Color.Gray;
                case "Green":
                    return Styles.Color.Green;
                case "Grey":
                    return Styles.Color.Grey;
                case "Orange":
                    return Styles.Color.Orange;
                case "Red":
                    return Styles.Color.Red;
                case "Yellow":
                    return Styles.Color.Yellow;
                default:
                    return Styles.Color.Gray;
            }
        }

        public static string[] StylesColorStrings = new string[]
        {
            "Aqua", "Blue", "Gray", "Green", "Orange", "Red", "Yellow"
        };

        public static void SetDirtyBeforeChange(UnityEngine.Object obj, string name)
        {
            Undo.RecordObject(obj, name);
        }

        public static void SetDirtyAfterChange(UnityEngine.Object obj)
        {
            EditorUtility.SetDirty(obj);
        }

        public static void TryAddScriptingDefineSymbols(string newDefine)
        {
            MoreEditorUtility.TryAddScriptingDefineSymbols(newDefine);
        }

    }

}
