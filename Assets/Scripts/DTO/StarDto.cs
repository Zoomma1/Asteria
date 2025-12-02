[System.Serializable]
public class StarDto
{
    public string id;
    public string name;
    public float raHours;
    public float decDeg;
    public float mag;
}

[System.Serializable]
public class StarApiResponse
{
    public StarDto[] stars;
}