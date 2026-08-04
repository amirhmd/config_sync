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
  column "authentication_type" {
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

  check "ck_devices_authentication_type" {
    expr = "authentication_type IN ('password', 'private_key')"
  }

  check "ck_devices_authentication_credential" {
    expr = <<-SQL
      (authentication_type = 'password'
        AND password_encrypted IS NOT NULL
        AND private_key_encrypted IS NULL)
      OR
      (authentication_type = 'private_key'
        AND password_encrypted IS NULL
        AND private_key_encrypted IS NOT NULL)
    SQL
  }
}
