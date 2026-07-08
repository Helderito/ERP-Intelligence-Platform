import { FormEvent, useEffect, useState } from "react";
import { authorizationService, Permission, Role } from "../auth/authorizationService";
import { useAuth } from "../auth/useAuth";

export function RolesPage() {
  const { session } = useAuth();
  const [roles, setRoles] = useState<Role[]>([]);
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [name, setName] = useState("");
  const [selectedPermissions, setSelectedPermissions] = useState<Record<string, string>>({});
  const [error, setError] = useState<string | null>(null);

  async function loadData() {
    if (!session) {
      return;
    }

    const [nextRoles, nextPermissions] = await Promise.all([
      authorizationService.getRoles(session),
      authorizationService.getPermissions(session)
    ]);

    setRoles(nextRoles);
    setPermissions(nextPermissions);
  }

  useEffect(() => {
    loadData().catch(() => setError("Nao foi possivel carregar roles."));
  }, [session]);

  async function handleCreateRole(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!session) {
      return;
    }

    try {
      await authorizationService.createRole(session, name);
      setName("");
      await loadData();
    } catch {
      setError("Nao foi possivel criar a role.");
    }
  }

  async function assignPermission(roleId: string) {
    if (!session || !selectedPermissions[roleId]) {
      return;
    }

    await authorizationService.assignPermissions(session, roleId, [selectedPermissions[roleId]]);
    await loadData();
  }

  return (
    <section className="space-y-6">
      <div className="border-b border-slate-200 pb-5">
        <h2 className="text-2xl font-semibold">Gestao de roles</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">
          Criacao, atualizacao e associacao de permissions a roles.
        </p>
      </div>

      <form className="flex max-w-xl gap-3" onSubmit={handleCreateRole}>
        <input
          value={name}
          onChange={(event) => setName(event.target.value)}
          required
          placeholder="Nome da role"
          className="min-w-0 flex-1 rounded-md border border-slate-300 px-3 py-2 text-sm"
        />
        <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white" type="submit">
          Criar
        </button>
      </form>

      {error ? <p className="text-sm font-medium text-red-700">{error}</p> : null}

      <div className="grid gap-4">
        {roles.map((role) => (
          <article key={role.id} className="rounded-md border border-slate-200 bg-white p-4 shadow-sm">
            <div className="flex items-start justify-between gap-4">
              <div>
                <h3 className="text-lg font-semibold">{role.name}</h3>
                <p className="mt-1 text-sm text-slate-600">{role.isActive ? "Ativa" : "Inativa"}</p>
                <p className="mt-2 text-sm text-slate-500">{role.permissionIds.length} permissions associadas</p>
              </div>
              <button
                type="button"
                onClick={() => session && authorizationService.deactivateRole(session, role.id).then(loadData)}
                className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
              >
                Desativar
              </button>
            </div>

            <div className="mt-4 flex gap-3">
              <select
                value={selectedPermissions[role.id] ?? ""}
                onChange={(event) =>
                  setSelectedPermissions((current) => ({ ...current, [role.id]: event.target.value }))
                }
                className="min-w-0 flex-1 rounded-md border border-slate-300 px-3 py-2 text-sm"
              >
                <option value="">Selecionar permission</option>
                {permissions.map((permission) => (
                  <option key={permission.id} value={permission.id}>
                    {permission.code}
                  </option>
                ))}
              </select>
              <button
                type="button"
                onClick={() => assignPermission(role.id)}
                className="rounded-md bg-slate-900 px-4 py-2 text-sm font-semibold text-white"
              >
                Atribuir
              </button>
            </div>
          </article>
        ))}
      </div>
    </section>
  );
}
