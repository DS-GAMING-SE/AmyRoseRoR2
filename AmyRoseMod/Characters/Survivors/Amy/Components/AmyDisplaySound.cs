using UnityEngine;
using RoR2;

namespace AmyRoseMod.Characters.Survivors.Amy.Components
{
    public class AmyDisplaySound : MonoBehaviour
    {
        private void Start()
        {
            Util.PlaySound("Play_amyrose_display", gameObject);
        }
    }
}