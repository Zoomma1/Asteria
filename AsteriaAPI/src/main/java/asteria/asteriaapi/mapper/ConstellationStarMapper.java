package asteria.asteriaapi.mapper;

import asteria.asteriaapi.dal.model.postgres.Constellation;
import asteria.asteriaapi.dto.Response.ConstellationStarResponseDto;
import asteria.asteriaapi.dto.Response.LinkedStarListResponseDto;
import asteria.asteriaapi.dto.Response.StarResponseDto;

import java.util.List;

public class ConstellationStarMapper {
    public static ConstellationStarResponseDto fromConstellationToDto(Constellation constellation) {
        ConstellationStarResponseDto dto = new ConstellationStarResponseDto();
        List<StarResponseDto> starDtos = constellation.getStars()
                .stream()
                .map(StarMapper::toDto)
                .toList();
        dto.setStars(StarListMapper.toDto(starDtos));

        LinkedStarListResponseDto linkedStarListDto = new LinkedStarListResponseDto();
        linkedStarListDto.setStars(constellation.getLinkedStars()
                .stream()
                .map(LinkedStarResponseMapper::toDto)
                .toList());

        dto.setLinkedStars(linkedStarListDto);
        return dto;
    }
}
