package asteria.asteriaapi.mapper;

import asteria.asteriaapi.dal.model.postgres.Constellation;
import asteria.asteriaapi.dto.Response.LinkedStarListResponseDto;
import asteria.asteriaapi.dto.Response.LinkedStarResponseDto;

import java.util.List;

public class LinkedStarListResponseMapper {
    public static LinkedStarListResponseDto toDto(List<LinkedStarResponseDto> linkedStarDtos) {
        LinkedStarListResponseDto dto = new LinkedStarListResponseDto();
        dto.setStars(linkedStarDtos);
        return dto;
    }

    public static LinkedStarListResponseDto fromConstellationToDto(Constellation constellation) {
        LinkedStarListResponseDto dto = new LinkedStarListResponseDto();
        List<LinkedStarResponseDto> starDtos = constellation.getLinkedStars().stream()
                .map(LinkedStarResponseMapper::toDto)
                .toList();
        dto.setStars(starDtos);
        return dto;
    }
}
