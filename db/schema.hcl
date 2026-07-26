schema "public" {
}

table "devices" {
  schema = schema.public

  column "id" {
    type = text
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
  column "password" {
    type = text
    null = true
  }
  column "private_key" {
    type = text
    null = true
  }

  primary_key {
    columns = [column.id]
  }
}