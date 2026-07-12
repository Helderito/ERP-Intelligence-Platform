export function PageHeader({ title, description }: { title: string; description: string }) {
  return (
    <div className="border-b border-slate-200 pb-5">
      <h2 className="text-2xl font-semibold">{title}</h2>
      <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">{description}</p>
    </div>
  );
}
