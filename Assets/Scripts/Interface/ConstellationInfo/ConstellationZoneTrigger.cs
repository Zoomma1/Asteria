using UnityEngine;
using DTO;

namespace Interface.ConstellationInfo
{
    public class ConstellationZoneTrigger : MonoBehaviour
    {
        private ConstellationData constellationData;
        private ConstellationZoneManager manager;
        
        void Start()
        {
            constellationData = GetComponent<ConstellationData>();
            manager = FindObjectOfType<ConstellationZoneManager>();
        }
        
        void OnTriggerStay(Collider other)
        {
            if (other.GetComponent<Camera>() != null)
            {
                if (manager != null && constellationData != null && constellationData.Constellation != null)
                {
                    manager.ShowConstellationInfo(constellationData.Constellation);
                }
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Camera>() != null)
            {
                if (manager != null)
                {
                    manager.HideConstellationInfo();
                }
            }
        }
    }
}
