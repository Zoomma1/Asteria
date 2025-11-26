ALTER TABLE stars ADD COLUMN figure_constellation_id BIGINT REFERENCES constellations(id);
