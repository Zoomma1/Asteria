package asteria.asteriaapi.dto;

import asteria.asteriaapi.Model.Star;

import java.util.List;

public class StarApiResponse {

    private List<Star> stars;

    public StarApiResponse(List<Star> stars) {
        this.stars = stars;
    }

    public List<Star> getStars() {
        return stars;
    }

    public void setStars(List<Star> stars) {
        this.stars = stars;
    }
}
