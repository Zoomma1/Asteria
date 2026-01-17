package asteria.asteriaapi.init;

import asteria.asteriaapi.dal.model.postgres.Constellation;
import asteria.asteriaapi.dal.model.postgres.LinkedStar;
import asteria.asteriaapi.dal.model.postgres.Star;
import asteria.asteriaapi.dal.postgres.repository.ConstellationRepository;
import asteria.asteriaapi.dal.postgres.repository.LinkedStarRepository;
import asteria.asteriaapi.dal.postgres.repository.StarRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.core.Ordered;
import org.springframework.core.annotation.Order;
import org.springframework.core.io.Resource;
import org.springframework.stereotype.Component;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.HashMap;
import java.util.Map;
import java.util.Optional;

@Component
@RequiredArgsConstructor
@Order(Ordered.LOWEST_PRECEDENCE)
public class ConstellationBaseInit implements ApplicationRunner {

    private final StarRepository starRepo;
    private final ConstellationRepository constRepo;
    private final LinkedStarRepository linkedStarRepo;

    @Value("classpath:data/constellationship.fab")
    private Resource constellationshipFab;

    @Value("classpath:data/ConstellationCodes.csv")
    private Resource constellationCodes;

    @Override
    public void run(ApplicationArguments args) throws Exception {

        if (constRepo.count() > 0 && linkedStarRepo.count() > 0) {
            return;
        }

        loadConstellationRelationShips();
        assignStarsToConstellations();
    }

    private void loadConstellationRelationShips() throws Exception {
        Map<String, String> codeToName = loadConstellationCodes();

        try (BufferedReader br = new BufferedReader(
                new InputStreamReader(constellationshipFab.getInputStream()))) {

            String line;
            while ((line = br.readLine()) != null) {
                line = line.trim();

                if (line.isEmpty() || line.startsWith("#")) {
                    continue;
                }

                String[] tokens = line.split("\\s+");
                if (tokens.length < 3) {
                    continue;
                }

                String conCode = tokens[0];          // ex : "UMi"
                int segmentCount;
                try {
                    segmentCount = Integer.parseInt(tokens[1]);
                } catch (NumberFormatException e) {
                    continue;
                }

                int expectedHipCount = 2 * segmentCount;
                int availableHipCount = tokens.length - 2;
                if (availableHipCount < 2) {
                    continue;
                }
                if (availableHipCount < expectedHipCount) {
                    segmentCount = availableHipCount / 2;
                }

                String fullName = codeToName.getOrDefault(conCode, conCode);

                Constellation constellation = constRepo.findByCon(fullName)
                        .orElseGet(() -> {
                            Constellation c = new Constellation();
                            c.setCon(fullName);
                            return constRepo.save(c);
                        });

                // pour chaque paire HIP1 HIP2 -> un segment
                for (int i = 0; i < segmentCount; i++) {
                    int idx = 2 + 2 * i;
                    String hip1Str = tokens[idx];
                    String hip2Str = tokens[idx + 1];

                    try {
                        int hip1 = Integer.parseInt(hip1Str);
                        int hip2 = Integer.parseInt(hip2Str);

                        Optional<Star> s1Opt = starRepo.findByHip(hip1);
                        Optional<Star> s2Opt = starRepo.findByHip(hip2);

                        if (s1Opt.isPresent() && s2Opt.isPresent()) {
                            LinkedStar ls = new LinkedStar();
                            ls.setConstellation(constellation);
                            ls.setFromStar(s1Opt.get());
                            ls.setToStar(s2Opt.get());

                            linkedStarRepo.save(ls);
                        } else {
                            System.err.println("HIP not found: " + hip1 + " or " + hip2 + " for " + conCode);
                        }

                    } catch (NumberFormatException e) {
                        System.err.println("Invalid HIP number: " + hip1Str + " or " + hip2Str);
                    }
                }
            }
        }
    }

    /**
     * Charge le mapping code IAU -> nom complet de constellation.
     * ConstellationCodes.csv doit être du type :
     * code,name
     * UMi,Ursa Minor
     * ...
     */
    private Map<String, String> loadConstellationCodes() throws Exception {
        Map<String, String> conCodes = new HashMap<>();

        try (BufferedReader br = new BufferedReader(
                new InputStreamReader(constellationCodes.getInputStream()))) {

            String line = br.readLine();
            while ((line = br.readLine()) != null) {
                String[] parts = line.split(",");
                if (parts.length >= 2) {
                    String code = parts[0].trim();   // "UMi"
                    String name = parts[1].trim();   // "Ursa Minor"
                    conCodes.put(code, name);
                }
            }
        }
        return conCodes;
    }

    private void assignStarsToConstellations() {
        System.out.println("Assigning stars to constellations based on linked segments...");

        var allLinks = linkedStarRepo.findAll();
        int count = 0;

        for (var link : allLinks) {
            Constellation constellation = link.getConstellation();

            Star from = link.getFromStar();
            Star to = link.getToStar();

            if (from.getFigureConstellation() == null) {
                from.setFigureConstellation(constellation);
                starRepo.save(from);
                count++;
            }

            if (to.getFigureConstellation() == null) {
                to.setFigureConstellation(constellation);
                starRepo.save(to);      // Here we are not using batch save because it's unlikely to have more than a few hundred stars to update
                count++;
            }
        }

        System.out.println("Assigned " + count + " stars to their constellations.");
    }

}
