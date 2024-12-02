local typedefs = require "kong.db.schema.typedefs"

return {
  name = "feature-check",
  fields = {
    { config = {
        type = "record",
        fields = {
            { required_feature = { type = "string", required = true } },
        },
      },
    },
  },
}