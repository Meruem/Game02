using System;
using System.Collections.Generic;
using Assets.Scripts.Messages;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class PubSub : MonoBehaviour
    {
        private readonly Dictionary<Type, List<Action<IMessage>>> _subscribers = new Dictionary<Type, List<Action<IMessage>>>();

        private static PubSub _globalPubSub;

        public static PubSub GlobalPubSub
        {
            get
            {
                return _globalPubSub ?? (_globalPubSub = new PubSub()); 
            }
        }

        public bool IsContextAware = false;
        
        /// <summary>
        /// Publishes message only in current game object scope.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public void PublishMessage<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            List<Action<IMessage>> list;
            if (_subscribers.TryGetValue(typeof (TMessage), out list))
            {
                if (list == null) return;

                for (var i = 0; i < list.Count; i++)
                {
                    list[i](message);
                }
            }
        }

        /// <summary>
        /// Publishes message up the game object tree until it finds game object with IsContextAware = true
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public void PublishMessageInContext<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            var context = FindContextAwareObject();
            if (context == null) return;

            context.PublishMessage(message);
        }

        private PubSub FindContextAwareObject()
        {
            var context = gameObject.GetComponent<PubSub>();
            var go = gameObject;
            while ((context == null || !context.IsContextAware) && go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                context = go.GetComponent<PubSub>();
            }

            if (context == null || !context.IsContextAware) return null;
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
        public void Subscribe<TMessage>(Action<IMessage> action)
            where TMessage : IMessage
        {
            List<Action<IMessage>> list;
            var found = _subscribers.TryGetValue(typeof (TMessage), out list);
            if (!found)
            {
                list = new List<Action<IMessage>>();
                _subscribers[typeof (TMessage)] = list; 
            }

            list.Add(action);
        }

        public void SubscribeInContext<TMessage>(Action<IMessage> action)
            where TMessage : IMessage
        {
            var context = FindContextAwareObject();
            if (context == null) return;

            context.Subscribe<TMessage>(action);
        }

        /// <summary>
        /// Subscribes to global message.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="action"></param>
        public void SubscribeGlobal<TMessage>(Action<IMessage> action)
            where TMessage : IMessage
        {
            GlobalPubSub.Subscribe<TMessage>(action);
        }
    }
}
