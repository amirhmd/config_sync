schema "public" {
}

table "devices" {
  schema = schema.public

  column "id" {
    type = bigint
    identity {
      generated = ALWAYS
    }
  }

  column "name" {
    type = varchar(64)
  }

  column "host" {
    type = text
  }

  column "port" {
    type = int
  }

  column "username" {
    type = text
  }

  column "password_encrypted" {
    type = bytea
    null = true
  }

  column "private_key_encrypted" {
    type = bytea
    null = true
  }

  column "created_at" {
    type    = timestamptz
    default = sql("now()")
  }

  primary_key {
    columns = [column.id]
  }

  index "ux_devices_name" {
    unique  = true
    columns = [column.name]
  }

  check "ck_devices_authentication_credential" {
    expr = "num_nonnulls(password_encrypted, private_key_encrypted) = 1"
  }
}