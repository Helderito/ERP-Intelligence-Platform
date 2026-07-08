import { useEffect, useState } from "react";
import { authorizationService, Permission } from "../auth/authorizationService";
import { useAuth } from "../auth/useAuth";

export function PermissionsPage() {
  const { session } = useAuth();
  const [permissions, setPermissions] = useState<Permission[]>([]);

  useEffect(() => {
    if (!session) {
      return;
    }

    authorizationService.getPermissions(session).then(setPermissions);
  }, [session]);

  return (
    <section className="space-y-6">
      <div className="border-b border-slate-200 pb-5">
        <h2 className="text-2xl font-semibold">Permissions</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">
          Catalogo inicial semeado por migration.
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        {permissions.map((permission) => (
          <article key={permission.id} className="rounded-md border border-slate-200 bg-white p-4 shadow-sm">
            <h3 className="font-semibold">{permission.code}</h3>
            <p className="mt-2 text-sm text-slate-600">{permission.description}</p>
          </article>
        ))}
      </div>
    </section>
  );
}
