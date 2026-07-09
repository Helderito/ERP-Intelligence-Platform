import { Navigate } from "react-router-dom";
import { useAuth } from "./useAuth";

export function ProtectedRoute({
  children,
  requiredPermission
}: {
  children: React.ReactNode;
  requiredPermission?: string;
}) {
  const { hasPermission, isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredPermission && !hasPermission(requiredPermission)) {
    return <Navigate to="/" replace />;
  }

  return children;
}
