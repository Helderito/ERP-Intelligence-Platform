import { AuthenticationSession } from "../auth/authenticationService";

export type Product = {
  id: string;
  code: string;
  name: string;
  categoryId: string;
  unitOfMeasureId: string;
  isActive: boolean;
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

export type PagedResult<TItem> = {
  page: number;
  pageSize: number;
  totalRecords: number;
  items: TItem[];
};

export type SaveProductRequest = {
  code?: string;
  name: string;
  categoryId: string;
  unitOfMeasureId: string;
};

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5000";

async function request<TResponse>(
  session: AuthenticationSession,
  path: string,
  options: RequestInit = {}
): Promise<TResponse> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${session.accessToken}`,
      ...options.headers
    }
  });

  if (!response.ok) {
    throw new Error("Nao foi possivel concluir o pedido do catalogo.");
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return response.json() as Promise<TResponse>;
}

export const productCatalogService = {
  searchProducts(session: AuthenticationSession, search: string, page = 1, pageSize = 20) {
    const parameters = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString()
    });

    if (search.trim()) {
      parameters.set("search", search.trim());
    }

    return request<PagedResult<Product>>(session, `/products?${parameters.toString()}`);
  },

  getProduct(session: AuthenticationSession, productId: string) {
    return request<Product>(session, `/products/${productId}`);
  },

  createProduct(session: AuthenticationSession, product: SaveProductRequest) {
    return request<Product>(session, "/products", {
      method: "POST",
      body: JSON.stringify(product)
    });
  },

  updateProduct(session: AuthenticationSession, productId: string, product: SaveProductRequest) {
    return request<Product>(session, `/products/${productId}`, {
      method: "PUT",
      body: JSON.stringify({
        name: product.name,
        categoryId: product.categoryId,
        unitOfMeasureId: product.unitOfMeasureId
      })
    });
  },

  deactivateProduct(session: AuthenticationSession, productId: string) {
    return request<void>(session, `/products/${productId}`, {
      method: "DELETE"
    });
  },

  getCategories(session: AuthenticationSession) {
    return request<Category[]>(session, "/categories");
  },

  getUnitsOfMeasure(session: AuthenticationSession) {
    return request<UnitOfMeasure[]>(session, "/units-of-measure");
  }
};
