import { FormEvent, useEffect, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  Company,
  CompanyFiscalProfile,
  companyManagementService
} from "../tenancy/companyManagementService";
import { ErrorBanner } from "../shared/masterData/ErrorBanner";
import { PageHeader } from "../shared/masterData/PageHeader";

const emptyProfile: CompanyFiscalProfile = { nif: "", vatRegime: "General", fiscalAddress: "" };
const emptyEstablishment = { code: "", name: "", establishmentNumber: "" };

export function CompanyPage() {
  const { session } = useAuth();
  const [company, setCompany] = useState<Company | null>(null);
  const [name, setName] = useState("");
  const [profile, setProfile] = useState<CompanyFiscalProfile>(emptyProfile);
  const [establishment, setEstablishment] = useState(emptyEstablishment);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!session?.companyId) {
      return;
    }

    companyManagementService.getCompany(session, session.companyId)
      .then((loaded) => {
        setCompany(loaded);
        setName(loaded.name);
        setProfile(loaded.fiscalProfile ?? emptyProfile);
      })
      .catch(() => setError("Não foi possível carregar os dados da empresa."));
  }, [session?.companyId, session?.accessToken]);

  async function saveCompany(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session || !company) return;

    try {
      const updated = await companyManagementService.updateCompany(session, company.id, name);
      setCompany(updated);
      setError(null);
    } catch {
      setError("Não foi possível guardar os dados da empresa.");
    }
  }

  async function saveFiscalProfile(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session || !company) return;

    try {
      const updated = await companyManagementService.updateFiscalProfile(session, company.id, profile);
      setCompany(updated);
      setError(null);
    } catch {
      setError("Não foi possível guardar o perfil fiscal.");
    }
  }

  async function addEstablishment(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!session || !company) return;

    try {
      const updated = await companyManagementService.addEstablishment(session, company.id, establishment);
      setCompany(updated);
      setEstablishment(emptyEstablishment);
      setError(null);
    } catch {
      setError("Não foi possível adicionar o estabelecimento.");
    }
  }

  return (
    <section className="space-y-8">
      <PageHeader title="Empresa" description="Perfil da empresa e estabelecimentos." />
      <ErrorBanner message={error} />

      {company ? (
        <>
          <form onSubmit={saveCompany} className="max-w-2xl space-y-4 border-b border-slate-200 pb-8">
            <h2 className="text-lg font-semibold">Dados gerais</h2>
            <label className="block text-sm font-medium text-slate-700">
              Nome
              <input
                value={name}
                onChange={(event) => setName(event.target.value)}
                required
                className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
              />
            </label>
            <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-medium text-white" type="submit">
              Guardar empresa
            </button>
          </form>

          <form onSubmit={saveFiscalProfile} className="max-w-2xl space-y-4 border-b border-slate-200 pb-8">
            <h2 className="text-lg font-semibold">Perfil fiscal</h2>
            <div className="grid gap-4 sm:grid-cols-2">
              <label className="block text-sm font-medium text-slate-700">
                NIF
                <input
                  value={profile.nif}
                  onChange={(event) => setProfile((current) => ({ ...current, nif: event.target.value }))}
                  required
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                />
              </label>
              <label className="block text-sm font-medium text-slate-700">
                Regime de IVA
                <select
                  value={profile.vatRegime}
                  onChange={(event) => setProfile((current) => ({ ...current, vatRegime: event.target.value }))}
                  className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
                >
                  <option value="General">Geral</option>
                  <option value="Simplified">Simplificado</option>
                  <option value="CashVat">IVA de caixa</option>
                  <option value="Exclusion">Exclusão</option>
                </select>
              </label>
            </div>
            <label className="block text-sm font-medium text-slate-700">
              Morada fiscal
              <textarea
                value={profile.fiscalAddress}
                onChange={(event) => setProfile((current) => ({ ...current, fiscalAddress: event.target.value }))}
                required
                rows={3}
                className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
              />
            </label>
            <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-medium text-white" type="submit">
              Guardar perfil fiscal
            </button>
          </form>

          <section className="space-y-4">
            <h2 className="text-lg font-semibold">Estabelecimentos</h2>
            <div className="overflow-x-auto border-y border-slate-200">
              <table className="w-full text-left text-sm">
                <thead className="bg-slate-100 text-slate-600">
                  <tr><th className="px-3 py-2">Código</th><th className="px-3 py-2">Nome</th><th className="px-3 py-2">Número AGT</th></tr>
                </thead>
                <tbody>
                  {company.establishments.map((item) => (
                    <tr key={item.id} className="border-t border-slate-200">
                      <td className="px-3 py-2 font-medium">{item.code}</td>
                      <td className="px-3 py-2">{item.name}</td>
                      <td className="px-3 py-2">{item.establishmentNumber}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <form onSubmit={addEstablishment} className="grid gap-4 md:grid-cols-[1fr_2fr_1fr_auto] md:items-end">
              <label className="block text-sm font-medium text-slate-700">Código
                <input value={establishment.code} onChange={(event) => setEstablishment((current) => ({ ...current, code: event.target.value }))} required className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm" />
              </label>
              <label className="block text-sm font-medium text-slate-700">Nome
                <input value={establishment.name} onChange={(event) => setEstablishment((current) => ({ ...current, name: event.target.value }))} required className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm" />
              </label>
              <label className="block text-sm font-medium text-slate-700">Número AGT
                <input value={establishment.establishmentNumber} onChange={(event) => setEstablishment((current) => ({ ...current, establishmentNumber: event.target.value }))} required className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm" />
              </label>
              <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-medium text-white" type="submit">Adicionar</button>
            </form>
          </section>
        </>
      ) : null}
    </section>
  );
}
