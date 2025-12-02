package asteria.asteriaapi.dto.Response;

import lombok.Data;

import java.util.List;

@Data
public class ConstellationListResponseDto {
    private List<ConstellationResponseDto> constellations;
}
