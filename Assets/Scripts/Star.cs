using UnityEngine;

namespace Feature1
{
    [System.Serializable]
    public class Star
{
    public int starID;
    public string starName;
    public string constellationID;
    public string constellationName;
    public string history;
    public Vector3 position;
    
    public Star(int id, string name, string constID, string constName, string hist, Vector3 pos)
    {
        starID = id;
        starName = name;
        constellationID = constID;
        constellationName = constName;
        history = hist;
        position = pos;
    }
}
}

