using System.Collections.Generic;

namespace DTO
{
    [System.Serializable]
    public class ConstellationDto
    {
        public string id;
        public string name;
        public ConstellationStarBlockDto stars;
    }
}