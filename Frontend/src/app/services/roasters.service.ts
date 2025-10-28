import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { getPaginationParams } from '../utils/pagination.utils';
import { Roaster } from '../models/roaster';

@Injectable({
  providedIn: 'root',
})
export class RoastersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;

  getRoasters(page?: number, pageSize?: number) {
    return this.http.get<Roaster[]>(`${this.baseUrl}roasters/`, {
      observe: 'response',
      params: getPaginationParams(page, pageSize),
    });
  }

  getRoaster(id: number) {
    return this.http.get<Roaster>(`${this.baseUrl}roasters/${id}`);
  }

  searchRoasters(search: string) {
    return this.http.get<Roaster[]>(`${this.baseUrl}roasters/?s=${search}`);
  }

  createRoaster(model: any) {
    console.log('TODO');
  }

  updateRoaster(id: number, model: any) {
    console.log('TODO');
  }
}
