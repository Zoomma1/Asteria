CREATE TABLE stars (
    id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    hip INT UNIQUE,
    hr INT,
    hd INT,
    proper VARCHAR(100),
    con VARCHAR(100),
    ra DOUBLE PRECISION,
    dec DOUBLE PRECISION,
    mag DOUBLE PRECISION
);

CREATE INDEX idx_stars_hip ON stars(hip);
CREATE INDEX idx_stars_hr ON stars(hr);
CREATE INDEX idx_stars_con ON stars(con);

CREATE TABLE constellations (
    id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    con VARCHAR(100)
);
