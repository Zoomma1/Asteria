CREATE TABLE linked_stars (
    id BIGSERIAL PRIMARY KEY,
    constellation_id BIGINT NOT NULL REFERENCES constellations(id),
    from_star_id BIGINT NOT NULL REFERENCES stars(id),
    to_star_id BIGINT NOT NULL REFERENCES stars(id)
);

CREATE INDEX idx_linked_stars_constellation
    ON linked_stars(constellation_id);
