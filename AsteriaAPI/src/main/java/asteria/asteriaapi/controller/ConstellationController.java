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
}
