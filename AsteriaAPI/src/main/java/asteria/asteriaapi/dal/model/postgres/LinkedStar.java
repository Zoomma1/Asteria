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
    private Star fromStar;

    @ManyToOne
    private Star toStar;
}
