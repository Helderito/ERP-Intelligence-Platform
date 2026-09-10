import { createBrowserRouter } from "react-router-dom";
import { ProtectedRoute } from "./auth/ProtectedRoute";
import { AppLayout } from "./shared/AppLayout";
import { DashboardPage } from "./pages/DashboardPage";
import { CustomersPage } from "./pages/CustomersPage";
import { LoginPage } from "./pages/LoginPage";
import { PermissionsPage } from "./pages/PermissionsPage";
import { ProductsPage } from "./pages/ProductsPage";
import { RolesPage } from "./pages/RolesPage";
import { SettingsPage } from "./pages/SettingsPage";
import { SuppliersPage } from "./pages/SuppliersPage";
import { UserRolesPage } from "./pages/UserRolesPage";
import { WarehousesPage } from "./pages/WarehousesPage";
import { CategoriesPage } from "./pages/CategoriesPage";
import { TaxCodesPage } from "./pages/TaxCodesPage";
import { UnitsOfMeasurePage } from "./pages/UnitsOfMeasurePage";

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
        path: "customers",
        element: (
          <ProtectedRoute requiredPermission="customers.manage">
            <CustomersPage />
          </ProtectedRoute>
        )
      },
      {
        path: "suppliers",
        element: (
          <ProtectedRoute requiredPermission="suppliers.manage">
            <SuppliersPage />
          </ProtectedRoute>
        )
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
        path: "warehouses",
        element: (
          <ProtectedRoute requiredPermission="warehouses.manage">
            <WarehousesPage />
          </ProtectedRoute>
        )
      },
      {
        path: "categories",
        element: (
          <ProtectedRoute requiredPermission="reference.manage">
            <CategoriesPage />
          </ProtectedRoute>
        )
      },
      {
        path: "units-of-measure",
        element: (
          <ProtectedRoute requiredPermission="reference.manage">
            <UnitsOfMeasurePage />
          </ProtectedRoute>
        )
      },
      {
        path: "tax-codes",
        element: (
          <ProtectedRoute requiredPermission="reference.manage">
            <TaxCodesPage />
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
