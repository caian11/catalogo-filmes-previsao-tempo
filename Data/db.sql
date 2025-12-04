CREATE TABLE filmes (
                        id              SERIAL PRIMARY KEY,
                        tmdb_id         INTEGER      NOT NULL UNIQUE,
                        titulo          VARCHAR(255) NOT NULL,
                        titulo_original VARCHAR(255),
                        sinopse         TEXT,
                        data_lancamento DATE,
                        genero          VARCHAR(255),
                        poster_path     VARCHAR(500),
                        lingua          VARCHAR(10),
                        duracao         INTEGER,
                        nota_media      NUMERIC(3,1),
                        elenco_principal TEXT,
                        cidade_referencia VARCHAR(255),
                        latitude        NUMERIC(9,6),
                        longitude       NUMERIC(9,6),
                        data_criacao    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                        data_atualizacao TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
