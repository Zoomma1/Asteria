package asteria.asteriaapi.dal.postgres.repository;

import asteria.asteriaapi.dal.model.postgres.Constellation;
import asteria.asteriaapi.dal.model.postgres.LinkedStar;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface LinkedStarRepository extends JpaRepository<LinkedStar, Long> {
}
