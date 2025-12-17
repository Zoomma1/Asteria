package asteria.asteriaapi.dal.model.postgres;

import jakarta.persistence.*;
import lombok.Data;

@Entity
@Table(name = "stars")
@Data
public class Star {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(insertable = false, updatable = false)
    private Long id;

    private Integer hip;
    private Integer hr;
    private Integer hd;
    private Integer flam;

    private String bayer;
    private String proper;
    private String con;

    private Double ra;
    private Double dec;
    private Double mag;

    @ManyToOne
    @JoinColumn(name = "figure_constellation_id")
    private Constellation figureConstellation;
}
