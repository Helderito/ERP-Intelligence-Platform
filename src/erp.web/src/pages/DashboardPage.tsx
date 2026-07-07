const foundationItems = [
  "Backend ASP.NET Core",
  "Frontend React",
  "PostgreSQL",
  "Redis",
  "Docker Compose",
  "CI inicial"
];

export function DashboardPage() {
  return (
    <section className="space-y-6">
      <div className="border-b border-slate-200 pb-5">
        <h2 className="text-2xl font-semibold">Painel inicial</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">
          Base tecnica da Sprint 01, sem funcionalidades de negocio.
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {foundationItems.map((item) => (
          <article key={item} className="rounded-md border border-slate-200 bg-white p-4 shadow-sm">
            <p className="text-sm font-medium text-slate-500">Fundacao</p>
            <h3 className="mt-2 text-lg font-semibold">{item}</h3>
          </article>
        ))}
      </div>
    </section>
  );
}
