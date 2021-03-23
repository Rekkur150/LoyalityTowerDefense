using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class ArrowAOE : AreaOfEffect
    {

        [Header("ArrowAOE")]
        [Tooltip("The damage of the AOE")]
        public float Damage;

        [Tooltip("The number of frames before the AOE destorys itself")]
        public int FramesBeforeDestory;

        [Tooltip("The current frame of this AOE")]
        private int CurrentFrame = 0;

        protected override void Effect(GameObject character)
        {
            if (isServer)
            {
                Character charObj;

                character.TryGetComponent(out Character temp);

                if (temp == null)
                {
                    character.transform.parent.gameObject.TryGetComponent(out Character temp2);

                    if (temp2 == null) return;

                    charObj = temp2;
                } else
                {
                    charObj = temp;
                }

                charObj.SetHealth(charObj.GetHealth() - Damage);
            }
        }

        private void LateUpdate()
        {
            if (isServer)
            {
                if (characters.Count > 0)
                {
                    ApplyAreaOfEffect();
                    ServerDestroy();
                }

                if (CurrentFrame++ > FramesBeforeDestory)
                {
                    ServerDestroy();
                }
            }
        }
    }
}


