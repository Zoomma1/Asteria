package asteria.asteriaapi.mapper;

import asteria.asteriaapi.dto.Response.ConstellationListResponseDto;
import asteria.asteriaapi.dto.Response.ConstellationResponseDto;

import java.util.List;

public class ConstellationListMapper {
    public static ConstellationListResponseDto toDto(List<ConstellationResponseDto> constellationDto) {
        ConstellationListResponseDto dto = new ConstellationListResponseDto();
        dto.setConstellations(constellationDto);
        return dto;
    }
}
