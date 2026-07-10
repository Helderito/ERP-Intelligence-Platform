import { createBrowserRouter } from "react-router-dom";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { AppLayout } from "./shared/AppLayout";
import { DashboardPage } from "./pages/DashboardPage";
import { LoginPage } from "./pages/LoginPage";
import { PermissionsPage } from "./pages/PermissionsPage";
import { ProductsPage } from "./pages/ProductsPage";
import { RolesPage } from "./pages/RolesPage";
import { SettingsPage } from "./pages/SettingsPage";
import { UserRolesPage } from "./pages/UserRolesPage";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginPage />
  },
  {
    path: "/",
    element: (
      <ProtectedRoute>
        <AppLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <DashboardPage />
      },
      {
        path: "settings",
        element: <SettingsPage />
      },
      {
        path: "products",
        element: (
          <ProtectedRoute requiredPermission="catalog.manage">
            <ProductsPage />
          </ProtectedRoute>
        )
      },
      {
        path: "roles",
        element: (
          <ProtectedRoute requiredPermission="roles.manage">
            <RolesPage />
          </ProtectedRoute>
        )
      },
      {
        path: "permissions",
        element: (
          <ProtectedRoute requiredPermission="roles.manage">
            <PermissionsPage />
          </ProtectedRoute>
        )
      },
      {
        path: "users/roles",
        element: (
          <ProtectedRoute requiredPermission="users.manage">
            <UserRolesPage />
          </ProtectedRoute>
        )
      }
    ]
  }
]);
