import { FormEvent } from "react";

type SearchBarProps = {
  value: string;
  onChange: (value: string) => void;
  onSubmit: (event: FormEvent<HTMLFormElement>) => void;
  placeholder: string;
};

export function SearchBar({ value, onChange, onSubmit, placeholder }: SearchBarProps) {
  return (
    <form className="flex gap-3" onSubmit={onSubmit}>
      <input
        value={value}
        onChange={(event) => onChange(event.target.value)}
        placeholder={placeholder}
        className="min-w-0 flex-1 rounded-md border border-slate-300 px-3 py-2 text-sm"
      />
      <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white" type="submit">
        Pesquisar
      </button>
    </form>
  );
}
