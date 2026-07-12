import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { SuppliersPage } from "./SuppliersPage";

const serviceMocks = vi.hoisted(() => ({
  searchSuppliers: vi.fn(),
  getSupplier: vi.fn(),
  createSupplier: vi.fn(),
  updateSupplier: vi.fn(),
  deactivateSupplier: vi.fn()
}));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: {
      accessToken: "access-token",
      refreshToken: "refresh-token",
      userId: "user-id",
      email: "admin@example.com",
      permissions: ["suppliers.manage"]
    },
    hasPermission: () => true,
    logout: vi.fn()
  })
}));

vi.mock("../masterData/supplierManagementService", () => ({
  supplierManagementService: serviceMocks
}));

const sampleListItem = {
  id: "supplier-1",
  code: "SUP-001",
  name: "Fornecedor Exemplo",
  isActive: true
};

const sampleSupplier = {
  ...sampleListItem,
  createdAtUtc: "2026-01-01T00:00:00Z",
  updatedAtUtc: null,
  deactivatedAtUtc: null,
  contacts: [
    {
      id: "contact-1",
      name: "Ana Silva",
      email: "ana@example.com",
      phone: "+244 900 000 000"
    }
  ],
  addresses: [
    {
      id: "address-1",
      line1: "Rua Principal",
      line2: null,
      city: "Luanda",
      postalCode: "1000",
      country: "Angola"
    }
  ]
};

describe("SuppliersPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.searchSuppliers.mockResolvedValue({
      page: 1,
      pageSize: 20,
      totalRecords: 1,
      items: [sampleListItem]
    });
    serviceMocks.getSupplier.mockResolvedValue(sampleSupplier);
    serviceMocks.createSupplier.mockResolvedValue({
      ...sampleSupplier,
      id: "supplier-2",
      code: "SUP-002",
      name: "Novo Fornecedor"
    });
    serviceMocks.updateSupplier.mockResolvedValue({
      ...sampleSupplier,
      name: "Fornecedor Atualizado"
    });
    serviceMocks.deactivateSupplier.mockResolvedValue(undefined);
  });

  it("renders suppliers returned by the service", async () => {
    render(<SuppliersPage />);

    expect(await screen.findByText("Fornecedor Exemplo")).toBeInTheDocument();
    expect(screen.getByText("1 fornecedores encontrados")).toBeInTheDocument();
  });

  it("loads supplier detail when selecting a list item", async () => {
    const user = userEvent.setup();
    render(<SuppliersPage />);

    await screen.findByText("Fornecedor Exemplo");
    await user.click(screen.getByRole("button", { name: /SUP-001/i }));

    await waitFor(() => {
      expect(serviceMocks.getSupplier).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "supplier-1"
      );
    });

    expect(await screen.findByText("Ana Silva")).toBeInTheDocument();
    expect(screen.getByText(/Rua Principal/)).toBeInTheDocument();
  });

  it("creates a supplier with contacts and addresses", async () => {
    const user = userEvent.setup();
    render(<SuppliersPage />);

    await screen.findByText("Fornecedor Exemplo");

    await user.type(screen.getByLabelText("Codigo"), "SUP-002");
    await user.type(screen.getByLabelText("Nome"), "Novo Fornecedor");
    await user.type(screen.getByLabelText("Nome do contacto 1"), "Carlos Lopes");
    await user.type(screen.getByLabelText("Email do contacto 1"), "carlos@example.com");
    await user.type(screen.getByLabelText("Telefone do contacto 1"), "+351 900 000 000");
    await user.type(screen.getByLabelText("Linha 1 da morada 1"), "Avenida Nova");
    await user.type(screen.getByLabelText("Cidade da morada 1"), "Lisboa");
    await user.type(screen.getByLabelText("Codigo postal da morada 1"), "1200-001");

    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => {
      expect(serviceMocks.createSupplier).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        {
          code: "SUP-002",
          name: "Novo Fornecedor",
          contacts: [
            {
              name: "Carlos Lopes",
              email: "carlos@example.com",
              phone: "+351 900 000 000"
            }
          ],
          addresses: [
            {
              line1: "Avenida Nova",
              line2: "",
              city: "Lisboa",
              postalCode: "1200-001",
              country: "Angola"
            }
          ]
        }
      );
    });
  });

  it("edits a supplier after loading detail", async () => {
    const user = userEvent.setup();
    render(<SuppliersPage />);

    await screen.findByText("Fornecedor Exemplo");
    await user.click(screen.getByRole("button", { name: /SUP-001/i }));
    await screen.findByText("Ana Silva");

    await user.click(screen.getByRole("button", { name: "Editar" }));

    const nameInput = screen.getByLabelText("Nome");
    await user.clear(nameInput);
    await user.type(nameInput, "Fornecedor Atualizado");
    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => {
      expect(serviceMocks.updateSupplier).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "supplier-1",
        expect.objectContaining({
          name: "Fornecedor Atualizado"
        })
      );
    });
  });

  it("shows an error when loading suppliers fails", async () => {
    serviceMocks.searchSuppliers.mockRejectedValue(new Error("network"));

    render(<SuppliersPage />);

    expect(await screen.findByText("Nao foi possivel carregar os fornecedores.")).toBeInTheDocument();
  });
});
