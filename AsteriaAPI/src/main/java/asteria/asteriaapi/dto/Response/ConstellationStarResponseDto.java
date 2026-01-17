package asteria.asteriaapi.dto.Response;

import lombok.Data;

import java.util.List;

@Data
public class ConstellationStarResponseDto {
    private StarListResponseDto stars;
    private LinkedStarListResponseDto linkedStars;
}
