import { AuthenticationSession } from "../auth/authenticationService";
import { apiRequest } from "../shared/apiClient";
import { MasterDataListItem, PagedResult } from "../shared/masterData/types";

export type Product = MasterDataListItem & {
  categoryId: string;
  unitOfMeasureId: string;
  createdAtUtc: string;
  updatedAtUtc: string | null;
  deactivatedAtUtc: string | null;
};

export type Category = {
  id: string;
  code: string;
  name: string;
};

export type UnitOfMeasure = {
  id: string;
  code: string;
  name: string;
};

export type SaveProductRequest = {
  code?: string;
  name: string;
  categoryId: string;
  unitOfMeasureId: string;
};

export const productCatalogService = {
  searchProducts(session: AuthenticationSession, search: string, page = 1, pageSize = 20) {
    const parameters = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString()
    });

    if (search.trim()) {
      parameters.set("search", search.trim());
    }

    return apiRequest<PagedResult<MasterDataListItem>>(
      session,
      `/products?${parameters.toString()}`,
      {},
      "Nao foi possivel concluir o pedido do catalogo."
    );
  },

  getProduct(session: AuthenticationSession, productId: string) {
    return apiRequest<Product>(
      session,
      `/products/${productId}`,
      {},
      "Nao foi possivel concluir o pedido do catalogo."
    );
  },

  createProduct(session: AuthenticationSession, product: SaveProductRequest) {
    return apiRequest<Product>(
      session,
      "/products",
      {
        method: "POST",
        body: JSON.stringify(product)
      },
      "Nao foi possivel concluir o pedido do catalogo."
    );
  },

  updateProduct(session: AuthenticationSession, productId: string, product: SaveProductRequest) {
    return apiRequest<Product>(
      session,
      `/products/${productId}`,
      {
        method: "PUT",
        body: JSON.stringify({
          name: product.name,
          categoryId: product.categoryId,
          unitOfMeasureId: product.unitOfMeasureId
        })
      },
      "Nao foi possivel concluir o pedido do catalogo."
    );
  },

  deactivateProduct(session: AuthenticationSession, productId: string) {
    return apiRequest<void>(
      session,
      `/products/${productId}`,
      {
        method: "DELETE"
      },
      "Nao foi possivel concluir o pedido do catalogo."
    );
  },

  getCategories(session: AuthenticationSession) {
    return apiRequest<Category[]>(session, "/categories", {}, "Nao foi possivel concluir o pedido do catalogo.");
  },

  getUnitsOfMeasure(session: AuthenticationSession) {
    return apiRequest<UnitOfMeasure[]>(
      session,
      "/units-of-measure",
      {},
      "Nao foi possivel concluir o pedido do catalogo."
    );
  }
};
