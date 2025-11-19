package asteria.asteriaapi.Controller;

import asteria.asteriaapi.Model.Star;
import asteria.asteriaapi.Services.StarService;
import asteria.asteriaapi.dto.StarApiResponse;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/stars")
@CrossOrigin(origins = "*")
public class StarController {

    private final StarService starService;

    public StarController(StarService starService) {
        this.starService = starService;
    }

    @GetMapping
    public StarApiResponse getStars(
            @RequestParam(defaultValue = "-99") double minMag,
            @RequestParam(defaultValue = "99") double maxMag,
            @RequestParam(defaultValue = "500") int limit
    ) {
        List<Star> stars = starService.getStars(minMag, maxMag, limit);
        return new StarApiResponse(stars);
    }
}
