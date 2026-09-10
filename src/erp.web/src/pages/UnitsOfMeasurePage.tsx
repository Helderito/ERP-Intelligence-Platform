import { unitOfMeasureManagementService } from "../masterData/unitOfMeasureManagementService";
import { ManagedReferenceDataPage } from "../shared/masterData/ManagedReferenceDataPage";

export function UnitsOfMeasurePage() {
  return <ManagedReferenceDataPage
    title="Unidades de Medida"
    description="Gestão das unidades usadas para classificar quantidades de produtos."
    entityLabel="unidades"
    formEntityName="unidade de medida"
    emptyMessage="Nenhuma unidade de medida encontrada."
    loadError="Não foi possível carregar as unidades de medida."
    saveError="Não foi possível guardar a unidade de medida."
    deactivateError="Não foi possível desativar a unidade de medida."
    service={unitOfMeasureManagementService}
  />;
}
