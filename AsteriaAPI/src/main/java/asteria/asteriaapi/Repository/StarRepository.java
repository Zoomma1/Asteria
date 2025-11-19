package asteria.asteriaapi.Repository;

import asteria.asteriaapi.Model.Star;
import jakarta.annotation.PostConstruct;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.io.Resource;
import org.springframework.stereotype.Repository;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;
import java.util.*;

@Repository
public class StarRepository {

    @Value("classpath:data/star.csv")
    private Resource starCsv;

    private final List<Star> stars = new ArrayList<>();

    @PostConstruct
    public void init() {
        try {
            loadFromCsv();
        } catch (Exception e) {
            throw new RuntimeException("Erreur chargement CSV étoiles", e);
        }
    }

    private void loadFromCsv() throws IOException {
        try (BufferedReader br = new BufferedReader(
                new InputStreamReader(starCsv.getInputStream(), StandardCharsets.UTF_8))) {

            String header = br.readLine();
            String line;
            int idCounter = 0;

            while ((line = br.readLine()) != null) {
                if (line.isBlank()) continue;

                String[] c = line.split(",", -1);

                String name = c[1].trim();      // Name
                String hr   = c[0].trim();      // HR number
                double vmag = parse(c[12]);     // Vmag

                // fallback si nom vide
                if (name.isEmpty()) name = "HR " + hr;

                // RA J2000
                double RAh = parse(c[19]);
                double RAm = parse(c[20]);
                double RAs = parse(c[21]);
                double raHours = RAh + (RAm / 60.0) + (RAs / 3600.0);

                // DEC J2000
                String sign = c[22].trim();
                double DEd = parse(c[23]);
                double DEm = parse(c[24]);
                double DEs = parse(c[25]);
                double decDeg = DEd + (DEm / 60.0) + (DEs / 3600.0);
                if (sign.equals("-")) decDeg = -decDeg;

                Star star = new Star(
                        "star-" + idCounter,
                        name,
                        raHours,
                        decDeg,
                        vmag
                );

                stars.add(star);
                idCounter++;
            }
        }
    }

    private double parse(String s) {
        if (s == null || s.isBlank()) return 0;
        return Double.parseDouble(s.trim());
    }

    public List<Star> findAll() {
        return Collections.unmodifiableList(stars);
    }
}
