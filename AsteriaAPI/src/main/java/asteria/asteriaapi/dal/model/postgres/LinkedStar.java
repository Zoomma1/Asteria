package asteria.asteriaapi.dal.model.postgres;

import jakarta.persistence.*;
import lombok.Data;

@Entity
@Table(name = "linked_stars")
@Data
public class LinkedStar {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(optional = false)
    @JoinColumn(name = "constellation_id")
    private Constellation constellation;

    @ManyToOne
    @JoinColumn(name = "from_star_id")
    private Star fromStar;

    @ManyToOne
    @JoinColumn(name = "to_star_id")
    private Star toStar;
}
