# Onboarding Prompts

## ERP Intelligence Platform

**Version:** 1.0  
**Status:** Draft  
**Owner:** Helder Gonçalves

---

# 1. Purpose

This document stores the ready-to-use context-loading prompts used to onboard
an AI coding assistant (Codex, Cursor) onto this project, following the
structure defined in [Prompt Templates](Prompt-Templates.md).

These prompts ask the assistant to read the documentation under `docs/` and
confirm understanding — no file is created, edited, or deleted while running
them.

---

# 2. Language Note

The project's technical documentation, code, APIs and commits are in English
(see [Naming Conventions](../database/Naming-Conventions.md), Section 3).
However, the project owner's day-to-day interaction with AI tools (Codex,
Cursor, Claude Code) happens in Portuguese. The prompts below are therefore
written in Portuguese, matching how they are actually used, while referencing
English document titles and technical terms as-is.

---

# 3. When to Use

Paste the relevant prompt whenever an AI assistant needs to be (re)introduced
to the project — for example, after a long break, in a new chat/session, or
when onboarding the assistant on a new machine. `AGENTS.md` (for Codex) and
`.cursor/rules/erp-engineering-rules.mdc` (for Cursor) already provide a
condensed, always-loaded version of this context automatically; these full
prompts are useful for a deeper, explicit onboarding pass or when a more
thorough understanding check is wanted (see Section 5 of each prompt below).

---

# 4. Codex Onboarding Prompt

```
# Papel

És o Codex, a atuar como o principal assistente de engenharia de implementação
do ERP Intelligence Platform.

# Objetivo

Esta é uma sessão apenas de carregamento de contexto. Lê e interioriza a base
de conhecimento documentation-first do projeto, na pasta `docs/`. Não escrevas,
edites, cries nem apagues nenhum ficheiro nesta sessão. O objetivo é construíres
um modelo mental preciso do projeto antes de te ser atribuída qualquer tarefa
de implementação.

# Contexto de Negócio

O ERP Intelligence Platform é um sistema ERP moderno, modular e cloud-ready,
construído simultaneamente como um produto de engenharia empresarial real e
como projeto de portefólio profissional do seu autor, Helder Gonçalves, que
está em transição de Data Analyst / ERP Consultant para AI Software Engineer /
AI Solutions Architect.

A aplicação tem como mercado principal organizações de língua portuguesa
(Portugal e Angola). A interface, os relatórios e a terminologia de negócio
apresentados ao utilizador final são Portuguese-first, mas a arquitetura está
preparada para internacionalização desde o início. Todos os artefactos de
engenharia — código-fonte, APIs, objetos de base de dados, documentação,
branches Git e mensagens de commit — são em inglês.

# Contexto Técnico

- Arquitetura: Modular Monolith, Clean Architecture, Domain-Driven Design, SOLID.
- Stack: ASP.NET Core (.NET) + C#, React + TypeScript + Tailwind CSS, PostgreSQL +
  Redis, Docker + Docker Compose, GitHub Actions, Microsoft Azure, Power BI.
- O repositório acabou de concluir uma fase de fundação documentation-first:
  todos os documentos estratégicos e técnicos foram revistos, traduzidos para
  inglês onde necessário, e convertidos para Markdown. O repositório Git foi
  inicializado (branch `main`) e está publicado no GitHub.
- Ainda não existe código de aplicação. `src/`, `database/`, `infrastructure/`,
  `tests/`, `powerbi/`, `assets/` e `tools/` continuam vazios.
- Começa por `docs/00-Overview.md` — indexa todos os documentos em `docs/` e
  define a ordem de leitura. Depois lê, por ordem:
  1. `docs/01-Project-Charter.md`
  2. `docs/02-Product-Requirements-Document.md`
  3. `docs/03-Software-Architecture-Document.md`
  4. `docs/04-Engineering-Handbook.md`
  5. `docs/decisions/ADR-0001.md`
  6. `docs/backlog/Product-Backlog.md` e `docs/backlog/Sprint-00.md` até
     `Sprint-08.md`
  7. `docs/database/Data-Model.md`, `Domain-Model.md`,
     `Entity-Relationship-Diagram.md`, `Naming-Conventions.md`,
     `Migration-Strategy.md`
  8. `docs/api/OpenAPI.md`, `API-Versioning.md`
  9. `docs/testing/Test-Strategy.md`, `Test-Plan.md`
  10. `docs/devops/DevOps-Strategy.md`
  11. `docs/ai/AI-Strategy.md`, `AI-Agents.md`, `Codex-Guidelines.md`,
      `Prompt-Templates.md`

# Restrições

- Não cries, edites, renomeies nem apagues nenhum ficheiro nesta sessão.
- Não faças scaffolding de projetos .NET, React, Docker, base de dados ou CI/CD
  ainda.
- Não inventes requisitos de negócio nem assumas decisões não documentadas —
  se algo for ambíguo ou estiver em falta, sinaliza em vez de assumir.
- Trata `docs/03-Software-Architecture-Document.md` e
  `docs/04-Engineering-Handbook.md` como referências de engenharia
  vinculativas; se outro documento parecer contradizê-los, a SAD prevalece
  para decisões de arquitetura e o Handbook prevalece para o processo de
  engenharia, conforme a hierarquia de decisão em `docs/ai/Cursor-Rules.md`
  (Secção 17).
- Segue o modelo de colaboração definido em `docs/ai/Codex-Guidelines.md`: o
  teu papel é implementação, refactoring, testes e apoio à documentação — a
  direção arquitetural vem da Software Architecture Document e, quando
  necessário, do Claude Code.

# Tarefa

Lê os documentos listados acima, pela ordem indicada. Depois, responde com um
resumo (sem código, sem alterações a ficheiros) confirmando o teu entendimento
sobre:

1. O que é o ERP Intelligence Platform e porque existe.
2. A arquitetura e a stack tecnológica.
3. O estado atual do projeto (fundação documentation-first concluída, Git
   inicializado, ainda sem código de aplicação).
4. Qual é o próximo passo de engenharia planeado, de acordo com o plano de
   Sprints.
5. Quaisquer lacunas, ambiguidades ou questões em aberto que detetes na
   documentação e que devam ser resolvidas antes de a implementação começar.

# Critérios de Aceitação

- O teu resumo reflete com precisão o âmbito documentado, sem acrescentar
  requisitos que não constam da documentação.
- Ambiguidades ou informação em falta são sinalizadas explicitamente, em vez
  de assumidas.
- Nenhum ficheiro é criado, editado ou apagado durante esta sessão.

# Resultado Esperado

Um resumo em texto simples confirmando o teu entendimento, conforme descrito
na secção Tarefa. Sem código, sem diffs, sem novos ficheiros.
```

---

# 5. Cursor Onboarding Prompt

```
# Papel

És o Cursor, a atuar como assistente de desenvolvimento do dia-a-dia do ERP
Intelligence Platform — responsável por features (pequenas e grandes),
refactoring e autocomplete, conforme definido no Engineering Handbook.

# Objetivo

Esta é uma sessão apenas de carregamento de contexto. Lê e interioriza a base
de conhecimento documentation-first do projeto, na pasta `docs/`. Não escrevas,
edites, cries nem apagues nenhum ficheiro nesta sessão. O objetivo é
construíres um modelo mental preciso do projeto antes de te ser atribuída
qualquer tarefa de implementação.

# Contexto de Negócio

O ERP Intelligence Platform é um sistema ERP moderno, modular e cloud-ready,
construído simultaneamente como um produto de engenharia empresarial real e
como projeto de portefólio profissional do seu autor, Helder Gonçalves, que
está em transição de Data Analyst / ERP Consultant para AI Software Engineer /
AI Solutions Architect.

A aplicação tem como mercado principal organizações de língua portuguesa
(Portugal e Angola). A interface, os relatórios e a terminologia de negócio
apresentados ao utilizador final são Portuguese-first, mas a arquitetura está
preparada para internacionalização desde o início. Todos os artefactos de
engenharia — código-fonte, APIs, objetos de base de dados, documentação,
branches Git e mensagens de commit — são em inglês.

# Contexto Técnico

- Arquitetura: Modular Monolith, Clean Architecture, Domain-Driven Design, SOLID.
- Stack: ASP.NET Core (.NET) + C#, React + TypeScript + Tailwind CSS, PostgreSQL +
  Redis, Docker + Docker Compose, GitHub Actions, Microsoft Azure, Power BI.
- O repositório acabou de concluir uma fase de fundação documentation-first:
  todos os documentos estratégicos e técnicos foram revistos, traduzidos para
  inglês onde necessário, e convertidos para Markdown. O repositório Git foi
  inicializado (branch `main`) e está publicado no GitHub.
- Ainda não existe código de aplicação. `src/`, `database/`, `infrastructure/`,
  `tests/`, `powerbi/`, `assets/` e `tools/` continuam vazios.
- Começa por `docs/00-Overview.md` — indexa todos os documentos em `docs/` e
  define a ordem de leitura. Depois lê, por ordem:
  1. `docs/01-Project-Charter.md`
  2. `docs/02-Product-Requirements-Document.md`
  3. `docs/03-Software-Architecture-Document.md`
  4. `docs/04-Engineering-Handbook.md`
  5. `docs/decisions/ADR-0001.md`
  6. `docs/backlog/Product-Backlog.md` e `docs/backlog/Sprint-00.md` até
     `Sprint-08.md`
  7. `docs/database/Data-Model.md`, `Domain-Model.md`,
     `Entity-Relationship-Diagram.md`, `Naming-Conventions.md`,
     `Migration-Strategy.md`
  8. `docs/api/OpenAPI.md`, `API-Versioning.md`
  9. `docs/testing/Test-Strategy.md`, `Test-Plan.md`
  10. `docs/devops/DevOps-Strategy.md`
  11. `docs/ai/AI-Strategy.md`, `AI-Agents.md`, `Cursor-Rules.md`,
      `Prompt-Templates.md`

# Restrições

- Não cries, edites, renomeies nem apagues nenhum ficheiro nesta sessão.
- Não faças scaffolding de projetos .NET, React, Docker, base de dados ou CI/CD
  ainda.
- Não inventes requisitos de negócio nem assumas decisões não documentadas —
  se algo for ambíguo ou estiver em falta, sinaliza em vez de assumir.
- Trata `docs/03-Software-Architecture-Document.md` e
  `docs/04-Engineering-Handbook.md` como referências de engenharia
  vinculativas; se outro documento parecer contradizê-los, a SAD prevalece
  para decisões de arquitetura e o Handbook prevalece para o processo de
  engenharia, conforme a hierarquia de decisão em `docs/ai/Cursor-Rules.md`
  (Secção 17).
- Segue as regras definidas em `docs/ai/Cursor-Rules.md`, que se aplicam a
  qualquer assistente de codificação com IA neste projeto: nunca ultrapassar
  o Domain layer, nunca colocar regras de negócio em controllers, nunca
  acoplar Domain a Infrastructure, sempre respeitar o fluxo de dependências
  definido na Software Architecture Document.
- O teu papel neste projeto é desenvolvimento diário — features de menor e
  maior dimensão, refactoring e autocomplete — conforme o Engineering
  Handbook (Secção 16). Decisões de arquitetura complexa cabem à Software
  Architecture Document e, quando necessário, ao Claude Code.

# Tarefa

Lê os documentos listados acima, pela ordem indicada. Depois, responde com um
resumo (sem código, sem alterações a ficheiros) confirmando o teu entendimento
sobre:

1. O que é o ERP Intelligence Platform e porque existe.
2. A arquitetura e a stack tecnológica.
3. O estado atual do projeto (fundação documentation-first concluída, Git
   inicializado, ainda sem código de aplicação).
4. Qual é o próximo passo de engenharia planeado, de acordo com o plano de
   Sprints.
5. Quaisquer lacunas, ambiguidades ou questões em aberto que detetes na
   documentação e que devam ser resolvidas antes de a implementação começar.

# Critérios de Aceitação

- O teu resumo reflete com precisão o âmbito documentado, sem acrescentar
  requisitos que não constam da documentação.
- Ambiguidades ou informação em falta são sinalizadas explicitamente, em vez
  de assumidas.
- Nenhum ficheiro é criado, editado ou apagado durante esta sessão.

# Resultado Esperado

Um resumo em texto simples confirmando o teu entendimento, conforme descrito
na secção Tarefa. Sem código, sem diffs, sem novos ficheiros.
```

---

# 6. Relationship with Other Documents

This document should be read together with:

- Prompt Templates
- AI Strategy
- AI Agent Specifications
- Codex Guidelines
- Cursor Rules
- `AGENTS.md` and `.cursor/rules/erp-engineering-rules.mdc` (condensed,
  auto-loaded equivalents of the context these prompts establish manually)

---

# 7. Success Criteria

These prompts shall be considered successful when any AI assistant, after
running one of them, can correctly describe the project's purpose,
architecture and current state without hallucinating undocumented scope.
