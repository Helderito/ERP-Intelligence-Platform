import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { UnitsOfMeasurePage } from "./UnitsOfMeasurePage";

const serviceMocks = vi.hoisted(() => ({ list: vi.fn(), create: vi.fn(), update: vi.fn(), deactivate: vi.fn() }));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: { accessToken: "token", refreshToken: "refresh", userId: "user", email: "admin@example.com", permissions: ["reference.manage"] },
    hasPermission: () => true,
    logout: vi.fn()
  })
}));
vi.mock("../masterData/unitOfMeasureManagementService", () => ({ unitOfMeasureManagementService: serviceMocks }));

const unit = { id: "unit-1", code: "UNIT", name: "Unidade", isActive: true };

describe("UnitsOfMeasurePage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.list.mockResolvedValue([unit]);
    serviceMocks.create.mockResolvedValue({ ...unit, id: "unit-2", code: "BOX", name: "Caixa" });
    serviceMocks.update.mockResolvedValue({ ...unit, name: "Unidade Comercial" });
    serviceMocks.deactivate.mockResolvedValue(undefined);
  });

  it("renders units of measure", async () => {
    render(<UnitsOfMeasurePage />);
    expect(await screen.findByText("Unidade")).toBeInTheDocument();
  });

  it("creates a unit of measure", async () => {
    const user = userEvent.setup();
    render(<UnitsOfMeasurePage />);
    await screen.findByText("Unidade");
    await user.type(screen.getByLabelText("Código"), "BOX");
    await user.type(screen.getByLabelText("Nome"), "Caixa");
    await user.click(screen.getByRole("button", { name: "Guardar" }));
    await waitFor(() => expect(serviceMocks.create).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "token" }),
      { code: "BOX", name: "Caixa" }
    ));
  });

  it("edits a unit of measure", async () => {
    const user = userEvent.setup();
    render(<UnitsOfMeasurePage />);
    await screen.findByText("Unidade");
    await user.click(screen.getByRole("button", { name: /UNIT/i }));
    await user.click(screen.getByRole("button", { name: "Editar" }));
    await user.clear(screen.getByLabelText("Nome"));
    await user.type(screen.getByLabelText("Nome"), "Unidade Comercial");
    await user.click(screen.getByRole("button", { name: "Guardar" }));
    await waitFor(() => expect(serviceMocks.update).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "token" }),
      "unit-1",
      expect.objectContaining({ name: "Unidade Comercial" })
    ));
  });

  it("shows an error when loading fails", async () => {
    serviceMocks.list.mockRejectedValue(new Error("network"));
    render(<UnitsOfMeasurePage />);
    expect(await screen.findByText("Não foi possível carregar as unidades de medida.")).toBeInTheDocument();
  });
});
