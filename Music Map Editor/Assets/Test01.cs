using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonicBloom.Koreo
{
    public class Test01 : MonoBehaviour
    {
        public GameObject cube;

        [EventID]
        public string eventID;

        private float laneNum;
        private Vector3 position;
        private Quaternion rotation;

        // Start is called before the first frame update
        private void Start()
        {
            Koreographer.Instance.RegisterForEvents(eventID, transformGameObject);
        }

        private void OnDestroy()
        {
            if (Koreographer.Instance != null)
            {
                Koreographer.Instance.UnregisterForAllEvents(this);
            }
        }

        private void transformGameObject(KoreographyEvent evt)
        {
            laneNum = Mathf.Floor(evt.GetFloatValue() - 67f) % 4f;
            Debug.Log(laneNum);
            position = new Vector3(laneNum * 3.55f - 4.35f, 0f, 0f);
            rotation = Quaternion.Euler(Vector3.zero);
            Instantiate(cube, position, rotation, gameObject.transform);
        }
    }
}