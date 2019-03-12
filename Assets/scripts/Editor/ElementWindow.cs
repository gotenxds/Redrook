using UnityEngine;
using UnityEditor;
using BayatGames.SaveGameFree;

public class ElementWindow : EditorWindow
{
    public Ailment[] ailments;
    public Element[] elements;

    [MenuItem("Tools/David/Elements")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(ElementWindow)).Show();
    }

    void OnGUI()
    {
        if (elements == null)
        {
            ailments = SaveGame.Load<Ailment[]>("Ailments") ?? new Ailment[] { };
            elements = SaveGame.Load<Element[]>("Elements") ?? new Element[] { };
        }
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty elementsProperty = serializedObject.FindProperty("elements");
        SerializedProperty ailmentsProperty = serializedObject.FindProperty("ailments");
        EditorGUILayout.PropertyField(elementsProperty, true);
        EditorGUILayout.PropertyField(ailmentsProperty, true);

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Save"))
        {
            Save();
        }
    }

    void Save()
    {
        SaveGame.Save<Ailment[]>("Ailments", ailments);
        SaveGame.Save<Element[]>("Elements", elements);
    }

}