import { FormEvent, useEffect, useMemo, useState } from "react";
import { useAuth } from "../auth/useAuth";
import {
  Category,
  Product,
  productCatalogService,
  SaveProductRequest,
  UnitOfMeasure
} from "../masterData/productCatalogService";
import { ErrorBanner } from "../shared/masterData/ErrorBanner";
import { EntityDetailPanel } from "../shared/masterData/EntityDetailPanel";
import { EntityFormPanel } from "../shared/masterData/EntityFormPanel";
import { EntityList } from "../shared/masterData/EntityList";
import { PageHeader } from "../shared/masterData/PageHeader";
import { SearchBar } from "../shared/masterData/SearchBar";
import { MasterDataListItem } from "../shared/masterData/types";

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
  const [products, setProducts] = useState<MasterDataListItem[]>([]);
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

  async function handleSelectProduct(item: MasterDataListItem) {
    if (!session) {
      return;
    }

    try {
      const product = await productCatalogService.getProduct(session, item.id);
      setSelectedProduct(product);
      setError(null);
    } catch {
      setError("Nao foi possivel carregar o detalhe do produto.");
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
      <PageHeader
        title="Catalogo de produtos"
        description="Produtos, categorias e unidades de medida de referencia para os proximos modulos."
      />

      <ErrorBanner message={error} />

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1.4fr)_minmax(320px,0.8fr)]">
        <div className="space-y-4">
          <SearchBar
            value={search}
            onChange={setSearch}
            onSubmit={handleSearch}
            placeholder="Pesquisar por codigo ou nome"
          />

          <EntityList
            items={products}
            totalRecords={totalRecords}
            entityLabel="produtos"
            emptyMessage="Nenhum produto encontrado."
            onSelect={handleSelectProduct}
          />

          {selectedProduct ? (
            <EntityDetailPanel
              code={selectedProduct.code}
              name={selectedProduct.name}
              isActive={selectedProduct.isActive}
              onEdit={() => beginEdit(selectedProduct)}
              onDeactivate={() => handleDeactivateProduct(selectedProduct.id)}
            >
              <dl className="grid gap-3 text-sm sm:grid-cols-2">
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
            </EntityDetailPanel>
          ) : null}
        </div>

        <EntityFormPanel
          title={editingProduct ? "Editar produto" : "Criar produto"}
          onSubmit={handleSaveProduct}
          onCancel={
            editingProduct
              ? () => {
                  setEditingProduct(null);
                  setForm(emptyForm);
                }
              : undefined
          }
        >
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
        </EntityFormPanel>
      </div>
    </section>
  );
}
