drop database skill_gate_db;

\skill_gate_db;

CREATE DATABASE skill_gate_db;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE product
(
    id             UUID PRIMARY KEY,
    name           TEXT        NOT NULL,
    description    TEXT,
    price          DECIMAL     NOT NULL,
    stock_quantity INT         NOT NULL,
    image_url      TEXT,
    status         INT         NOT NULL DEFAULT 1,
    created_at     TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at     TIMESTAMPTZ,
    deleted_at     TIMESTAMPTZ,
    version        BIGINT      NOT NULL DEFAULT 1,
    created_by     UUID,
    updated_by     UUID,
    deleted_by     UUID,
    created_by_ip  TEXT,
    updated_by_ip  TEXT,
    deleted_by_ip  TEXT
);
CREATE INDEX idx_product_name ON product (name);

CREATE TABLE role
(
    id            UUID PRIMARY KEY,
    name          TEXT        NOT NULL,
    role_key      TEXT        NOT NULL,
    description   TEXT,
    status        INT         NOT NULL DEFAULT 1,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at    TIMESTAMPTZ,
    deleted_at    TIMESTAMPTZ,
    version       BIGINT      NOT NULL DEFAULT 1,
    created_by    UUID,
    updated_by    UUID,
    deleted_by    UUID,
    created_by_ip TEXT,
    updated_by_ip TEXT,
    deleted_by_ip TEXT
);
CREATE INDEX idx_role_name ON role (name);
CREATE INDEX idx_role_key ON role (role_key);

CREATE TABLE "user"
(
    id                      UUID PRIMARY KEY,
    first_name              TEXT,
    last_name               TEXT,
    email                   TEXT        NOT NULL,
    phone_number            TEXT        NOT NULL,
    user_name               TEXT        NOT NULL,
    dob                     TIMESTAMPTZ,
    email_confirmed         BOOLEAN     NOT NULL DEFAULT FALSE,
    phone_number_confirmed  BOOLEAN     NOT NULL DEFAULT FALSE,
    password_hash           TEXT        NOT NULL,
    last_password_change_at TIMESTAMPTZ,
    last_login_at           TIMESTAMPTZ,
    is_locked_out           BOOLEAN     NOT NULL DEFAULT FALSE,
    lockout_end             TIMESTAMPTZ,
    total_logins            BIGINT      NOT NULL DEFAULT 0,
    two_factor_secret       TEXT,
    two_factor_enabled      BOOLEAN     NOT NULL DEFAULT FALSE,
    token_version           UUID        NOT NULL,
    status                  INT         NOT NULL DEFAULT 1,
    created_at              TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at              TIMESTAMPTZ,
    deleted_at              TIMESTAMPTZ,
    version                 BIGINT      NOT NULL DEFAULT 1,
    created_by              UUID,
    updated_by              UUID,
    deleted_by              UUID,
    created_by_ip           TEXT,
    updated_by_ip           TEXT,
    deleted_by_ip           TEXT
);
CREATE INDEX idx_user_email ON "user" (email);
CREATE INDEX idx_user_user_name ON "user" (user_name);

CREATE TABLE user_role
(
    id            UUID PRIMARY KEY,
    user_id       UUID        NOT NULL,
    role_id       UUID        NOT NULL,
    status        INT         NOT NULL DEFAULT 1,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at    TIMESTAMPTZ,
    deleted_at    TIMESTAMPTZ,
    version       BIGINT      NOT NULL DEFAULT 1,
    created_by    UUID,
    updated_by    UUID,
    deleted_by    UUID,
    created_by_ip TEXT,
    updated_by_ip TEXT,
    deleted_by_ip TEXT,
    FOREIGN KEY (user_id) REFERENCES "user" (id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES role (id) ON DELETE CASCADE
);
CREATE INDEX idx_user_role_user_id ON user_role (user_id);
CREATE INDEX idx_user_role_role_id ON user_role (role_id);



CREATE OR REPLACE PROCEDURE create_product(
    p_name TEXT,
    p_description TEXT,
    p_price DECIMAL,
    p_stock_quantity INT,
    p_image_url TEXT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO product (id, name, description, price, stock_quantity, image_url, status, created_at)
    VALUES (uuid_generate_v4(), p_name, p_description, p_price, p_stock_quantity, p_image_url, 1, now());
END;
$$;


-- Функция для получения продукта по ID
CREATE OR REPLACE FUNCTION get_product_by_id(p_id UUID)
    RETURNS TABLE (
                      id UUID,
                      name TEXT,
                      description TEXT,
                      price DECIMAL,
                      stock_quantity INT,
                      image_url TEXT,
                      status INT,
                      created_at TIMESTAMPTZ,
                      updated_at TIMESTAMPTZ
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT id, name, description, price, stock_quantity, image_url, status, created_at, updated_at
        FROM product
        WHERE id = p_id;
END;
$$;


-- Процедура для обновления информации о продукте
CREATE OR REPLACE PROCEDURE update_product(
    p_id UUID,
    p_name TEXT,
    p_description TEXT,
    p_price DECIMAL,
    p_stock_quantity INT,
    p_image_url TEXT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE product
    SET name = p_name,
        description = p_description,
        price = p_price,
        stock_quantity = p_stock_quantity,
        image_url = p_image_url,
        updated_at = now()
    WHERE id = p_id;
END;
$$;


-- Процедура для удаления продукта
CREATE OR REPLACE PROCEDURE delete_product(p_id UUID)
    LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM product WHERE id = p_id;
END;
$$;



-- Процедура для создания новой роли
CREATE OR REPLACE PROCEDURE create_role(
    p_name TEXT,
    p_role_key TEXT,
    p_description TEXT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO role (id, name, role_key, description, status, created_at)
    VALUES (uuid_generate_v4(), p_name, p_role_key, p_description, 1, now());
END;
$$;


-- Функция для получения роли по ID
CREATE OR REPLACE FUNCTION get_role_by_id(p_id UUID)
    RETURNS TABLE (
                      id UUID,
                      name TEXT,
                      role_key TEXT,
                      description TEXT,
                      status INT,
                      created_at TIMESTAMPTZ,
                      updated_at TIMESTAMPTZ
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT id, name, role_key, description, status, created_at, updated_at
        FROM role
        WHERE id = p_id;
END;
$$;


-- Процедура для обновления информации о роли
CREATE OR REPLACE PROCEDURE update_role(
    p_id UUID,
    p_name TEXT,
    p_role_key TEXT,
    p_description TEXT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE role
    SET name = p_name,
        role_key = p_role_key,
        description = p_description,
        updated_at = now()
    WHERE id = p_id;
END;
$$;


-- Процедура для удаления роли
CREATE OR REPLACE PROCEDURE delete_role(p_id UUID)
    LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM role WHERE id = p_id;
END;
$$;


-- Процедура для назначения роли пользователю
CREATE OR REPLACE PROCEDURE create_user_role(
    p_user_id UUID,
    p_role_id UUID
)
    LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO user_role (id, user_id, role_id, status, created_at)
    VALUES (uuid_generate_v4(), p_user_id, p_role_id, 1, now());
END;
$$;


-- Функция для получения ролей пользователя по ID
CREATE OR REPLACE FUNCTION get_user_roles(p_user_id UUID)
    RETURNS TABLE (
                      id UUID,
                      user_id UUID,
                      role_id UUID,
                      status INT,
                      created_at TIMESTAMPTZ
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT id, user_id, role_id, status, created_at
        FROM user_role
        WHERE user_id = p_user_id;
END;
$$;


-- Процедура для удаления роли пользователя
CREATE OR REPLACE PROCEDURE delete_user_role(
    p_user_id UUID,
    p_role_id UUID
)
    LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM user_role
    WHERE user_id = p_user_id AND role_id = p_role_id;
END;
$$;


-- Процедура для создания нового пользователя
CREATE OR REPLACE PROCEDURE create_user(
    p_first_name TEXT,
    p_last_name TEXT,
    p_email TEXT,
    p_phone_number TEXT,
    p_user_name TEXT,
    p_password_hash TEXT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO "user" (id, first_name, last_name, email, phone_number, user_name, password_hash, status, created_at)
    VALUES (uuid_generate_v4(), p_first_name, p_last_name, p_email, p_phone_number, p_user_name, p_password_hash, 1, now());
END;
$$;


-- Функция для получения пользователя по ID
CREATE OR REPLACE FUNCTION get_user_by_id(p_id UUID)
    RETURNS TABLE (
                      id UUID,
                      first_name TEXT,
                      last_name TEXT,
                      email TEXT,
                      phone_number TEXT,
                      user_name TEXT,
                      status INT,
                      created_at TIMESTAMPTZ,
                      updated_at TIMESTAMPTZ
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT id, first_name, last_name, email, phone_number, user_name, status, created_at, updated_at
        FROM "user"
        WHERE id = p_id;
END;
$$;


-- Процедура для обновления информации о пользователе
CREATE OR REPLACE PROCEDURE update_user(
    p_id UUID,
    p_first_name TEXT,
    p_last_name TEXT,
    p_email TEXT,
    p_phone_number TEXT,
    p_user_name TEXT
)
    LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE "user"
    SET first_name = p_first_name,
        last_name = p_last_name,
        email = p_email,
        phone_number = p_phone_number,
        user_name = p_user_name,
        updated_at = now()
    WHERE id = p_id;
END;
$$;


-- Процедура для удаления пользователя
CREATE OR REPLACE PROCEDURE delete_user(p_id UUID)
    LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM "user" WHERE id = p_id;
END;
$$;
