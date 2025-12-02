package asteria.asteriaapi.mapper;

import asteria.asteriaapi.dal.model.postgres.LinkedStar;
import asteria.asteriaapi.dto.Response.LinkedStarResponseDto;

public class LinkedStarResponseMapper {
    public static LinkedStarResponseDto toDto(LinkedStar star) {
        LinkedStarResponseDto dto = new LinkedStarResponseDto();
        dto.setFromStarHip(star.getFromStar().getHip());
        dto.setToStarHip(star.getToStar().getHip());
        return dto;
    }
}
