﻿using Assets.Scripts.Messages;
using Assets.Scripts.Messages.Input;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Actors.Character
{
    public class CharacterInput : MonoBehaviour
    {
        private bool _mousePressed;
        private bool _previousShiftKey;

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mousePressed = true;
            }

            if (Input.GetMouseButtonDown(1))
            {
                this.GetPubSub().PublishMessageInContext(new FireInputMessage(isSecondary:true));
            }

            bool newShiftKey = Input.GetKey(KeyCode.LeftShift);
            if (_previousShiftKey != newShiftKey)
            {
                _previousShiftKey = newShiftKey;
                this.GetPubSub().PublishMessageInContext(new ShieldInputMessage(newShiftKey));
            }
        }

        public void FixedUpdate()
        {
            float xMovement = Input.GetAxisRaw("Horizontal");
            float yMovement = Input.GetAxisRaw("Vertical");

            var movement = new Vector2(xMovement, yMovement);

            if (_mousePressed)
            {
                this.GetPubSub().PublishMessageInContext(new FireInputMessage());
                _mousePressed = false;
            }

            this.GetPubSub().PublishMessageInContext(new MoveInputMessage(movement.normalized));
        }
    }
}
