// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace PixelCrushers.DialogueSystem
{

    public class UniqueIDWindow : EditorWindow
    {

        [MenuItem("Tools/Pixel Crushers/Dialogue System/Tools/Unique ID Tool", false, 3)]
        public static void Init()
        {
            EditorWindow.GetWindow(typeof(UniqueIDWindow), false, "Unique IDs");
        }

        // Private fields for the window:

        private UniqueIDWindowPrefs prefs = null;

        private bool verbose = false;

        private Vector2 scrollPosition = Vector2.zero;

        void OnEnable()
        {
            minSize = new Vector2(340, 128);
            if (prefs == null) prefs = UniqueIDWindowPrefs.Load();
        }

        void OnDisable()
        {
            if (prefs != null) prefs.Save();
        }

        void OnGUI()
        {
            // Validate prefs:
            if (prefs == null) prefs = UniqueIDWindowPrefs.Load(); 
            if (prefs.databases == null) prefs.databases = new List<DialogueDatabase>();

            // Draw window:
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            try
            {
                EditorGUILayout.HelpBox("This tool reassigns IDs so all Actors, Items, Locations, Conversations have unique IDs. " +
                                        "This allows your project to load multiple dialogue databases at runtime without conflicting IDs. " +
                                        "Actors, Items, and Locations with the same name will be assigned the same ID. " +
                                        "All conversations will have unique IDs, even if two conversations have the same title.",
                                        MessageType.None);
                DrawDatabaseSection();
                DrawButtonSection();
            }
            finally
            {
                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// Draws the database list section.
        /// </summary>
        private void DrawDatabaseSection()
        {
            DrawDatabaseHeader();
            DrawDatabaseList();
        }

        private void DrawDatabaseHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Dialogue Databases", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("All", EditorStyles.miniButtonLeft, GUILayout.Width(48)))
            {
                if (EditorUtility.DisplayDialog("Add All Databases in Project",
                                                string.Format("Do you want to find and add all dialogue databases in the entire project?", EditorWindowTools.GetCurrentDirectory()),
                                                "Ok", "Cancel"))
                {
                    AddAllDatabasesInProject();
                }
            }
            if (GUILayout.Button("Folder", EditorStyles.miniButtonMid, GUILayout.Width(48)))
            {
                if (EditorUtility.DisplayDialog("Add All Databases in Folder",
                                                string.Format("Do you want to find and add all dialogue databases in the folder {0}?", EditorWindowTools.GetCurrentDirectory()),
                                                "Ok", "Cancel"))
                {
                    AddAllDatabasesInFolder(EditorWindowTools.GetCurrentDirectory(), false);
                }
            }
            if (GUILayout.Button("+", EditorStyles.miniButtonRight, GUILayout.Width(22)))
            {
                prefs.databases.Add(null);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDatabaseList()
        {
            EditorWindowTools.StartIndentedSection();
            DialogueDatabase databaseToDelete = null;
            for (int i = 0; i < prefs.databases.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                prefs.databases[i] = EditorGUILayout.ObjectField(prefs.databases[i], typeof(DialogueDatabase), false) as DialogueDatabase;
                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(22))) databaseToDelete = prefs.databases[i];
                EditorGUILayout.EndHorizontal();

            }
            if (databaseToDelete != null) prefs.databases.Remove(databaseToDelete);
            EditorWindowTools.EndIndentedSection();
        }

        private void AddAllDatabasesInProject()
        {
            prefs.databases.Clear();
            AddAllDatabasesInFolder("Assets", true);
        }

        private void AddAllDatabasesInFolder(string folderPath, bool recursive)
        {
            string[] filePaths = Directory.GetFiles(folderPath, "*.asset", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (string filePath in filePaths)
            {
                string assetPath = filePath.Replace("\\", "/");
                DialogueDatabase database = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogueDatabase)) as DialogueDatabase;
                if ((database != null) && (!prefs.databases.Contains(database)))
                {
                    prefs.databases.Add(database);
                }
            }
        }

        private void DrawButtonSection()
        {
            verbose = EditorGUILayout.Toggle("Verbose Logging", verbose);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawClearButton();
            DrawProcessButton();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the Clear button, and clears the prefs if clicked.
        /// </summary>
        private void DrawClearButton()
        {
            if (GUILayout.Button("Clear", GUILayout.Width(100)))
            {
                prefs.Clear();
                UniqueIDWindowPrefs.DeleteEditorPrefs();
            }
        }

        /// <summary>
        /// Draws the Process button, and processes the databases if clicked.
        /// </summary>
        private void DrawProcessButton()
        {
            if (GUILayout.Button("Process", GUILayout.Width(100))) ProcessDatabases();
        }

        private class IDConversion
        {
            public int oldID = 0;
            public int newID = 0;
            public IDConversion() { }
            public IDConversion(int oldID, int newID)
            {
                this.oldID = oldID;
                this.newID = newID;
            }
        }

        private class MasterIDs
        {
            public Dictionary<string, IDConversion> actors = new Dictionary<string, IDConversion>();
            public Dictionary<string, IDConversion> items = new Dictionary<string, IDConversion>();
            public Dictionary<string, IDConversion> locations = new Dictionary<string, IDConversion>();
            public Dictionary<string, IDConversion> variables = new Dictionary<string, IDConversion>();
            public int highestActorID = 0;
            public int highestItemID = 0;
            public int highestLocationID = 0;
            public int highestVariableID = 0;
            public int highestConversationID = 0;
        }

        public void ProcessDatabases()
        {
            try
            {
                List<DialogueDatabase> distinct = prefs.databases.Distinct().ToList();
                MasterIDs masterIDs = new MasterIDs();
                for (int i = 0; i < distinct.Count; i++)
                {
                    var database = distinct[i];
                    if (database != null)
                    {
                        EditorUtility.DisplayProgressBar("Processing Databases (Phase 1/2)", database.name, i / prefs.databases.Count);
                        GetNewIDs(database, masterIDs);
                    }
                }
                for (int i = 0; i < distinct.Count; i++)
                {
                    var database = distinct[i];
                    if (database != null)
                    {
                        EditorUtility.DisplayProgressBar("Processing Databases (Phase 2/2)", database.name, i / prefs.databases.Count);
                        ProcessDatabase(database, masterIDs);
                        EditorUtility.SetDirty(database);
                    }
                }
                Debug.Log(string.Format("{0}: Assigned unique IDs to {1} databases.", DialogueDebug.Prefix, prefs.databases.Count));
                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void GetNewIDs(DialogueDatabase database, MasterIDs masterIDs)
        {
            if (verbose) Debug.Log(string.Format("{0}: Determining new IDs for database {1}", DialogueDebug.Prefix, database.name));
            GetNewActorIDs(database, masterIDs);
            GetNewItemIDs(database, masterIDs);
            GetNewLocationIDs(database, masterIDs);
            GetNewVariableIDs(database, masterIDs);
        }

        private void ProcessDatabase(DialogueDatabase database, MasterIDs masterIDs)
        {
            if (verbose) Debug.Log(string.Format("{0}: Converting IDs in database {1}", DialogueDebug.Prefix, database.name));
            ProcessConversations(database, masterIDs);
            ProcessActors(database, masterIDs);
            ProcessItems(database, masterIDs);
            ProcessLocations(database, masterIDs);
            ProcessVariables(database, masterIDs);
        }

        private void GetNewActorIDs(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var actor in database.actors)
            {
                if (masterIDs.actors.ContainsKey(actor.Name))
                {
                    masterIDs.actors[actor.Name].oldID = actor.id;
                }
                else
                {
                    int newID;
                    if (actor.id <= masterIDs.highestActorID)
                    {
                        masterIDs.highestActorID++;
                        newID = masterIDs.highestActorID;
                    }
                    else
                    {
                        masterIDs.highestActorID = actor.id;
                        newID = actor.id;
                    }
                    masterIDs.actors.Add(actor.Name, new IDConversion(actor.id, newID));
                }
            }
        }

        private void GetNewItemIDs(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var item in database.items)
            {
                if (masterIDs.items.ContainsKey(item.Name))
                {
                    masterIDs.items[item.Name].oldID = item.id;
                }
                else
                {
                    int newID;
                    if (item.id <= masterIDs.highestItemID)
                    {
                        masterIDs.highestItemID++;
                        newID = masterIDs.highestItemID;
                    }
                    else
                    {
                        masterIDs.highestItemID = item.id;
                        newID = item.id;
                    }
                    masterIDs.items.Add(item.Name, new IDConversion(item.id, newID));
                }
            }
        }

        private void GetNewLocationIDs(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var location in database.locations)
            {
                if (masterIDs.locations.ContainsKey(location.Name))
                {
                    masterIDs.locations[location.Name].oldID = location.id;
                }
                else
                {
                    int newID;
                    if (location.id <= masterIDs.highestLocationID)
                    {
                        masterIDs.highestLocationID++;
                        newID = masterIDs.highestLocationID;
                    }
                    else
                    {
                        masterIDs.highestLocationID = location.id;
                        newID = location.id;
                    }
                    masterIDs.locations.Add(location.Name, new IDConversion(location.id, newID));
                }
            }
        }

        private void GetNewVariableIDs(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var variable in database.variables)
            {
                if (masterIDs.variables.ContainsKey(variable.Name))
                {
                    masterIDs.variables[variable.Name].oldID = variable.id;
                }
                else
                {
                    int newID;
                    if (variable.id <= masterIDs.highestVariableID)
                    {
                        masterIDs.highestVariableID++;
                        newID = masterIDs.highestVariableID;
                    }
                    else
                    {
                        masterIDs.highestVariableID = variable.id;
                        newID = variable.id;
                    }
                    masterIDs.variables.Add(variable.Name, new IDConversion(variable.id, newID));
                }
            }
        }

        private void ProcessFieldIDs(DialogueDatabase database, List<Field> fields, MasterIDs masterIDs)
        {
            foreach (var field in fields)
            {
                int oldID = Tools.StringToInt(field.value);
                switch (field.type)
                {
                    case FieldType.Actor:
                        Actor actor = database.GetActor(oldID);
                        if (actor != null) field.value = FindIDConversion(actor.Name, masterIDs.actors, oldID).ToString();
                        break;
                    case FieldType.Item:
                        Item item = database.GetItem(oldID);
                        if (item != null) field.value = FindIDConversion(item.Name, masterIDs.items, oldID).ToString();
                        break;
                    case FieldType.Location:
                        Location location = database.GetLocation(oldID);
                        if (location != null) field.value = FindIDConversion(location.Name, masterIDs.locations, oldID).ToString();
                        break;
                }
            }
        }

        private int FindIDConversion(string name, Dictionary<string, IDConversion> dict, int oldID)
        {
            if (dict.ContainsKey(name))
            {
                return dict[name].newID;
            }
            else
            {
                if (verbose) Debug.Log(string.Format("{0}: Warning: No ID conversion entry found for {1}", DialogueDebug.Prefix, name));
                return oldID;
            }
        }

        private void ProcessActors(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var actor in database.actors)
            {
                int newID = FindIDConversion(actor.Name, masterIDs.actors, actor.id);
                if (newID != actor.id)
                {
                    if (verbose) Debug.Log(string.Format("{0}: Actor {1}: ID [{2}]-->[{3}]", DialogueDebug.Prefix, actor.Name, actor.id, newID));
                    actor.id = newID;
                }
                ProcessFieldIDs(database, actor.fields, masterIDs);
            }
        }

        private void ProcessItems(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var item in database.items)
            {
                int newID = FindIDConversion(item.Name, masterIDs.items, item.id);
                if (newID != item.id)
                {
                    if (verbose) Debug.Log(string.Format("{0}: Item {1}: ID [{2}]-->[{3}]", DialogueDebug.Prefix, item.Name, item.id, newID));
                    item.id = newID;
                }
                ProcessFieldIDs(database, item.fields, masterIDs);
            }
        }

        private void ProcessLocations(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var location in database.locations)
            {
                int newID = FindIDConversion(location.Name, masterIDs.locations, location.id);
                if (newID != location.id)
                {
                    if (verbose) Debug.Log(string.Format("{0}: Location {1}: ID [{2}]-->[{3}]", DialogueDebug.Prefix, location.Name, location.id, newID));
                    location.id = newID;
                }
                ProcessFieldIDs(database, location.fields, masterIDs);
            }
        }

        private void ProcessVariables(DialogueDatabase database, MasterIDs masterIDs)
        {
            foreach (var variable in database.variables)
            {
                int newID = FindIDConversion(variable.Name, masterIDs.variables, variable.id);
                if (newID != variable.id)
                {
                    if (verbose) Debug.Log(string.Format("{0}: Variable {1}: ID [{2}]-->[{3}]", DialogueDebug.Prefix, variable.Name, variable.id, newID));
                    variable.id = newID;
                }
                ProcessFieldIDs(database, variable.fields, masterIDs);
            }
        }

        private void ProcessConversations(DialogueDatabase database, MasterIDs masterIDs)
        {
            Dictionary<int, int> newIDs = GetNewConversationIDs(database, masterIDs);
            foreach (var conversation in database.conversations)
            {
                if (newIDs.ContainsKey(conversation.id))
                {
                    if (verbose) Debug.Log(string.Format("{0}: Conversation '{1}': ID [{2}]-->[{3}]", DialogueDebug.Prefix, conversation.Title, conversation.id, newIDs[conversation.id]));
                    conversation.id = newIDs[conversation.id];
                    ProcessFieldIDs(database, conversation.fields, masterIDs);
                    foreach (DialogueEntry entry in conversation.dialogueEntries)
                    {
                        entry.conversationID = conversation.id;
                        ProcessFieldIDs(database, entry.fields, masterIDs);
                        foreach (var link in entry.outgoingLinks)
                        {
                            if (newIDs.ContainsKey(link.originConversationID)) link.originConversationID = newIDs[link.originConversationID];
                            if (newIDs.ContainsKey(link.destinationConversationID)) link.destinationConversationID = newIDs[link.destinationConversationID];
                        }
                    }
                }
            }
        }

        private Dictionary<int, int> GetNewConversationIDs(DialogueDatabase database, MasterIDs masterIDs)
        {
            Dictionary<int, int> newIDs = new Dictionary<int, int>();
            foreach (var conversation in database.conversations)
            {
                int newID = conversation.id;
                if (conversation.id <= masterIDs.highestConversationID)
                {
                    masterIDs.highestConversationID++;
                    newID = masterIDs.highestConversationID;
                    newIDs.Add(conversation.id, newID);
                }
                else
                {
                    masterIDs.highestConversationID = conversation.id;
                }
            }
            return newIDs;
        }

    }

}
