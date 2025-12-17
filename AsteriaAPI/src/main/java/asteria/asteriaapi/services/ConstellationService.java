package asteria.asteriaapi.services;

import asteria.asteriaapi.dal.postgres.repository.ConstellationRepository;
import asteria.asteriaapi.dto.Response.ConstellationListResponseDto;
import asteria.asteriaapi.dto.Response.StarListResponseDto;
import asteria.asteriaapi.mapper.ConstellationListMapper;
import asteria.asteriaapi.mapper.ConstellationMapper;
import asteria.asteriaapi.mapper.StarListMapper;
import asteria.asteriaapi.mapper.StarMapper;
import lombok.RequiredArgsConstructor;
import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@RequiredArgsConstructor
public class ConstellationService {
    private final ConstellationRepository constellationRepository;

    @EntityGraph
    @Transactional(readOnly = true)
    public ConstellationListResponseDto getConstellations() {
        var constellations = constellationRepository.findAll()
                .stream()
                .map(ConstellationMapper::toDto)
                .toList();

        return ConstellationListMapper.toDto(constellations);
    }
}
