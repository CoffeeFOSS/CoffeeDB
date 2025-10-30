export interface RoasterBase {
  name: string;
  alias?: string;
  location?: string;
  websiteUrl?: string;
  description?: string;
}

export interface Roaster extends RoasterBase {
  id: number;
}

export interface UpdateRoasterDto {
  name?: string;
  alias?: string;
  location?: string;
  websiteUrl?: string;
  description?: string;
}

export interface RoasterSearchParams {
  page?: number;
  pageSize?: number;
  name?: string;
  location?: string;
}
