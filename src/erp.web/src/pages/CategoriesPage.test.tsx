import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { CategoriesPage } from "./CategoriesPage";

const serviceMocks = vi.hoisted(() => ({ list: vi.fn(), create: vi.fn(), update: vi.fn(), deactivate: vi.fn() }));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: { accessToken: "token", refreshToken: "refresh", userId: "user", email: "admin@example.com", permissions: ["reference.manage"] },
    hasPermission: () => true,
    logout: vi.fn()
  })
}));
vi.mock("../masterData/categoryManagementService", () => ({ categoryManagementService: serviceMocks }));

const category = { id: "category-1", code: "FOOD", name: "Alimentação", isActive: true };

describe("CategoriesPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.list.mockResolvedValue([category]);
    serviceMocks.create.mockResolvedValue({ ...category, id: "category-2", code: "DRINKS", name: "Bebidas" });
    serviceMocks.update.mockResolvedValue({ ...category, name: "Alimentação Geral" });
    serviceMocks.deactivate.mockResolvedValue(undefined);
  });

  it("renders categories", async () => {
    render(<CategoriesPage />);
    expect(await screen.findByText("Alimentação")).toBeInTheDocument();
  });

  it("creates a category", async () => {
    const user = userEvent.setup();
    render(<CategoriesPage />);
    await screen.findByText("Alimentação");
    await user.type(screen.getByLabelText("Código"), "DRINKS");
    await user.type(screen.getByLabelText("Nome"), "Bebidas");
    await user.click(screen.getByRole("button", { name: "Guardar" }));
    await waitFor(() => expect(serviceMocks.create).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "token" }),
      { code: "DRINKS", name: "Bebidas" }
    ));
  });

  it("edits a category while keeping its code disabled", async () => {
    const user = userEvent.setup();
    render(<CategoriesPage />);
    await screen.findByText("Alimentação");
    await user.click(screen.getByRole("button", { name: /FOOD/i }));
    await user.click(screen.getByRole("button", { name: "Editar" }));
    expect(screen.getByLabelText("Código")).toBeDisabled();
    await user.clear(screen.getByLabelText("Nome"));
    await user.type(screen.getByLabelText("Nome"), "Alimentação Geral");
    await user.click(screen.getByRole("button", { name: "Guardar" }));
    await waitFor(() => expect(serviceMocks.update).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "token" }),
      "category-1",
      expect.objectContaining({ name: "Alimentação Geral" })
    ));
  });

  it("shows an error when loading fails", async () => {
    serviceMocks.list.mockRejectedValue(new Error("network"));
    render(<CategoriesPage />);
    expect(await screen.findByText("Não foi possível carregar as categorias.")).toBeInTheDocument();
  });
});
