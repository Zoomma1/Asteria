package asteria.asteriaapi.services;

import asteria.asteriaapi.dal.postgres.repository.StarRepository;
import asteria.asteriaapi.dto.Response.StarResponseDto;
import asteria.asteriaapi.mapper.StarMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class StarService {

    private final StarRepository repo;

    public List<StarResponseDto> getAll(double minMag, double maxMag) {

        return repo.findAll().stream()
                .filter(s -> s.getMag() != null && s.getMag() >= minMag && s.getMag() <= maxMag)
                .map(StarMapper::toDto)
                .toList();
    }

    public List<StarResponseDto> getStars(double minMag, double maxMag, int limit) {
        return repo.findAll().stream()
                .filter(s -> s.getMag() != null && s.getMag() >= minMag && s.getMag() <= maxMag)
                .map(StarMapper::toDto)
                .limit(limit > 0 ? limit : Integer.MAX_VALUE)
                .toList();
    }

    public List<StarResponseDto> getStarsInConstellations(double minMag, double maxMag) {
        return repo.findAll().stream()
                .filter(s -> s.getMag() != null && s.getMag() >= minMag && s.getMag() <= maxMag)
                .filter(s -> s.getFigureConstellation() != null)
                .map(StarMapper::toDto)
                .toList();
    }
}
