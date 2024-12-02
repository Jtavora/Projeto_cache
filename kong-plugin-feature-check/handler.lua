local kong = kong
local jwt_decoder = require "kong.plugins.jwt.jwt_parser"

local FeatureCheckHandler = {
  PRIORITY = 1000,
  VERSION = "1.0",
}

function FeatureCheckHandler:access(conf)
  -- Obtenha o token JWT do cabeçalho de autorização
  local token = kong.request.get_header("Authorization")
  if not token then
    return kong.response.exit(401, { message = "Missing JWT token" })
  end

  -- Decodifique o token JWT
  local decoded_token, err = jwt_decoder:new(token:sub(8)) -- Remove "Bearer " do token
  if err then
    return kong.response.exit(401, { message = "Invalid JWT token" })
  end

  -- Extraia a lista de features do token
  local features = decoded_token.claims.features
  if not features then
    return kong.response.exit(403, { message = "Access denied: no features found" })
  end

  -- Identifique qual serviço está sendo acessado
  local service_name = kong.router.get_service().name

  -- Defina as features necessárias para cada serviço
  local feature_requirements = {
    ["log-monitoring-service"] = "logCheck",
    ["ecommerce-service"] = "eCommerce"
  }

  -- Verifique se o serviço tem uma feature necessária
  local required_feature = feature_requirements[service_name]
  if not required_feature then
    return kong.response.exit(403, { message = "Access denied: unknown service" })
  end

  -- Verifique se o usuário tem a feature necessária
  for _, feature in ipairs(features) do
    if feature == "all" or feature == required_feature then
      return -- Permita o acesso
    end
  end

  return kong.response.exit(403, { message = "Access denied: feature not allowed" })
end

return FeatureCheckHandler