export interface RoasterBase {
  name: string;
  alias?: string;
  locationAddress?: string;
  websiteUrl?: string;
  description?: string;
}

export interface Roaster extends RoasterBase {
  id: number;
  distanceInKilometers?: number;
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
  lat?: number;
  long?: number;
  radius?: number;
}
