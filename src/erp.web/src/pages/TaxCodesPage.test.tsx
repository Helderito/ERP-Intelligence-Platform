import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { TaxCodesPage } from "./TaxCodesPage";

const serviceMocks = vi.hoisted(() => ({ list: vi.fn(), create: vi.fn(), update: vi.fn(), deactivate: vi.fn() }));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: { accessToken: "token", refreshToken: "refresh", userId: "user", email: "admin@example.com", permissions: ["reference.manage"] },
    hasPermission: () => true,
    logout: vi.fn()
  })
}));
vi.mock("../masterData/taxCodeManagementService", () => ({ taxCodeManagementService: serviceMocks }));

const taxCode = {
  id: "tax-1", code: "VAT14", name: "IVA 14%", rate: 14, isActive: true,
  createdAtUtc: "2026-01-01T00:00:00Z", updatedAtUtc: null, deactivatedAtUtc: null
};

describe("TaxCodesPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.list.mockResolvedValue([taxCode]);
    serviceMocks.create.mockResolvedValue({ ...taxCode, id: "tax-2", code: "ZERO", name: "Isento", rate: 0 });
    serviceMocks.update.mockResolvedValue({ ...taxCode, name: "IVA Geral", rate: 15 });
    serviceMocks.deactivate.mockResolvedValue(undefined);
  });

  it("renders tax codes", async () => {
    render(<TaxCodesPage />);
    expect(await screen.findByText("IVA 14%")).toBeInTheDocument();
  });

  it("creates a tax code", async () => {
    const user = userEvent.setup();
    render(<TaxCodesPage />);
    await screen.findByText("IVA 14%");
    await user.type(screen.getByLabelText("Código"), "ZERO");
    await user.type(screen.getByLabelText("Nome"), "Isento");
    await user.clear(screen.getByLabelText("Taxa (%)"));
    await user.type(screen.getByLabelText("Taxa (%)"), "0");
    await user.click(screen.getByRole("button", { name: "Guardar" }));
    await waitFor(() => expect(serviceMocks.create).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "token" }),
      { code: "ZERO", name: "Isento", rate: 0 }
    ));
  });

  it("edits a tax code while keeping its code disabled", async () => {
    const user = userEvent.setup();
    render(<TaxCodesPage />);
    await screen.findByText("IVA 14%");
    await user.click(screen.getByRole("button", { name: /VAT14/i }));
    await user.click(screen.getByRole("button", { name: "Editar" }));
    expect(screen.getByLabelText("Código")).toBeDisabled();
    await user.clear(screen.getByLabelText("Nome"));
    await user.type(screen.getByLabelText("Nome"), "IVA Geral");
    await user.clear(screen.getByLabelText("Taxa (%)"));
    await user.type(screen.getByLabelText("Taxa (%)"), "15");
    await user.click(screen.getByRole("button", { name: "Guardar" }));
    await waitFor(() => expect(serviceMocks.update).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "token" }),
      "tax-1",
      expect.objectContaining({ name: "IVA Geral", rate: 15 })
    ));
  });

  it("shows an error when loading fails", async () => {
    serviceMocks.list.mockRejectedValue(new Error("network"));
    render(<TaxCodesPage />);
    expect(await screen.findByText("Não foi possível carregar os códigos de imposto.")).toBeInTheDocument();
  });
});
