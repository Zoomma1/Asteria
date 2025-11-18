package asteria.asteriaapi.Services;

import asteria.asteriaapi.Model.Star;
import asteria.asteriaapi.Repository.StarRepository;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

@Service
public class StarService {

    private final StarRepository repository;

    public StarService(StarRepository repository) {
        this.repository = repository;
    }

    public List<Star> getStars(double minMag, double maxMag, int limit) {
        List<Star> filtered = repository.findAll().stream()
                .filter(s -> s.getMag() >= minMag && s.getMag() <= maxMag)
                .toList();

        List<Star> shuffled = new ArrayList<>(filtered);
        Collections.shuffle(shuffled);

        System.out.println("Returning stars count: " + shuffled.size());

        return shuffled.stream()
                .limit(limit)
                .toList();
    }

}
