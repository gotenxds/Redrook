// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    public delegate string GetCustomSaveDataDelegate();

    /// <summary>
    /// A static class for saving and loading game data using the Dialogue System's 
    /// Lua environment. It allows you to save or load a game with a single line of code.
    /// 
    /// For more information, see @ref saveLoadSystem
    /// </summary>
    public static class PersistentDataManager
    {

        /// <summary>
        /// Set <c>true</c> to include actor data in save data, <c>false</c> to exclude.
        /// </summary>
        public static bool includeActorData = true;

        /// <summary>
        /// Set this <c>true</c> to include all item fields in saved-game data. This is
        /// <c>false</c> by default to minimize the size of the saved-game data by only
        /// recording State and Track (for quests).
        /// </summary>
        public static bool includeAllItemData = false;

        /// <summary>
        /// Set <c>true</c> to include location data in save data, <c>false</c> to exclude.
        /// </summary>
        public static bool includeLocationData = false;

        /// <summary>
        /// Set <c>true</c> to include all conversation fields, <c>false</c> to exclude.
        /// </summary>
        public static bool includeAllConversationFields = false;

        /// <summary>
        /// Set this <c>true</c> to exclude Conversation[#].Dialog[#].SimStatus values from
        /// saved-game data. If you don't use SimStatus in your Lua conditions, there's no
        /// need to save it.
        /// </summary>
        public static bool includeSimStatus = false;

        /// <summary>
        /// Optional field to use when saving a conversation's SimStatus info (e.g., Title). 
        /// If blank, uses conversation ID.
        /// </summary>
        public static string saveConversationSimStatusWithField = string.Empty;

        /// <summary>
        /// Optional field to use when saving a dialogue entry's SimStatus info (e.g,. Title).
        /// If blank, uses entry's ID.
        /// </summary>
        public static string saveDialogueEntrySimStatusWithField = string.Empty;

        /// <summary>
        /// Set <c>true</c> to include the status & relationship tables in save data,
        /// <c>false</c> to exclude.
        /// </summary>
        public static bool includeRelationshipAndStatusData = true;

        /// <summary>
        /// Initialize variables that were added to database after saved game.")]
        /// </summary>
        public static bool initializeNewVariables = true;

        /// <summary>
        /// Initialize new SimStatus values for entries that were added to database after saved game.
        /// </summary>
        public static bool initializeNewSimStatus = true;

        /// <summary>
        /// PersistentDataManager will call this delegate (if set) to add custom data
        /// to the saved-game data string. The custom data should be valid Lua code.
        /// </summary>
        public static GetCustomSaveDataDelegate GetCustomSaveData = null;

        public enum RecordPersistentDataOn
        {
            /// <summary>
            /// Inform all components on all GameObjects in the scene to record their persistent data
            /// if supported.
            /// </summary>
            AllGameObjects,

            /// <summary>
            /// Inform only components that have registered to receive notifications to record their
            /// persistent data.
            /// </summary>
            OnlyRegisteredGameObjects,

            /// <summary>
            /// Entirely skip informing any GameObjects to record their persistent data.
            /// </summary>
            NoGameObjects
        }

        public static RecordPersistentDataOn recordPersistentDataOn = RecordPersistentDataOn.AllGameObjects;

        private static HashSet<GameObject> listeners = new HashSet<GameObject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go">GameObject that should receive notifications.</param>
        public static void RegisterPersistentData(GameObject go)
        {
            if (go == null || !Application.isPlaying) return;
            listeners.Add(go);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="go">GameObject that should no longer receive notifications.</param>
        public static void UnregisterPersistentData(GameObject go)
        {
            if (!Application.isPlaying) return;
            listeners.Remove(go);
        }

        /// <summary>
        /// Resets the Lua environment -- for example, when starting a new game.
        /// </summary>
        /// <param name='databaseResetOptions'>
        /// The database reset options can be:
        /// 
        /// - RevertToDefault: Removes all but the default database, then resets it.
        /// - KeepAllLoaded: Keeps all loaded databases in memory and just resets them.
        /// </param>
        public static void Reset(DatabaseResetOptions databaseResetOptions)
        {
            DialogueManager.ResetDatabase(databaseResetOptions);
        }

        /// <summary>
        /// Resets the Lua environment -- for example, when starting a new game -- keeping all loaded database
        /// in memory and just resetting them.
        /// </summary>
        public static void Reset()
        {
            Reset(DatabaseResetOptions.KeepAllLoaded);
        }

        /// <summary>
        /// Sends the OnRecordPersistentData message to all GameObjects in the scene to give them 
        /// an opportunity to record their state in the Lua environment. You can limit which GameObjects
        /// receive messages by changing recordPersistentDataOn.
        /// </summary>
        public static void Record()
        {
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Recording persistent data to Lua environment.", new System.Object[] { DialogueDebug.Prefix }));
            SendPersistentDataMessage("OnRecordPersistentData");
        }

        /// <summary>
        /// Sends the OnApplyPersistentData message to all game objects in the scene to give them an 
        /// opportunity to retrieve their state from the Lua environment. If calling this after loading
        /// a new scene, you may want to wait one frame to allow other GameObject's Start methods
        /// to complete first. You can limit which GameObjects receive messages by changing 
        /// recordPersistentDataOn.
        /// </summary>
        public static void Apply()
        {
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Applying persistent data from Lua environment.", new System.Object[] { DialogueDebug.Prefix }));
            SendPersistentDataMessage("OnApplyPersistentData");
            DialogueManager.SendUpdateTracker(); // Update quest tracker HUD.
        }

        private static void SendPersistentDataMessage(string message)
        {
            switch (recordPersistentDataOn)
            {
                case RecordPersistentDataOn.AllGameObjects:
                    Tools.SendMessageToEveryone(message);
                    break;
                case RecordPersistentDataOn.OnlyRegisteredGameObjects:
                    foreach (var go in listeners)
                    {
                        if (go != null) go.SendMessage(message, SendMessageOptions.DontRequireReceiver);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sends the OnLevelWillBeUnloaded message to all game objects in the scene in case they
        /// need to change their behavior. For example, scripts that do something special when 
        /// destroyed during play may not want to do the same thing when being destroyed by a 
        /// level unload.
        /// </summary>
        public static void LevelWillBeUnloaded()
        {
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Broadcasting that level will be unloaded.", new System.Object[] { DialogueDebug.Prefix }));
            SendPersistentDataMessage("OnLevelWillBeUnloaded");
        }

        /// <summary>
        /// Loads a saved game by applying a saved-game string.
        /// </summary>
        /// <param name='saveData'>
        /// A saved-game string previously returned by GetSaveData().
        /// </param>
        /// <param name='databaseResetOptions'>
        /// Database reset options.
        /// </param>
        public static void ApplySaveData(string saveData, DatabaseResetOptions databaseResetOptions = DatabaseResetOptions.KeepAllLoaded)
        {
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Resetting Lua environment.", new System.Object[] { DialogueDebug.Prefix }));
            DialogueManager.ResetDatabase(databaseResetOptions);
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Updating Lua environment with saved data.", new System.Object[] { DialogueDebug.Prefix }));
            Lua.Run(saveData, DialogueDebug.logInfo);
            ExpandCompressedSimStatusData();
            RefreshRelationshipAndStatusTablesFromLua();
            if (initializeNewVariables)
            {
                InitializeNewVariablesFromDatabase();
                InitializeNewQuestEntriesFromDatabase();
                if (includeSimStatus) InitializeNewSimStatusFromDatabase();
            }
            Apply();
        }

        // Previously limited save data to 1 MB, since this was webplayer's PlayerPrefs limit.
        // Save data can now be unlimited size.
        //---Was:
        //private const int stringDataStartCapacity = 10240;
        //private const int stringDataMaxCapacity = 1048576;

        /// <summary>
        /// Saves a game by retrieving the Lua environment and returning it as a saved-game string. 
        /// This method calls Record() to allow all game objects in the scene to record their state 
        /// to the Lua environment first. The returned string is human-readable Lua code.
        /// </summary>
        /// <returns>
        /// The saved-game data.
        /// </returns>
        /// <remarks>
        /// To reduce saved-game data size, only the following information is recorded from the 
        /// Chat Mapper tables (Item[], Actor[], etc):
        /// 
        /// - <c>Actor[]</c>: all data
        /// - <c>Item[]</c>: only <c>State</c> (for quest log system)
        /// - <c>Location[]</c>: nothing
        /// - <c>Variable[]</c>: current value of each variable
        /// - <c>Conversation[]</c>: SimStatus
        /// - Relationship and status information is recorded
        /// </remarks>
        public static string GetSaveData()
        {
            Record();
            var sb = new StringBuilder(); //---Was: new StringBuilder(stringDataStartCapacity, stringDataMaxCapacity);
            AppendDialogueSystemData(sb);
            string saveData = sb.ToString();
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Saved data: {1}", new System.Object[] { DialogueDebug.Prefix, saveData }));
            return saveData;
        }

        public static void AppendDialogueSystemData(StringBuilder sb)
        {
            if (sb == null) return;
            AppendVariableData(sb);
            AppendItemData(sb);
            AppendLocationData(sb);
            if (includeActorData) AppendActorData(sb);
            AppendConversationData(sb);
            if (includeRelationshipAndStatusData) AppendRelationshipAndStatusTables(sb);
            if (GetCustomSaveData != null) sb.Append(GetCustomSaveData());
        }

        /// <summary>
        /// Appends the user variable table to a (saved-game) string.
        /// </summary>
        public static void AppendVariableData(StringBuilder sb)
        {
            try
            {
                LuaTableWrapper variableTable = Lua.Run("return Variable").asTable;
                if (variableTable == null)
                {
                    if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: Persistent Data Manager couldn't access Lua Variable[] table", new System.Object[] { DialogueDebug.Prefix }));
                    return;
                }
                sb.Append("Variable={");
                var first = true;
                foreach (var key in variableTable.keys)
                {
                    if (!first) sb.Append(", ");
                    first = false;
                    var value = variableTable[key.ToString()];
                    sb.AppendFormat("{0}={1}", new System.Object[] { GetFieldKeyString(key), GetFieldValueString(value) });
                }
                sb.Append("}; ");
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: GetSaveData() failed to get variable data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Appends the item table to a (saved-game) string.
        /// </summary>
        public static void AppendItemData(StringBuilder sb)
        {
            try
            {
                LuaTableWrapper itemTable = Lua.Run("return Item").asTable;
                if (itemTable == null)
                {
                    if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: Persistent Data Manager couldn't access Lua Item[] table", new System.Object[] { DialogueDebug.Prefix }));
                    return;
                }
                foreach (var title in itemTable.keys)
                {
                    LuaTableWrapper fields = itemTable[title.ToString()] as LuaTableWrapper;
                    bool onlySaveQuestData = !includeAllItemData && (DialogueManager.masterDatabase.items.Find(i => string.Equals(DialogueLua.StringToTableIndex(i.Name), title)) != null);
                    if (fields != null)
                    {
                        if (onlySaveQuestData)
                        {
                            // If in the database, just record quest statuses and tracking:
                            foreach (var fieldKey in fields.keys)
                            {
                                string fieldTitle = fieldKey.ToString();
                                if (fieldTitle.EndsWith("State"))
                                {
                                    sb.AppendFormat("Item[\"{0}\"].{1}=\"{2}\"; ", new System.Object[] { DialogueLua.StringToTableIndex(title), (System.Object)fieldTitle, (System.Object)fields[fieldTitle] });
                                }
                                else if (string.Equals(fieldTitle, "Track"))
                                {
                                    sb.AppendFormat("Item[\"{0}\"].Track={1}; ", new System.Object[] { DialogueLua.StringToTableIndex(title), fields[fieldTitle].ToString().ToLower() });
                                }
                            }
                        }
                        else
                        {
                            // If not in the database, record all fields:
                            sb.AppendFormat("Item[\"{0}\"]=", new System.Object[] { DialogueLua.StringToTableIndex(title) });
                            AppendFields(sb, fields);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: GetSaveData() failed to get item data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        private static void AppendFields(StringBuilder sb, LuaTableWrapper fields)
        {
            sb.Append("{");
            try
            {
                if (fields != null)
                {
                    foreach (var key in fields.keys)
                    {
                        var value = fields[key.ToString()];
                        sb.AppendFormat("{0}={1}, ", new System.Object[] { GetFieldKeyString(key), GetFieldValueString(value) });
                    }
                }
            }
            finally
            {
                sb.Append("}; ");
            }

        }

        private static Regex matchValidVarName = new Regex("^[a-zA-Z_][a-zA-Z0-9_]*$");

        private static string GetFieldKeyString(string key)
        {
            key = DialogueLua.StringToTableIndex(key);
            return matchValidVarName.IsMatch(key) ? key : ("[\"" + key + "\"]");
        }

        private static string GetFieldValueString(object o)
        {
            if (o == null)
            {
                return "nil";
            }
            else
            {
                System.Type type = o.GetType();
                if (type == typeof(string))
                {
                    return string.Format("\"{0}\"", new System.Object[] { DialogueLua.DoubleQuotesToSingle(o.ToString().Replace("\n", "\\n")) });
                }
                else if (type == typeof(bool))
                {
                    return o.ToString().ToLower();
                }
                else
                {
                    return o.ToString();
                }
            }
        }

        /// <summary>
        /// Appends the location table to a (saved-game) string. Currently doesn't save anything unless
        /// includeLocationData is true.
        /// </summary>
        public static void AppendLocationData(StringBuilder sb)
        {
            if (!includeLocationData) return;
            try
            {
                LuaTableWrapper locationTable = Lua.Run("return Location").asTable;
                if (locationTable == null)
                {
                    if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: Persistent Data Manager couldn't access Lua Location[] table", new System.Object[] { DialogueDebug.Prefix }));
                    return;
                }
                sb.Append("Location={");
                var first = true;
                foreach (var key in locationTable.keys)
                {
                    LuaTableWrapper fields = locationTable[key.ToString()] as LuaTableWrapper;
                    if (!first) sb.Append(", ");
                    first = false;
                    sb.Append(GetFieldKeyString(key));
                    sb.Append("={");
                    try
                    {
                        AppendAssetFieldData(sb, fields);
                    }
                    finally
                    {
                        sb.Append("}");
                    }
                }
                sb.Append("}; ");
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: GetSaveData() failed to get location data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Appends the actor table to a (saved-game) string.
        /// </summary>
        public static void AppendActorData(StringBuilder sb)
        {
            try
            {
                LuaTableWrapper actorTable = Lua.Run("return Actor").asTable;
                if (actorTable == null)
                {
                    if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: Persistent Data Manager couldn't access Lua Actor[] table", new System.Object[] { DialogueDebug.Prefix }));
                    return;
                }
                sb.Append("Actor={");
                var first = true;
                foreach (var key in actorTable.keys)
                {
                    LuaTableWrapper fields = actorTable[key.ToString()] as LuaTableWrapper;
                    if (!first) sb.Append(", ");
                    first = false;
                    sb.Append(GetFieldKeyString(key));
                    sb.Append("={");
                    try
                    {
                        AppendAssetFieldData(sb, fields);
                    }
                    finally
                    {
                        sb.Append("}");
                    }
                }
                sb.Append("}; ");
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: GetSaveData() failed to get actor data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Appends an actor record to a saved-game string.
        /// </summary>
        private static void AppendAssetFieldData(StringBuilder sb, LuaTableWrapper fields)
        {
            if (fields == null) return;
            var first = true;
            foreach (var key in fields.keys)
            {
                if (!first) sb.Append(", ");
                first = false;
                var value = fields[key.ToString()];
                sb.AppendFormat("{0}={1}", new System.Object[] { GetFieldKeyString(key), GetFieldValueString(value) });
            }
        }

        /// <summary>
        /// Appends the conversation table to a (saved-game) string. To conserve space, only the
        /// SimStatus is recorded. If includeSimStatus is <c>false</c>, nothing is recorded.
        /// The exception is if includeAllConversationFields is true.
        /// </summary>
        public static void AppendConversationData(StringBuilder sb)
        {
            if (includeAllConversationFields || DialogueManager.instance.persistentDataSettings.includeAllConversationFields)
            {
                AppendAllConversationFields(sb);
            }
            if (includeSimStatus && DialogueManager.instance.includeSimStatus)
            {
                AppendSimStatus(sb);
            }
        }

        /// <summary>
        /// Appends all conversation fields to a saved-game string. Note that this doesn't
        /// append the fields inside each dialogue entry, just the fields in the conversation
        /// objects themselves.
        /// </summary>
        private static void AppendAllConversationFields(StringBuilder sb)
        {
            try
            {
                LuaTableWrapper conversationTable = Lua.Run("return Conversation").asTable;
                if (conversationTable == null)
                {
                    if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: Persistent Data Manager couldn't access Lua Conversation[] table", new System.Object[] { DialogueDebug.Prefix }));
                    return;
                }
                foreach (var convIndex in conversationTable.keys) // Loop through conversations:
                {
                    LuaTableWrapper fields = Lua.Run("return Conversation[" + convIndex + "]").asTable;
                    if (fields == null) continue;
                    sb.Append("Conversation[" + convIndex + "]={");
                    try
                    {
                        var first = true;
                        foreach (var key in fields.keys)
                        {
                            if (string.Equals(key, "Dialog")) continue;
                            if (!first) sb.Append(", ");
                            first = false;
                            var value = fields[key.ToString()];
                            sb.AppendFormat("{0}={1}", new System.Object[] { GetFieldKeyString(key), GetFieldValueString(value) });
                        }
                    }
                    finally
                    {
                        sb.Append("}; ");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: GetSaveData() failed to get conversation data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        // Used by SimStatus methods:
        private static bool useConversationID = true;
        private static bool useEntryID = true;

        /// <summary>
        /// Appends SimStatus for all conversations.
        /// </summary>
        private static void AppendSimStatus(StringBuilder sb)
        { 
            try
            {
#if USE_NLUA
                var useConversationID = string.IsNullOrEmpty(saveConversationSimStatusWithField);
                var useEntryID = string.IsNullOrEmpty(saveDialogueEntrySimStatusWithField);
                foreach (var conversation in DialogueManager.MasterDatabase.conversations)
                {
                    if (useConversationID)
                    {
                        sb.AppendFormat("Conversation[{0}].SimX=\"", conversation.id);
                    }
                    else
                    {
                        var fieldValue = DialogueLua.StringToTableIndex(conversation.LookupValue(saveConversationSimStatusWithField));
                        if (string.IsNullOrEmpty(fieldValue)) fieldValue = conversation.id.ToString();
                        sb.AppendFormat("Variable[\"Conversation_SimX_{0}\"]=\"", fieldValue);
                    }
                    var dialogTable = Lua.Run("return Conversation[" + conversation.id + "].Dialog").AsLuaTable;
                    var first = true;
                    for (int i = 0; i < conversation.dialogueEntries.Count; i++)
                    {
                        var entry = conversation.dialogueEntries[i];
                        var entryID = entry.id;
                        var dialogFields = dialogTable[entryID] as NLua.LuaTable;
                        if (dialogFields != null)
                        {
                            if (!first) sb.Append(";");
                            first = false;
                            sb.Append(useEntryID ? entryID.ToString() : Field.LookupValue(entry.fields, saveDialogueEntrySimStatusWithField));
                            sb.Append(";");
                            var simStatus = dialogFields["SimStatus"].ToString();
                            sb.Append(SimStatusToChar(simStatus));
                        }
                    }
                    sb.Append("\"; ");
                }
#else
                useConversationID = string.IsNullOrEmpty(saveConversationSimStatusWithField);
                useEntryID = string.IsNullOrEmpty(saveDialogueEntrySimStatusWithField);
                var conversationTable = Lua.environment.GetValue("Conversation") as Language.Lua.LuaTable;
                if (conversationTable == null) return;
                for (int i = 0; i < conversationTable.List.Count; i++)
                {
                    var conversationID = i + 1;
                    var fieldTable = conversationTable.List[i] as Language.Lua.LuaTable;
                    AppendSimStatusForConversation(sb, conversationTable, conversationID, fieldTable);
                }
                foreach (var kvp in conversationTable.Dict)
                {
                    if (kvp.Key == null || kvp.Value == null || !(kvp.Value is Language.Lua.LuaTable)) continue;
                    var conversationID = Tools.StringToInt(kvp.Key.ToString());
                    var fieldTable = kvp.Value as Language.Lua.LuaTable;
                    AppendSimStatusForConversation(sb, conversationTable, conversationID, fieldTable);
                }
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: GetSaveData() failed to get conversation data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Appends the SimStatus info for a single conversation.
        /// </summary>
        /// <returns>Returns the number of dialogue entries in the conversation.</returns>
        private static int AppendSimStatusForConversation(StringBuilder sb, Language.Lua.LuaTable conversationTable, int conversationID, Language.Lua.LuaTable fieldTable)
        {
            if (sb == null || conversationTable == null || fieldTable == null) return 0;
            var dialogTable = fieldTable.GetValue("Dialog") as Language.Lua.LuaTable;
            if (dialogTable == null) return 0;
            var conversation = DialogueManager.masterDatabase.GetConversation(conversationID);
            if (conversation == null) return 0;
            if (useConversationID)
            {
                sb.AppendFormat("Conversation[{0}].SimX=\"", conversationID);
            }
            else
            {
                sb.AppendFormat("Variable[\"Conversation_SimX_{0}\"]=\"", DialogueLua.StringToTableIndex(conversation.LookupValue(saveConversationSimStatusWithField)));
            }
            var first = true;
            for (int i = 0; i < dialogTable.List.Count; i++)
            {
                var entryID = i + 1;
                var entryIDString = entryID.ToString();
                var simStatusTable = dialogTable.List[i] as Language.Lua.LuaTable;
                if (!first) sb.Append(";");
                first = false;
                if (useEntryID)
                {
                    sb.Append(entryIDString);
                }
                else
                {
                    var entry = conversation.GetDialogueEntry(entryID);
                    var fieldName = (entry != null) ? Field.LookupValue(entry.fields, saveDialogueEntrySimStatusWithField) : entryIDString;
                    sb.Append(fieldName);
                }
                sb.Append(";");
                var simStatus = simStatusTable.GetValue("SimStatus").ToString();
                sb.Append(SimStatusToChar(simStatus));
            }
            foreach (var kvp2 in dialogTable.KeyValuePairs)
            {
                var entryIDString = kvp2.Key.ToString();
                var entryID = Tools.StringToInt(entryIDString);
                var simStatusTable = kvp2.Value as Language.Lua.LuaTable;
                if (!first) sb.Append(";");
                first = false;
                if (useEntryID)
                {
                    sb.Append(entryIDString);
                }
                else
                {
                    var entry = conversation.GetDialogueEntry(entryID);
                    var field = (entry != null) ? Field.Lookup(entry.fields, saveDialogueEntrySimStatusWithField) : null;
                    var fieldName = (field != null) ? field.value : entryIDString;
                    sb.Append(fieldName);
                }
                sb.Append(";");
                var simStatus = simStatusTable.GetValue("SimStatus").ToString();
                sb.Append(SimStatusToChar(simStatus));
            }
            sb.Append("\"; ");
            return conversation.dialogueEntries.Count;
        }

        /// <summary>
        /// When reapplying saved data, expands compress SimX info into conversations' SimStatus tables.
        /// </summary>
        private static void ExpandCompressedSimStatusData()
        {
            if (!(includeSimStatus && DialogueManager.instance.includeSimStatus)) return;
            try
            {
                var luaStringSimX = new Language.Lua.LuaString("SimX");
                useConversationID = string.IsNullOrEmpty(saveConversationSimStatusWithField);
                useEntryID = string.IsNullOrEmpty(saveDialogueEntrySimStatusWithField);
                var conversationTable = Lua.environment.GetValue("Conversation") as Language.Lua.LuaTable;
                if (conversationTable == null) return;
                var sb = new StringBuilder();
                for (int i = 0; i < conversationTable.List.Count; i++)
                {
                    var conversationID = i + 1;
                    var fieldTable = conversationTable.List[i] as Language.Lua.LuaTable;
                    ExpandSimStatusForConversation(sb, conversationID, conversationID.ToString(), fieldTable, luaStringSimX);
                }
                foreach (var kvp in conversationTable.Dict)
                {
                    if (kvp.Key == null || kvp.Value == null || !(kvp.Value is Language.Lua.LuaTable)) continue;
                    var conversationIDString = kvp.Key.ToString();
                    var conversationID = Tools.StringToInt(conversationIDString);
                    var fieldTable = kvp.Value as Language.Lua.LuaTable;
                    ExpandSimStatusForConversation(sb, conversationID, conversationIDString, fieldTable, luaStringSimX);
                }
                Lua.Run(sb.ToString());


            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: ApplySaveData() failed to re-expand compressed SimStatus data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Expands SimX for a conversation.
        /// </summary>
        private static void ExpandSimStatusForConversation(StringBuilder sb, int conversationID, string conversationIDString, Language.Lua.LuaTable fieldTable, Language.Lua.LuaString luaStringSimX)
        {
            var dialogTable = fieldTable.GetValue("Dialog") as Language.Lua.LuaTable;
            if (dialogTable == null)
            {
                dialogTable = new Language.Lua.LuaTable();
                fieldTable.AddRaw("Dialog", dialogTable);
            }
            dialogTable.List.Clear();
            dialogTable.Dict.Clear();
            var conversation = DialogueManager.masterDatabase.GetConversation(conversationID);
            if (conversation == null) return;

            string simX;
            if (useConversationID)
            {
                var simXLuaValue = fieldTable.GetValue(luaStringSimX);
                if (simXLuaValue == null) return;
                simX = simXLuaValue.ToString();
                sb.AppendFormat("Conversation[{0}].SimX=nil;", conversationIDString);
            }
            else
            {
                var fieldValue = DialogueLua.StringToTableIndex(conversation.LookupValue(saveConversationSimStatusWithField));
                if (string.IsNullOrEmpty(fieldValue)) fieldValue = conversation.id.ToString();
                simX = Lua.Run("return Variable[\"Conversation_SimX_" + fieldValue + "\"]").asString;
                sb.Append("Variable[\"Conversation_SimX_" + fieldValue + "\"]=nil;");
            }
            if (string.IsNullOrEmpty(simX) || string.Equals(simX, "nil")) Debug.Log("ERR " + conversationID); //!!!
            if (string.IsNullOrEmpty(simX) || string.Equals(simX, "nil")) return;

            var simXFields = simX.Split(';');
            var numFields = simXFields.Length / 2;

            Dictionary<string, int> simStatusFieldValueToID = null;
            if (!useEntryID)
            {
                simStatusFieldValueToID = new Dictionary<string, int>();
                for (int i = 0; i < conversation.dialogueEntries.Count; i++)
                {
                    var entry = conversation.dialogueEntries[i];
                    var field = (entry != null) ? Field.Lookup(entry.fields, saveDialogueEntrySimStatusWithField) : null;
                    simStatusFieldValueToID[(field != null) ? field.value : entry.id.ToString()] = entry.id;
                }
            }

            for (int i = 0; i < numFields; i++)
            {
                var simXEntryIDValue = simXFields[2 * i];
                var simStatus = CharToSimStatus(simXFields[(2 * i) + 1][0]);
                var simStatusTable = new Language.Lua.LuaTable();
                simStatusTable.AddRaw("SimStatus", new Language.Lua.LuaString(simStatus));
                if (useEntryID)
                {
                    dialogTable.AddRaw(Tools.StringToInt(simXEntryIDValue), simStatusTable);
                }
                else
                {
                    if (simStatusFieldValueToID.ContainsKey(simXEntryIDValue))
                    {
                        dialogTable.AddRaw(simStatusFieldValueToID[simXEntryIDValue], simStatusTable);
                    }
                }
            }
        }

        private static char SimStatusToChar(string simStatus)
        {
            switch (simStatus)
            {
                default:
                    return 'X';
                case "Untouched":
                    return 'u';
                case "WasDisplayed":
                    return 'd';
                case "WasOffered":
                    return 'o';
            }
        }

        private static string CharToSimStatus(char c)
        {
            switch (c)
            {
                default:
                    return "ERROR";
                case 'u':
                    return "Untouched";
                case 'd':
                    return "WasDisplayed";
                case 'o':
                    return "WasOffered";
            }
        }

        /// <summary>
        /// Appends the relationship and status tables to a (saved-game) string.
        /// </summary>
        /// <param name="sb">StringBuilder to append to.</param>
        public static void AppendRelationshipAndStatusTables(StringBuilder sb)
        {
            try
            {
                sb.Append(DialogueLua.GetStatusTableAsLua());
                sb.Append(DialogueLua.GetRelationshipTableAsLua());
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: GetSaveData() failed to get relationship and status data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Instructs the Dialogue System to refresh its internal relationship and status tables
        /// from the values in the Lua environment. Call this after putting new values in the
        /// Lua environment, such as when loading a saved game.
        /// </summary>
        public static void RefreshRelationshipAndStatusTablesFromLua()
        {
            DialogueLua.RefreshStatusTableFromLua();
            DialogueLua.RefreshRelationshipTableFromLua();
        }

        /// <summary>
        /// Instructs the Dialogue System to add any missing variables that are in the master 
        /// database but not in Lua.
        /// </summary>
        public static void InitializeNewVariablesFromDatabase()
        {
            try
            {
                LuaTableWrapper variableTable = Lua.Run("return Variable").asTable;
                if (variableTable == null)
                {
                    if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: Persistent Data Manager couldn't access Lua Variable[] table", new System.Object[] { DialogueDebug.Prefix }));
                    return;
                }
                var database = DialogueManager.masterDatabase;
                if (database == null) return;
                var inLua = new HashSet<string>(variableTable.keys);
                for (int i = 0; i < database.variables.Count; i++)
                {
                    var variable = database.variables[i];
                    var variableName = variable.Name;
                    var variableIndex = DialogueLua.StringToTableIndex(variableName);
                    if (!inLua.Contains(variableIndex))
                    {
                        switch (variable.Type)
                        {
                            case FieldType.Boolean:
                                DialogueLua.SetVariable(variableName, variable.InitialBoolValue);
                                break;
                            case FieldType.Actor:
                            case FieldType.Item:
                            case FieldType.Location:
                            case FieldType.Number:
                                DialogueLua.SetVariable(variableName, variable.InitialFloatValue);
                                break;
                            default:
                                DialogueLua.SetVariable(variableName, variable.InitialValue);
                                break;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: InitializeNewVariablesFromDatabase() failed to get variable data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Instructs the Dialogue System to add any missing quest entries that are in the master 
        /// database but not in Lua.
        /// </summary>
        public static void InitializeNewQuestEntriesFromDatabase()
        {
            try
            {
                var luaCode = string.Empty;
                var database = DialogueManager.MasterDatabase;
                if (database == null) return;
                for (int i = 0; i < database.items.Count; i++)
                {
                    if (database.items[i].IsItem) continue;
                    var dbQuest = database.items[i];
                    var questName = dbQuest.Name;
                    var dbEntryCount = dbQuest.LookupInt("Entry Count");
                    var luaEntryCount = DialogueLua.GetQuestField(questName, "Entry Count").AsInt;
                    if (luaEntryCount < dbEntryCount)
                    {
                        luaCode += "Item[\"" + DialogueLua.StringToTableIndex(questName) + "\"].Entry_Count=" + dbEntryCount + "; ";
                        for (int j = 0; j < dbQuest.fields.Count; j++)
                        {
                            var field = dbQuest.fields[j];
                            if (field.title.StartsWith("Entry ") && !field.title.EndsWith(" Count"))
                            {
                                luaCode += "Item[\"" + DialogueLua.StringToTableIndex(questName) + "\"]." +
                                    DialogueLua.StringToTableIndex(field.title) + " = " +
                                    DialogueLua.ValueAsString(field.type, field.value) + "; ";
                            }
                        }
                    }
                    Lua.Run(luaCode);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("{0}: InitializeNewQuestEntriesFromDatabase() failed to get quest data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
            }
        }

        /// <summary>
        /// Initializes SimStatus for entries that were added to the database after the saved game.
        /// </summary>
        public static void InitializeNewSimStatusFromDatabase()
        {
            if (!(includeSimStatus && initializeNewSimStatus)) return;
            try
            {
                var database = DialogueManager.MasterDatabase;
                if (database == null) return;
                var missingConversations = new List<Conversation>();
                var fakeLoadedDatabases = new List<DialogueDatabase>();
                var luaCode = string.Empty;
                for (int i = 0; i < database.conversations.Count; i++)
                {
                    var conversation = database.conversations[i];
                    var convTableIdent = "Conversation[" + conversation.id + "]";
                    var convTable = Lua.Run("return " + convTableIdent).AsTable;
                    if (!convTable.IsValid)
                    {
                        missingConversations.Add(conversation);
                    }
                    else
                    {
                        for (int j = 0; j < conversation.dialogueEntries.Count; j++)
                        {
                            var entry = conversation.dialogueEntries[j];
                            var dialogTableIdent = convTableIdent + ".Dialog[" + entry.id + "]";
                            var entryTable = Lua.Run("return " + dialogTableIdent).AsTable;
                            if (!entryTable.IsValid)
                            {
                                luaCode += dialogTableIdent + "={SimStatus=\"Untouched\"}; ";
                            }
                        }
                    }
                }
                Lua.Run(luaCode, DialogueDebug.LogInfo);
                DialogueLua.AddToConversationTable(missingConversations, fakeLoadedDatabases);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Dialogue System: InitializeNewSimStatusFromDatabase() failed: " + e.Message);
            }
        }

        //======================================================================
        // Asynchronous saving:

        public static int asyncGameObjectBatchSize = 1000;
        public static int asyncDialogueEntryBatchSize = 100;

        public class AsyncSaveOperation
        {
            public bool isDone = false;
            public string content = string.Empty;
        }

        public static AsyncSaveOperation GetSaveDataAsync()
        {
            var asyncOp = new AsyncSaveOperation();
            DialogueManager.instance.StartCoroutine(GetSaveDataAsyncCoroutine(asyncOp));
            return asyncOp;
        }

        private static IEnumerator GetSaveDataAsyncCoroutine(AsyncSaveOperation asyncOp)
        {
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Saving data asynchronously...", new System.Object[] { DialogueDebug.Prefix, asyncGameObjectBatchSize }));

            switch (recordPersistentDataOn)
            {
                case RecordPersistentDataOn.AllGameObjects:
                    yield return DialogueManager.instance.StartCoroutine(Tools.SendMessageToEveryoneAsync("OnRecordPersistentData", asyncGameObjectBatchSize));
                    break;
                case RecordPersistentDataOn.OnlyRegisteredGameObjects:
                    int count = 0;
                    foreach (var go in listeners)
                    {
                        if (go != null)
                        {
                            go.SendMessage("OnRecordPersistentData", SendMessageOptions.DontRequireReceiver);
                            count++;
                            if (count > asyncGameObjectBatchSize)
                            {
                                count = 0;
                                yield return null;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            StringBuilder sb = new StringBuilder(); //---Was: new StringBuilder(stringDataStartCapacity, stringDataMaxCapacity);
            AppendVariableData(sb);
            yield return null;
            AppendItemData(sb);
            yield return null;
            AppendLocationData(sb);
            yield return null;
            if (includeActorData) AppendActorData(sb);
            yield return null;
            yield return DialogueManager.instance.StartCoroutine(AppendConversationDataAsync(sb));
            yield return null;
            if (includeRelationshipAndStatusData) AppendRelationshipAndStatusTables(sb);
            if (GetCustomSaveData != null) sb.Append(GetCustomSaveData());
            string saveData = sb.ToString();
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Saved data asynchronously: {1}", new System.Object[] { DialogueDebug.Prefix, saveData }));
            asyncOp.content = saveData;
            asyncOp.isDone = true;
        }

        /// <summary>
        /// Sends the OnRecordPersistentData message to all game objects in the scene to give them 
        /// an opportunity to record their state in the Lua environment. Runs in batches specified
        /// by  the value of asyncGameObjectBatchSize.
        /// </summary>
        public static void RecordAsync()
        {
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Recording persistent data to Lua environment in batches of {1} GameObjects.", new System.Object[] { DialogueDebug.Prefix, asyncGameObjectBatchSize }));
            DialogueManager.instance.StartCoroutine(Tools.SendMessageToEveryoneAsync("OnRecordPersistentData", asyncGameObjectBatchSize));
        }

        private static IEnumerator AppendConversationDataAsync(StringBuilder sb)
        {
            if (includeAllConversationFields || DialogueManager.instance.persistentDataSettings.includeAllConversationFields)
            {
                AppendAllConversationFields(sb);
            }
            if (includeSimStatus && DialogueManager.instance.includeSimStatus)
            {
                var count = 0;
#if USE_NLUA
                var useConversationID = string.IsNullOrEmpty(saveConversationSimStatusWithField);
                var useEntryID = string.IsNullOrEmpty(saveDialogueEntrySimStatusWithField);
                foreach (var conversation in DialogueManager.MasterDatabase.conversations)
                {
                    if (useConversationID)
                    {
                        sb.AppendFormat("Conversation[{0}].SimX=\"", conversation.id);
                    }
                    else
                    {
                        var fieldValue = DialogueLua.StringToTableIndex(conversation.LookupValue(saveConversationSimStatusWithField));
                        if (string.IsNullOrEmpty(fieldValue)) fieldValue = conversation.id.ToString();
                        sb.AppendFormat("Variable[\"Conversation_SimX_{0}\"]=\"", fieldValue);
                    }
                    var dialogTable = Lua.Run("return Conversation[" + conversation.id + "].Dialog").AsLuaTable;
                    var first = true;
                    for (int i = 0; i < conversation.dialogueEntries.Count; i++)
                    {
                        try
                        {
                            var entry = conversation.dialogueEntries[i];
                            var entryID = entry.id;
                            var dialogFields = dialogTable[entryID] as NLua.LuaTable;
                            if (dialogFields != null)
                            {
                                if (!first) sb.Append(";");
                                first = false;
                                sb.Append(useEntryID ? entryID.ToString() : Field.LookupValue(entry.fields, saveDialogueEntrySimStatusWithField));
                                sb.Append(";");
                                var simStatus = dialogFields["SimStatus"].ToString();
                                sb.Append(SimStatusToChar(simStatus));
                            }
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError(string.Format("{0}: GetSaveData() failed to get conversation data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
                        }
                        count++;
                        if (count >= asyncDialogueEntryBatchSize)
                        {
                            count = 0;
                            yield return null;
                        }
                    }
                    sb.Append("\"; ");
                }
#else
                useConversationID = string.IsNullOrEmpty(saveConversationSimStatusWithField);
                useEntryID = string.IsNullOrEmpty(saveDialogueEntrySimStatusWithField);
                var conversationTable = Lua.environment.GetValue("Conversation") as Language.Lua.LuaTable;
                if (conversationTable == null) yield break;
                for (int i = 0; i < conversationTable.List.Count; i++)
                {
                    var conversationID = i + 1;
                    var fieldTable = conversationTable.List[i] as Language.Lua.LuaTable;
                    count += AppendSimStatusForConversation(sb, conversationTable, conversationID, fieldTable);
                    if (count >= asyncDialogueEntryBatchSize)
                    {
                        count = 0;
                        yield return null;
                    }
                }
                foreach (var kvp in conversationTable.Dict)
                {
                    if (kvp.Key == null || kvp.Value == null || !(kvp.Value is Language.Lua.LuaTable)) continue;
                    var conversationID = Tools.StringToInt(kvp.Key.ToString());
                    var fieldTable = kvp.Value as Language.Lua.LuaTable;
                    count += AppendSimStatusForConversation(sb, conversationTable, conversationID, fieldTable);
                    if (count >= asyncDialogueEntryBatchSize)
                    {
                        count = 0;
                        yield return null;
                    }
                }


                //--- Old:
                //var useConversationID = string.IsNullOrEmpty(saveConversationSimStatusWithField);
                //var useEntryID = string.IsNullOrEmpty(saveDialogueEntrySimStatusWithField);
                //foreach (var conversation in DialogueManager.masterDatabase.conversations)
                //{
                //    if (useConversationID)
                //    {
                //        sb.AppendFormat("Conversation[{0}].SimX=\"", conversation.id);
                //    }
                //    else
                //    {
                //        sb.AppendFormat("Variable[\"Conversation_SimX_{0}\"]=\"", DialogueLua.StringToTableIndex(conversation.LookupValue(saveConversationSimStatusWithField)));
                //    }
                //    var conversationTable = Lua.Run("return Conversation[" + conversation.id + "]").asTable;
                //    var dialogTable = conversationTable.luaTable.GetValue("Dialog") as Language.Lua.LuaTable;
                //    var first = true;
                //    for (int i = 0; i < conversation.dialogueEntries.Count; i++)
                //    {
                //        try
                //        {
                //            var entry = conversation.dialogueEntries[i];
                //            var entryID = entry.id;
                //            var dialogFields = dialogTable.GetValue(entryID) as Language.Lua.LuaTable;
                //            if (dialogFields != null)
                //            {
                //                if (!first) sb.Append(";");
                //                first = false;
                //                sb.Append(useEntryID ? entryID.ToString() : Field.LookupValue(entry.fields, saveDialogueEntrySimStatusWithField));
                //                sb.Append(";");
                //                var simStatus = dialogFields.GetValue("SimStatus").ToString();
                //                sb.Append(SimStatusToChar(simStatus));
                //            }
                //        }
                //        catch (System.Exception e)
                //        {
                //            Debug.LogError(string.Format("{0}: GetSaveData() failed to get conversation data: {1}", new System.Object[] { DialogueDebug.Prefix, e.Message }));
                //        }
                //        count++;
                //        if (count >= asyncDialogueEntryBatchSize)
                //        {
                //            count = 0;
                //            yield return null;
                //        }
                //    }
                //    sb.Append("\"; ");
                //}
#endif
            }
        }

    }

}
