# ⚡ Guia Rápido - Deploy Railway

## 1️⃣ Fazer Push do Código

```bash
git add .
git commit -m "Deploy para Railway"
git push origin main
```

---

## 2️⃣ No Railway - Criar PostgreSQL

1. Acesse [railway.app](https://railway.app)
2. **New Project** → **Provision PostgreSQL**
3. Clique no PostgreSQL → **Variables** → Copie `DATABASE_URL`

---

## 3️⃣ No Railway - Deploy da API

1. No mesmo projeto → **New** → **GitHub Repo**
2. Selecione seu repositório `Nasa-Backend`
3. Clique no serviço da API

---

## 4️⃣ Configurar Variáveis (IMPORTANTE!)

Vá em **Variables** e adicione:

```
ASPNETCORE_ENVIRONMENT = Production
DATABASE_URL = (cole a URL do PostgreSQL)
ASPNETCORE_URLS = http://0.0.0.0:8080
PORT = 8080
```

**⚠️ IMPORTANTE**: Cole a `DATABASE_URL` que você copiou do PostgreSQL!

---

## 5️⃣ Configurar Porta

1. Vá em **Settings** → **Networking**
2. **Port**: `8080`
3. Habilite **Public Networking**

---

## 6️⃣ Aguardar Deploy

1. Vá em **Deployments**
2. Aguarde o build terminar (2-5 minutos)
3. Quando aparecer **"Deployment successful"**, está pronto!

---

## 7️⃣ Testar a API

```
https://seu-app.up.railway.app/health
https://seu-app.up.railway.app/swagger
https://seu-app.up.railway.app/api/RastreamentoTubaroes/v1/
```

---

## 🚨 Se Houver Erros

### Ver os Logs
1. **Deployments** → Clique no deployment → **View logs**

### Erros Comuns

**"Connection refused"**
- Verifique se `DATABASE_URL` está correta
- Certifique-se de que copiou do PostgreSQL do Railway

**"502 Bad Gateway"**
- Verifique se `PORT` = `8080`
- Verifique se `ASPNETCORE_URLS` = `http://0.0.0.0:8080`
- Reinicie o serviço: **Settings** → **Restart**

**"Application failed to start"**
1. Clique em **View logs**
2. Procure por linhas com `ERROR` ou `Exception`
3. Me envie o erro específico para eu ajudar!

---

## ✅ Checklist Final

- [ ] PostgreSQL criado no Railway
- [ ] `DATABASE_URL` copiada do PostgreSQL
- [ ] Código no GitHub (pushed)
- [ ] API conectada ao GitHub
- [ ] 4 variáveis configuradas (ASPNETCORE_ENVIRONMENT, DATABASE_URL, ASPNETCORE_URLS, PORT)
- [ ] Porta 8080 configurada
- [ ] Public Networking habilitado
- [ ] Build concluído com sucesso
- [ ] `/health` retorna 200 OK

---

## 🆘 Se Precisar de Ajuda

**Me envie:**
1. Print ou cópia dos logs (clique em **View logs**)
2. Print das variáveis de ambiente (aba **Variables**)
3. A mensagem de erro específica

---

**Qualquer dúvida, só perguntar! 🚀**

