package asteria.asteriaapi.dal.postgres.repository;

import asteria.asteriaapi.dal.model.postgres.Constellation;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface ConstellationRepository extends JpaRepository<Constellation, Long> {
    Optional<Constellation> findByCon(String con);
}
