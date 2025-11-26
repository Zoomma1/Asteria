package asteria.asteriaapi.dto.Response;

import lombok.Data;

@Data
public class StarResponseDto {

    private Integer hip;
    private String proper;
    private String con;

    private float hr;
    private double ra;
    private double dec;
    private double mag;
}
