using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Loyality
{
    public class MetalPickup : Object
    {
        [Header("Metal Pickup")]
        [Tooltip("The metal value of this pick up")]
        public float MetalValue;

        [Tooltip("The size multiplier of the metal fragment, value * size")]
        public float SizeMultiplier;

        [Tooltip("The metal fragment itself")]
        public GameObject MetalFragment;

        private void Start()
        {
            MetalFragment.transform.localScale = MetalFragment.transform.localScale * (MetalValue * SizeMultiplier);
            MetalFragment.transform.localPosition = new Vector3(0, (MetalFragment.transform.localScale.y * (MetalValue * SizeMultiplier))/2, 0);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (isServer)
            {
                other.gameObject.TryGetComponent(out Player temp);

                if (temp != null)
                {
                    temp.SetMetal(temp.GetMetal() + MetalValue);
                    ServerDestroy();
                }
            }
        }
    }
}

