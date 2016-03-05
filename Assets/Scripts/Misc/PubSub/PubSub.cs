using System;
using System.Collections.Generic;
using Assets.Scripts.Messages;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class PubSub : MonoBehaviour
    {
        private class SubscriberAction<T>
            where T : IMessage
        {
            public Action<T> Action { get; private set; }
            public Func<T, bool> Filter { get; private set; }

            public SubscriberAction(Action<T> action, Func<T, bool> filter = null)
            {
                Action = action;
                Filter = filter;
            }
        }

        private readonly Dictionary<Type, List<SubscriberAction<IMessage>>> _subscribers = new Dictionary<Type, List<SubscriberAction<IMessage>>>();

        private static PubSub _globalPubSub;
        private static PubSubSettings _pubSubSettings;

        public static PubSub GlobalPubSub
        {
            get
            {
                if (_globalPubSub == null)
                {
                    var globalPubSubGameObjectName = GameObjectNameMapping.GetObjectName(GameObjectNames.GlobalPubSub);
                    var go = GameObject.Find(globalPubSubGameObjectName) ?? new GameObject(globalPubSubGameObjectName);
                    _globalPubSub = go.GetComponent<PubSub>() ?? go.AddComponent<PubSub>();
                }

                return _globalPubSub;
            }
        }

        public static PubSubSettings PubSubSettings
        {
            get
            {
                if (_pubSubSettings == null)
                {
                    var name = GameObjectNameMapping.GetObjectName(GameObjectNames.PubSubSettings);
                    var go = GameObject.Find(name) ?? new GameObject(name);
                    _pubSubSettings = go.GetComponent<PubSubSettings>() ?? go.AddComponent<PubSubSettings>();
                }

                return _pubSubSettings;
            }
        }

        public bool IsRoot = false;
        
        /// <summary>
        /// Publishes message only in current game object scope.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public bool PublishMessage<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            if (PubSubSettings.DebugAllMessages)
            {
                Debug.LogFormat("[{0}] Published message: {1}", gameObject.GetInstanceID(), message);
            }

            List<SubscriberAction<IMessage>> list;
            if (_subscribers.TryGetValue(typeof (TMessage), out list))
            {
                if (list == null || list.Count == 0)
                {
                    if (PubSubSettings.DebugMissedMessages)
                    {
                        Debug.LogFormat("No subscriber was found for message of type {0}.", message);
                    }
                    return false;
                }

                for (var i = 0; i < list.Count; i++)
                {
                    var filter = list[i].Filter;
                    if (filter == null || filter(message))
                    {
                        list[i].Action(message);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Publishes message up the game object tree until it finds game object with IsRoot = true
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public void PublishMessageInContext<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var context = gameObject.GetComponent<PubSub>();
            var go = gameObject;
            while ((context == null || !context.IsRoot) && go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                context = go.GetComponent<PubSub>();
            }
            if (context == null) return;

            context.PublishMessage(message);
        }

        public void PublishBubbleMessage<TMessage>(TMessage message, bool stopWhenHandled)
            where TMessage : IMessage
        {
            var context = gameObject.GetComponent<PubSub>();
            var go = gameObject;
            while ((context == null || !context.IsRoot) && go.transform.parent != null)
            {
                if (context != null)
                {
                    var handled = context.PublishMessage(message);
                    if (stopWhenHandled && handled) return;
                }

                go = go.transform.parent.gameObject;
                context = go.GetComponent<PubSub>();
            }
            if (context == null) return;

            context.PublishMessage(message);
        }

        private PubSub FindRootContext()
        {
            var context = gameObject.GetComponent<PubSub>();
            var go = gameObject;
            while ((context == null || !context.IsRoot) && go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                context = go.GetComponent<PubSub>();
            }

            if (context == null || !context.IsRoot) return null;
            return context;
        }

        /// <summary>
        /// Publishes to global pub sub.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public void PublishMessageGlobal<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            _globalPubSub.PublishMessage(message);
        }

        /// <summary>
        /// Subscribes to local message.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="action"></param>
        /// <param name="filter"></param>
        public void Subscribe<TMessage>(Action<IMessage> action, Func<IMessage, bool> filter = null)
            where TMessage : IMessage
        {
            List<SubscriberAction<IMessage>> list;
            var found = _subscribers.TryGetValue(typeof (TMessage), out list);
            if (!found)
            {
                list = new List<SubscriberAction<IMessage>>();
                _subscribers[typeof (TMessage)] = list; 
            }

            list.Add(new SubscriberAction<IMessage>(action, filter));
        }

        public void SubscribeInContext<TMessage>(Action<IMessage> action, Func<IMessage, bool> filter = null)
            where TMessage : IMessage
        {
            var context = FindRootContext();
            if (context == null) return;

            context.Subscribe<TMessage>(action, filter);
        }

        /// <summary>
        /// Subscribes to global message.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="action"></param>
        /// <param name="filter"></param>
        public void SubscribeGlobal<TMessage>(Action<IMessage> action, Func<IMessage, bool> filter = null)
            where TMessage : IMessage
        {
            GlobalPubSub.Subscribe<TMessage>(action, filter);
        }
    }
}
