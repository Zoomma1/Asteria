package asteria.asteriaapi.dal.postgres.repository;

import asteria.asteriaapi.dal.model.postgres.Star;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface StarRepository extends JpaRepository<Star, Long> {
    Optional<Star> findByHip(Integer hip);
    Optional<Star> findByHd(Integer hd);
    List<Star> findByConAndFlam(String con, Integer flam);
    List<Star> findByConAndBayer(String con, String bayer);

    Optional<Star> findByProper(String designation);
}
