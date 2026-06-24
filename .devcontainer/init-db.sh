#!/bin/bash
set -e

psql -U postgres <<EOF
CREATE DATABASE example;
CREATE USER calc WITH PASSWORD 'password';
GRANT ALL PRIVILEGES ON DATABASE example TO calc;

\c example

CREATE SCHEMA IF NOT EXISTS acme;
GRANT ALL ON SCHEMA acme TO calc;
ALTER USER calc SET search_path TO acme;
SET search_path TO acme;

-- ============================================================
-- SEQUENCES
-- ============================================================
CREATE SEQUENCE IF NOT EXISTS oil_fields_seq START 1 INCREMENT 50;
CREATE SEQUENCE IF NOT EXISTS dev_targets_seq START 1 INCREMENT 50;
CREATE SEQUENCE IF NOT EXISTS wells_seq START 1 INCREMENT 50;
CREATE SEQUENCE IF NOT EXISTS cluster_stations_seq START 1 INCREMENT 50;
CREATE SEQUENCE IF NOT EXISTS water_samples_seq START 1 INCREMENT 50;
CREATE SEQUENCE IF NOT EXISTS calc_projects_seq START 1 INCREMENT 50;
CREATE SEQUENCE IF NOT EXISTS calc_water_samples_seq START 1 INCREMENT 50;

-- ============================================================
-- OIL FIELDS (месторождения)
-- ============================================================
CREATE TABLE IF NOT EXISTS oil_fields (
    id          bigint PRIMARY KEY,
    name        text NOT NULL,
    created_at  timestamp NOT NULL DEFAULT now(),
    updated_at  timestamp NOT NULL DEFAULT now(),
    created_by  text NOT NULL DEFAULT '',
    updated_by  text NOT NULL DEFAULT ''
);

-- ============================================================
-- DEV TARGETS (объекты разработки)
-- ============================================================
CREATE TABLE IF NOT EXISTS dev_targets (
    id          bigint PRIMARY KEY,
    oil_field_id bigint NOT NULL REFERENCES oil_fields(id),
    name        text NOT NULL,
    created_at  timestamp NOT NULL DEFAULT now(),
    updated_at  timestamp NOT NULL DEFAULT now(),
    created_by  text NOT NULL DEFAULT '',
    updated_by  text NOT NULL DEFAULT ''
);

-- ============================================================
-- WELLS (скважины)
-- ============================================================
CREATE TABLE IF NOT EXISTS wells (
    id          bigint PRIMARY KEY,
    oil_field_id bigint NOT NULL REFERENCES oil_fields(id),
    name        text NOT NULL,
    created_at  timestamp NOT NULL DEFAULT now(),
    updated_at  timestamp NOT NULL DEFAULT now(),
    created_by  text NOT NULL DEFAULT '',
    updated_by  text NOT NULL DEFAULT ''
);

-- ============================================================
-- CLUSTER STATIONS (КНС / ДНС)
-- ============================================================
CREATE TABLE IF NOT EXISTS cluster_stations (
    id          bigint PRIMARY KEY,
    name        text NOT NULL,
    station_type smallint NOT NULL,  -- 1=DNS, 2=KNS
    created_at  timestamp NOT NULL DEFAULT now(),
    updated_at  timestamp NOT NULL DEFAULT now(),
    created_by  text NOT NULL DEFAULT '',
    updated_by  text NOT NULL DEFAULT ''
);

-- ============================================================
-- WATER SAMPLES (исходные пробы)
-- ============================================================
CREATE TABLE IF NOT EXISTS water_samples (
    id            bigint PRIMARY KEY,
    station_id    bigint REFERENCES cluster_stations(id),
    oil_field_id  bigint REFERENCES oil_fields(id),
    dev_target_id bigint REFERENCES dev_targets(id),
    well_id       bigint REFERENCES wells(id),
    water_type    smallint NOT NULL,  -- 1=Reservoir, 2=Injection
    sampled_at    timestamp NOT NULL,
    chloride      double precision,
    carbonate     double precision,
    bicarbonate   double precision,
    calcium       double precision,
    magnesium     double precision,
    sodium        double precision,
    sulfate       double precision,
    created_at    timestamp NOT NULL DEFAULT now(),
    updated_at    timestamp NOT NULL DEFAULT now(),
    created_by    text NOT NULL DEFAULT '',
    updated_by    text NOT NULL DEFAULT ''
);

-- ============================================================
-- CALC PROJECTS (расчётные проекты)
-- ============================================================
CREATE TABLE IF NOT EXISTS calc_projects (
    id           bigint PRIMARY KEY,
    oil_field_id bigint NOT NULL REFERENCES oil_fields(id),
    dev_target_id bigint NOT NULL REFERENCES dev_targets(id),
    name         text NOT NULL,
    created_at   timestamp NOT NULL DEFAULT now(),
    updated_at   timestamp NOT NULL DEFAULT now(),
    created_by   text NOT NULL DEFAULT '',
    updated_by   text NOT NULL DEFAULT ''
);

-- ============================================================
-- CALC WATER SAMPLES (пробы в расчётном проекте)
-- ============================================================
CREATE TABLE IF NOT EXISTS calc_water_samples (
    id                bigint PRIMARY KEY,
    calc_project_id   bigint NOT NULL REFERENCES calc_projects(id),
    source_sample_id  bigint REFERENCES water_samples(id),
    well_name         text,
    station_name      text,
    sampled_at        timestamp NOT NULL,
    water_type        smallint NOT NULL,  -- 1=Reservoir, 2=Injection
    chloride          double precision,
    carbonate         double precision,
    bicarbonate       double precision,
    sulfate           double precision,
    calcium           double precision,
    magnesium         double precision,
    sodium            double precision,
    created_at        timestamp NOT NULL DEFAULT now(),
    updated_at        timestamp NOT NULL DEFAULT now(),
    created_by        text NOT NULL DEFAULT '',
    updated_by        text NOT NULL DEFAULT ''
);

-- ============================================================
-- CALC WATER SAMPLE EQUIVALENTS (эквиваленты, one-to-one PK=FK)
-- ============================================================
CREATE TABLE IF NOT EXISTS calc_water_sample_equivalents (
    sample_id   bigint PRIMARY KEY REFERENCES calc_water_samples(id),
    chloride    double precision,
    carbonate   double precision,
    bicarbonate double precision,
    sulfate     double precision,
    calcium     double precision,
    magnesium   double precision,
    sodium      double precision,
    created_at  timestamp NOT NULL DEFAULT now(),
    updated_at  timestamp NOT NULL DEFAULT now(),
    created_by  text NOT NULL DEFAULT '',
    updated_by  text NOT NULL DEFAULT ''
);

-- ============================================================
-- PERMISSIONS
-- ============================================================

-- Read-only tables
GRANT SELECT ON oil_fields, dev_targets, wells, cluster_stations, water_samples TO calc;

-- Read-only sequences
GRANT SELECT ON calc_projects_seq, calc_water_samples_seq TO calc;

-- Read-write tables
GRANT SELECT, INSERT, UPDATE, DELETE ON calc_projects, calc_water_samples, calc_water_sample_equivalents TO calc;

-- ============================================================
-- SEED DATA
-- ============================================================

-- Oil fields
INSERT INTO oil_fields (id, name) VALUES (1, 'Самотлорское');
INSERT INTO oil_fields (id, name) VALUES (2, 'Приобское');

-- Dev targets for Samotlor
INSERT INTO dev_targets (id, oil_field_id, name) VALUES (1, 1, 'АВ1-2');
INSERT INTO dev_targets (id, oil_field_id, name) VALUES (2, 1, 'БВ8');
-- Dev targets for Priobskoye
INSERT INTO dev_targets (id, oil_field_id, name) VALUES (3, 2, 'АС10');
INSERT INTO dev_targets (id, oil_field_id, name) VALUES (4, 2, 'АС12');

-- Wells for Samotlor
INSERT INTO wells (id, oil_field_id, name) VALUES (1, 1, 'SK-101');
INSERT INTO wells (id, oil_field_id, name) VALUES (2, 1, 'SK-102');
INSERT INTO wells (id, oil_field_id, name) VALUES (3, 1, 'SK-201');
-- Wells for Priobskoye
INSERT INTO wells (id, oil_field_id, name) VALUES (4, 2, 'PK-101');
INSERT INTO wells (id, oil_field_id, name) VALUES (5, 2, 'PK-102');

-- Cluster stations (1=DNS, 2=KNS)
INSERT INTO cluster_stations (id, name, station_type) VALUES (1, 'ДНС-1 Самотлор', 1);
INSERT INTO cluster_stations (id, name, station_type) VALUES (2, 'ДНС-2 Самотлор', 1);
INSERT INTO cluster_stations (id, name, station_type) VALUES (3, 'КНС-1 Приобское', 2);
INSERT INTO cluster_stations (id, name, station_type) VALUES (4, 'КНС-2 Приобское', 2);

-- Water samples (1=Reservoir, 2=Injection)
INSERT INTO water_samples (id, station_id, oil_field_id, dev_target_id, well_id, water_type, sampled_at, chloride, carbonate, bicarbonate, calcium, magnesium, sodium, sulfate)
VALUES (1, 1, 1, 1, 1, 1, '2025-06-01', 120.5, 0, 350.2, 80.1, 30.2, 150.3, 200.4);

INSERT INTO water_samples (id, station_id, oil_field_id, dev_target_id, well_id, water_type, sampled_at, chloride, carbonate, bicarbonate, calcium, magnesium, sodium, sulfate)
VALUES (2, 1, 1, 1, 1, 2, '2025-06-01', 140.0, 0, 400.0, 90.0, 35.0, 180.0, 250.0);

INSERT INTO water_samples (id, station_id, oil_field_id, dev_target_id, well_id, water_type, sampled_at, chloride, carbonate, bicarbonate, calcium, magnesium, sodium, sulfate)
VALUES (3, 2, 1, 2, 3, 1, '2025-06-15', 95.0, 5.0, 280.0, 65.0, 25.0, 110.0, 150.0);

INSERT INTO water_samples (id, station_id, oil_field_id, dev_target_id, well_id, water_type, sampled_at, chloride, carbonate, bicarbonate, calcium, magnesium, sodium, sulfate)
VALUES (4, 3, 2, 3, 4, 1, '2025-06-10', 80.0, 10.0, 200.0, 50.0, 20.0, 90.0, 120.0);

INSERT INTO water_samples (id, station_id, oil_field_id, dev_target_id, well_id, water_type, sampled_at, chloride, carbonate, bicarbonate, calcium, magnesium, sodium, sulfate)
VALUES (5, 4, 2, 4, 5, 2, '2025-06-20', 110.0, 0, 320.0, 75.0, 28.0, 130.0, 180.0);

-- Calc projects
INSERT INTO calc_projects (id, oil_field_id, dev_target_id, name)
VALUES (1, 1, 1, 'Самотлорское АВ1-2 — июнь 2025');

INSERT INTO calc_projects (id, oil_field_id, dev_target_id, name)
VALUES (2, 1, 2, 'Самотлорское БВ8 — июнь 2025');

INSERT INTO calc_projects (id, oil_field_id, dev_target_id, name)
VALUES (3, 2, 3, 'Приобское АС10 — июнь 2025');

-- Calc water samples
INSERT INTO calc_water_samples (id, calc_project_id, source_sample_id, well_name, station_name, sampled_at, water_type, chloride, carbonate, bicarbonate, sulfate, calcium, magnesium, sodium)
VALUES (1, 1, 1, 'SK-101', 'ДНС-1 Самотлор', '2025-06-01', 1, 120.5, 0, 350.2, 200.4, 80.1, 30.2, 150.3);

INSERT INTO calc_water_samples (id, calc_project_id, source_sample_id, well_name, station_name, sampled_at, water_type, chloride, carbonate, bicarbonate, sulfate, calcium, magnesium, sodium)
VALUES (2, 1, 2, 'SK-101', 'ДНС-1 Самотлор', '2025-06-01', 2, 140.0, 0, 400.0, 250.0, 90.0, 35.0, 180.0);

INSERT INTO calc_water_samples (id, calc_project_id, source_sample_id, well_name, station_name, sampled_at, water_type, chloride, carbonate, bicarbonate, sulfate, calcium, magnesium, sodium)
VALUES (3, 2, 3, 'SK-201', 'ДНС-2 Самотлор', '2025-06-15', 1, 95.0, 5.0, 280.0, 150.0, 65.0, 25.0, 110.0);

INSERT INTO calc_water_samples (id, calc_project_id, source_sample_id, well_name, station_name, sampled_at, water_type, chloride, carbonate, bicarbonate, sulfate, calcium, magnesium, sodium)
VALUES (4, 3, 4, 'PK-101', 'КНС-1 Приобское', '2025-06-10', 1, 80.0, 10.0, 200.0, 120.0, 50.0, 20.0, 90.0);

INSERT INTO calc_water_samples (id, calc_project_id, source_sample_id, well_name, station_name, sampled_at, water_type, chloride, carbonate, bicarbonate, sulfate, calcium, magnesium, sodium)
VALUES (5, 3, 5, 'PK-102', 'КНС-2 Приобское', '2025-06-20', 2, 110.0, 0, 320.0, 180.0, 75.0, 28.0, 130.0);

EOF

echo "Database initialized successfully"
