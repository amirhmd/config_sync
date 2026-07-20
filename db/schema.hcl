schema "public" {
}

table "configuration_entries" {
  schema = schema.public

  column "id" {
    type = uuid
  }
  column "key" {
    type = text
  }
  column "value" {
    type = text
  }
  column "source" {
    type = text
  }
  column "fetched_at" {
    type = timestamptz
  }

  primary_key {
    columns = [column.id]
  }
}