using UnityEngine;
using Feature1;

namespace Feature1
{
    public class StarData : MonoBehaviour
{
    public Star star;
    
    // Accesseurs pour faciliter l'accès aux données
    public int StarID => star.starID;
    public string StarName => star.starName;
    public string ConstellationID => star.constellationID;
    public string ConstellationName => star.constellationName;
    public string History => star.history;
        public Vector3 Position => star.position;
    }
}

