// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This class defines the template that the Dialogue Database Editor will use when creating
    /// new dialogue database assets such as actors and conversations. The Dialogue Database Editor
    /// stores a copy of the template in EditorPrefs using the TemplateTools class. The equivalent 
    /// in Chat Mapper is Project Preferences.
    /// </summary>
    [System.Serializable]
    public class Template
    {

        public bool treatItemsAsQuests = true;

        public List<Field> actorFields = new List<Field>();
        public List<Field> itemFields = new List<Field>();
        public List<Field> questFields = new List<Field>();
        public List<Field> locationFields = new List<Field>();
        public List<Field> variableFields = new List<Field>();
        public List<Field> conversationFields = new List<Field>();
        public List<Field> dialogueEntryFields = new List<Field>();

        public List<string> actorPrimaryFieldTitles = new List<string>();
        public List<string> itemPrimaryFieldTitles = new List<string>();
        public List<string> questPrimaryFieldTitles = new List<string>();
        public List<string> locationPrimaryFieldTitles = new List<string>();
        public List<string> variablePrimaryFieldTitles = new List<string>();
        public List<string> conversationPrimaryFieldTitles = new List<string>();
        public List<string> dialogueEntryPrimaryFieldTitles = new List<string>();

        public Color npcLineColor = Color.red;
        public Color pcLineColor = Color.blue;
        public Color repeatLineColor = Color.gray;

        public static Template FromDefault()
        {
            Template template = new Template();
            template.actorFields.Clear();
            template.actorFields.Add(new Field("Name", string.Empty, FieldType.Text));
            template.actorFields.Add(new Field("Pictures", "[]", FieldType.Files));
            template.actorFields.Add(new Field("Description", string.Empty, FieldType.Text));
            template.actorFields.Add(new Field("IsPlayer", "False", FieldType.Boolean));

            template.itemFields.Clear();
            template.itemFields.Add(new Field("Name", string.Empty, FieldType.Text));
            template.itemFields.Add(new Field("Pictures", "[]", FieldType.Files));
            template.itemFields.Add(new Field("Description", string.Empty, FieldType.Text));
            template.itemFields.Add(new Field("Is Item", "True", FieldType.Boolean));

            template.questFields.Clear();
            template.questFields.Add(new Field("Name", string.Empty, FieldType.Text));
            template.questFields.Add(new Field("Pictures", "[]", FieldType.Files));
            template.questFields.Add(new Field("Description", string.Empty, FieldType.Text));
            template.questFields.Add(new Field("Success Description", string.Empty, FieldType.Text));
            template.questFields.Add(new Field("Failure Description", string.Empty, FieldType.Text));
            template.questFields.Add(new Field("State", "unassigned", FieldType.Text));
            template.questFields.Add(new Field("Is Item", "False", FieldType.Boolean));

            template.locationFields.Clear();
            template.locationFields.Add(new Field("Name", string.Empty, FieldType.Text));
            template.locationFields.Add(new Field("Pictures", "[]", FieldType.Files));
            template.locationFields.Add(new Field("Description", string.Empty, FieldType.Text));

            template.variableFields.Add(new Field("Name", string.Empty, FieldType.Text));
            template.variableFields.Add(new Field("Initial Value", string.Empty, FieldType.Text));
            template.variableFields.Add(new Field("Description", string.Empty, FieldType.Text));

            template.conversationFields.Add(new Field("Title", string.Empty, FieldType.Text));
            template.conversationFields.Add(new Field("Pictures", "[]", FieldType.Files));
            template.conversationFields.Add(new Field("Description", string.Empty, FieldType.Text));
            template.conversationFields.Add(new Field("Actor", "0", FieldType.Actor));
            template.conversationFields.Add(new Field("Conversant", "0", FieldType.Actor));

            template.dialogueEntryFields.Add(new Field("Title", string.Empty, FieldType.Text));
            template.dialogueEntryFields.Add(new Field("Pictures", "[]", FieldType.Files));
            template.dialogueEntryFields.Add(new Field("Description", string.Empty, FieldType.Text));
            template.dialogueEntryFields.Add(new Field("Actor", string.Empty, FieldType.Actor));
            template.dialogueEntryFields.Add(new Field("Conversant", string.Empty, FieldType.Actor));
            template.dialogueEntryFields.Add(new Field("Menu Text", string.Empty, FieldType.Text));
            template.dialogueEntryFields.Add(new Field("Dialogue Text", string.Empty, FieldType.Text));
            template.dialogueEntryFields.Add(new Field("Parenthetical", string.Empty, FieldType.Text));
            template.dialogueEntryFields.Add(new Field("Audio Files", "[]", FieldType.Files));
            template.dialogueEntryFields.Add(new Field("Video File", string.Empty, FieldType.Text));
            template.dialogueEntryFields.Add(new Field("Sequence", string.Empty, FieldType.Text));

            return template;
        }

        public Actor CreateActor(int id, string name, bool isPlayer)
        {
            Actor actor = new Actor();
            actor.fields = CreateFields(actorFields);
            actor.id = id;
            actor.Name = name;
            actor.IsPlayer = isPlayer;
            return actor;
        }

        public Item CreateItem(int id, string name)
        {
            Item item = new Item();
            item.id = id;
            item.fields = CreateFields(itemFields);
            item.Name = name;
            return item;
        }

        public Location CreateLocation(int id, string name)
        {
            Location location = new Location();
            location.id = id;
            location.fields = CreateFields(locationFields);
            location.Name = name;
            return location;
        }

        public Variable CreateVariable(int id, string name, string value)
        {
            Variable variable = new Variable();
            variable.fields = CreateFields(variableFields);
            variable.id = id;
            variable.Name = name;
            variable.InitialValue = value;
            return variable;
        }

        public Variable CreateVariable(int id, string name, string value, FieldType type)
        {
            Variable variable = new Variable();
            variable.fields = CreateFields(variableFields);
            variable.id = id;
            variable.Name = name;
            variable.InitialValue = value;
            variable.Type = type;
            return variable;
        }

        public Conversation CreateConversation(int id, string title)
        {
            Conversation conversation = new Conversation();
            conversation.id = id;
            conversation.fields = CreateFields(conversationFields);
            conversation.Title = title;
            return conversation;
        }

        public DialogueEntry CreateDialogueEntry(int id, int conversationID, string title)
        {
            DialogueEntry entry = new DialogueEntry();
            entry.fields = CreateFields(dialogueEntryFields);
            entry.id = id;
            entry.conversationID = conversationID;
            entry.Title = title;
            return entry;
        }

        public List<Field> CreateFields(List<Field> templateFields)
        {
            List<Field> fields = new List<Field>();
            foreach (var templateField in templateFields)
            {
                fields.Add(new Field(templateField.title, templateField.value, templateField.type, templateField.typeString));
            }
            return fields;
        }

    }

}
