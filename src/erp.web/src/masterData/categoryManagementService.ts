import { createManagedReferenceDataService } from "./managedReferenceDataService";

export const categoryManagementService = createManagedReferenceDataService(
  "/categories",
  "Não foi possível concluir o pedido de categorias."
);
