import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { WarehousesPage } from "./WarehousesPage";

const serviceMocks = vi.hoisted(() => ({
  searchWarehouses: vi.fn(),
  getWarehouse: vi.fn(),
  createWarehouse: vi.fn(),
  updateWarehouse: vi.fn(),
  deactivateWarehouse: vi.fn(),
  getWarehouseTypes: vi.fn()
}));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: {
      accessToken: "access-token",
      refreshToken: "refresh-token",
      userId: "user-id",
      email: "admin@example.com",
      permissions: ["warehouses.manage"]
    },
    hasPermission: () => true,
    logout: vi.fn()
  })
}));

vi.mock("../masterData/warehouseManagementService", () => ({
  warehouseManagementService: serviceMocks
}));

const mainType = { id: "type-main", code: "MAIN", name: "Armazém principal" };
const transitType = { id: "type-transit", code: "TRANSIT", name: "Trânsito" };
const sampleListItem = { id: "warehouse-1", code: "WH-001", name: "Armazém Luanda", isActive: true };
const sampleWarehouse = {
  ...sampleListItem,
  warehouseTypeId: mainType.id,
  warehouseTypeName: mainType.name,
  createdAtUtc: "2026-01-01T00:00:00Z",
  updatedAtUtc: null,
  deactivatedAtUtc: null
};

describe("WarehousesPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.searchWarehouses.mockResolvedValue({
      page: 1,
      pageSize: 20,
      totalRecords: 1,
      items: [sampleListItem]
    });
    serviceMocks.getWarehouseTypes.mockResolvedValue([mainType, transitType]);
    serviceMocks.getWarehouse.mockResolvedValue(sampleWarehouse);
    serviceMocks.createWarehouse.mockResolvedValue({
      ...sampleWarehouse,
      id: "warehouse-2",
      code: "WH-002",
      name: "Novo Armazém"
    });
    serviceMocks.updateWarehouse.mockResolvedValue({
      ...sampleWarehouse,
      name: "Armazém Atualizado",
      warehouseTypeId: transitType.id,
      warehouseTypeName: transitType.name
    });
    serviceMocks.deactivateWarehouse.mockResolvedValue(undefined);
  });

  it("renders warehouses returned by the service", async () => {
    render(<WarehousesPage />);

    expect(await screen.findByText("Armazém Luanda")).toBeInTheDocument();
    expect(screen.getByText("1 armazéns encontrados")).toBeInTheDocument();
  });

  it("loads warehouse detail when selecting a list item", async () => {
    const user = userEvent.setup();
    render(<WarehousesPage />);

    await screen.findByText("Armazém Luanda");
    await user.click(screen.getByRole("button", { name: /WH-001/i }));

    await waitFor(() => expect(serviceMocks.getWarehouse).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "access-token" }),
      "warehouse-1"
    ));
    expect(screen.getAllByText("Armazém principal")).toHaveLength(2);
  });

  it("creates a warehouse with a selected type", async () => {
    const user = userEvent.setup();
    render(<WarehousesPage />);

    await screen.findByText("Armazém Luanda");
    await user.type(screen.getByLabelText("Código"), "WH-002");
    await user.type(screen.getByLabelText("Nome"), "Novo Armazém");
    await user.selectOptions(screen.getByLabelText("Tipo de armazém"), transitType.id);
    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => expect(serviceMocks.createWarehouse).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "access-token" }),
      { code: "WH-002", name: "Novo Armazém", warehouseTypeId: transitType.id }
    ));
  });

  it("edits a warehouse without changing its code", async () => {
    const user = userEvent.setup();
    render(<WarehousesPage />);

    await screen.findByText("Armazém Luanda");
    await user.click(screen.getByRole("button", { name: /WH-001/i }));
    await user.click(await screen.findByRole("button", { name: "Editar" }));
    expect(screen.getByLabelText("Código")).toBeDisabled();

    await user.clear(screen.getByLabelText("Nome"));
    await user.type(screen.getByLabelText("Nome"), "Armazém Atualizado");
    await user.selectOptions(screen.getByLabelText("Tipo de armazém"), transitType.id);
    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => expect(serviceMocks.updateWarehouse).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "access-token" }),
      "warehouse-1",
      expect.objectContaining({ name: "Armazém Atualizado", warehouseTypeId: transitType.id })
    ));
  });

  it("deactivates a warehouse", async () => {
    const user = userEvent.setup();
    render(<WarehousesPage />);

    await screen.findByText("Armazém Luanda");
    await user.click(screen.getByRole("button", { name: /WH-001/i }));
    await user.click(await screen.findByRole("button", { name: "Desativar" }));

    await waitFor(() => expect(serviceMocks.deactivateWarehouse).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "access-token" }),
      "warehouse-1"
    ));
  });

  it("shows an error when loading warehouses fails", async () => {
    serviceMocks.searchWarehouses.mockRejectedValue(new Error("network"));
    render(<WarehousesPage />);

    expect(await screen.findByText("Não foi possível carregar os armazéns.")).toBeInTheDocument();
  });
});
