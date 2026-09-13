import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { CompanyPage } from "./CompanyPage";

const serviceMocks = vi.hoisted(() => ({
  getCompany: vi.fn(),
  updateCompany: vi.fn(),
  updateFiscalProfile: vi.fn(),
  addEstablishment: vi.fn()
}));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: {
      companyId: "company-1",
      accessToken: "access-token",
      refreshToken: "refresh-token",
      userId: "user-1",
      email: "admin@example.com",
      permissions: ["company.manage"]
    }
  })
}));

vi.mock("../tenancy/companyManagementService", () => ({ companyManagementService: serviceMocks }));

const sampleCompany = {
  id: "company-1",
  name: "Empresa Principal",
  isActive: true,
  createdAtUtc: "2026-01-01T00:00:00Z",
  updatedAtUtc: null,
  fiscalProfile: null,
  establishments: [{ id: "est-1", code: "MAIN", name: "Sede", establishmentNumber: "001" }]
};

describe("CompanyPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.getCompany.mockResolvedValue(sampleCompany);
    serviceMocks.updateCompany.mockResolvedValue({ ...sampleCompany, name: "Empresa Atualizada" });
    serviceMocks.updateFiscalProfile.mockResolvedValue(sampleCompany);
    serviceMocks.addEstablishment.mockResolvedValue({
      ...sampleCompany,
      establishments: [...sampleCompany.establishments, { id: "est-2", code: "LOB", name: "Lobito", establishmentNumber: "002" }]
    });
  });

  it("renders the company and its establishments", async () => {
    render(<CompanyPage />);
    expect(await screen.findByDisplayValue("Empresa Principal")).toBeInTheDocument();
    expect(screen.getByText("Sede")).toBeInTheDocument();
  });

  it("updates the company name", async () => {
    const user = userEvent.setup();
    render(<CompanyPage />);
    await screen.findByDisplayValue("Empresa Principal");
    const name = screen.getAllByLabelText("Nome", { selector: "input" })[0];
    await user.clear(name);
    await user.type(name, "Empresa Atualizada");
    await user.click(screen.getByRole("button", { name: "Guardar empresa" }));
    await waitFor(() => expect(serviceMocks.updateCompany).toHaveBeenCalledWith(
      expect.objectContaining({ accessToken: "access-token" }),
      "company-1",
      "Empresa Atualizada"
    ));
  });

  it("adds an establishment", async () => {
    const user = userEvent.setup();
    render(<CompanyPage />);
    await screen.findByText("Sede");
    await user.type(screen.getByLabelText("Código"), "LOB");
    await user.type(screen.getAllByLabelText("Nome", { selector: "input" })[1], "Lobito");
    await user.type(screen.getByLabelText("Número AGT"), "002");
    await user.click(screen.getByRole("button", { name: "Adicionar" }));
    await waitFor(() => expect(serviceMocks.addEstablishment).toHaveBeenCalled());
  });

  it("shows an error when loading fails", async () => {
    serviceMocks.getCompany.mockRejectedValue(new Error("network"));
    render(<CompanyPage />);
    expect(await screen.findByText("Não foi possível carregar os dados da empresa.")).toBeInTheDocument();
  });
});
