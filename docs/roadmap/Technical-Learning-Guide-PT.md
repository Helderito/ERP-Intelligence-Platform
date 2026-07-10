# Guia Técnico de Aprendizagem

## ERP Intelligence Platform

**Versão:** 1.0  
**Estado:** Documento Vivo  
**Responsável:** Helder Gonçalves  
**Idioma:** Português  
**Documento-guia:** [Technical-Learning-Guide.md](Technical-Learning-Guide.md)

---

# 1. Propósito

Este documento explica, em português claro e acessível, os conceitos técnicos, ferramentas, princípios de engenharia e decisões de arquitetura aplicados no ERP Intelligence Platform.

Enquanto o [Learning Journal](Learning-Journal.md) regista *o que aconteceu* em cada Sprint, este guia explica *os conceitos por trás do que foi aplicado* — o que são, para que servem, e porque foram escolhidos para este projeto especificamente.

Este documento destina-se a estudantes, developers juniores, consultores de ERP, analistas de dados, e a qualquer profissional em transição para engenharia de software que queira perceber como e porquê o ERP Intelligence Platform foi construído desta forma — sem precisar de ler cada ficheiro de código, documento de Sprint ou decisão de arquitetura individualmente.

Termos técnicos que são padrão da indústria (Clean Architecture, Pull Request, Repository Pattern, etc.) mantêm-se em inglês, mas são sempre explicados em português.

---

# 2. Como Usar Este Documento

Os conceitos estão agrupados por área (Engenharia de Software, Backend, Frontend, Base de Dados, DevOps, Testes, Segurança, Inteligência Artificial). Cada conceito segue, sempre que fizer sentido, esta estrutura:

- **O que é?**
- **Para que serve?**
- **Por que foi usado neste projeto?**
- **Como foi aplicado no projeto?**
- **Exemplo prático**
- **Erros comuns a evitar**
- **Relação com outros conceitos**

Este é um documento vivo: cresce a cada Sprint fechado. A versão atual cobre os conceitos aplicados nos Sprints 00, 01, 02, 03 e 04.

---

# 3. Visão Geral do Projeto

O **ERP Intelligence Platform** é uma plataforma ERP (Enterprise Resource Planning — sistema de gestão empresarial) moderna, modular e preparada para a cloud, construída com Clean Architecture, Domain-Driven Design e SOLID, combinando engenharia de software empresarial com desenvolvimento assistido por Inteligência Artificial.

O projeto tem um duplo objetivo: ser um produto de engenharia real e servir como veículo de aprendizagem estruturada para o seu autor, na transição de Data Analyst / Consultor ERP para Engenheiro de Software / Arquiteto de Soluções de IA. A interface é pensada primeiro para utilizadores de língua portuguesa (Portugal e Angola), mas a arquitetura está preparada para internacionalização desde o início.

---

# 4. Engenharia de Software

## Documentation-First Development

**O que é?** Uma abordagem em que a documentação — visão do produto, requisitos, arquitetura, regras de engenharia — é escrita e revista *antes* de qualquer linha de código de aplicação ser criada.

**Para que serve?** Evita que decisões importantes sejam tomadas de forma improvisada ou só na cabeça de quem programa. Torna as decisões visíveis, revistas e discutíveis antes de terem um custo de implementação associado.

**Por que foi usado neste projeto?** Porque o projeto pretende demonstrar disciplina de engenharia real, não apenas produzir código rapidamente. Documentar primeiro obriga a pensar no "porquê" antes do "como".

**Como foi aplicado no projeto?** Todo o Sprint 00 foi dedicado a produzir e rever a documentação completa (Project Charter, PRD, SAD, Engineering Handbook, Backlog, Sprints) antes de existir qualquer projeto de código. Uma inconsistência real no backlog (dois Sprints a implementar autenticação em duplicado) foi encontrada e corrigida *nesta fase*, sem custar nenhuma linha de código reescrita.

**Erros comuns a evitar:** tratar a documentação como um exercício burocrático feito depois do código "para ficar bonito" — nesse caso perde-se o benefício principal, que é apanhar erros antes de custarem trabalho de implementação.

**Relação com outros conceitos:** é o princípio que justifica a existência de todos os outros documentos do projeto (SAD, ADRs, Engineering Handbook) e do próprio [Learning Journal](Learning-Journal.md).

---

## Clean Architecture

**O que é?** Um estilo de arquitetura de software que organiza o código em camadas concêntricas — Domain (domínio de negócio), Application (casos de uso), Infrastructure (detalhes técnicos: base de dados, ficheiros, APIs externas) e uma camada de apresentação (neste projeto, a API) — em que as camadas mais internas nunca dependem das mais externas.

**Para que serve?** Mantém as regras de negócio protegidas de detalhes técnicos que mudam com frequência (qual base de dados usar, qual framework web). Isso torna o sistema mais fácil de testar, de manter e de evoluir sem reescrever o "coração" do negócio.

**Por que foi usado neste projeto?** É a base arquitetural escolhida no [ADR-0001](../decisions/ADR-0001.md), por equilibrar simplicidade de engenharia com capacidade de evolução a longo prazo — importante tanto para um produto real como para um projeto de aprendizagem.

**Como foi aplicado no projeto?** A solução .NET está dividida em `ERP.Domain`, `ERP.Application`, `ERP.Infrastructure` e `ERP.Api`, cada um o seu próprio projeto, com referências que só apontam "para dentro" (a API depende da Application e da Infrastructure; a Infrastructure depende da Application e do Domain; o Domain não depende de mais nada, exceto do Shared Kernel — ver abaixo).

**Exemplo prático:** o `ERP.Domain` não sabe que existe PostgreSQL, EF Core ou ASP.NET Core. Só a `ERP.Infrastructure` sabe disso. Se um dia se trocasse PostgreSQL por outra base de dados, o Domain não mudaria uma linha.

**Erros comuns a evitar:** colocar regras de negócio dentro de Controllers da API (fica difícil de testar e reutilizar), ou deixar o Domain depender de bibliotecas de infraestrutura "só por conveniência".

**Relação com outros conceitos:** funciona em conjunto com Domain-Driven Design (que define *o que* vive dentro do Domain) e é validada automaticamente por Testes de Arquitetura (ver secção de Testes).

---

## Domain-Driven Design (DDD)

**O que é?** Uma abordagem de design de software que modela o código à volta dos conceitos reais do negócio (por exemplo, "Cliente", "Utilizador", "Encomenda"), organizados em *Bounded Contexts* (fronteiras claras onde um determinado vocabulário de negócio se aplica) e *Aggregates* (grupos de objetos tratados como uma unidade consistente, com uma única "porta de entrada" — o Aggregate Root).

**Para que serve?** Evita que o código se torne um conjunto de tabelas de base de dados disfarçadas. Garante que as regras de negócio (por exemplo, "um email tem de ser válido antes de se criar um utilizador") vivem num único sítio, protegidas.

**Por que foi usado neste projeto?** Um ERP tem, por natureza, muitos conceitos de negócio interligados (Clientes, Fornecedores, Stock, Vendas). DDD dá vocabulário e estrutura para organizar isso sem que tudo dependa de tudo.

**Como foi aplicado no projeto?** O [Domain Model](../database/Domain-Model.md) define os Bounded Contexts (Identity, Master Data, Inventory, Sales, …). No Sprint 02, o Bounded Context de Identity ganhou os seus primeiros Aggregates reais: `User` e `RefreshToken`, cada um com o seu Aggregate Root, e *Value Objects* (`EmailAddress`, `PasswordHash`) — objetos sem identidade própria, comparados pelo seu valor, não por um Id. No Sprint 03, o mesmo contexto foi expandido com `Role`, `Permission`, `UserRole` e `RolePermission` para suportar autorização baseada em permissões. No Sprint 04, nasceu o primeiro Bounded Context fora de Identity: Master Data, com o Aggregate `Product`, o Value Object `ProductCode` e as entidades de referência `Category` e `UnitOfMeasure`.

**Exemplo prático:** `EmailAddress.Create("User@Example.com")` valida o formato do email e normaliza-o para minúsculas antes de o `User` sequer existir — a regra de negócio "um email tem de ser válido" vive dentro do próprio Value Object, não espalhada pela aplicação.

**Erros comuns a evitar:** criar Aggregates demasiado grandes (que tentam fazer tudo) ou permitir que código fora do Aggregate Root altere diretamente os seus dados internos.

**Relação com outros conceitos:** é a base do [Shared Kernel](#shared-kernel) (que fornece os blocos de construção — `Entity`, `ValueObject`, `IDomainEvent` — usados por todos os Bounded Contexts) e das [Architecture Decision Records](#architecture-decision-records-adr).

---

## Modular Monolith

**O que é?** Uma arquitetura em que a aplicação é implantada como uma única unidade (ao contrário de microserviços, que são várias aplicações independentes), mas internamente organizada em módulos com fronteiras bem definidas e baixo acoplamento entre eles.

**Para que serve?** Dá grande parte dos benefícios de organização dos microserviços (módulos independentes, responsabilidades claras) sem a complexidade operacional de gerir múltiplos serviços, bases de dados e redes desde o primeiro dia.

**Por que foi usado neste projeto?** O [ADR-0001](../decisions/ADR-0001.md) documenta a decisão: para um ERP em fase inicial, a consistência de negócio (por exemplo, garantir que um stock nunca fica negativo) é normalmente mais importante do que a capacidade de implantar cada módulo de forma independente — algo que os microserviços otimizam, mas que o projeto ainda não precisa.

**Como foi aplicado no projeto?** Toda a aplicação corre como um único processo (`ERP.Api`), mas os Bounded Contexts (Identity, Master Data, …) mantêm-se como áreas de código separadas dentro do Domain. No Sprint 04, isto deixou de ser apenas uma promessa arquitetural: `ERP.Domain.MasterData` e `ERP.Application.MasterData` passaram a existir ao lado de Identity, partilhando o mesmo deployable e a mesma base de dados, mas sem acoplar Product a Identity, Inventory ou Sales.

**Relação com outros conceitos:** é uma decisão de nível mais alto do que Clean Architecture — Clean Architecture organiza as *camadas* dentro de cada módulo; Modular Monolith organiza os *módulos* dentro da aplicação.

---

## Shared Kernel

**O que é?** Um pequeno conjunto de tipos e conceitos partilhados entre vários Bounded Contexts, quando faz sentido não duplicar esse vocabulário em cada um deles. Não é uma camada arquitetural (como Domain ou Infrastructure) — é uma base comum que o próprio Domain pode usar.

**Para que serve?** Evita duplicação de código como classes de igualdade ou gestão de eventos de domínio, que seriam praticamente idênticas em cada Bounded Context se não existisse um sítio comum para as definir.

**Por que foi usado neste projeto?** Porque `User`, `RefreshToken` (Identity) e, no futuro, `Customer`, `Product` (Master Data) partilham conceitos DDD idênticos: têm uma identidade própria (`Entity`), podem ter Value Objects com igualdade por valor (`ValueObject`), e podem levantar eventos de domínio (`IDomainEvent`).

**Como foi aplicado no projeto?** O projeto `ERP.SharedKernel` foi criado no Sprint 02 com três blocos de construção: `Entity<TId>`, `ValueObject` e `IDomainEvent`. Inicialmente, o Domain não tinha sequer referência ao Shared Kernel, e o teste de arquitetura existente proibia *qualquer* referência a partir do Domain — o que impedia fisicamente o seu uso. Isto foi corrigido: o `ERP.Domain` passou a referenciar apenas o `ERP.SharedKernel`, e `User`, `RefreshToken`, `EmailAddress` e `PasswordHash` foram refeitos para herdar destes tipos em vez de reimplementar a mesma lógica. No Sprint 03, `Role`, `Permission`, `UserRole` e `RolePermission` já nasceram a usar o mesmo padrão. No Sprint 04, o padrão repetiu-se fora de Identity: `Product`, `Category` e `UnitOfMeasure` herdam de `Entity<Guid>`, e `ProductCode` herda de `ValueObject`.

**Erros comuns a evitar:** construir um Shared Kernel e não o usar de facto (foi exatamente o que aconteceu inicialmente neste projeto, e foi corrigido em revisão) — se o primeiro Bounded Context não o usa, é pouco provável que os seguintes o venham a usar.

**Relação com outros conceitos:** suporta diretamente o Domain-Driven Design e é protegido pelos Testes de Arquitetura.

---

## Architecture Decision Records (ADR)

**O que é?** Documentos curtos que registam uma decisão de arquitetura importante: o contexto, as opções consideradas, a decisão tomada, as consequências (positivas e negativas) e as referências relacionadas.

**Para que serve?** Evita que decisões importantes fiquem só na memória de quem as tomou. Permite que, meses depois, se perceba *porque* é que algo foi feito de determinada forma — não apenas *o que* foi feito.

**Por que foi usado neste projeto?** Para tornar as decisões de arquitetura explícitas, revistas e rastreáveis, em vez de implícitas no código ou decididas de forma inconsistente Sprint a Sprint.

**Como foi aplicado no projeto?** Existem três ADRs até agora: [ADR-0001](../decisions/ADR-0001.md) (Modular Monolith + Clean Architecture + DDD), [ADR-0002](../decisions/ADR-0002.md) (CQRS aplicado apenas onde a leitura e a escrita divergem genuinamente — não em todo o lado) e [ADR-0003](../decisions/ADR-0003.md) (suporte multi-empresa adiado para depois do MVP, decisão consciente e não um esquecimento).

**Relação com outros conceitos:** as ADRs são o mecanismo formal para registar decisões que, de outra forma, ficariam apenas implícitas na [Software Architecture Document](../03-Software-Architecture-Document.md).

---

## Documentation Drift (Desalinhamento entre Documentação e Código)

**O que é?** O fenómeno em que a documentação deixa de refletir o estado real do código — não porque esteja errada desde o início, mas porque o código evoluiu e a documentação não acompanhou.

**Para que serve (evitar)?** Documentação desalinhada é pior do que nenhuma documentação, porque continua a ser lida e confiada como se fosse verdade. Um leitor (humano ou IA) que confie num diagrama desatualizado toma decisões erradas com confiança total.

**Por que foi relevante neste projeto?** Depois do Sprint 02, o `Domain-Model.md`, o `Entity-Relationship-Diagram.md` e o `C4-Diagrams.md` descreviam os Aggregates `User`/`RefreshToken` (já implementados) e `Role`/`Permission`/todos os Aggregates de Master Data (ainda só planeados) exatamente da mesma forma — sem nenhuma distinção visual ou textual entre "isto já existe" e "isto é o plano".

**Como foi aplicado no projeto?** Antes de começar o Sprint 03, os três documentos foram auditados contra o código real e cada Aggregate passou a ter uma etiqueta explícita — "*Implemented, Sprint 02*" ou "*Planned, Sprint 0X*" — com link para o Sprint correspondente. Também se confirmou, contra o [ADR-0001](../decisions/ADR-0001.md), que nenhuma correção alterava uma decisão de arquitetura aprovada — era só a documentação a apanhar o código, nunca o contrário.

**Erros comuns a evitar:** assumir que documentação escrita corretamente numa fase inicial do projeto permanece correta para sempre — precisa de ser revista sempre que o código que descreve muda, tal como os testes precisam de ser corridos novamente a cada alteração.

**Relação com outros conceitos:** é a razão de existir do [Learning Journal](Learning-Journal.md) e deste próprio guia — ambos são, em parte, um mecanismo para forçar essa revisão a acontecer no fim de cada Sprint, em vez de "algum dia".

---

# 5. Fluxo de Trabalho: Git e GitHub

## Git e GitHub

**O que é?** Git é um sistema de controlo de versões — regista o histórico de alterações a um projeto. GitHub é uma plataforma que aloja repositórios Git na cloud e acrescenta ferramentas de colaboração (Pull Requests, Actions, Projects).

**Para que serve?** Permite trabalhar em alterações isoladamente (branches), rever o histórico completo do projeto, reverter erros, e colaborar (mesmo que, neste momento, o projeto tenha um único autor humano a trabalhar com múltiplos assistentes de IA).

**Como foi aplicado no projeto?** O repositório `Helderito/ERP-Intelligence-Platform` foi criado no Sprint 00, com a branch `main` como branch principal.

**Relação com outros conceitos:** é a base sobre a qual assentam Branch Protection, Pull Requests e Integração Contínua (CI).

---

## Branch Protection

**O que é?** Regras configuradas no GitHub que impedem certas ações diretas sobre uma branch — por exemplo, impedir `push` direto à `main`, exigir que as alterações passem por um Pull Request, ou impedir que a branch seja apagada ou reescrita à força (`force-push`).

**Para que serve?** Garante que nenhuma alteração chega à branch principal sem passar por um processo mínimo de revisão, mesmo em projetos com um único contribuidor — protege contra erros acidentais, não apenas contra má-fé.

**Por que foi usado neste projeto?** Para impor a disciplina de "toda a alteração passa por Pull Request", definida no Engineering Handbook, desde o início — antes de existir qualquer código, para que se tornasse hábito e não uma regra adicionada tarde demais.

**Como foi aplicado no projeto?** A branch `main` está configurada para exigir Pull Request mesmo para o dono do repositório (`enforce_admins` ativo), sem `force-push` nem eliminação da branch permitidos.

**Erros comuns a evitar:** configurar a proteção mas continuar a fazer *merge* sem rever de facto o conteúdo — a proteção só é útil se a revisão associada for real.

---

## Pull Requests e Code Review

**O que é?** Um Pull Request (PR) é um pedido para integrar as alterações de uma branch (normalmente `feature/...`) na branch principal. Code Review é o processo de examinar essas alterações antes de aceitar o PR.

**Para que serve?** Cria um ponto de paragem deliberado entre "código escrito" e "código integrado", onde erros, inconsistências ou desvios da arquitetura podem ser detetados antes de se tornarem parte permanente do projeto.

**Por que foi usado neste projeto?** Porque a implementação de código passou a ser feita pelo Codex, e a revisão é feita no Claude Code — o Pull Request é o ponto de entrega entre os dois.

**Como foi aplicado no projeto?** Em cada Sprint com código (01, 02 e 03), o Codex implementou numa branch `feature/...` e abriu um Pull Request. A revisão no Claude Code não se limita a ler a descrição do PR — volta a correr `dotnet build` e `dotnet test` de forma independente, e já encontrou problemas reais em Sprints anteriores (uma password commitada no Sprint 01; duplicação de código do Shared Kernel no Sprint 02) que não eram visíveis apenas pela descrição do PR.

**Erros comuns a evitar:** confiar cegamente na descrição de verificação de um Pull Request sem voltar a correr os testes — foi exatamente isso que permitiu apanhar os dois problemas reais mencionados acima.

---

# 6. Backend

## ASP.NET Core

**O que é?** A framework da Microsoft para construir APIs e aplicações web em C#.

**Como foi aplicado no projeto?** O projeto `ERP.Api` usa ASP.NET Core para expor endpoints REST, com Controllers (por exemplo, `AuthController`), Dependency Injection, Health Checks, e o middleware de autenticação JWT Bearer.

**Relação com outros conceitos:** é a camada mais externa da Clean Architecture do projeto — depende da Application e da Infrastructure, nunca o contrário.

---

## Dependency Injection (Injeção de Dependências)

**O que é?** Um padrão em que uma classe não cria as suas próprias dependências (por exemplo, um repositório ou um serviço) — em vez disso, recebe-as "de fora", normalmente através do construtor.

**Para que serve?** Torna o código mais fácil de testar (podem-se passar versões falsas/mock das dependências em testes) e reduz o acoplamento entre classes.

**Como foi aplicado no projeto?** O `AuthController` recebe um `AuthenticationService` no construtor; o `AuthenticationService` recebe `IUserRepository`, `IRefreshTokenRepository`, `IPasswordHasher` e `IJwtTokenGenerator` — nenhuma destas classes cria as suas próprias dependências. As implementações concretas são registadas centralmente em métodos `AddApplication()` / `AddInfrastructure()`.

**Relação com outros conceitos:** é o mecanismo técnico que torna possível o Dependency Inversion Principle da Clean Architecture — a Application define interfaces (`IUserRepository`), e é a Infrastructure que as implementa, nunca o contrário.

---

## Entity Framework Core (EF Core) e Migrations

**O que é?** Um ORM (Object-Relational Mapper) para .NET — traduz objetos C# em linhas de tabelas de base de dados relacional, e vice-versa. As *Migrations* são ficheiros gerados automaticamente que descrevem como evoluir o esquema da base de dados de uma versão para a seguinte.

**Para que serve?** Evita escrever SQL manualmente para a maior parte das operações, e mantém o esquema da base de dados sincronizado com o código através de histórico versionado (as migrations ficam no Git, tal como o resto do código).

**Como foi aplicado no projeto?** O `AppDbContext` mapeia `User` e `RefreshToken`. Como estas entidades usam Value Objects (`EmailAddress`, `PasswordHash`), a configuração usa `OwnsOne` — uma forma de dizer ao EF Core "este Value Object faz parte da mesma tabela do seu dono, não é uma tabela à parte". A migration `AddIdentityAuthentication` foi gerada no Sprint 02 para criar as tabelas `User` e `RefreshToken`; no Sprint 03, a migration `AddRolesAndPermissions` adicionou `Role`, `Permission`, `UserRole` e `RolePermission`, com seed inicial do catálogo mínimo de permissões. No Sprint 04, a migration `AddProductCatalog` adicionou `Product`, `Category` e `UnitOfMeasure`, semeando `General`, `Unit` e `Kilogram`, e também a permission `catalog.manage`.

**Erros comuns a evitar:** editar manualmente ficheiros de migration já aplicados, ou esquecer de rever o SQL gerado antes de o aplicar em produção (ver [Migration Strategy](../database/Migration-Strategy.md)).

---

## Master Data

**O que é?** Dados mestres são entidades relativamente estáveis que servem de base a muitos processos operacionais: produtos, clientes, fornecedores, armazéns, categorias, unidades de medida, moedas, condições de pagamento.

**Para que serve?** Num ERP, módulos transacionais como Vendas, Compras e Inventário dependem destes dados. Uma venda não deve inventar o produto; deve referenciar um produto já existente no catálogo.

**Como foi aplicado no projeto?** O Sprint 04 implementou a fundação do Product Catalog dentro de Master Data: `Product` como Aggregate Root, `ProductCode` como Value Object imutável, e `Category`/`UnitOfMeasure` como entidades de referência semeadas por migration. Stock, preço, imagens, variantes e códigos de barras ficaram explicitamente fora do Aggregate para não misturar Product Catalog com Inventory ou Pricing.

**Relação com outros conceitos:** Master Data usa DDD para proteger regras de domínio, EF Core para persistência, API REST para exposição e RBAC para garantir que apenas utilizadores com `catalog.manage` alteram o catálogo.

---

## CRUD com Paginação e Pesquisa

**O que é?** CRUD significa Create, Read, Update, Delete — criar, consultar, atualizar e remover/desativar dados. Em APIs reais, listagens devem suportar paginação (`page`, `pageSize`) e, quando útil, pesquisa (`search`) para não devolver milhares de registos de uma vez.

**Como foi aplicado no projeto?** O Sprint 04 adicionou `GET /products?page=1&pageSize=20&search=...`, `GET /products/{id}`, `POST /products`, `PUT /products/{id}` e `DELETE /products/{id}`. O delete é uma desativação lógica: `Product.IsActive` passa a `false`, mas o registo continua na base de dados para histórico e referências futuras.

**Erros comuns a evitar:** apagar fisicamente dados mestres que podem já estar referenciados por documentos futuros, ou criar endpoints de listagem sem paginação.

---

## Health Checks

**O que é?** Um endpoint (`/health`) que responde se a aplicação está operacional.

**Para que serve?** Permite que ferramentas de infraestrutura (Docker, orquestradores, monitorização) verifiquem automaticamente se a aplicação está viva, sem intervenção humana.

**Como foi aplicado no projeto?** Configurado no Sprint 01 (`app.MapHealthChecks("/health")`) e usado, por exemplo, no `docker-compose.yml`, para o serviço `web` só arrancar depois de o `api` estar saudável.

---

## Swagger / OpenAPI

**O que é?** Uma especificação (OpenAPI) e uma ferramenta (Swagger) que geram documentação interativa de uma API REST diretamente a partir do código.

**Para que serve?** Permite explorar e testar os endpoints da API pelo browser, sem precisar de escrever pedidos HTTP manualmente.

**Como foi aplicado no projeto?** Disponível em `/swagger` apenas em ambiente de desenvolvimento. As convenções de desenho da API estão documentadas em [OpenAPI.md](../api/OpenAPI.md) e [API-Versioning.md](../api/API-Versioning.md).

---

## Serilog

**O que é?** Uma biblioteca de logging estruturado para .NET — em vez de escrever apenas texto simples, permite registar eventos com dados estruturados associados (por exemplo, o Id de um utilizador, não apenas "utilizador autenticado").

**Para que serve?** Facilita a análise de logs mais tarde, especialmente em produção, onde não há forma de "pôr um breakpoint" para depurar.

**Como foi aplicado no projeto?** Configurado no `Program.cs` desde o Sprint 01, a escrever para a consola.

---

# 7. Frontend

## React

**O que é?** Uma biblioteca JavaScript/TypeScript para construir interfaces de utilizador baseadas em componentes reutilizáveis.

**Como foi aplicado no projeto?** A aplicação `erp.web` usa React para a interface — desde o layout base e navegação (Sprint 01) até à página de login e gestão de sessão (Sprint 02).

---

## TypeScript

**O que é?** Uma extensão da linguagem JavaScript que acrescenta tipos estáticos — o código é verificado quanto a erros de tipo antes de ser executado.

**Para que serve?** Apanha uma classe inteira de erros (por exemplo, tratar um número como texto) antes de o código chegar sequer a correr.

**Como foi aplicado no projeto?** Todo o `erp.web` é escrito em TypeScript, incluindo os tipos que representam a sessão de autenticação (`AuthenticationSession`) e os pedidos à API.

---

## Tailwind CSS

**O que é?** Uma biblioteca de CSS baseada em classes utilitárias (por exemplo, `flex`, `p-4`, `text-lg`) aplicadas diretamente no HTML/JSX, em vez de escrever ficheiros CSS separados para cada componente.

**Como foi aplicado no projeto?** Configurado no Sprint 01 para o layout e páginas do `erp.web`.

---

# 8. Base de Dados

## PostgreSQL

**O que é?** Um sistema de gestão de base de dados relacional, open-source.

**Para que serve?** Armazena de forma persistente e estruturada os dados de negócio da aplicação (utilizadores, clientes, produtos, …).

**Como foi aplicado no projeto?** Corre como serviço no `docker-compose.yml`, com um utilizador e password fornecidos por variáveis de ambiente (nunca fixos no código — ver Gestão de Secrets). As tabelas `User` e `RefreshToken` foram criadas no Sprint 02.

---

## Redis

**O que é?** Uma base de dados em memória, usada tipicamente para cache ou para dados de curta duração que precisam de ser lidos muito rapidamente.

**Como foi aplicado no projeto?** Está provisionado no `docker-compose.yml` desde o Sprint 01, mas ainda não é usado por nenhum código da aplicação — é infraestrutura preparada para uso futuro (por exemplo, cache de sessões ou de dados frequentemente consultados).

---

# 9. DevOps

## Docker e Docker Compose

**O que é?** Docker empacota uma aplicação e tudo o que ela precisa para correr (runtime, dependências) numa "imagem" que corre de forma idêntica em qualquer máquina. Docker Compose descreve, num único ficheiro, vários serviços Docker que trabalham em conjunto (por exemplo, API + Base de Dados + Frontend).

**Para que serve?** Elimina o "na minha máquina funciona" — se corre no Docker de alguém, corre no Docker de qualquer pessoa, incluindo em produção.

**Como foi aplicado no projeto?** `docker-compose.yml` define os serviços `api`, `web`, `postgres` e `redis`. Os `Dockerfile` da API e do frontend usam *multi-stage builds* — uma fase para compilar, outra, mais pequena, só para correr a aplicação já compilada.

---

## GitHub Actions e CI (Integração Contínua)

**O que é?** GitHub Actions é a ferramenta de automação do GitHub. CI (Continuous Integration) é a prática de validar automaticamente cada alteração (compilar, correr testes) antes de ela ser aceite.

**Para que serve?** Garante que nenhuma alteração que "parte" o projeto passa despercebida — o feedback é automático e imediato, não depende de alguém se lembrar de correr os testes manualmente.

**Como foi aplicado no projeto?** O workflow `.github/workflows/ci.yml`, criado no Sprint 01, corre `dotnet build`/`dotnet test` e `npm test`/`npm run build` em cada Pull Request. No Sprint 02, os testes de integração passaram a incluir um contentor PostgreSQL real (via Testcontainers), também executado automaticamente em CI.

---

## Gestão de Secrets (Secrets Management)

**O que é?** O conjunto de práticas para lidar com informação sensível (passwords, chaves de assinatura, tokens de API) sem a expor em locais onde não devia estar — nomeadamente, no histórico do Git.

**Para que serve?** Um segredo commitado no Git fica no histórico do repositório para sempre, mesmo que seja removido num commit posterior. Isto é verdade mesmo em repositórios privados, e ainda mais grave em repositórios públicos como este.

**Por que foi usado neste projeto?** Porque, no Sprint 01, uma password de base de dados foi commitada por engano no `appsettings.json`. A correção não foi só remover a password — foi perceber que este era o primeiro ficheiro deste tipo no projeto, e que o padrão usado aqui seria copiado por todos os Sprints seguintes.

**Como foi aplicado no projeto?** `appsettings.json` (ficheiro commitado) nunca contém segredos — apenas configuração sem credenciais. Os segredos reais vivem em `appsettings.Development.json` (excluído do Git via `.gitignore`) ou em variáveis de ambiente. No Sprint 02, esta prática foi reforçada: o `docker-compose.yml` passou a exigir `POSTGRES_PASSWORD` e `JWT_SIGNING_KEY` como variáveis de ambiente obrigatórias, falhando explicitamente se não estiverem definidas, em vez de usar um valor por omissão inseguro.

**Erros comuns a evitar:** assumir que "é só um ambiente de desenvolvimento" torna um segredo commitado inofensivo — o hábito que se cria no primeiro ficheiro é o que se repete em todos os seguintes.

---

# 10. Testes

## Testes Unitários e de Integração

**O que é?** Testes unitários verificam uma unidade de código isolada (por exemplo, uma regra de validação de um Value Object), normalmente sem tocar em base de dados ou rede. Testes de integração verificam que várias partes do sistema funcionam corretamente em conjunto — incluindo, muitas vezes, uma base de dados real.

**Como foi aplicado no projeto?** `tests/ERP.UnitTests` cobre regras de domínio (por exemplo, `EmailAddress` rejeita formatos inválidos, `Role` rejeita nomes vazios, `User.AssignRole` regista o evento esperado, `ProductCode` normaliza SKUs e `Product` mantém o código imutável após criação). `tests/ERP.IntegrationTests` cobre o fluxo completo de autenticação — registo, login com password errada rejeitado, login válido, acesso a `/auth/me` sem token rejeitado, acesso autenticado aceite, renovação do token, logout, e reutilização do token revogado rejeitada — e, desde o Sprint 03, cobre também autorização: criar role, atribuir permission, atribuir role a utilizador, fazer login e validar acesso permitido/proibido. No Sprint 04, passou ainda a cobrir Product Catalog: criar, obter, pesquisar, atualizar e desativar Product contra PostgreSQL real. Tudo isto corre contra uma instância real de PostgreSQL, criada e destruída automaticamente através de *Testcontainers*.

**Relação com outros conceitos:** dá confiança real de que o sistema funciona, complementando a Integração Contínua, que garante que estes testes correm em cada alteração.

---

## Testes de Arquitetura (Architecture Tests)

**O que é?** Testes automatizados que não verificam o comportamento do código, mas sim se a sua *estrutura* respeita as regras de arquitetura definidas — por exemplo, "o Domain nunca pode depender da Infrastructure".

**Para que serve?** Transforma uma regra de arquitetura, que de outra forma só existiria em documentação (fácil de esquecer ou ignorar sem querer), numa verificação automática que falha o build se for violada.

**Como foi aplicado no projeto?** `DomainReferenceTests`, criado no Sprint 01, verifica que o `ERP.Domain` não referencia código de outros projetos. No Sprint 02, este teste precisou de ser ajustado: a versão original proibia *qualquer* referência, incluindo ao Shared Kernel — o que estava correto para "não depender de Application/Infrastructure/Api", mas incorreto para o Shared Kernel, que faz parte do vocabulário do próprio Domain. O teste foi corrigido para verificar exatamente a regra pretendida: o Domain pode depender do Shared Kernel, mas de mais nada.

**Erros comuns a evitar:** assumir que um teste de arquitetura "verde" significa que a arquitetura está correta — pode estar simplesmente a impor uma regra demasiado rígida ou demasiado permissiva, como aconteceu aqui.

---

# 11. Segurança e Autenticação

## Autenticação com JWT (JSON Web Token)

**O que é?** Um JWT é um "cartão de identificação" digital, assinado digitalmente, que prova que um utilizador está autenticado sem que o servidor precise de guardar o seu estado de sessão. Contém informação (*claims*) como o Id do utilizador e uma data de expiração.

**Para que serve?** Permite que a API valide pedidos de utilizadores autenticados de forma rápida (só verificando a assinatura do token) sem consultar a base de dados a cada pedido.

**Por que foi usado neste projeto?** É a abordagem definida na [Software Architecture Document](../03-Software-Architecture-Document.md) desde o início do projeto, e é o standard de facto para autenticação em APIs REST modernas.

**Como foi aplicado no projeto?** O endpoint `POST /auth/login` devolve um `accessToken` (JWT) válido por um curto período (configurável, `AccessTokenMinutes`). O `GET /auth/me`, protegido com `[Authorize]`, só responde com sucesso se receber um JWT válido no cabeçalho `Authorization: Bearer ...`. A chave usada para assinar os tokens nunca está no `appsettings.json` commitado — a aplicação recusa-se mesmo a arrancar se a chave não estiver configurada (ver Gestão de Secrets).

**Erros comuns a evitar:** dar aos tokens de acesso um tempo de vida muito longo (torna-os mais perigosos se forem roubados) — por isso existem também os Refresh Tokens.

**Relação com outros conceitos:** depende diretamente de uma Gestão de Secrets correta (a chave de assinatura é, ela própria, um segredo).

---

## Refresh Tokens

**O que é?** Um token de vida mais longa, guardado de forma mais cuidadosa, usado exclusivamente para obter um novo Access Token quando o anterior expira — sem obrigar o utilizador a fazer login outra vez.

**Para que serve?** Equilibra segurança (Access Tokens de vida curta, menos perigosos se roubados) com conveniência (o utilizador não tem de reintroduzir a password a cada poucos minutos).

**Como foi aplicado no projeto?** Cada `RefreshToken` (Aggregate no Domain) sabe se está expirado (`IsExpired`) e se foi revogado (`IsRevoked`). Ao fazer `POST /auth/refresh`, o token antigo é revogado e um novo é emitido — isto chama-se "rotação" de Refresh Tokens. Ao fazer logout, o Refresh Token é revogado explicitamente, e deixa de poder ser reutilizado (verificado pelo teste de integração do Sprint 02).

**Relação com outros conceitos:** trabalha em conjunto com os Access Tokens JWT — nunca seriam usados isoladamente.

---

## RBAC (Role-Based Access Control)

**O que é?** Um modelo de autorização em que utilizadores recebem *roles* (papéis) e as roles agregam *permissions* (permissões). O utilizador não recebe normalmente permissões uma a uma; recebe uma role que representa uma responsabilidade operacional.

**Para que serve?** Torna a autorização mais fácil de gerir. Em vez de configurar cada utilizador manualmente em dezenas de endpoints, configura-se a role uma vez e atribui-se essa role aos utilizadores certos.

**Como foi aplicado no projeto?** No Sprint 03, o Identity Bounded Context passou a ter `Role`, `Permission`, `UserRole` e `RolePermission`. O catálogo mínimo inicial de permissões é semeado pela migration `AddRolesAndPermissions`, com `roles.manage` e `users.manage`. A API permite criar/atualizar/desativar roles, listar permissions, atribuir permissions a roles e atribuir roles a utilizadores.

**O problema do "primeiro administrador" (bootstrap):** Um sistema RBAC tem um paradoxo inicial. Se gerir roles exige a permissão `roles.manage`, e essa permissão só chega através de uma role atribuída por alguém que já a tenha... quem cria a primeira role numa base de dados vazia? Ninguém — ficava tudo bloqueado. Este projeto resolve-o de duas formas combinadas: uma role `Administrator` (com todas as permissões) é semeada pela migration, e o **primeiro utilizador a registar-se** recebe automaticamente essa role. Os utilizadores seguintes não recebem nenhuma role por omissão. Foi uma lacuna encontrada em revisão — a implementação cumpria todos os critérios de aceitação, mas não havia forma de a usar de ponta a ponta sem esta correção.

**Erros comuns a evitar:** desenhar o mecanismo de permissões e esquecer o bootstrap — o sistema parece completo (e passa nos testes) mas é impossível de operar numa instalação nova.

**Relação com outros conceitos:** RBAC depende de autenticação, mas não é a mesma coisa. Autenticação responde "quem és?"; autorização responde "o que podes fazer?".

---

## Policy-Based Authorization

**O que é?** Um modelo do ASP.NET Core em que endpoints são protegidos por políticas nomeadas, por exemplo `[Authorize(Policy = "roles.manage")]`. A política decide que requisitos precisam de ser satisfeitos para o pedido continuar.

**Para que serve?** Permite expressar regras de acesso de forma explícita no endpoint e manter a lógica de verificação centralizada, em vez de espalhar `if` manuais pelos Controllers.

**Como foi aplicado no projeto?** O Sprint 03 adicionou um `IAuthorizationPolicyProvider` customizado que cria policies dinamicamente a partir do código da permission, e um `PermissionAuthorizationHandler` que consulta as permissões efetivas do utilizador autenticado em runtime. Um utilizador sem token recebe `401 Unauthorized`; um utilizador autenticado mas sem a permission necessária recebe `403 Forbidden`.

**Relação com outros conceitos:** usa claims do JWT para identificar o utilizador e Application Services para carregar as suas permissions reais. Isto mantém os Controllers finos e preserva a separação entre API, Application e Infrastructure.

---

## Hashing de Passwords (BCrypt)

**O que é?** Uma forma de transformar uma password em texto (por exemplo, "MinhaPassword123") num valor irreversível, de forma que, mesmo que a base de dados seja comprometida, a password original não possa ser recuperada diretamente. BCrypt é o algoritmo usado neste projeto.

**Para que serve?** Se as passwords fossem guardadas em texto simples, qualquer acesso não autorizado à base de dados exporia imediatamente as passwords de todos os utilizadores.

**Como foi aplicado no projeto?** `PasswordHasher.Hash()` usa BCrypt para gerar o hash no registo; `PasswordHasher.Verify()` compara a password introduzida no login com o hash guardado, sem nunca "descodificar" o hash de volta para texto (porque isso é matematicamente impossível por design).

**Erros comuns a evitar:** usar algoritmos de hash genéricos (como MD5 ou SHA-256 sozinhos) para passwords — estes são rápidos demais, o que os torna vulneráveis a ataques de força bruta. BCrypt é deliberadamente lento para dificultar esses ataques.

---

# 12. Inteligência Artificial

## Desenvolvimento Assistido por IA (AI-Assisted Engineering)

**O que é?** Usar ferramentas de Inteligência Artificial como parte ativa do processo de engenharia — não apenas para gerar código, mas para planear, rever, documentar e validar decisões.

**Por que foi usado neste projeto?** É um dos objetivos centrais do projeto: aprender a colaborar eficazmente com IA mantendo rigor de engenharia, não abdicando do julgamento humano.

**Como foi aplicado no projeto?** Definido formalmente na [AI Strategy](../ai/AI-Strategy.md) e no [Engineering Handbook](../04-Engineering-Handbook.md): a IA é uma assistente, nunca substitui o pensamento crítico, e todo o código gerado é revisto.

---

## Claude Code, Codex e Cursor Agent — papéis distintos

**O que é?** Três ferramentas de IA distintas, cada uma com um papel definido no projeto, em vez de serem usadas de forma intercambiável.

**Por que foi usado neste projeto?** Ferramentas diferentes têm pontos fortes diferentes. Definir papéis claros evita confusão sobre "quem faz o quê" e mantém consistência entre Sprints.

**Como foi aplicado no projeto?** Documentado em [Claude-Guidelines.md](../ai/Claude-Guidelines.md), [Codex-Guidelines.md](../ai/Codex-Guidelines.md), [Cursor-Rules.md](../ai/Cursor-Rules.md) e [AI-Agents.md](../ai/AI-Agents.md). Na prática, a partir do Sprint 01: o Claude Code prepara o plano (estrutura da solução, decomposição em tarefas, prompt de handoff) e revê o Pull Request depois de implementado; o Codex implementa o código. Esta divisão foi usada de forma consistente nos Sprints 01, 02, 03 e 04, e as revisões de código são feitas sempre no Claude Code — os resultados são partilhados com o Codex apenas como contexto, não como pedido de nova ação.

**Erros comuns a evitar:** usar a ferramenta errada para a tarefa errada (por exemplo, pedir a uma ferramenta de implementação para tomar uma decisão de arquitetura não documentada) — daí a importância da hierarquia de decisão definida em [Cursor-Rules.md](../ai/Cursor-Rules.md).

---

## Prompt Engineering

**O que é?** A prática de estruturar deliberadamente os pedidos feitos a um assistente de IA — papel, objetivo, contexto de negócio, contexto técnico, restrições, tarefa, critérios de aceitação, resultado esperado — em vez de pedir de forma vaga.

**Para que serve?** Reduz ambiguidade e aumenta a probabilidade de o resultado estar alinhado com o que é realmente necessário, sem suposições erradas por parte da IA.

**Como foi aplicado no projeto?** A estrutura está definida em [Prompt-Templates.md](../ai/Prompt-Templates.md), e os prompts reais de handoff para o Codex (Sprints 01, 02, 03 e 04) seguem-na explicitamente, incluindo sempre uma secção clara de "fora de âmbito" para evitar scope creep.

**Relação com outros conceitos:** os [Onboarding Prompts](../ai/Onboarding-Prompts.md) são um exemplo guardado e reutilizável desta prática.

---

# 13. Relação com Outros Documentos

Este documento deve ser lido em conjunto com:

- [Technical Learning Guide Guidelines](Technical-Learning-Guide.md) — as regras que definem como este documento deve ser escrito e mantido.
- [Learning Journal](Learning-Journal.md) — o registo cronológico do que foi feito e aprendido em cada Sprint; este guia explica os conceitos, o Learning Journal explica a evolução real do projeto.
- [Learning Roadmap](Learning-Roadmap.md) — o plano de aprendizagem que este guia acompanha.
- [Software Architecture Document](../03-Software-Architecture-Document.md), [Engineering Handbook](../04-Engineering-Handbook.md) e as [Architecture Decision Records](../decisions/) — as fontes primárias de verdade sobre arquitetura; este guia explica-as, não as substitui.

---

# 14. Critérios de Sucesso

Este guia é considerado útil quando:

- um leitor sem experiência prévia em engenharia de software consegue perceber os conceitos aqui explicados;
- cada conceito está ligado de forma concreta ao ERP Intelligence Platform, não explicado de forma abstrata e genérica;
- o documento evolui a cada Sprint fechado, tal como o Learning Journal;
- o documento explica não só *o que* foi construído, mas *porque* foi construído dessa forma.
