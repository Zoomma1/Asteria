using System.Collections.Generic;
using UnityEngine;

namespace StarFieldInteraction
{
    /// <summary>
    /// Centralized registry of all star data loaded from APIs
    /// Allows StarInteractableSimple to fetch data by HIP ID
    /// </summary>
    public class StarDataRegistry : MonoBehaviour
    {
        public static StarDataRegistry Instance { get; private set; }
        
        // HIP ID -> Star Data
        private Dictionary<int, StarData> starDatabase = new Dictionary<int, StarData>();
        
        [System.Serializable]
        public class StarData
        {
            public string name;
            public int hipId;
            public double rightAscension;
            public double declination;
            public double magnitude;
            public string constellationName;
            
            public StarData(string name, int hipId, double ra, double dec, double mag, string constellationName = "")
            {
                this.name = name;
                this.hipId = hipId;
                this.rightAscension = ra;
                this.declination = dec;
                this.magnitude = mag;
                this.constellationName = constellationName;
            }
        }
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Debug.LogWarning("[StarDataRegistry] Duplicate instance destroyed!");
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre les scènes
            
            Debug.Log("[StarDataRegistry] Initialized");
        }
        
        /// <summary>
        /// Register a star in the database
        /// Called by star generation scripts (StarFieldFromApi, ConstellationFieldFromApi)
        /// </summary>
        public void RegisterStar(int hipId, string name, double ra, double dec, double mag, string constellationName = "")
        {
            if (hipId <= 0)
            {
                Debug.LogWarning($"[StarDataRegistry] Cannot register star with invalid HIP ID: {hipId}");
                return;
            }
            
            if (!starDatabase.ContainsKey(hipId))
            {
                starDatabase[hipId] = new StarData(name, hipId, ra, dec, mag, constellationName);
                Debug.Log($"[StarDataRegistry] Registered star HIP {hipId}: {name}");
            }
        }
        
        /// <summary>
        /// Get star data by HIP ID
        /// Returns null if not found
        /// </summary>
        public StarData GetStarData(int hipId)
        {
            if (starDatabase.TryGetValue(hipId, out StarData data))
            {
                return data;
            }
            
            Debug.LogWarning($"[StarDataRegistry] Star with HIP {hipId} not found in registry");
            return null;
        }
        
        /// <summary>
        /// Check if a star is registered
        /// </summary>
        public bool HasStar(int hipId)
        {
            return starDatabase.ContainsKey(hipId);
        }
        
        /// <summary>
        /// Get total number of registered stars
        /// </summary>
        public int GetStarCount()
        {
            return starDatabase.Count;
        }
        
        /// <summary>
        /// Try to extract HIP ID from GameObject name
        /// Format: "Constellation_HIP12345" or "HIP 12345" or "Star_HIP12345"
        /// </summary>
        public static int ExtractHipIdFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return 0;
            
            // Look for "HIP" followed by digits
            int hipIndex = name.IndexOf("HIP");
            if (hipIndex >= 0)
            {
                string afterHip = name.Substring(hipIndex + 3);
                string digits = "";
                
                foreach (char c in afterHip)
                {
                    if (char.IsDigit(c))
                        digits += c;
                    else if (digits.Length > 0)
                        break; // Stop at first non-digit after we found digits
                }
                
                if (int.TryParse(digits, out int hipId))
                {
                    return hipId;
                }
            }
            
            return 0;
        }
    }
}
