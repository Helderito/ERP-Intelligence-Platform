export function ErrorBanner({ message }: { message: string | null }) {
  if (!message) {
    return null;
  }

  return <p className="text-sm font-medium text-red-700">{message}</p>;
}
