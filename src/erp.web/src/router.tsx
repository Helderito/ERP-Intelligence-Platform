import { createBrowserRouter } from "react-router-dom";
import { AppLayout } from "./shared/AppLayout";
import { DashboardPage } from "./pages/DashboardPage";
import { SettingsPage } from "./pages/SettingsPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <AppLayout />,
    children: [
      {
        index: true,
        element: <DashboardPage />
      },
      {
        path: "settings",
        element: <SettingsPage />
      }
    ]
  }
]);
