import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  CreateRoasterDto,
  Roaster,
  RoasterSearchParams,
  UpdateRoasterDto,
} from '../models/roaster';
import { getHttpParams } from '../utils/params.utils';

@Injectable({
  providedIn: 'root',
})
export class RoastersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getRoasters(roasterSearchParams: RoasterSearchParams) {
    return this.http.get<Roaster[]>(`${this.baseUrl}roasters/`, {
      observe: 'response',
      params: getHttpParams(roasterSearchParams),
    });
  }

  getRoaster(id: number) {
    return this.http.get<Roaster>(`${this.baseUrl}roasters/${id}`);
  }

  createRoaster(createRoasterDto: CreateRoasterDto) {
    return this.http.post<Roaster>(
      `${this.baseUrl}roasters/create`,
      createRoasterDto,
    );
  }

  updateRoaster(id: number, updateRoasterDto: UpdateRoasterDto) {
    return this.http.patch<Roaster>(
      `${this.baseUrl}roasters/${id}`,
      updateRoasterDto,
    );
  }

  onDeleteRoaster(id: number) {
    return this.http.delete<void>(`${this.baseUrl}roasters/${id}`);
  }
}
