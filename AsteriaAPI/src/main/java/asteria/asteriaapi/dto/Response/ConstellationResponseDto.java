package asteria.asteriaapi.dto.Response;

import lombok.Data;

@Data
public class ConstellationResponseDto {
    private Integer id;
    private String name;
    private ConstellationStarResponseDto stars;
}
