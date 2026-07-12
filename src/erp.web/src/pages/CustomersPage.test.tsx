import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { CustomersPage } from "./CustomersPage";

const serviceMocks = vi.hoisted(() => ({
  searchCustomers: vi.fn(),
  getCustomer: vi.fn(),
  createCustomer: vi.fn(),
  updateCustomer: vi.fn(),
  deactivateCustomer: vi.fn()
}));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: {
      accessToken: "access-token",
      refreshToken: "refresh-token",
      userId: "user-id",
      email: "admin@example.com",
      permissions: ["customers.manage"]
    },
    hasPermission: () => true,
    logout: vi.fn()
  })
}));

vi.mock("../masterData/customerManagementService", () => ({
  customerManagementService: serviceMocks
}));

const sampleListItem = {
  id: "customer-1",
  code: "CUS-001",
  name: "Cliente Exemplo",
  isActive: true
};

const sampleCustomer = {
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

describe("CustomersPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.searchCustomers.mockResolvedValue({
      page: 1,
      pageSize: 20,
      totalRecords: 1,
      items: [sampleListItem]
    });
    serviceMocks.getCustomer.mockResolvedValue(sampleCustomer);
    serviceMocks.createCustomer.mockResolvedValue({
      ...sampleCustomer,
      id: "customer-2",
      code: "CUS-002",
      name: "Novo Cliente"
    });
    serviceMocks.updateCustomer.mockResolvedValue({
      ...sampleCustomer,
      name: "Cliente Atualizado"
    });
    serviceMocks.deactivateCustomer.mockResolvedValue(undefined);
  });

  it("renders customers returned by the service", async () => {
    render(<CustomersPage />);

    expect(await screen.findByText("Cliente Exemplo")).toBeInTheDocument();
    expect(screen.getByText("1 clientes encontrados")).toBeInTheDocument();
  });

  it("loads customer detail when selecting a list item", async () => {
    const user = userEvent.setup();
    render(<CustomersPage />);

    await screen.findByText("Cliente Exemplo");
    await user.click(screen.getByRole("button", { name: /CUS-001/i }));

    await waitFor(() => {
      expect(serviceMocks.getCustomer).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "customer-1"
      );
    });

    expect(await screen.findByText("Ana Silva")).toBeInTheDocument();
    expect(screen.getByText(/Rua Principal/)).toBeInTheDocument();
  });

  it("creates a customer with contacts and addresses", async () => {
    const user = userEvent.setup();
    render(<CustomersPage />);

    await screen.findByText("Cliente Exemplo");

    await user.type(screen.getByLabelText("Codigo"), "CUS-002");
    await user.type(screen.getByLabelText("Nome"), "Novo Cliente");
    await user.type(screen.getByLabelText("Nome do contacto 1"), "Carlos Lopes");
    await user.type(screen.getByLabelText("Email do contacto 1"), "carlos@example.com");
    await user.type(screen.getByLabelText("Telefone do contacto 1"), "+351 900 000 000");
    await user.type(screen.getByLabelText("Linha 1 da morada 1"), "Avenida Nova");
    await user.type(screen.getByLabelText("Cidade da morada 1"), "Lisboa");
    await user.type(screen.getByLabelText("Codigo postal da morada 1"), "1200-001");

    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => {
      expect(serviceMocks.createCustomer).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        {
          code: "CUS-002",
          name: "Novo Cliente",
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

  it("edits a customer after loading detail", async () => {
    const user = userEvent.setup();
    render(<CustomersPage />);

    await screen.findByText("Cliente Exemplo");
    await user.click(screen.getByRole("button", { name: /CUS-001/i }));
    await screen.findByText("Ana Silva");

    await user.click(screen.getByRole("button", { name: "Editar" }));

    const nameInput = screen.getByLabelText("Nome");
    await user.clear(nameInput);
    await user.type(nameInput, "Cliente Atualizado");
    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => {
      expect(serviceMocks.updateCustomer).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "customer-1",
        expect.objectContaining({
          name: "Cliente Atualizado"
        })
      );
    });
  });

  it("deactivates a customer after loading detail", async () => {
    const user = userEvent.setup();
    render(<CustomersPage />);

    await screen.findByText("Cliente Exemplo");
    await user.click(screen.getByRole("button", { name: /CUS-001/i }));
    await screen.findByText("Ana Silva");

    await user.click(screen.getByRole("button", { name: "Desativar" }));

    await waitFor(() => {
      expect(serviceMocks.deactivateCustomer).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "customer-1"
      );
    });
  });

  it("shows an error when loading customers fails", async () => {
    serviceMocks.searchCustomers.mockRejectedValue(new Error("network"));

    render(<CustomersPage />);

    expect(await screen.findByText("Nao foi possivel carregar os clientes.")).toBeInTheDocument();
  });
});
