package asteria.asteriaapi.mapper;

import asteria.asteriaapi.dal.model.postgres.Constellation;
import asteria.asteriaapi.dto.Response.*;


public class ConstellationMapper {
    public static ConstellationResponseDto toDto(Constellation constellation) {

        ConstellationResponseDto dto = new ConstellationResponseDto();
        dto.setId(Math.toIntExact(constellation.getId()));
        dto.setName(constellation.getCon());

        ConstellationStarResponseDto stars = ConstellationStarMapper.fromConstellationToDto(constellation);
        dto.setStars(stars);

        return dto;
    }
}
