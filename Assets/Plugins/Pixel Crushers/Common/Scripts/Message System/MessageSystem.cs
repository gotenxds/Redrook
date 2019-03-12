// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// General purpose message system.
    /// </summary>
    public static class MessageSystem
    {

        public class ListenerInfo
        {
            public IMessageHandler listener;
            public string message;
            public string parameter;

            public ListenerInfo() { }

            public ListenerInfo(IMessageHandler listener, string message, string parameter)
            {
                this.listener = listener;
                this.message = message;
                this.parameter = parameter;
            }

            public void Assign(IMessageHandler listener, string message, string parameter)
            {
                this.listener = listener;
                this.message = message;
                this.parameter = parameter;
            }

            public void Clear()
            {
                this.listener = null;
                this.message = null;
                this.parameter = null;
            }
        }

        private static List<ListenerInfo> s_listenerInfo = new List<ListenerInfo>();

        private static Pool<ListenerInfo> s_listenerInfoPool = new Pool<ListenerInfo>();

        private static bool s_sendInEditMode = false;

        private static bool s_debug = false;

        /// <summary>
        /// Send messages even when not playing.
        /// </summary>
        public static bool sendInEditMode
        {
            get { return s_sendInEditMode; }
            set { s_sendInEditMode = value; }
        }

        /// <summary>
        /// Log message system activity.
        /// </summary>
        public static bool debug
        {
            get { return s_debug && Debug.isDebugBuild; }
            set { s_debug = value; }
        }

        private static List<ListenerInfo> listenerInfo { get { return s_listenerInfo; } }

        private static Pool<ListenerInfo> listenerInfoPool { get { return s_listenerInfoPool; } }

        /// <summary>
        /// Checks if the specified listener, message, and parameter is registered with the message system.
        /// </summary>
        /// <param name="listener">Listener to check.</param>
        /// <param name="message">Message to check.</param>
        /// <param name="parameter">Parameter to check, or blank for any parameter.</param>
        /// <returns></returns>
        public static bool IsListenerRegistered(IMessageHandler listener, string message, string parameter)
        {
            for (int i = 0; i < listenerInfo.Count; i++)
            {
                var x = listenerInfo[i];
                if (x.listener == listener && string.Equals(x.message, message) && (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(x.parameter)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, string message, string parameter)
        {
            if (debug) Debug.Log("MessageSystem.AddListener(listener=" + listener + ": " + message + "," + parameter + ")");
            if (IsListenerRegistered(listener, message, parameter)) return;
            var info = listenerInfoPool.Get();
            info.Assign(listener, message, parameter);
            listenerInfo.Add(info);
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, StringField message, StringField parameter)
        {
            AddListener(listener, StringField.GetStringValue(message), StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, StringField message, string parameter)
        {
            AddListener(listener, StringField.GetStringValue(message), parameter);
        }

        /// <summary>
        /// Adds a listener.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to listen for.</param>
        /// <param name="parameter">Message parameter to listen for, or blank for any parameter with the message.</param>
        public static void AddListener(IMessageHandler listener, string message, StringField parameter)
        {
            AddListener(listener, message, StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for.</param>
        /// <param name="parameter">Messaeg parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, string message, string parameter)
        {
            if (debug) Debug.Log("MessageSystem.RemoveListener(listener=" + listener + ": " + message + "," + parameter + ")");
            if (listenerInfo.Count <= 0) return;
            for (int i = listenerInfo.Count - 1; i >= 0; i--)
            {
                var x = listenerInfo[i];
                if (x.listener == listener && string.Equals(x.message, message) && string.Equals(x.parameter, parameter))
                {
                    listenerInfo.RemoveAt(i);
                    x.Clear();
                    listenerInfoPool.Release(x);
                }
            }
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for.</param>
        /// <param name="parameter">Messaeg parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, StringField message, StringField parameter)
        {
            RemoveListener(listener, StringField.GetStringValue(message), StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for.</param>
        /// <param name="parameter">Messaeg parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, StringField message, string parameter)
        {
            RemoveListener(listener, StringField.GetStringValue(message), parameter);
        }

        /// <summary>
        /// Removes a listener from listening to a specific message and parameter.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="message">Message to no longer listen for.</param>
        /// <param name="parameter">Messaeg parameter, or blank for all parameters.</param>
        public static void RemoveListener(IMessageHandler listener, string message, StringField parameter)
        {
            RemoveListener(listener, message, StringField.GetStringValue(parameter));
        }

        /// <summary>
        /// Removes a listener from listening to all messages.
        /// </summary>
        public static void RemoveListener(IMessageHandler listener)
        {
            if (debug) Debug.Log("MessageSystem.RemoveListener(listener=" + listener + ")");
            listenerInfo.RemoveAll(x => (x.listener == listener));
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, string message, string parameter, params object[] values)
        {
            if (!(Application.isPlaying || sendInEditMode)) return;
            if (debug) Debug.Log("MessageSystem.SendMessage(sender=" + sender +
                ((target == null) ? string.Empty : (" target=" + target)) +
                ": " + message + "," + parameter + ")");
            var messageArgs = new MessageArgs(sender, target, message, parameter, values); // struct passed on stack; no heap allocated.
            for (int i = 0; i < listenerInfo.Count; i++)
            {
                var x = listenerInfo[i];
                if (string.Equals(x.message, message) && (string.Equals(x.parameter, parameter) || string.IsNullOrEmpty(x.parameter)))
                {
                    try
                    {
                        x.listener.OnMessage(messageArgs);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Message System exception sending '" + message + "'/'" + parameter + "' to " + x.listener + ": " + e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, StringField message, string parameter, params object[] values)
        {
            SendMessageWithTarget(sender, target, StringField.GetStringValue(message), parameter, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, StringField message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, target, StringField.GetStringValue(message), StringField.GetStringValue(parameter), values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="target">Intended recipient, or null for any.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessageWithTarget(object sender, object target, string message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, target, message, StringField.GetStringValue(parameter), values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, string message, string parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message, parameter, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, StringField message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message.value, parameter.value, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, StringField message, string parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message.value, parameter, values);
        }

        /// <summary>
        /// Sends a message to listeners.
        /// </summary>
        /// <param name="sender">Object/info about object that's sending the message.</param>
        /// <param name="message">Message.</param>
        /// <param name="parameter">Message parameter.</param>
        /// <param name="values">Any number of additional values to send with message.</param>
        public static void SendMessage(object sender, string message, StringField parameter, params object[] values)
        {
            SendMessageWithTarget(sender, null, message, parameter.value, values);
        }

        /// <summary>
        /// Sends a message. If the message contains a colon (:), the part after the 
        /// colon is sent as the parameter. If it contains a second colon, the part 
        /// after the second colon is sent as a value.
        /// </summary>
        public static void SendCompositeMessage(object sender, string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            var parameter = string.Empty;
            object value = null;
            if (message.Contains(":")) // Parameter?
            {
                var colonPos = message.IndexOf(':');
                parameter = message.Substring(colonPos + 1);
                message = message.Substring(0, colonPos);

                if (parameter.Contains(":")) // Value?
                {
                    colonPos = parameter.IndexOf(':');
                    var valueString = parameter.Substring(colonPos + 1);
                    parameter = parameter.Substring(0, colonPos);
                    int valueInt;
                    bool isNumeric = int.TryParse(valueString, out valueInt);
                    if (isNumeric) value = valueInt; else value = valueString;
                }
            }
            if (value == null)
            {
                SendMessage(sender, message, parameter);
            }
            else
            {
                SendMessage(sender, message, parameter, value);
            }
        }


    }
}
