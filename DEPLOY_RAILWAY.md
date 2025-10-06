# 🚀 Guia de Deploy da API Nasa no Railway

## 📋 Pré-requisitos

1. **Conta no Railway**: Crie uma conta gratuita em [railway.app](https://railway.app)
2. **Git instalado**: Certifique-se de ter o Git instalado
3. **Repositório Git**: Seu código precisa estar em um repositório Git (GitHub, GitLab, ou Bitbucket)

---

## 🔧 Passo 1: Preparar o Repositório

### 1.1 Inicializar o Git (se ainda não tiver)

```bash
git init
git add .
git commit -m "Initial commit - Preparando para deploy no Railway"
```

### 1.2 Criar Repositório no GitHub

1. Acesse [github.com](https://github.com) e crie um novo repositório
2. Conecte seu repositório local ao GitHub:

```bash
git remote add origin https://github.com/seu-usuario/seu-repositorio.git
git branch -M main
git push -u origin main
```

---

## 🗄️ Passo 2: Criar Banco de Dados PostgreSQL no Railway

### 2.1 Acessar Railway

1. Faça login em [railway.app](https://railway.app)
2. Clique em **"New Project"**
3. Selecione **"Provision PostgreSQL"**

### 2.2 Obter String de Conexão

1. Clique no serviço PostgreSQL criado
2. Vá na aba **"Variables"**
3. Você verá uma variável chamada `DATABASE_URL` - copie esse valor
4. **IMPORTANTE**: Anote essas informações, você vai precisar delas!

Exemplo de formato:
```
postgresql://postgres:senha@containers-us-west-xxx.railway.app:7432/railway
```

---

## 🚢 Passo 3: Deploy da API no Railway

### 3.1 Criar Novo Serviço

1. No mesmo projeto do Railway (onde está o PostgreSQL), clique em **"New"**
2. Selecione **"GitHub Repo"**
3. Conecte sua conta do GitHub (se ainda não conectou)
4. Selecione o repositório da sua API

### 3.2 Configurar Variáveis de Ambiente

1. Clique no serviço da API que acabou de criar
2. Vá na aba **"Variables"**
3. Adicione as seguintes variáveis:

| Variável | Valor |
|----------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `DATABASE_URL` | (cole a string de conexão do PostgreSQL) |
| `ASPNETCORE_URLS` | `http://0.0.0.0:8080` |

**IMPORTANTE**: A variável `DATABASE_URL` deve ser a mesma do seu banco PostgreSQL no Railway!

### 3.3 Configurar Porta

1. Ainda nas configurações do serviço
2. Vá em **"Settings"**
3. Em **"Networking"**, configure:
   - **Port**: `8080`
4. Habilite **"Public Networking"** para gerar uma URL pública

---

## 🔄 Passo 4: Ajustar appsettings.Production.json

O arquivo `appsettings.Production.json` já foi criado com a configuração correta para usar a variável `DATABASE_URL`.

**Se você precisar usar a string de conexão completa**, edite o arquivo para:

```json
{
  "ConnectionStrings": {
    "NpgsqlDbConnection": "sua-connection-string-aqui"
  }
}
```

---

## ✅ Passo 5: Verificar o Deploy

### 5.1 Acompanhar o Build

1. No Railway, vá na aba **"Deployments"**
2. Clique no deployment mais recente
3. Acompanhe os logs em tempo real
4. Aguarde até ver a mensagem: ✅ **"Build Complete"**

### 5.2 Verificar Logs da Aplicação

1. Vá na aba **"Logs"** (ou **"Observability"** → **"Logs"**)
2. Verifique se a aplicação iniciou corretamente
3. Procure por mensagens como:
   ```
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: http://0.0.0.0:8080
   ```

### 5.3 Testar a API

1. Vá em **"Settings"** → **"Networking"**
2. Copie a URL pública (exemplo: `https://seu-app.up.railway.app`)
3. Teste os endpoints:
   ```
   https://seu-app.up.railway.app/api/RastreamentoTubaroes/v1/
   https://seu-app.up.railway.app/swagger
   ```

---

## 🔍 Passo 6: Configurar Migrações do Banco (se necessário)

Se você usa Entity Framework Migrations:

### 6.1 Aplicar Migrations Remotamente

Existem duas opções:

**Opção A: Via Railway CLI**
```bash
railway run dotnet ef database update --project src/Nasa.API
```

**Opção B: Adicionar ao Dockerfile** (antes do ENTRYPOINT)
```dockerfile
# Adicione antes do ENTRYPOINT
RUN dotnet ef database update
```

---

## 🛠️ Passo 7: Configurações Adicionais (Opcional)

### 7.1 Domínio Customizado

1. Vá em **"Settings"** → **"Domains"**
2. Clique em **"Add Custom Domain"**
3. Siga as instruções para configurar seu domínio

### 7.2 Health Checks

Adicione um endpoint de health check no seu `Program.cs`:

```csharp
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
```

### 7.3 Configurar IP Whitelist para Produção

Como a API está na nuvem, você precisa ajustar o `IpWhitelist` no `appsettings.Production.json`:

```json
{
  "IpWhitelist": {
    "AllowedIps": [
      "0.0.0.0/0"  // Permite todos (ou configure IPs específicos)
    ]
  }
}
```

---

## 🚨 Troubleshooting

### Problema: "Connection refused" ou "Database connection error"

**Solução:**
1. Verifique se a variável `DATABASE_URL` está configurada corretamente
2. Certifique-se de que o PostgreSQL está no mesmo projeto do Railway
3. Teste a conexão manualmente usando as credenciais

### Problema: "Port 5000 already in use"

**Solução:**
1. Certifique-se de que `ASPNETCORE_URLS` está definida como `http://0.0.0.0:8080`
2. Verifique a porta configurada no Railway (deve ser 8080)

### Problema: "404 Not Found" nos endpoints

**Solução:**
1. Verifique se o Swagger está acessível em `/swagger`
2. Certifique-se de usar o caminho completo: `/api/RastreamentoTubaroes/v1/`
3. Verifique os logs para erros de roteamento

### Problema: Build falha

**Solução:**
1. Verifique os logs de build no Railway
2. Certifique-se de que todos os arquivos `.csproj` estão no repositório
3. Teste o build localmente:
   ```bash
   docker build -t nasa-api .
   docker run -p 8080:8080 nasa-api
   ```

---

## 📝 Comandos Úteis

### Testar Docker Localmente

```bash
# Build da imagem
docker build -t nasa-api .

# Rodar container
docker run -p 8080:8080 -e DATABASE_URL="sua-connection-string" nasa-api

# Testar
curl http://localhost:8080/health
```

### Push de Alterações

```bash
git add .
git commit -m "Descrição das mudanças"
git push origin main
```

O Railway automaticamente fará o redeploy após cada push!

---

## 🎉 Pronto!

Sua API está no ar! 🚀

**URL da sua API:** `https://seu-app.up.railway.app`
**Swagger:** `https://seu-app.up.railway.app/swagger`

---

## 💡 Dicas Finais

1. **Monitore os custos**: Railway oferece $5 de crédito grátis por mês
2. **Configure alertas**: Vá em "Settings" → "Notifications"
3. **Backup do banco**: Configure backups regulares do PostgreSQL
4. **Logs**: Use a aba "Observability" para monitorar sua aplicação
5. **Variáveis secretas**: NUNCA commit credenciais no código, use as variáveis de ambiente do Railway

---

## 📚 Recursos Adicionais

- [Documentação Railway](https://docs.railway.app/)
- [.NET Docker](https://learn.microsoft.com/pt-br/dotnet/core/docker/introduction)
- [PostgreSQL no Railway](https://docs.railway.app/databases/postgresql)

