package asteria.asteriaapi.mapper;

import asteria.asteriaapi.dal.model.postgres.Star;
import asteria.asteriaapi.dto.Response.StarListResponseDto;
import asteria.asteriaapi.dto.Response.StarResponseDto;

import java.util.List;

public class StarListMapper {
    public static StarListResponseDto toDto(List<StarResponseDto> stars) {
        return new StarListResponseDto(stars);
    }
}
