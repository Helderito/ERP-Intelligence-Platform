import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/useAuth";

const navigationItems = [
  { label: "Painel", to: "/" },
  { label: "Definicoes", to: "/settings" }
];

export function AppLayout() {
  const { session, logout } = useAuth();

  return (
    <div className="min-h-screen bg-slate-50 text-slate-950">
      <header className="border-b border-slate-200 bg-white">
        <div className="mx-auto flex max-w-7xl items-center justify-between px-6 py-4">
          <div>
            <p className="text-sm font-medium text-brand-700">ERP Intelligence Platform</p>
            <h1 className="text-xl font-semibold">Fundacao da plataforma</h1>
          </div>
          <nav aria-label="Navegacao principal" className="flex gap-2">
            {navigationItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  [
                    "rounded-md px-3 py-2 text-sm font-medium transition",
                    isActive
                      ? "bg-brand-600 text-white"
                      : "text-slate-600 hover:bg-slate-100 hover:text-slate-950"
                  ].join(" ")
                }
              >
                {item.label}
              </NavLink>
            ))}
            <button
              type="button"
              onClick={logout}
              className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-100"
            >
              Sair
            </button>
          </nav>
        </div>
        <div className="mx-auto max-w-7xl px-6 pb-3 text-sm text-slate-600">
          Sessao iniciada como <span className="font-medium text-slate-900">{session?.email}</span>
        </div>
      </header>

      <main className="mx-auto max-w-7xl px-6 py-8">
        <Outlet />
      </main>
    </div>
  );
}
