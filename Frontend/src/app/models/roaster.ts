export interface RoasterBase {
  name: string;
  alias?: string;
  locationAddress?: string;
  websiteUrl?: string;
  description?: string;
}

export interface Roaster extends RoasterBase {
  id: number;
}

export interface CreateRoasterDto extends RoasterBase {}

export interface UpdateRoasterDto {
  name?: string;
  alias?: string;
  locationAddress?: string;
  websiteUrl?: string;
  description?: string;
}

export interface RoasterSearchParams {
  page?: number;
  pageSize?: number;
  name?: string;
  locationAddress?: string;
}
