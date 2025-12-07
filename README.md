# catalogo-filmes-previsao-tempo

## Dependências
**Rodar:**


dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.4
dotnet add package TMDbLib


---

## Observações

- Criar o banco de dados `catalogo_filmes` no PostgreSQL.
- O script para criar a tabela *Filme* está em:  
  `catalogo-filmes-previsao-tempo/Data/db.sql`.
- Atualizar dados de conexão no arquivo  
  `catalogo-filmes-previsao-tempo/appsettings.json`.

---

## Variáveis de Ambiente e API Keys (Segurança)


## Ambiente de Desenvolvimento — User Secrets

Use **User Secrets** para guardar a API Key localmente sem expor no repositório.

### Ativar:
```bash
dotnet user-secrets init
````

### Definir a API key:

```bash
dotnet user-secrets set "TMDb:ApiKey" "SUA_API_KEY_AQUI"
```

O arquivo `secrets.json` fica salvo fora do repositório (e não vai para o Git).


 Observação: No .NET, separadores hierárquicos usam **dois underscores `__`**.

