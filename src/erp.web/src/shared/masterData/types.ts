export type MasterDataListItem = {
  id: string;
  code: string;
  name: string;
  isActive: boolean;
};

export type PagedResult<TItem> = {
  page: number;
  pageSize: number;
  totalRecords: number;
  items: TItem[];
};
