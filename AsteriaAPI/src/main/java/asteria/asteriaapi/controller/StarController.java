package asteria.asteriaapi.controller;

import asteria.asteriaapi.dto.Response.StarResponseDto;
import asteria.asteriaapi.dto.Response.StarListResponseDto;
import asteria.asteriaapi.mapper.StarListMapper;
import asteria.asteriaapi.services.StarService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/stars")
@RequiredArgsConstructor
public class StarController {

    private final StarService service;

    @GetMapping
    public StarListResponseDto getStars(
            @RequestParam(defaultValue = "-2") double minMag,
            @RequestParam(defaultValue = "6.5") double maxMag,
            @RequestParam(required = false) int limit
    ) {
        List<StarResponseDto> stars = service.getStars(minMag, maxMag, limit);
        return StarListMapper.toDto(stars);
    }

    @GetMapping("/constellations")
    public StarListResponseDto getStarsInConstellations(
            @RequestParam(defaultValue = "-2") double minMag,
            @RequestParam(defaultValue = "6.5") double maxMag
    ) {
        List<StarResponseDto> stars = service.getStarsInConstellations(minMag, maxMag);
        return StarListMapper.toDto(stars);
    }
}
