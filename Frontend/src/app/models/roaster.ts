export interface RoasterBase {
  name: string;
  alias?: string;
  locationAddress?: string;
  locationCoordinates?: {
    latitude: number;
    longitude: number;
  };
  websiteUrl?: string;
  description?: string;
}

export interface Roaster extends RoasterBase {
  id: number;
  distanceInKilometers?: number;
}

export interface CreateRoasterDto {
  comment: string;
  name: string;
  alias?: string;
  locationAddress?: string;
  locationCoordinateLatitude?: number;
  locationCoordinateLongitude?: number;
  websiteUrl?: string;
  description?: string;
}

export interface UpdateRoasterDto {
  comment: string;
  name: string;
  alias?: string;
  locationAddress?: string;
  locationCoordinateLatitude?: number;
  locationCoordinateLongitude?: number;
  websiteUrl?: string;
  description?: string;
}

export interface RoasterSearchParams {
  page?: number;
  pageSize?: number;
  name?: string;
  address?: string;
  lat?: number;
  long?: number;
  radius?: number;
}
