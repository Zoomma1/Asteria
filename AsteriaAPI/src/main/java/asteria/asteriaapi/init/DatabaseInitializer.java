package asteria.asteriaapi.init;

import asteria.asteriaapi.dal.model.postgres.Star;
import asteria.asteriaapi.dal.model.postgres.Constellation;
import asteria.asteriaapi.dal.postgres.repository.StarRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.core.io.Resource;
import org.springframework.stereotype.Component;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.util.*;

@Component
@RequiredArgsConstructor
public class DatabaseInitializer implements ApplicationRunner {

    @Value("classpath:data/hygv42.csv")
    private Resource hygCsv;

    private final StarRepository repo;

    @Override
    public void run(ApplicationArguments args) throws Exception {
        importHyg();
    }

    private void importHyg() throws Exception {
        List<Star> stars = new ArrayList<>();
        Map<Integer, Constellation> conMap = new HashMap<>();
        Set<Integer> processedHip = new HashSet<>();

        if (repo.count() > 0) {
            return; // Data already imported
        }

        try (BufferedReader br = new BufferedReader(new InputStreamReader(hygCsv.getInputStream()))) {
            br.readLine(); // skip header
            String line;

            while ((line = br.readLine()) != null) {
                String[] c = line.split(",", -1);

                Integer hip = parseInt(c[1]);
                Integer hr = parseInt(c[3]);
                Integer hd = parseInt(c[2]);

                String proper = !Objects.equals(c[6], "") ? "hip " + hip : c[6];
                String con = emptyToNull(c[29]);

                String bayer = emptyToNull(c[27]);
                Integer flam = parseInt(c[28]);

                double ra = Double.parseDouble(c[7]);
                double dec = Double.parseDouble(c[8]);
                double mag = Double.parseDouble(c[13]);

                if (hip != null && !processedHip.contains(hip)) {
                    processedHip.add(hip);

                    Star s = new Star();
                    s.setHip(hip);
                    s.setHr(hr);
                    s.setHd(hd);
                    s.setProper(proper);
                    s.setCon(con);
                    s.setRa(ra);
                    s.setDec(dec);
                    s.setMag(mag);
                    s.setBayer(bayer);
                    s.setFlam(flam);
                    stars.add(s);
                }
            }
        }

        // Batch save stars
        repo.saveAll(stars);
    }

    private Integer parseInt(String s) {
        s = s.trim().replace("\"", "");
        if (s.isBlank()) return null;
        return Integer.parseInt(s);
    }

    private String emptyToNull(String s) {
        if (s == null || s.isBlank()) return null;
        return s;
    }
}
