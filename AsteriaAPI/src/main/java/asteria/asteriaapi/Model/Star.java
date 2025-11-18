package asteria.asteriaapi.Model;

public class Star {

    private String id;
    private String name;
    private double raHours;
    private double decDeg;
    private double mag;

    public Star() {
    }

    public Star(String id, String name, double raHours, double decDeg, double mag) {
        this.id = id;
        this.name = name;
        this.raHours = raHours;
        this.decDeg = decDeg;
        this.mag = mag;
    }

    public String getId() { return id; }
    public void setId(String id) { this.id = id; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public double getRaHours() { return raHours; }
    public void setRaHours(double raHours) { this.raHours = raHours; }

    public double getDecDeg() { return decDeg; }
    public void setDecDeg(double decDeg) { this.decDeg = decDeg; }

    public double getMag() { return mag; }
    public void setMag(double mag) { this.mag = mag; }
}
