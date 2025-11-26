package asteria.asteriaapi.dal.model.postgres;

import jakarta.persistence.*;
import lombok.Data;

import java.util.List;

@Entity
@Table(name = "constellations")
@Data
public class Constellation {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(insertable = false, updatable = false)
    private Long id;
    private String con;

    @OneToMany(mappedBy = "figureConstellation")
    private List<Star> stars;

    @OneToMany(mappedBy = "constellation")
    private List<LinkedStar> linkedStars;
}
