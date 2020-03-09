//----------------------------------------------
//            	   Koreographer
//    Copyright © 2014-2016 Sonic Bloom, LLC
//----------------------------------------------

using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Koreographer/Demos/Musical Impulse")]
    public class MusicalImpulse : MonoBehaviour
    {
        [EventID]
        public string eventID;

        public float jumpSpeed = 3f;
        public float postion;

        private Rigidbody rigidbodyCom;

        private void Start()
        {
            // Register for Koreography Events.  This sets up the callback.
            Koreographer.Instance.RegisterForEvents(eventID, AddImpulse);

            rigidbodyCom = GetComponent<Rigidbody>();
        }

        private void OnDestroy()
        {
            // Sometimes the Koreographer Instance gets cleaned up before hand.
            //  No need to worry in that case.
            if (Koreographer.Instance != null)
            {
                Koreographer.Instance.UnregisterForAllEvents(this);
            }
        }

        private void AddImpulse(KoreographyEvent evt)
        {
            // Add impulse by overriding the Vertical component of the Velocity.
            Vector3 vel = rigidbodyCom.velocity;
            vel.y = jumpSpeed;

            rigidbodyCom.velocity = vel;
            transform.position = new Vector3(evt.GetFloatValue() * 0.1f, 0, 0);
        }
    }
}