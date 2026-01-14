using DTO;
using UnityEngine;

namespace Interface.ConstellationInfo
{
    public class ConstellationData : MonoBehaviour
    {
        public ConstellationDto Constellation;
        
        public void Init(ConstellationDto constellation)
        {
            Constellation = constellation;
        }
        
        public string ConstellationName => Constellation.name;
        public ConstellationStarBlockDto Stars => Constellation.stars;
    }
}