export function StatusLabel({ isActive }: { isActive: boolean }) {
  return (
    <span className={isActive ? "text-emerald-700" : "text-slate-500"}>
      {isActive ? "Ativo" : "Inativo"}
    </span>
  );
}
