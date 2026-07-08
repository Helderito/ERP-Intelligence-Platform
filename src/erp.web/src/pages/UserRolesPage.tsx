import { FormEvent, useEffect, useState } from "react";
import { authorizationService, Role } from "../auth/authorizationService";
import { useAuth } from "../auth/useAuth";

export function UserRolesPage() {
  const { session } = useAuth();
  const [roles, setRoles] = useState<Role[]>([]);
  const [userId, setUserId] = useState("");
  const [roleId, setRoleId] = useState("");
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!session) {
      return;
    }

    authorizationService.getRoles(session).then(setRoles);
  }, [session]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!session || !roleId) {
      return;
    }

    await authorizationService.assignRolesToUser(session, userId, [roleId]);
    setMessage("Role atribuida ao utilizador.");
  }

  return (
    <section className="space-y-6">
      <div className="border-b border-slate-200 pb-5">
        <h2 className="text-2xl font-semibold">Roles de utilizadores</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">
          Atribuicao direta de roles a um utilizador existente.
        </p>
      </div>

      <form className="max-w-xl space-y-4" onSubmit={handleSubmit}>
        <label className="block">
          <span className="text-sm font-medium text-slate-700">UserId</span>
          <input
            value={userId}
            onChange={(event) => setUserId(event.target.value)}
            required
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
          />
        </label>

        <label className="block">
          <span className="text-sm font-medium text-slate-700">Role</span>
          <select
            value={roleId}
            onChange={(event) => setRoleId(event.target.value)}
            required
            className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
          >
            <option value="">Selecionar role</option>
            {roles
              .filter((role) => role.isActive)
              .map((role) => (
                <option key={role.id} value={role.id}>
                  {role.name}
                </option>
              ))}
          </select>
        </label>

        <button type="submit" className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white">
          Atribuir role
        </button>
      </form>

      {message ? <p className="text-sm font-medium text-green-700">{message}</p> : null}
    </section>
  );
}
