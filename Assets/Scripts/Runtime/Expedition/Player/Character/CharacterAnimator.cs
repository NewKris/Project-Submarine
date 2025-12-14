using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using WereHorse.Runtime.Common;

namespace WereHorse.Runtime.Expedition.Player.Character {
    public class CharacterAnimator : NetworkAnimator {
        public void SetMoving(bool value) {
            if (IsOwner) {
                Animator.SetBool("Moving", value);
            }
        }

        public void SetMoveDirection(Vector2 value) {
            if (IsOwner) {
                Animator.SetFloat("Move X", value.x);
                Animator.SetFloat("Move Y", value.y);
            }
        }

        public void SetIsCarrying(bool value) {
            if (IsOwner) {
                Animator.SetLayerWeight(1, value ? 1 : 0);
            }
        }

        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }
}