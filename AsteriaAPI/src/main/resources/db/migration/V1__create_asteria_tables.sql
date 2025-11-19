CREATE TABLE stars (
    id SERIAL PRIMARY KEY,
    hr VARCHAR(10),
    hd VARCHAR(10),
    hip VARCHAR(10),
    name VARCHAR(100),
    ra_hours DOUBLE PRECISION,
    dec_deg DOUBLE PRECISION,
    vmag DOUBLE PRECISION
);

CREATE INDEX idx_stars_hr ON stars(hr);


CREATE TABLE star_names (
    id SERIAL PRIMARY KEY,
    hr VARCHAR(10),
    hd VARCHAR(10),
    hip VARCHAR(10),
    proper_name VARCHAR(150)
);

CREATE INDEX idx_star_names_hr ON star_names(hr);
CREATE INDEX idx_star_names_hd ON star_names(hd);
CREATE INDEX idx_star_names_hip ON star_names(hip);


CREATE TABLE star_constellations (
    id SERIAL PRIMARY KEY,
    hr VARCHAR(10),
    constellation VARCHAR(50)
);

CREATE INDEX idx_star_constellations_hr ON star_constellations(hr);
