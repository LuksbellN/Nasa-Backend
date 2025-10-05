# Filtro de IP Whitelist

## Descrição

Este filtro permite restringir o acesso a endpoints específicos baseado em uma lista de IPs autorizados (whitelist). O filtro suporta:

- **IPs individuais**: `127.0.0.1`, `192.168.1.100`
- **Wildcards**: `192.168.1.*`, `10.0.*.*`
- **Notação CIDR**: `10.0.0.0/8`, `192.168.0.0/16`
- **IPv6**: `::1`, `2001:db8::/32`

## Como Usar

### 1. Aplicar em um Endpoint Específico

```csharp
[HttpPost]
[Route("v1/")]
[IpWhitelist] // Apenas IPs da whitelist podem acessar
public async Task<IActionResult> Post([FromBody] MeuDto dto)
{
    // Seu código aqui
}
```

### 2. Aplicar no Controller Inteiro

```csharp
[Route("api/[controller]")]
[ApiController]
[IpWhitelist] // Todos os endpoints deste controller exigem IP autorizado
public class AdminController : BaseController
{
    // Todos os métodos herdam a restrição de IP
}
```

### 3. Usar Configuração Customizada

Você pode especificar uma chave de configuração diferente:

```csharp
[HttpDelete]
[Route("v1/{id}")]
[IpWhitelist(ConfigurationKey = "IpWhitelist:AdminIps")]
public async Task<IActionResult> Delete(int id)
{
    // Usa uma lista de IPs diferente
}
```

## Configuração

### appsettings.json

```json
{
  "IpWhitelist": {
    "AllowedIps": [
      "127.0.0.1",          // Localhost IPv4
      "::1",                // Localhost IPv6
      "192.168.1.*",        // Toda a subnet 192.168.1.x
      "10.0.0.0/8"          // Range CIDR 10.0.0.0 a 10.255.255.255
    ],
    "AdminIps": [
      "192.168.1.100",
      "192.168.1.101"
    ]
  }
}
```

### appsettings.Development.json

Para ambiente de desenvolvimento, você pode permitir ranges mais amplas:

```json
{
  "IpWhitelist": {
    "AllowedIps": [
      "127.0.0.1",
      "::1",
      "192.168.*.*"        // Qualquer IP da rede local
    ]
  }
}
```

## Exemplos de Padrões de IP

### IPs Individuais
```json
"AllowedIps": [
  "127.0.0.1",
  "192.168.1.100",
  "2001:0db8:85a3::8a2e:0370:7334"
]
```

### Wildcards
```json
"AllowedIps": [
  "192.168.1.*",         // 192.168.1.0 a 192.168.1.255
  "10.0.*.*",            // 10.0.0.0 a 10.0.255.255
  "172.16.*.1"           // Todos os .1 em 172.16.x.x
]
```

### Notação CIDR
```json
"AllowedIps": [
  "10.0.0.0/8",          // 10.0.0.0 a 10.255.255.255
  "172.16.0.0/12",       // 172.16.0.0 a 172.31.255.255
  "192.168.0.0/16",      // 192.168.0.0 a 192.168.255.255
  "192.168.1.0/24"       // 192.168.1.0 a 192.168.1.255
]
```

## Comportamento

### IP Autorizado
- Status: `200 OK`
- A requisição prossegue normalmente

### IP Não Autorizado
```json
{
  "message": "IP não autorizado para acessar este recurso",
  "statusCode": 403,
  "ip": "203.0.113.50"
}
```

### IP Não Detectado
```json
{
  "message": "Não foi possível validar o IP da requisição",
  "statusCode": 403
}
```

## Logs

O filtro gera logs informativos:

```
[Information] Validando IP: 192.168.1.100
[Information] IP autorizado: 192.168.1.100
```

Ou logs de alerta para IPs não autorizados:

```
[Warning] IP não autorizado tentou acessar o endpoint: 203.0.113.50
```

## Considerações de Segurança

1. **Proxies e Load Balancers**: Se sua aplicação está atrás de um proxy ou load balancer, certifique-se de configurar o `ForwardedHeadersOptions` no `Program.cs`:

```csharp
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("10.0.0.1")); // IP do seu proxy
});
```

2. **Lista Vazia**: Se a lista de IPs autorizados estiver vazia, **todos os acessos serão bloqueados**.

3. **Ambiente de Desenvolvimento**: Use configurações mais permissivas em `appsettings.Development.json` para facilitar o desenvolvimento local.

4. **IPv6**: O filtro normaliza automaticamente endereços IPv6 mapeados para IPv4 (ex: `::ffff:192.168.1.1` → `192.168.1.1`).

## Testando

### Teste Manual com cURL

```bash
# Teste de IP autorizado
curl -X POST https://localhost:5001/api/rastreamentotubaroes/v1/ \
  -H "Content-Type: application/json" \
  -d '{"lat": -23.5, "lon": -46.6}'

# Teste de IP não autorizado (via proxy)
curl -X POST https://localhost:5001/api/rastreamentotubaroes/v1/ \
  -H "Content-Type: application/json" \
  -H "X-Forwarded-For: 203.0.113.50" \
  -d '{"lat": -23.5, "lon": -46.6}'
```

### Teste em Ambiente de Produção

1. Adicione o IP do servidor de produção na whitelist
2. Configure os headers forwarded se estiver usando load balancer
3. Monitore os logs para verificar IPs sendo bloqueados indevidamente
4. Ajuste as ranges CIDR conforme necessário

## Troubleshooting

### Problema: Meu IP está sendo bloqueado mesmo estando na whitelist

**Solução**: Verifique o IP real que está chegando:
1. Olhe os logs da aplicação - eles mostram o IP detectado
2. Verifique se está usando proxy/load balancer e configure os headers forwarded
3. Teste com `192.168.*.*` temporariamente para confirmar que é problema de match

### Problema: Lista de IPs vazia não bloqueia requisições

**Solução**: Isso não deveria acontecer. Se acontecer, verifique se:
1. O filtro está registrado corretamente no `Program.cs`
2. O atributo `[IpWhitelist]` está presente no endpoint
3. Não há outro filtro sobrescrevendo o comportamento

### Problema: IPv6 não está funcionando

**Solução**: 
1. Certifique-se de que o Kestrel está configurado para aceitar IPv6
2. Adicione `::1` (localhost IPv6) na whitelist para testes locais
3. Use notação CIDR para ranges IPv6: `2001:db8::/32`

