package asteria.asteriaapi.mapper;

import asteria.asteriaapi.dal.model.postgres.Star;
import asteria.asteriaapi.dto.Response.StarResponseDto;

public class StarMapper {
    public static StarResponseDto toDto(Star s) {
        StarResponseDto dto = new StarResponseDto();
        dto.setHip(s.getHip());
        dto.setProper(s.getProper());
        dto.setCon(s.getCon());
        dto.setRa(s.getRa());
        dto.setDec(s.getDec());
        dto.setMag(s.getMag());
        dto.setHr(s.getHr() != null ? s.getHr() : 0);
        return dto;
    }
}
