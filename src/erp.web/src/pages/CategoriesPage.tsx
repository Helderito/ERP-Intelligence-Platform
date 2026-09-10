import { categoryManagementService } from "../masterData/categoryManagementService";
import { ManagedReferenceDataPage } from "../shared/masterData/ManagedReferenceDataPage";

export function CategoriesPage() {
  return <ManagedReferenceDataPage
    title="Categorias"
    description="Gestão das categorias disponíveis no catálogo de produtos."
    entityLabel="categorias"
    formEntityName="categoria"
    emptyMessage="Nenhuma categoria encontrada."
    loadError="Não foi possível carregar as categorias."
    saveError="Não foi possível guardar a categoria."
    deactivateError="Não foi possível desativar a categoria."
    service={categoryManagementService}
  />;
}
