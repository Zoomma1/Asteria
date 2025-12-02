package asteria.asteriaapi.dto.Response;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.util.List;

@Data
@AllArgsConstructor
public class StarListResponseDto {
    private List<StarResponseDto> stars;
}
