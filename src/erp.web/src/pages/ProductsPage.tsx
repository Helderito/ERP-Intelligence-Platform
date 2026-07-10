import { FormEvent, useEffect, useMemo, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  Category,
  Product,
  productCatalogService,
  SaveProductRequest,
  UnitOfMeasure
} from "../masterData/productCatalogService";

type ProductFormState = {
  code: string;
  name: string;
  categoryId: string;
  unitOfMeasureId: string;
};

const emptyForm: ProductFormState = {
  code: "",
  name: "",
  categoryId: "",
  unitOfMeasureId: ""
};

export function ProductsPage() {
  const { session } = useAuth();
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [unitsOfMeasure, setUnitsOfMeasure] = useState<UnitOfMeasure[]>([]);
  const [search, setSearch] = useState("");
  const [totalRecords, setTotalRecords] = useState(0);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [form, setForm] = useState<ProductFormState>(emptyForm);
  const [error, setError] = useState<string | null>(null);

  const categoriesById = useMemo(
    () => new Map(categories.map((category) => [category.id, category])),
    [categories]
  );
  const unitsById = useMemo(
    () => new Map(unitsOfMeasure.map((unit) => [unit.id, unit])),
    [unitsOfMeasure]
  );

  async function loadData(nextSearch = search) {
    if (!session) {
      return;
    }

    const [productResult, nextCategories, nextUnitsOfMeasure] = await Promise.all([
      productCatalogService.searchProducts(session, nextSearch),
      productCatalogService.getCategories(session),
      productCatalogService.getUnitsOfMeasure(session)
    ]);

    setProducts(productResult.items);
    setTotalRecords(productResult.totalRecords);
    setCategories(nextCategories);
    setUnitsOfMeasure(nextUnitsOfMeasure);

    if (!form.categoryId && nextCategories[0]) {
      setForm((current) => ({ ...current, categoryId: current.categoryId || nextCategories[0].id }));
    }

    if (!form.unitOfMeasureId && nextUnitsOfMeasure[0]) {
      setForm((current) => ({
        ...current,
        unitOfMeasureId: current.unitOfMeasureId || nextUnitsOfMeasure[0].id
      }));
    }
  }

  useEffect(() => {
    loadData().catch(() => setError("Nao foi possivel carregar o catalogo de produtos."));
  }, [session]);

  async function handleSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    try {
      await loadData(search);
      setError(null);
    } catch {
      setError("Nao foi possivel pesquisar produtos.");
    }
  }

  async function handleSaveProduct(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!session) {
      return;
    }

    const payload: SaveProductRequest = {
      code: form.code,
      name: form.name,
      categoryId: form.categoryId,
      unitOfMeasureId: form.unitOfMeasureId
    };

    try {
      const savedProduct = editingProduct
        ? await productCatalogService.updateProduct(session, editingProduct.id, payload)
        : await productCatalogService.createProduct(session, payload);

      setSelectedProduct(savedProduct);
      setEditingProduct(null);
      setForm({
        ...emptyForm,
        categoryId: categories[0]?.id ?? "",
        unitOfMeasureId: unitsOfMeasure[0]?.id ?? ""
      });
      await loadData();
      setError(null);
    } catch {
      setError("Nao foi possivel guardar o produto.");
    }
  }

  async function handleDeactivateProduct(productId: string) {
    if (!session) {
      return;
    }

    try {
      await productCatalogService.deactivateProduct(session, productId);
      setSelectedProduct(null);
      setEditingProduct(null);
      await loadData();
      setError(null);
    } catch {
      setError("Nao foi possivel desativar o produto.");
    }
  }

  function beginEdit(product: Product) {
    setEditingProduct(product);
    setForm({
      code: product.code,
      name: product.name,
      categoryId: product.categoryId,
      unitOfMeasureId: product.unitOfMeasureId
    });
  }

  return (
    <section className="space-y-6">
      <div className="border-b border-slate-200 pb-5">
        <h2 className="text-2xl font-semibold">Catalogo de produtos</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-600">
          Produtos, categorias e unidades de medida de referencia para os proximos modulos.
        </p>
      </div>

      {error ? <p className="text-sm font-medium text-red-700">{error}</p> : null}

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.4fr)_minmax(320px,0.8fr)]">
        <div className="space-y-4">
          <form className="flex gap-3" onSubmit={handleSearch}>
            <input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Pesquisar por codigo ou nome"
              className="min-w-0 flex-1 rounded-md border border-slate-300 px-3 py-2 text-sm"
            />
            <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white" type="submit">
              Pesquisar
            </button>
          </form>

          <div className="overflow-hidden rounded-md border border-slate-200 bg-white shadow-sm">
            <div className="border-b border-slate-200 px-4 py-3 text-sm text-slate-600">
              {totalRecords} produtos encontrados
            </div>
            <div className="divide-y divide-slate-200">
              {products.map((product) => (
                <button
                  key={product.id}
                  type="button"
                  onClick={() => setSelectedProduct(product)}
                  className="grid w-full grid-cols-[120px_minmax(0,1fr)_100px] gap-4 px-4 py-3 text-left text-sm transition hover:bg-slate-50"
                >
                  <span className="font-mono font-semibold text-slate-900">{product.code}</span>
                  <span className="truncate font-medium text-slate-800">{product.name}</span>
                  <span className={product.isActive ? "text-emerald-700" : "text-slate-500"}>
                    {product.isActive ? "Ativo" : "Inativo"}
                  </span>
                </button>
              ))}
              {products.length === 0 ? (
                <p className="px-4 py-6 text-sm text-slate-500">Nenhum produto encontrado.</p>
              ) : null}
            </div>
          </div>

          {selectedProduct ? (
            <article className="rounded-md border border-slate-200 bg-white p-4 shadow-sm">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="font-mono text-sm font-semibold text-brand-700">{selectedProduct.code}</p>
                  <h3 className="mt-1 text-xl font-semibold">{selectedProduct.name}</h3>
                </div>
                <span className="rounded-md bg-slate-100 px-3 py-1 text-sm font-medium text-slate-700">
                  {selectedProduct.isActive ? "Ativo" : "Inativo"}
                </span>
              </div>
              <dl className="mt-4 grid gap-3 text-sm sm:grid-cols-2">
                <div>
                  <dt className="font-medium text-slate-500">Categoria</dt>
                  <dd className="mt-1 text-slate-900">
                    {categoriesById.get(selectedProduct.categoryId)?.name ?? selectedProduct.categoryId}
                  </dd>
                </div>
                <div>
                  <dt className="font-medium text-slate-500">Unidade de medida</dt>
                  <dd className="mt-1 text-slate-900">
                    {unitsById.get(selectedProduct.unitOfMeasureId)?.name ?? selectedProduct.unitOfMeasureId}
                  </dd>
                </div>
              </dl>
              <div className="mt-4 flex gap-3">
                <button
                  type="button"
                  onClick={() => beginEdit(selectedProduct)}
                  className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
                >
                  Editar
                </button>
                <button
                  type="button"
                  onClick={() => handleDeactivateProduct(selectedProduct.id)}
                  className="rounded-md border border-red-200 px-3 py-2 text-sm font-medium text-red-700"
                >
                  Desativar
                </button>
              </div>
            </article>
          ) : null}
        </div>

        <form className="space-y-4 rounded-md border border-slate-200 bg-white p-4 shadow-sm" onSubmit={handleSaveProduct}>
          <div>
            <h3 className="text-lg font-semibold">{editingProduct ? "Editar produto" : "Criar produto"}</h3>
          </div>
          <label className="block text-sm font-medium text-slate-700">
            Codigo
            <input
              value={form.code}
              onChange={(event) => setForm((current) => ({ ...current, code: event.target.value }))}
              required
              disabled={Boolean(editingProduct)}
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm disabled:bg-slate-100"
            />
          </label>
          <label className="block text-sm font-medium text-slate-700">
            Nome
            <input
              value={form.name}
              onChange={(event) => setForm((current) => ({ ...current, name: event.target.value }))}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
            />
          </label>
          <label className="block text-sm font-medium text-slate-700">
            Categoria
            <select
              value={form.categoryId}
              onChange={(event) => setForm((current) => ({ ...current, categoryId: event.target.value }))}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
            >
              <option value="">Selecionar categoria</option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </select>
          </label>
          <label className="block text-sm font-medium text-slate-700">
            Unidade de medida
            <select
              value={form.unitOfMeasureId}
              onChange={(event) => setForm((current) => ({ ...current, unitOfMeasureId: event.target.value }))}
              required
              className="mt-1 w-full rounded-md border border-slate-300 px-3 py-2 text-sm"
            >
              <option value="">Selecionar unidade</option>
              {unitsOfMeasure.map((unit) => (
                <option key={unit.id} value={unit.id}>
                  {unit.name}
                </option>
              ))}
            </select>
          </label>
          <div className="flex gap-3">
            <button className="rounded-md bg-brand-600 px-4 py-2 text-sm font-semibold text-white" type="submit">
              Guardar
            </button>
            {editingProduct ? (
              <button
                type="button"
                onClick={() => {
                  setEditingProduct(null);
                  setForm(emptyForm);
                }}
                className="rounded-md border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
              >
                Cancelar
              </button>
            ) : null}
          </div>
        </form>
      </div>
    </section>
  );
}
