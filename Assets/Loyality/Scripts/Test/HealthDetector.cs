using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Loyality
{
    public class HealthDetector : MonoBehaviour
    {

        public Character character;

        // Update is called once per frame
        void Update()
        {
            Debug.Log(character.GetHealth());
        }
    }
}


