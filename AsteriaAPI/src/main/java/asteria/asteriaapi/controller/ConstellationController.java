package asteria.asteriaapi.controller;

import asteria.asteriaapi.dto.Response.ConstellationListResponseDto;
import asteria.asteriaapi.dto.Response.StarListResponseDto;
import asteria.asteriaapi.services.ConstellationService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;


@RestController
@RequestMapping("/constellations")
@RequiredArgsConstructor
public class ConstellationController {

    private final ConstellationService service;

    @GetMapping
    public ConstellationListResponseDto getConstellation() {
        return service.getConstellations();
    }

    @GetMapping("/starsInConstellations")
    public StarListResponseDto getStarsInConstellations(
            @RequestParam(defaultValue = "-2") double minMag,
            @RequestParam(defaultValue = "6.5") double maxMag
    ) {
        return service.getStarsInConstellations(minMag, maxMag);
    }
}
