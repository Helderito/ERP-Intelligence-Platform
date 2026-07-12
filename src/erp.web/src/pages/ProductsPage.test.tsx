import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { ProductsPage } from "./ProductsPage";

const serviceMocks = vi.hoisted(() => ({
  searchProducts: vi.fn(),
  getProduct: vi.fn(),
  createProduct: vi.fn(),
  updateProduct: vi.fn(),
  deactivateProduct: vi.fn(),
  getCategories: vi.fn(),
  getUnitsOfMeasure: vi.fn()
}));

vi.mock("../auth/useAuth", () => ({
  useAuth: () => ({
    session: {
      accessToken: "access-token",
      refreshToken: "refresh-token",
      userId: "user-id",
      email: "admin@example.com",
      permissions: ["products.manage"]
    },
    hasPermission: () => true,
    logout: vi.fn()
  })
}));

vi.mock("../masterData/productCatalogService", () => ({
  productCatalogService: serviceMocks
}));

const sampleCategory = {
  id: "category-1",
  code: "CAT-001",
  name: "Categoria A"
};

const sampleUnit = {
  id: "unit-1",
  code: "UN",
  name: "Unidade"
};

const sampleListItem = {
  id: "product-1",
  code: "SKU-001",
  name: "Produto Exemplo",
  isActive: true
};

const sampleProduct = {
  ...sampleListItem,
  categoryId: "category-1",
  unitOfMeasureId: "unit-1",
  createdAtUtc: "2026-01-01T00:00:00Z",
  updatedAtUtc: null,
  deactivatedAtUtc: null
};

describe("ProductsPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    serviceMocks.searchProducts.mockResolvedValue({
      page: 1,
      pageSize: 20,
      totalRecords: 1,
      items: [sampleListItem]
    });
    serviceMocks.getProduct.mockResolvedValue(sampleProduct);
    serviceMocks.getCategories.mockResolvedValue([sampleCategory]);
    serviceMocks.getUnitsOfMeasure.mockResolvedValue([sampleUnit]);
    serviceMocks.createProduct.mockResolvedValue({
      ...sampleProduct,
      id: "product-2",
      code: "SKU-002",
      name: "Novo Produto"
    });
    serviceMocks.updateProduct.mockResolvedValue({
      ...sampleProduct,
      name: "Produto Atualizado"
    });
    serviceMocks.deactivateProduct.mockResolvedValue(undefined);
  });

  it("renders products returned by the service", async () => {
    render(<ProductsPage />);

    expect(await screen.findByText("Produto Exemplo")).toBeInTheDocument();
    expect(screen.getByText("1 produtos encontrados")).toBeInTheDocument();
  });

  it("loads product detail when selecting a list item", async () => {
    const user = userEvent.setup();
    render(<ProductsPage />);

    await screen.findByText("Produto Exemplo");
    await user.click(screen.getByRole("button", { name: /SKU-001/i }));

    await waitFor(() => {
      expect(serviceMocks.getProduct).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "product-1"
      );
    });

    expect(await screen.findByRole("button", { name: "Editar" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Desativar" })).toBeInTheDocument();
  });

  it("creates a product", async () => {
    const user = userEvent.setup();
    render(<ProductsPage />);

    await screen.findByText("Produto Exemplo");

    await user.type(screen.getByLabelText("Codigo"), "SKU-002");
    await user.type(screen.getByLabelText("Nome"), "Novo Produto");
    await user.selectOptions(screen.getByLabelText("Categoria"), "category-1");
    await user.selectOptions(screen.getByLabelText("Unidade de medida"), "unit-1");
    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => {
      expect(serviceMocks.createProduct).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        {
          code: "SKU-002",
          name: "Novo Produto",
          categoryId: "category-1",
          unitOfMeasureId: "unit-1"
        }
      );
    });
  });

  it("edits a product after loading detail", async () => {
    const user = userEvent.setup();
    render(<ProductsPage />);

    await screen.findByText("Produto Exemplo");
    await user.click(screen.getByRole("button", { name: /SKU-001/i }));
    await screen.findByRole("button", { name: "Editar" });

    await user.click(screen.getByRole("button", { name: "Editar" }));

    const nameInput = screen.getByLabelText("Nome");
    await user.clear(nameInput);
    await user.type(nameInput, "Produto Atualizado");
    await user.click(screen.getByRole("button", { name: "Guardar" }));

    await waitFor(() => {
      expect(serviceMocks.updateProduct).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "product-1",
        expect.objectContaining({
          name: "Produto Atualizado",
          categoryId: "category-1",
          unitOfMeasureId: "unit-1"
        })
      );
    });
  });

  it("deactivates a product after loading detail", async () => {
    const user = userEvent.setup();
    render(<ProductsPage />);

    await screen.findByText("Produto Exemplo");
    await user.click(screen.getByRole("button", { name: /SKU-001/i }));
    await screen.findByRole("button", { name: "Desativar" });

    await user.click(screen.getByRole("button", { name: "Desativar" }));

    await waitFor(() => {
      expect(serviceMocks.deactivateProduct).toHaveBeenCalledWith(
        expect.objectContaining({ accessToken: "access-token" }),
        "product-1"
      );
    });
  });

  it("shows an error when loading products fails", async () => {
    serviceMocks.searchProducts.mockRejectedValue(new Error("network"));

    render(<ProductsPage />);

    expect(await screen.findByText("Nao foi possivel carregar o catalogo de produtos.")).toBeInTheDocument();
  });
});
